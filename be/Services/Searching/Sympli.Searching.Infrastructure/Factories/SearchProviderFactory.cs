using Microsoft.Extensions.DependencyInjection;
using Sympli.Searching.Core.Enums;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Infrastructure.Providers;

namespace Sympli.Searching.Infrastructure.Factories
{
    public class SearchProviderFactory : ISearchProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ISearchResultProvider GetProvider(SearchEngineEnum engine)
        {
            return engine switch
            {
                SearchEngineEnum.Google => _serviceProvider.GetRequiredService<GoogleSearchResultProvider>(),
                SearchEngineEnum.Bing => _serviceProvider.GetRequiredService<BingSearchResultProvider>(),
                _ => throw new ArgumentOutOfRangeException(nameof(engine), $"Unhandled engine: {engine}")
            };
        }
    }
}




