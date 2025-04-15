using MediatR;
using Sympli.Searching.Application.Queries;
using System.Reflection;

namespace Sympli.Searching.API.Extensions
{
    public static class MediatorRegisterExtension
    {
        public static IServiceCollection MediatorRegister(this IServiceCollection services)
        {
            // Register MediatR for CQRS handlers.
            // This will scan the assembly for all MediatR handlers and register them.
            // The assembly is the current executing assembly, which is where the MediatR handlers are defined.
            // This is a common pattern in ASP.NET Core applications to keep the code organized and maintainable.
            // It allows for easy separation of concerns and promotes the use of the CQRS pattern.
            // MediatR is a popular library for implementing the CQRS pattern in .NET applications.
            services.AddTransient<IRequestHandler<GetSearchResultsQuery, SearchResultsDto>, GetSearchResultsQueryHandler>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            return services;
        }
    }
}
