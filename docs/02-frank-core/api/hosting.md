# Frank.Core.Api — Hosting

Hosting in Frank Core provides the ASP.NET Core startup pipeline, environment‑aware configuration, platform composition, and endpoint discovery used by all products. It defines how an application boots, how middleware is applied, and how the API surface is assembled at runtime.

Frank’s hosting layer ensures that every product starts consistently, regardless of environment or deployment target.

---

## Program.cs Structure

The main entry point composes the platform in a predictable order:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add platform layers in sequence
builder.Services
    .AddCampFitFurDogsApiPlatform(builder.Configuration)
    .AddFrankCoreApiPlatform(builder.Configuration)
    .AddFrankIdentityApiPlatform(builder.Configuration);

// Adapt configuration to hosting environment
await Hosting.AdaptToHostingEnvironment(builder);

var app = builder.Build();

// Apply middleware in order
app
    .UseFrankCoreApiPlatform()
    .UseFrankIdentityApiPlatform();

// Discover and register endpoints
app.MapRegisteredApiEndpoints("/api");

await app.RunAsync();
```

Frank Core ensures that:

- services are registered in the correct order  
- middleware is applied consistently  
- endpoints are discovered automatically  
- environment‑specific configuration is applied before the app is built  

---

## Platform Composition Order

1. **CampFitFurDogs.Api**  
   Product‑specific services, endpoints, and configuration.

2. **Frank.Core.Api**  
   Platform middleware, routing, logging, CORS, exception handling.

3. **Frank.Identity.Api**  
   Authentication, authorization, identity primitives, OIDC integration.

This order ensures that:

- the product sits *on top* of the platform  
- the platform sits *on top* of identity  
- dependencies flow downward, never upward  

---

## Hosting Modules

Frank supports environment‑specific configuration via `IHostingModule`.

```csharp
public interface IHostingModule
{
    Task ApplyAsync(WebApplicationBuilder builder);
}

public sealed class RenderPrPreviewHostingModule : IHostingModule
{
    public async Task ApplyAsync(WebApplicationBuilder builder)
    {
        // PR preview specific configuration
        // e.g., in-memory DB, mock email, relaxed CORS
    }
}
```

Modules are discovered and applied automatically:

```csharp
public static class Hosting
{
    public static async Task AdaptToHostingEnvironment(WebApplicationBuilder builder)
    {
        var modules = ConstructHostingModules();
        foreach (var module in modules)
        {
            await module.ApplyAsync(builder);
        }
    }
}
```

This allows:

- PR preview environments  
- staging environments  
- production hardening  
- local development overrides  

without modifying Program.cs.

---

## Middleware Registration

Frank applies middleware in a strict, intentional order:

```csharp
public static IApplicationBuilder UseFrankCoreApiPlatform(this IApplicationBuilder app)
{
    return app
        .UseRouting()
        .UseCors("FrankCore")
        .UseSecurityHeaders()
        .UseInboundObservationContext()
        .UseExceptionHandling()
        .UseOutboundObservationContext();
}
```

### Why the order matters

- **Routing first** — establishes route context for all downstream middleware  
- **CORS early** — ensures cross‑origin checks happen before auth  
- **Security headers** — applied before the response is written  
- **Exception handling** — wraps the entire pipeline  
- **Observation middleware** — tracks inbound/outbound request lifecycle  

This ordering is part of Frank’s platform contract.

---

## Dependency Injection

Frank registers services in layered extension methods:

```csharp
public static IServiceCollection AddFrankCoreApiPlatform(
    this IServiceCollection services,
    IConfiguration configuration)
{
    return services
        .AddFrankCoreApiPlatformCors(configuration)
        .AddFrankCoreApiPlatformLogging()
        .AddFrankCoreApiPlatformSwagger()
        .AddFrankCoreApplication()
        .AddFrankCoreInfrastructure(configuration)
        .AddFrankCoreApiMiddleware();
}
```

Each extension:

- registers only its own services  
- avoids cross‑layer leakage  
- keeps concerns isolated and composable  

This makes the platform predictable and easy to extend.

---

## Endpoint Discovery

Frank automatically discovers endpoints via reflection:

```csharp
public static WebApplication MapRegisteredApiEndpoints(
    this WebApplication app,
    string basePath = "/api")
{
    var endpointTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => typeof(IEndpoint).IsAssignableFrom(p) && !p.IsInterface);

    foreach (var endpointType in endpointTypes)
    {
        var endpoint = (IEndpoint)Activator.CreateInstance(endpointType)!;
        var group = app.MapGroup(basePath);
        endpoint.Map(group);
    }

    return app;
}
```

This eliminates:

- manual route registration  
- duplicated routing logic  
- forgotten endpoints  

Every `IEndpoint` implementation is automatically mapped.

---

## Configuration Loading

Configuration is layered:

1. `appsettings.json` — defaults  
2. `appsettings.{Environment}.json` — environment overrides  
3. **Environment variables** — runtime overrides  

Example:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=...
Identity__Oidc__Authority=...
Identity__Oidc__ClientSecret=...
```

Frank ensures consistent configuration behavior across all products.

---

## Logging Setup

Frank configures structured JSON logging:

```csharp
public static ILoggingBuilder AddFrankCoreApiPlatformLogging(this ILoggingBuilder logging)
{
    return logging
        .ClearProviders()
        .AddConsole()
        .AddJsonConsole(options =>
        {
            options.IncludeScopes = true;
            options.UseUtcTimestamp = true;
        });
}
```

This ensures logs are:

- structured  
- searchable  
- timestamped in UTC  
- compatible with cloud log aggregation  

---

## HTTPS and Security

### Production

- HTTPS required  
- HSTS enabled  
- strict security headers  
- restrictive CORS  

### Development

- HTTPS optional  
- permissive CORS  
- verbose logging  

Frank adapts automatically based on environment.

---

## Health Checks

Frank exposes health endpoints:

```csharp
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

Used by:

- Kubernetes  
- Docker health probes  
- load balancers  

---

## Graceful Shutdown

Frank handles shutdown signals:

```csharp
app.Lifetime.ApplicationStopping.Register(async () =>
{
    Logger.LogInformation("Application shutting down");
    // Drain in-flight requests
    // Close database connections
    // Flush logs and traces
});
```

This ensures:

- no dropped requests  
- clean database disconnects  
- complete log flushing  

---
