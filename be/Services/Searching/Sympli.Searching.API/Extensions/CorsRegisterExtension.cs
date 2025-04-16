using Microsoft.Extensions.Configuration;

namespace Sympli.Searching.API.Extensions
{
    public static class CorsRegisterExtension
    {
        public static IServiceCollection CorsRegister(this IServiceCollection services, ConfigurationManager configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy("MyCorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
