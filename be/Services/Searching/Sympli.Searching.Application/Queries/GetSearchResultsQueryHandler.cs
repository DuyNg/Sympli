using MediatR;
using Sympli.Searching.Application.Queries;
using Sympli.Searching.Core.Enums;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Core.Utilities;

public class GetSearchResultsQueryHandler : IRequestHandler<GetSearchResultsQuery, SearchResultsDto>
{
    private readonly ISearchProviderFactory _providerFactory;

    public GetSearchResultsQueryHandler(ISearchProviderFactory providerFactory)
    {
        _providerFactory = providerFactory;
    }

    public async Task<SearchResultsDto> Handle(GetSearchResultsQuery request, CancellationToken cancellationToken)
    {
        var provider = _providerFactory.GetProvider(request.Engine);
        var result = await provider.GetRankPositionsAsync(request.Keyword, request.url);
        return new SearchResultsDto() 
        { 
            Positions = result, 
            Browser = EnumHelper.GetEnumDescription(request.Engine)
        };
    }
}
