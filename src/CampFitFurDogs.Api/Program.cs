using CampFitFurDogs.Api.Errors;
using CampFitFurDogs.Api.Hosting;
using CampFitFurDogs.Application;
using CampFitFurDogs.Infrastructure;
using FluentValidation;
using Frank.Api;
using Frank.Api.Hosting;
using Frank.DependencyInjection;

// AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
// {
//     Console.WriteLine("STARTUP EXCEPTION: " + e.Exception.GetType().Name + " - " + e.Exception.Message);
// };

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

// ────────────────────────────────────────────────────────────────
// ⭐ Cookie Authentication (required for AuthCallback tests)
// ────────────────────────────────────────────────────────────────
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "cfd.session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Path = "/";
    });

builder.Services.AddAuthorization();

// ────────────────────────────────────────────────────────────────
// Frank, Application, Infrastructure, API
// ────────────────────────────────────────────────────────────────
builder.Services.AddFrank([
    typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly // request dto validators
]);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Endpoint discovery
var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
EndpointDiscovery.AddEndpoints(apiAssembly);

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// ────────────────────────────────────────────────────────────────
// Middleware pipeline
// ────────────────────────────────────────────────────────────────
app.UseExceptionHandler(ExceptionHandlingMiddleware.Configure);

app.UseCors();
app.UseHttpsRedirection();

// ⭐ Required for cookie auth to work
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy" }))
   .WithName("HealthCheck");

app.Run();
