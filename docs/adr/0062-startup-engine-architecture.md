# ADR‑0062 — Startup Engine Architecture

## Status  
Accepted

## Context  
Historically, application startup logic lived directly in `Program.cs` and ad‑hoc extension methods. This caused several problems:

- Startup behavior was scattered across layers and modules.
- Ordering of middleware, DI registration, and configuration was implicit and fragile.
- Cross‑cutting concerns (authentication, authorization, CORS, logging, security headers, Swagger, etc.) were wired inconsistently.
- Test hosts were difficult to construct in a deterministic, reusable way.
- Hosting provider configuration (via HostingEngine) was not cleanly separated from application startup.

As the platform grew, it became necessary to:

- Centralize startup orchestration.
- Make startup **modular**, **deterministic**, and **layer‑agnostic**.
- Allow CampFitFurDogs to define startup behavior via **modules**, not engines.
- Integrate cleanly with **HostingEngine**, **Registration Engine**, and **guardrails**.
- Provide a predictable startup lifecycle for both production and test hosts.

A dedicated **StartupEngine** was introduced in Frank to solve this.

---

## Decision  

We introduce a **StartupEngine** in **Frank** that orchestrates application startup using a set of **startup modules** supplied by CampFitFurDogs.

### 1. StartupEngine

StartupEngine coordinates two phases:

- `AddAll(WebApplicationBuilder builder)` — before host build  
- `UseAll(WebApplication app)` — after host build  

Each phase runs across all registered startup modules in a deterministic order.

````csharp
public sealed class StartupEngine
{
    private readonly IReadOnlyList<IStartupModule> _modules;

    public StartupEngine(IReadOnlyList<IStartupModule> modules)
    {
        _modules = modules;
    }

    public void AddAll(WebApplicationBuilder builder)
    {
        foreach (var module in _modules)
            module.Add(builder);
    }

    public void UseAll(WebApplication app)
    {
        foreach (var module in _modules)
            module.Use(app);
    }
}
````

### 2. IStartupModule

A **startup module** encapsulates startup logic for a specific horizontal concern:

- API setup  
- Application setup  
- Authentication  
- Authorization  
- CORS  
- Logging  
- Infrastructure  
- Security headers  
- Swagger  

Each module implements:

````csharp
public interface IStartupModule
{
    void Add(WebApplicationBuilder builder);
    void Use(WebApplication app);
}
````

Modules are:

- deterministic  
- composable  
- focused  
- layer‑agnostic  
- free of business logic  

### 3. Startup Module Construction

CampFitFurDogs constructs its startup modules in a single place, defining the **startup order**:

````csharp
public static IStartupModule[] ConstructStartupModules()
{
    return
    [
        new ApiStartupModule(),
        new ApplicationStartupModule(),
        new AuthenticationStartupModule(),
        new AuthorizationStartupModule(),
        new CorsStartupModule(),
        new ExceptionHandlingStartupModule(),
        new InfrastructureStartupModule(),
        new LoggingStartupModule(),
        new SecurityHeadersStartupModule(),
        new SwaggerStartupModule()
    ];
}
````

### 4. Program.cs Integration

`Program.cs` delegates startup orchestration to StartupEngine:

````csharp
Startup.AddAllServices(builder);

var app = builder.Build();

Startup.UseAllServices(app);
````

Where:

````csharp
public static class Startup
{
    public static void AddAllServices(WebApplicationBuilder builder)
    {
        var modules = ConstructStartupModules();
        var engine = new StartupEngine(modules);

        engine.AddAll(builder);

        builder.Services.AddSingleton(engine);
        builder.Services.AddSingleton<IReadOnlyList<IStartupModule>>(modules);
    }

    public static void UseAllServices(WebApplication app)
    {
        var engine = app.Services.GetRequiredService<StartupEngine>();
        engine.UseAll(app);
    }
}
````

### 5. Interaction with HostingEngine

StartupEngine runs **after** HostingEngine:

- HostingEngine configures environment and hosting provider.
- StartupEngine configures DI, middleware, endpoints, and cross‑cutting pipelines.

Sequence:

````text
HostingEngine → StartupEngine.AddAll → Host Build → StartupEngine.UseAll
````

### 6. Interaction with Registration Engine

StartupEngine does **not** perform DI scanning or registration governance.

- Registration Engine governs DI registration.
- StartupEngine orchestrates module execution.

This keeps responsibilities clean:

- Registration Engine → **what** is registered.  
- StartupEngine → **when/how** startup logic runs.

### 7. Test Harness Integration

The test harness uses StartupEngine to build realistic test hosts:

````text
HostingEngine
StartupEngine.AddAll
Test overrides
Host Build
StartupEngine.UseAll
````

Test overrides always run **after** HostingEngine and StartupEngine, ensuring deterministic behavior.

---

## Consequences  

### Positive

- Centralized, deterministic startup orchestration.
- Clear separation between hosting configuration and application startup.
- Modular startup behavior via `IStartupModule`.
- Consistent wiring of cross‑cutting concerns (auth, CORS, logging, security headers, Swagger).
- Easier construction of test hosts with realistic startup behavior.
- Clean integration with HostingEngine and Registration Engine.
- Reduced complexity in `Program.cs`.

### Negative

- Contributors must understand the startup module model.
- Adding new cross‑cutting concerns requires authoring new modules.
- Startup order must be maintained carefully in the module list.

### Neutral

- StartupEngine does not change business logic; it only orchestrates startup.
- StartupEngine does not dictate hosting provider; HostingEngine remains responsible.

---

## Summary  

StartupEngine is a **Frank‑level** architectural capability that:

- orchestrates application startup via `IStartupModule` implementations,
- runs in two phases (`AddAll` and `UseAll`),
- integrates cleanly with HostingEngine and Registration Engine,
- provides a deterministic, modular, layer‑agnostic startup lifecycle.

CampFitFurDogs contributes **modules**, not engines, and StartupEngine ensures startup remains consistent, testable, and maintainable as the system grows.

