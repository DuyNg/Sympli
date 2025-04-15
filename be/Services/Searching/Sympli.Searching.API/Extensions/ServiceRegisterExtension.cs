using Sympli.Searching.Core.Constants;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Infrastructure.Factories;
using Sympli.Searching.Infrastructure.Providers;

namespace Sympli.Searching.API.Extensions
{
    public static class ServiceRegisterExtension
    {
        public static IServiceCollection ServicesRegister(this IServiceCollection services, ConfigurationManager configuration)
        {
            // Register the search provider from the Infrastructure layer using the Core interface.
            services.AddScoped<GoogleSearchResultProvider>();
            services.AddScoped<BingSearchResultProvider>();
            // Register the factory for creating search providers.
            services.AddScoped<ISearchProviderFactory, SearchProviderFactory>();

            // Register a named HttpClient for the Google Search API.
            services.AddHttpClient(CommonConstants.GoogleSearchClient, client =>
            {
                var url = configuration["GoogleSearch:Url"];
                client.BaseAddress = new Uri(url);
            });

            services.AddHttpClient(CommonConstants.BingSearchClient, client =>
            {
                var url = configuration["BingSearch:Url"];
                client.BaseAddress = new Uri(url);
            });

            return services;

        }
    }
}
