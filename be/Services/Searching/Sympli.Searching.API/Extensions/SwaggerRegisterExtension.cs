using Microsoft.OpenApi.Models;

namespace Sympli.Searching.API.Extensions
{
    public static class SwaggerRegisterExtension
    {
        public static IServiceCollection SwaggerRegister(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sympli Product API", Version = "v1" });
            });
            return services;
        }
    }
}
