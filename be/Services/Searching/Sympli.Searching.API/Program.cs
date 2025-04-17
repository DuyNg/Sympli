using Sympli.Searching.API.Extensions;
using Sympli.Searching.API.Middleware;
using Sympli.Searching.API.Router;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"environment: {environment}");
// Load environment-specific settings (e.g., Development, Production)
builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

var configuration = builder.Configuration;
var appName = configuration["CustomSettings:AppName"];
var version = configuration["CustomSettings:Version"];

Console.WriteLine($"App Name: {appName} - version: {version}");

var services = builder.Services;
services.CorsRegister(configuration);

// Register MediatR for CQRS handlers.
services.MediatorRegister();

// Register the Infrastructure layer services.
services.ServicesRegister(configuration);

// Optional: Add Swagger for API documentation.
services.SwaggerRegister();
services.AddHealthChecks();

var app = builder.Build();

// Configure middleware.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simpli.Searching.API v1"));
}


// Add the ErrorHandlingMiddleware to the pipeline
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("MyCorsPolicy");
// Add Health Checks endpoint
app.MapHealthChecks("/health");
app.UseHttpsRedirection();
// Minimal API endpoint for search.
app.MapSearchingEndpoints();

app.UseRouting();

app.Run();
