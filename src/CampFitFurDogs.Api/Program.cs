using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

using SharedKernel.Api;
using SharedKernel.Api.Hosting;
using SharedKernel.DependencyInjection;
using SharedKernel.Domain;

using CampFitFurDogs.Api.Errors;
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

app.UseExceptionHandler(ExceptionHandlingMiddleware.Configure);

app.UseCors();
app.UseHttpsRedirection();
app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy" }))
   .WithName("HealthCheck");

app.Run();
