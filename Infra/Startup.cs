using Application;
using Application.Features.Identity.Tokens;
using Application.Wrappers;
using Finbuckle.MultiTenant;
using Infra.Constants;
using Infra.Contexts;
using Infra.Contexts.Seeders;
using Infra.Identity.Auth;
using Infra.Identity.Models;
using Infra.Identity.Tokens;
using Infra.OpenApi;
using Infra.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Infra
{
    public static class Startup
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddDbContext<TenantDbContext>(o => o
                    .UseSqlServer(configuration.GetConnectionString("DBConnection")))
                .AddMultiTenant<ABCSchoolTenantInfo>()
                    .WithHeaderStrategy(TenancyConstants.TenandIdName)
                    .WithClaimStrategy(TenancyConstants.TenandIdName)
                    .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>()
                    .Services
                .AddDbContext<ApplicationDBContext>(opt => opt
                    .UseSqlServer(configuration.GetConnectionString("DBConnection")))
                .AddTransient<ITenantDBSeeder, TenantDbSeeder>()
                .AddTransient<ApplicationDBSeeder>()
                .AddIdentityServices()
                .AddPermissions()
                .OpenApiDocumentation(configuration);
        }

        public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider, CancellationToken ct = default)
        {
            using var scope = serviceProvider.CreateScope();

            await scope.ServiceProvider.GetRequiredService<ITenantDBSeeder>()
                .InitializeDatabaseAsync(ct);
        }

        internal static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            return services
                .AddIdentity<ApplicationUser, ApplicationRole>(op =>
                {
                    op.Password.RequiredLength = 8;
                    op.Password.RequireDigit = false;
                    op.Password.RequireLowercase = false;
                    op.Password.RequireUppercase = false;
                    op.Password.RequireNonAlphanumeric = false;
                    op.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders()
                .Services
                .AddScoped<ITokenService, TokenService>();
        }

        internal static IServiceCollection AddPermissions(this IServiceCollection services)
        {
            return services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }

        public static JwtSettings GetJwtSettings(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettings = config.GetSection(nameof(JwtSettings));

            services.Configure<JwtSettings>(jwtSettings);

            return jwtSettings.Get<JwtSettings>();
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            var secret = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            services
                .AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    };
                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";

                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("Token has expired"));
                                return context.Response.WriteAsync(result);
                            } else
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.ContentType = "application/json";

                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("InternalServerError in OnAuthenticationFailed"));
                                return context.Response.WriteAsync(result);
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";

                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized"));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";

                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access to this resource"));
                            return context.Response.WriteAsync(result);
                        }
                    };
                });

            services
                .AddAuthorization(opt =>
                {
                    foreach (var prop in typeof(SchoolPermissions)
                        .GetNestedTypes()
                        .SelectMany(e => e.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))) 
                    {
                        var propValue = prop.GetValue(null);

                        if (propValue is not null)
                        {
                            opt.AddPolicy(propValue.ToString(), policy => policy
                                .RequireClaim(ClaimConstants.Permission, propValue.ToString()));
                        }
                    }
                });

            return services;
        }

        internal static IServiceCollection OpenApiDocumentation(this IServiceCollection services, IConfiguration config)
        {
            var swaggerSettings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

            services.AddEndpointsApiExplorer();

            _ = services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.PostProcess = doc =>
                {
                    doc.Info.Title = swaggerSettings.Title;
                    doc.Info.Description = swaggerSettings.Description;
                    doc.Info.Contact = new OpenApiContact
                    {
                        Name = swaggerSettings.CoontactName,
                        Email = swaggerSettings.CoontactEmail,
                        Url = swaggerSettings.CoontactUrl
                    };
                    doc.Info.License = new OpenApiLicense
                    {
                        Name = swaggerSettings.LicenseName,
                        Url = swaggerSettings.LicenseUrl
                    };
                };

                document.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your bearer token",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });

                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
                document.OperationProcessors.Add(new SwaggerGlobalAuthProcessor());
                document.OperationProcessors.Add(new SwaggerHeaderAttributeProcessor());
            });
            
            return services;
        }

        public static IApplicationBuilder UseInfra(this IApplicationBuilder app)
        {
            return app
                .UseAuthentication()
                .UseMultiTenant()
                .UseAuthorization()
                .UseOpenApiDocumentation();
        }

        internal static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app)
        {
            app.UseOpenApi();
            app.UseSwaggerUi(o =>
            {
               o.DefaultModelExpandDepth = 0;
               o.DocExpansion = "none";
               o.TagsSorter = "alpha";
            });

            return app;
        }
    }
}
