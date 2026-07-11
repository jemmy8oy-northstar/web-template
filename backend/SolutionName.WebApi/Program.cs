using Scalar.AspNetCore;
using SolutionName.WebApi;
using SolutionName.WebApi.ExceptionHandling;
using SolutionName.WebApi.Routes;
using SolutionName.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBackendServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Global exception handling: AppExceptionHandler maps thrown AppExceptions to RFC 7807
// ProblemDetails; AddProblemDetails supplies the writer + standard fields.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<AppExceptionHandler>();

var app = builder.Build();

// Must sit at the top of the pipeline so it catches exceptions from everything below it.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/scalar/v1");
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
    if (dbContext is null)
        app.Logger.LogWarning("Skipping database migration — no connection string configured.");
    else
        dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapGroup("/api")
    .MapStatusRoutes()
    .WithOpenApi();

app.Run();
