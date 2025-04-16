using MediatR;
using Sympli.Searching.Application.Queries;
using Sympli.Searching.Core.Enums;

namespace Sympli.Searching.API.Router
{
    public static class SearchingEndpoints
    {
        public static void MapSearchingEndpoints(this IEndpointRouteBuilder app)
        {
            var productGroup = app.MapGroup("/api/search")
                .WithTags("Search by engine");

            productGroup.MapGet("/google/{keyword}/{url}", async (string keyword, string url, IMediator mediator) =>
            {
                var query = new GetSearchResultsQuery(keyword, url, SearchEngineEnum.Google);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("SearchWithGoogle")
            .WithTags("Search");

            productGroup.MapGet("/bing/{keyword}/{url}", async (string keyword, string url, IMediator mediator) =>
            {
                var query = new GetSearchResultsQuery(keyword, url, SearchEngineEnum.Bing);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("SearchWithBing")
            .WithTags("Search");
        }
    }
}
