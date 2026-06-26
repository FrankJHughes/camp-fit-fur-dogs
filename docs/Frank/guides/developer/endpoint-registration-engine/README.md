# Frank — Developer Guide — Endpoint Registration Engine  
*Current state and future direction of Frank’s endpoint discovery and mapping engine.*

The Endpoint Registration Engine provides a **convention‑based mechanism** for discovering, instantiating, and mapping API endpoints without requiring manual registration.  
It is a first‑class engine in the Frank architecture, aligned with:

- StartupEngine  
- RegistrationEngine  
- Domain Event Dispatcher  
- HostingEngine  

This guide documents the **current state** and the **intended future state** of the engine.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides the following components:

````csharp
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
````

````csharp
public static class EndpointRegistrationEngine
{
    private static readonly ConcurrentDictionary<Type, byte> _endpointTypes = new();

    public static void AddEndpoints(Assembly assembly)
    {
        var endpointTypes = assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes)
            _endpointTypes.TryAdd(type, 0);
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        foreach (var endpointType in _endpointTypes.Keys)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(endpointType)!;
            endpoint.Map(app);
        }
    }
}
````

````csharp
public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        EndpointRegistrationEngine.MapEndpoints(app);
        return app;
    }
}
````

---

## What exists today

- A simple **contract** (`IEndpoint`) for defining endpoint modules  
- A **discovery mechanism** that scans assemblies for endpoint types  
- A **registration engine** that instantiates and maps endpoints  
- A **fluent extension** (`app.MapEndpoints()`)  
- A **modular endpoint model** (one class per endpoint)  
- No DI required (constructed via `Activator.CreateInstance`)  
- Deterministic, convention‑based endpoint registration  

---

## What does *not* exist today

- no dependency injection support  
- no constructor injection  
- no endpoint ordering  
- no grouping or versioning conventions  
- no namespace/attribute filtering  
- no automatic assembly scanning  
- no StartupEngine integration  
- no diagnostics or logging  
- no metadata conventions (OpenAPI, authorization, validation)  

---

## Developer implications today

- endpoints must have **parameterless constructors**  
- endpoints must implement `IEndpoint`  
- developers must call `EndpointRegistrationEngine.AddEndpoints(assembly)`  
- developers must call `app.MapEndpoints()`  
- all routing logic must be inside `Map`  
- endpoints must be **stateless**  

The current capability is intentionally minimal and convention‑driven.

---

# 2. Future Intent (After Capability Expansion)

As the platform evolves, the Endpoint Registration Engine will expand into a full Frank engine with richer conventions and integration points.

---

## 2.1 Dependency Injection Support

Future enhancements may include:

- constructor injection for endpoints  
- scoped and transient endpoint instances  
- DI‑aware endpoint activation  

---

## 2.2 Endpoint Grouping and Versioning

Potential improvements:

- automatic grouping by namespace  
- versioning conventions  
- route prefix conventions  
- tag conventions  

---

## 2.3 Assembly Scanning Enhancements

Future support may include:

- automatic scanning of all loaded assemblies  
- attribute‑based filtering  
- module‑based endpoint registration  

---

## 2.4 Metadata and Conventions

Potential additions:

- automatic OpenAPI metadata  
- automatic authorization conventions  
- automatic validation conventions  
- automatic logging conventions  

---

## 2.5 Diagnostics and Observability

Future capabilities may include:

- endpoint discovery diagnostics  
- mapping logs  
- endpoint registration metrics  

These enhancements will transform the subsystem into a full‑featured **Endpoint Registration Engine** aligned with other Frank engines.

---

# 3. Developer Responsibilities (Current vs Future)

## Current Responsibilities

Developers must:

- implement `IEndpoint`  
- provide a parameterless constructor  
- implement the `Map` method  
- call `AddEndpoints(assembly)`  
- call `app.MapEndpoints()`  
- ensure endpoints do not require DI  
- ensure mapping is idempotent  
- keep endpoints stateless  

---

## Future Responsibilities

Once the capability expands, developers will:

- use constructor injection  
- rely on automatic assembly scanning  
- use grouping and versioning conventions  
- use metadata conventions (OpenAPI, authorization, validation)  
- use diagnostics to validate endpoint registration  
- follow engine‑level conventions for endpoint structure  

---

# 4. Architecture and Invariants

## 4.1 Endpoint Invariants

- endpoints must implement `IEndpoint`  
- endpoints must have a parameterless constructor  
- endpoints must define all routing inside `Map`  
- endpoints must be stateless  

---

## 4.2 Discovery Invariants

- only non‑abstract types implementing `IEndpoint` are discovered  
- discovery is explicit (developer calls `AddEndpoints`)  
- discovery is assembly‑based  
- discovery is cached in a thread‑safe dictionary  

---

## 4.3 Mapping Invariants

- endpoints are instantiated via `Activator.CreateInstance`  
- mapping is performed once per endpoint type  
- mapping order is not guaranteed  
- mapping is synchronous  

---

# 5. Example Usage (Developer Perspective)

## 5.1 Define an endpoint

````csharp
public sealed class GetCustomerEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/customers/{id}", (Guid id) =>
        {
            return Results.Ok(new { Id = id });
        });
    }
}
````

## 5.2 Register endpoints during startup

````csharp
var builder = WebApplication.CreateBuilder(args);

EndpointRegistrationEngine.AddEndpoints(typeof(GetCustomerEndpoint).Assembly);

var app = builder.Build();

app.MapEndpoints();

app.Run();
````

This demonstrates:

- defining an endpoint  
- discovering endpoints  
- mapping endpoints  
- running the application  

---

# 6. Summary

**Current State:**  
The Endpoint Registration Engine provides:

- a simple contract (`IEndpoint`)  
- assembly‑based endpoint discovery  
- convention‑based endpoint registration  
- a clean, modular endpoint model  

**Future Intent:**  
The engine will evolve to support:

- dependency injection  
- grouping and versioning  
- automatic assembly scanning  
- metadata conventions  
- diagnostics and observability  

As a developer:

- today, you implement `IEndpoint` and rely on discovery  
- in the future, Frank will automate more of the endpoint lifecycle  
