using Microsoft.Extensions.Options;
using Sympli.Searching.Core.Constants;
using Sympli.Searching.Core.Entities;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Infrastructure.Factories;
using Sympli.Searching.Infrastructure.Providers;
using System.Net.Http;

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

            services.Configure<AppSettings>(_ =>
            {
                var limit = 100;

                _.BingSearch = new SearchSetting
                {
                    Url = configuration["BingSearch:Url"] ?? throw new ArgumentNullException(nameof(SearchSetting)),
                    Limit = int.TryParse(configuration["BingSearch:Limit"], out limit) ? limit : throw new ArgumentNullException(nameof(SearchSetting))
                };

                _.GoogleSearch = new SearchSetting
                {
                    Url = configuration["GoogleSearch:Url"] ?? throw new ArgumentNullException(nameof(SearchSetting)),
                    Limit = int.TryParse(configuration["GoogleSearch:Limit"], out limit) ? limit : throw new ArgumentNullException(nameof(SearchSetting))
                };

                _.CacheingExpireation = int.TryParse(configuration["CacheingExpireation"], out int expireation) ? expireation : throw new ArgumentNullException(nameof(SearchSetting));
            });

            services.AddHttpClient(CommonConstants.GoogleSearchClient, (serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
                client.BaseAddress = new Uri(settings.GoogleSearch.Url);
                client.DefaultRequestHeaders.Add("User-Agent", CommonConstants.DefaultUserAgent);
            });

            services.AddHttpClient(CommonConstants.BingSearchClient, (serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
                client.BaseAddress = new Uri(settings.BingSearch.Url);
                client.DefaultRequestHeaders.Add("User-Agent", CommonConstants.DefaultUserAgent);
            });

            services.AddMemoryCache();

            return services;

        }
    }
}
