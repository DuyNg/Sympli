Sympli.sln
└── Services
    └── Others Services
         ├──..... 
    └── Searching
        ├── Sympli.Searching.Core
        │     ├── Sympli.Searching.Core.csproj
        │     ├── Entities
        │     │     └── SearchQuery.cs
        │     └── Interfaces
        │           └── ISearchResultProvider.cs
        ├── Sympli.Searching.Application
        │     ├── Sympli.Searching.Application.csproj
        │     └── Queries
        │           ├── GetSearchResultsQuery.cs
        │           └── GetSearchResultsQueryHandler.cs
        ├── Sympli.Searching.Infrastructure
        │     ├── Sympli.Searching.Infrastructure.csproj
        │     └── Providers
        │           └── GoogleSearchResultProvider.cs
        └── Sympli.Searching.API
              ├── Sympli.Searching.API.csproj
              ├── Program.cs
              └── appsettings.json
    