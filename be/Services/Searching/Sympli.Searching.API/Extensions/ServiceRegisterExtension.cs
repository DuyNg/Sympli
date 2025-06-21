using Microsoft.Extensions.Options;
using Polly;
using Sympli.Searching.Core.Constants;
using Sympli.Searching.Core.Entities;
using Sympli.Searching.Core.Exceptions;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Infrastructure.Factories;
using Sympli.Searching.Infrastructure.Providers;
using Sympli.Searching.Infrastructure.Services;

namespace Sympli.Searching.API.Extensions
{
    public static class ServiceRegisterExtension
    {
        public static IServiceCollection ServicesRegister(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<GoogleSearchResultProvider>();
            services.AddScoped<BingSearchResultProvider>();
            services.AddScoped<ISearchProviderFactory, SearchProviderFactory>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();

            services.Configure<AppSettings>(_ =>
            {
                var limit = 100;

                var bingUrl = configuration["BingSearch:Url"];
                if (string.IsNullOrEmpty(bingUrl))
                    throw new ConfigurationMissingException("BingSearch:Url");
                _.BingSearch = new SearchSetting
                {
                    Url = bingUrl,
                    Limit = int.TryParse(configuration["BingSearch:Limit"], out limit) ? limit : throw new ConfigurationMissingException("BingSearch:Limit")
                };

                var googleUrl = configuration["GoogleSearch:Url"];
                if (string.IsNullOrEmpty(googleUrl))
                    throw new ConfigurationMissingException("GoogleSearch:Url");
                _.GoogleSearch = new SearchSetting
                {
                    Url = googleUrl,
                    Limit = int.TryParse(configuration["GoogleSearch:Limit"], out limit) ? limit : throw new ConfigurationMissingException("GoogleSearch:Limit")
                };

                _.CacheingExpireation = int.TryParse(configuration["CacheingExpireation"], out int expiration) ? expiration : throw new ConfigurationMissingException("CacheingExpireation");
            });

            services.AddHttpClient(CommonConstants.GoogleSearchClient, (serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
                client.BaseAddress = new Uri(settings.GoogleSearch.Url);
                client.DefaultRequestHeaders.Add("User-Agent", CommonConstants.DefaultUserAgent);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddPolicyHandler(Polly.Extensions.Http.HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            services.AddHttpClient(CommonConstants.BingSearchClient, (serviceProvider, client) =>
            {
                var employees = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
                client.BaseAddress = new Uri(employees.BingSearch.Url);
                client.DefaultRequestHeaders.Add("User-Agent", CommonConstants.DefaultUserAgent);
                client.Timeout = TimeSpan.FromSeconds(30); // Add timeout for consistency
            })
            .AddPolicyHandler(Polly.Extensions.Http.HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))); // Add Polly policy

            services.AddMemoryCache();

            return services;
        }
    }
}