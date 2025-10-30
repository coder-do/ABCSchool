using Infra;

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

            var app = builder.Build();

            await app.Services.AddDatabaseInitializerAsync();

            app.UseHttpsRedirection();

            app.UseInfra();

            app.MapControllers();

            app.Run();
        }
    }
}
