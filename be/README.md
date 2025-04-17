# Structure
Sympli.sln
└── Services
    ├── OtherServices
    │     ├── ...
    └── Searching
        ├── Sympli.Searching.Core
        │     ├── Sympli.Searching.Core.csproj
        │     ├── Entities
        │     │     ├── SearchQuery.cs
        │     │     └── SearchSetting.cs
        │     ├── Interfaces
        │     │     └── ISearchResultProvider.cs
        │     └── Constants
        │           └── SearchEngineEnum.cs
        ├── Sympli.Searching.Application
        │     ├── Sympli.Searching.Application.csproj
        │     └── Queries
        │           ├── GetSearchResultsQuery.cs
        │           └── GetSearchResultsQueryHandler.cs
        ├── Sympli.Searching.Infrastructure
        │     ├── Sympli.Searching.Infrastructure.csproj
        │     ├── Providers
        │     │     ├── GoogleSearchResultProvider.cs
        │     │     └── BingSearchResultProvider.cs
        │     └── Services
        │           └── HttpClientWrapper.cs
        └── Sympli.Searching.API
              ├── Sympli.Searching.API.csproj
              ├── Program.cs
              ├── Extensions
              │     └── SwaggerRegisterExtension.cs
              ├── Middleware
              │     └── ErrorHandlingMiddleware.cs
              └── Router
                    └── SearchingEndpoints.cs
#Project Approach
1. RESTful API with Minimal API
The project uses a RESTful API design implemented with .NET 9's minimal API approach. This ensures lightweight and efficient endpoints for handling search queries. The endpoints are defined in the SearchingEndpoints class, which maps routes for Google and Bing search operations.

2. Clean Architecture
The project follows the principles of Clean Architecture to maintain a clear separation of concerns:

Core Layer: Contains business entities, interfaces, and constants. This layer is independent of any external frameworks.
Application Layer: Implements CQRS (Command Query Responsibility Segregation) with query handlers to process search requests.
Infrastructure Layer: Provides implementations for external dependencies like HTTP clients and caching.
API Layer: Exposes the application functionality through RESTful endpoints.
3. Technical Features
.NET 9: Leverages the latest features of .NET for performance and maintainability.
CQRS: Separates read and write operations for better scalability and maintainability.
Factory Pattern: Used to create instances of search providers (e.g., Google and Bing) dynamically.
Caching: Implements in-memory caching to reduce redundant API calls and improve performance.