using Sympli.Searching.Core.Enums;

namespace Sympli.Searching.Core.Interfaces
{
    public interface ISearchProviderFactory
    {
        ISearchResultProvider GetProvider(SearchEngineEnum engine);
    }
}
