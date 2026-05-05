using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

using SharedKernel.Api.Hosting;
using SharedKernel.DependencyInjection;
using SharedKernel.Api;

using CampFitFurDogs.Api.Hosting;
using CampFitFurDogs.Application;
using CampFitFurDogs.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Hosting-provider overrides (pluggable) ───────────────────────
// Add new providers here in priority order.  The first whose
// IsActive() returns true wins; the rest are skipped.
await builder.UseHostingProviders(
    new RenderHostingProvider());

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port)); // IPv4 ANY
});
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// 0. CORS: allow frontend host
var allowedOrigin = builder.Configuration["Frontend:BaseUrl"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigin ?? "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSharedKernel([
    typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
]);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
EndpointDiscovery.AddEndpoints(apiAssembly);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapEndpoints();
app.UseCors();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?.Error;
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = validationException.Errors
                .Select(e => new { e.PropertyName, e.ErrorMessage });

            await context.Response.WriteAsJsonAsync(new { Errors = errors });
            return;
        }

        // fallback for other exceptions
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
   .WithName("HealthCheck");

app.Run();
