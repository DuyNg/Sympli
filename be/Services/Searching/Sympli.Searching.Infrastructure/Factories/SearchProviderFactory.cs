using Sympli.Searching.Core.Enums;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Infrastructure.Providers;
using System;

namespace Sympli.Searching.Infrastructure.Factories
{
    public class SearchProviderFactory : ISearchProviderFactory
    {
        private readonly GoogleSearchResultProvider _google;
        private readonly BingSearchResultProvider _bing;

        public SearchProviderFactory(
            GoogleSearchResultProvider google,
            BingSearchResultProvider bing)
        {
            _google = google;
            _bing = bing;
        }

        public ISearchResultProvider GetProvider(SearchEngineEnum engine)
        {
            return engine switch
            {
                SearchEngineEnum.Google => _google,
                SearchEngineEnum.Bing => _bing,
                _ => throw new ArgumentOutOfRangeException(nameof(engine), $"Unhandled engine: {engine}")
            };
        }
    }
}
