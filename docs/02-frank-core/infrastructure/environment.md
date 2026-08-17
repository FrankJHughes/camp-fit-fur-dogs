# Frank.Core.Infrastructure — Environment

The `Environment` service provides a simple, testable abstraction for detecting the current deployment environment. It wraps ASP.NET Core’s `IHostEnvironment` and exposes a clean interface that can be used throughout the platform without tying domain or application logic to ASP.NET Core directly.

This document maps the Environment subsystem under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure/Environment
```

---

## Purpose

The Environment subsystem exists to:

- provide a consistent abstraction for environment detection  
- eliminate direct dependencies on `IHostEnvironment` outside infrastructure  
- enable environment‑specific behavior (logging, debugging, configuration)  
- support custom environment names beyond the ASP.NET Core defaults  
- keep domain and application layers environment‑agnostic  

It is the foundation for runtime environment awareness across the Frank platform.

---

## Service Interface

```csharp
public interface IEnvironment
{
    string Name { get; }
    bool IsProduction { get; }
    bool IsDevelopment { get; }
    bool IsEnvironment(string environmentName);
}
```

Key characteristics:

- exposes the environment name  
- provides convenience checks for common environments  
- supports custom environment names  
- avoids leaking ASP.NET Core types into higher layers  

---

## Implementation

ASP.NET Core provides the underlying environment detection:

```csharp
public sealed class AspNetEnvironment : IEnvironment
{
    private readonly IHostEnvironment _hostEnvironment;

    public AspNetEnvironment(IHostEnvironment hostEnvironment)
        => _hostEnvironment = hostEnvironment;

    public string Name => _hostEnvironment.EnvironmentName;
    public bool IsProduction => _hostEnvironment.IsProduction();
    public bool IsDevelopment => _hostEnvironment.IsDevelopment();
    public bool IsEnvironment(string environmentName)
        => _hostEnvironment.IsEnvironment(environmentName);
}
```

This implementation is registered in DI and used throughout the platform.

---

## Usage

Environment‑specific behavior is implemented by injecting `IEnvironment`:

```csharp
public static IServiceCollection AddFrankCoreInfrastructure(
    IServiceCollection services,
    IEnvironment environment)
{
    if (environment.IsProduction)
    {
        // Stricter logging, no debug endpoints
        services.AddScoped<ILoggingConfiguration, ProductionLoggingConfig>();
    }
    else
    {
        // Relaxed logging, allow debug endpoints
        services.AddScoped<ILoggingConfiguration, DevelopmentLoggingConfig>();
    }

    return services;
}
```

This keeps environment logic centralized and testable.

---

## Environment Names

Standard ASP.NET Core values:

- `Development` — local development  
- `Testing` — integration tests  
- `Production` — production deployment  

Custom environments used in Frank deployments:

- `RenderPrPreview` — Render.com PR preview  
- `Staging` — pre‑production validation  

Environment is set via the `ASPNETCORE_ENVIRONMENT` variable.

---

## How Environment Connects to the Broader Platform

The Environment service collaborates with:

- **Frank.Core.Infrastructure**  
  Logging, configuration, and hosting depend on environment detection.

- **Frank.Core.Application**  
  Handlers may enable or disable features based on environment.

- **Frank.Core.Api**  
  Middleware and endpoints adjust behavior depending on environment.

- **Frank.Core.EntityFrameworkCore**  
  Connection strings and migration strategies may vary by environment.

Environment detection is a cross‑cutting concern that influences every vertical slice.

---

## Runtime Collaboration Points

Environment interacts with the runtime by:

- controlling logging verbosity  
- enabling or disabling debug endpoints  
- selecting configuration sources  
- determining error‑handling behavior  
- shaping hosting and startup logic  

It is read once at startup and then injected throughout the platform.

---

## Composition Flow (API → Application → Domain → Infrastructure)

```
ASPNETCORE_ENVIRONMENT
    ↓
Infrastructure Environment Service
    ↓
Application Services
    ↓
Domain Logic (receives environment‑specific configuration)
    ↓
Hosting / Logging / Persistence
```

Environment awareness influences runtime behavior without polluting domain or application code.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure environment implementation.  
Whenever environment detection, hosting patterns, or configuration flows evolve, update this section to reflect the current platform architecture.
