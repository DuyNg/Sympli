using Sympli.Searching.API.Extensions;
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
services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Register MediatR for CQRS handlers.
services.MediatorRegister();

// Register the Infrastructure layer services.
services.ServicesRegister(configuration);

// Optional: Add Swagger for API documentation.
services.SwaggerRegister();

var app = builder.Build();

// Configure middleware.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simpli.Searching.API v1"));
}

// Add Health Checks endpoint
// app.MapHealthChecks("/health");

app.UseHttpsRedirection();

// Minimal API endpoint for search.
app.MapSearchingEndpoints();

app.Run();
