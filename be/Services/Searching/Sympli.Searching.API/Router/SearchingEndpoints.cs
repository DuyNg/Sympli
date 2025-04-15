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
                .RequireAuthorization()
                .WithTags("Search by engine");

            productGroup.MapGet("/google", async (string keyword, IMediator mediator) =>
            {
                var query = new GetSearchResultsQuery(keyword, SearchEngineEnum.Google);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("SearchWithGoogle")
            .WithTags("Search");

            productGroup.MapGet("/bing", async (string keyword, IMediator mediator) =>
            {
                var query = new GetSearchResultsQuery(keyword, SearchEngineEnum.Bing);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("SearchWithBing")
            .WithTags("Search");
        }
    }
}
