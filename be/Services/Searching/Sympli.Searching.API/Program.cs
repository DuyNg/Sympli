using Sympli.Searching.API.Extensions;
using Sympli.Searching.API.Middleware;
using Sympli.Searching.API.Router;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"environment: {environment}");
builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

var configuration = builder.Configuration;
var appName = configuration["CustomSettings:AppName"];
var version = configuration["CustomSettings:Version"];

Console.WriteLine($"App Name: {appName} - version: {version}");

var services = builder.Services;
services.CorsRegister(configuration);

// Add logging
services.AddLogging(logging =>
{
    logging.AddConsole(); // Log to console
    logging.AddDebug();   // Log to debug output (useful in development)
});

// Register MediatR for CQRS handlers
services.MediatorRegister();

// Register the Infrastructure layer services
services.ServicesRegister(configuration);

// Add Swagger for API documentation
services.SwaggerRegister();
services.AddHealthChecks();

// Catch startup exceptions
try
{
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simpli.Searching.API v1"));
    }

    app.UseErrorHandling();
    app.UseCors("MyCorsPolicy");
    app.MapHealthChecks("/health");
    app.UseHttpsRedirection();
    app.MapSearchingEndpoints();
    app.UseRouting();

    app.Run();
}
catch (Exception ex)
{
    if (builder is not null)
    {
        var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Application failed to start.");
        throw;
    }
    else
    {
        Console.WriteLine($"Application failed to start: {ex.Message}");
    }
}