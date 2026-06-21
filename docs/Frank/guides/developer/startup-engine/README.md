# Startup Engine — Developer Guide

The Startup Engine provides a **modular, ordered, deterministic startup pipeline** for ASP.NET Core applications.  
It enables developers to break application startup into small, focused modules that run in a predictable order.

This guide documents the architecture, contracts, invariants, and extension points for developers implementing or integrating with the Startup Engine.

---

## 1. Core Abstractions

### 1.1 IStartupModule

````csharp
public interface IStartupModule
{
    void Add(WebApplicationBuilder builder);
    void Use(WebApplication app);
}
````

### The Add Phase (Critical Invariant)

`Add(WebApplicationBuilder builder)` runs **before the DI container is built**.

This means:

- **Dependency Injection is NOT available in this phase.**
- You cannot resolve services.
- You cannot request scoped or singleton instances.
- You cannot use `IServiceProvider` (it does not exist yet).
- You cannot use `ActivatorUtilities` to resolve dependencies.
- You must not call `BuildServiceProvider()` — this creates a *second* container and breaks the app.

The Add phase is strictly for:

- registering services (`builder.Services.AddXyz()`)
- adding configuration sources
- adding logging providers
- adding options
- adding hosted services
- configuring authentication/authorization schemes
- configuring CORS, rate limiting, etc.

**Only service registration is allowed.  
No service resolution is allowed.**

### The Use Phase

`Use(WebApplication app)` runs **after the DI container is built**, so:

- DI is fully available
- Middleware can be added
- Endpoints can be mapped
- Services can be resolved safely

This is the correct place for:

- `app.UseXyz()`
- `app.MapXyz()`
- resolving services from DI
- configuring middleware pipelines

---

### 1.2 StartupModuleAttribute

````csharp
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StartupModuleAttribute : Attribute
{
    public int Order { get; }
    public StartupModuleAttribute(int order) => Order = order;
}
````

#### Purpose

Assigns deterministic ordering to startup modules.

#### Ordering Rules

- Lower `Order` runs first  
- Modules without the attribute default to `Order = 1000`  
- Ordering applies to both `Add()` and `Use()`  
- Ordering is stable and deterministic  

---

## 2. Startup Engine Runtime

### 2.1 StartupEngine

````csharp
public sealed class StartupEngine
{
    private readonly IReadOnlyList<IStartupModule> _modules;

    public StartupEngine(IEnumerable<IStartupModule> modules)
    {
        _modules = modules
            .OrderBy(m => m.GetType()
                .GetCustomAttribute<StartupModuleAttribute>()?.Order ?? 1000)
            .ToArray();
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

### Responsibilities

- Sort modules by `StartupModuleAttribute.Order`
- Execute all `Add()` methods in order
- Execute all `Use()` methods in order

### Execution Model

1. **Construction Phase**  
   - Modules are injected via DI  
   - Modules are sorted deterministically  

2. **Add Phase**  
   - Runs before the app is built  
   - Modules configure services and hosting  
   - **DI is NOT available**

3. **Use Phase**  
   - Runs after the app is built  
   - Modules configure middleware and endpoints  
   - **DI is fully available**

### Guarantees

- Ordering is deterministic  
- All modules run unless they throw  
- Exceptions are not swallowed  
- No module skipping or conditional logic (modules must implement their own conditions if needed)  

---

## 3. DI Registration

````csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupEngine(this IServiceCollection services)
    {
        services.AddSingleton<StartupEngine>();
        return services;
    }
}
````

#### Purpose

- Registers the StartupEngine as a singleton  
- Modules themselves must be registered separately (typically via AutoRegistration)  

---

## 4. Implementing a Startup Module

A typical module looks like this:

````csharp
[StartupModule(200)]
public sealed class SecurityHeadersModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddSecurityHeaders();
    }

    public void Use(WebApplication app)
    {
        app.UseSecurityHeaders();
    }
}
````

### Guidelines

- Keep modules small and focused  
- Use ordering to enforce dependencies  
- Avoid heavy logic in `Add()` or `Use()`  
- Avoid environment checks unless necessary  
- Avoid side effects  

---

## 5. Invariants

Developers must ensure:

### Add Phase Invariants

- **No service resolution occurs inside `Add()`**  
  The DI container does not exist yet.

- Only service *registration* is allowed.

- Never call `BuildServiceProvider()` inside `Add()`.

### Use Phase Invariants

- DI resolution is allowed only in `Use()`.
- Middleware and endpoints must be configured only in `Use()`.

### General Invariants

- `Add()` and `Use()` must be deterministic and idempotent  
- Modules must not assume ordering unless explicitly declared  
- Modules must not depend on other modules unless ordering guarantees it  

---

## 6. Anti‑Patterns

Avoid:

- **Resolving services inside `Add()`**  
  Example of what NOT to do:

  ````csharp
  // ❌ INVALID — DI container not built yet
  var myService = builder.Services.BuildServiceProvider().GetRequiredService<IMyService>();
  ````

- Using `BuildServiceProvider()` inside `Add()`  
- Adding middleware inside `Add()`  
- Registering endpoints inside `Add()`  
- Using static/global state inside modules  
- Relying on DI registration order instead of `[StartupModule(order)]`  
- Creating modules that depend on each other without ordering guarantees  

---

## 7. Summary

The Startup Engine enforces a strict separation of responsibilities:

- **Add() = service registration only**  
  No DI resolution. No middleware. No endpoints.

- **Use() = middleware + endpoints + runtime behavior**  
  DI is fully available.

This separation is essential for correctness, testability, and alignment with ASP.NET Core’s hosting model.

The Startup Engine provides:

- **Modular startup behavior**  
- **Deterministic ordering**  
- **Separation of builder‑phase and app‑phase logic**  
- **A clean, extensible startup pipeline**  

As a developer:

- You implement `IStartupModule`  
- You optionally assign an order  
- You register modules in DI  
- The Startup Engine handles the rest  
