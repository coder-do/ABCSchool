using Infra;
using Application;
using WebApi.Middlewares;
using Microsoft.IdentityModel.Logging;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddInfraServices(builder.Configuration);
            builder.Services.AddJwtAuthentication(
                builder.Services.GetJwtSettings(builder.Configuration));

            builder.Services.AddApplicationServices();

            IdentityModelEventSource.ShowPII = true;

            var app = builder.Build();

            await app.Services.AddDatabaseInitializerAsync();

            app.UseHttpsRedirection();

            app.UseInfra();

            app.UseMiddleware<ErrorHandlingMiddlware>();

            app.MapControllers();

            app.Run();
        }
    }
}
