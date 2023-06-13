using OzMaps.Web.Converters;

namespace OzMaps.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllersWithViews()
                .AddJsonOptions(options=>
                {
                    options.JsonSerializerOptions.Converters.Add(new GeoJsonConverter());
                });

            var app = builder.Build();

            app.UseRouting();
            app.UseCors(config =>
            {
                config.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });

            app.MapControllers();

            app.Run();
        }
    }
}