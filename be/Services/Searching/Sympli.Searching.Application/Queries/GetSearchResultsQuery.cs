using MediatR;
using Sympli.Searching.Core.Enums;

namespace Sympli.Searching.Application.Queries
{
    /// <summary>
    /// Query to fetch search results based on the provided keyword.
    /// </summary>
    public record GetSearchResultsQuery(string Keyword, string url, SearchEngineEnum Engine) : IRequest<SearchResultsDto>;

    /// <summary>
    /// DTO representing the search results.
    /// </summary>
    public class SearchResultsDto
    {
        public string Browser { get; set; }
        public string Positions { get; set; } = string.Empty;
    }
}
