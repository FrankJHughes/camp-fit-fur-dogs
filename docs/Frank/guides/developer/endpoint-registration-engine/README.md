# Frank Endpoint Registration Engine — Developer Guide

This guide documents the **current state** of the Frank Endpoint Registration Engine and the **intended future state** as the platform evolves.

The Endpoint Registration Engine provides a **convention‑based mechanism** for discovering, instantiating, and mapping API endpoints without requiring manual registration.  
It is a first‑class engine in the Frank architecture, aligned with the Startup Engine, Registration Engine, and Domain Event Dispatcher.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides the following components:

```csharp
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
```

```csharp
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
```

```csharp
public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        EndpointRegistrationEngine.MapEndpoints(app);
        return app;
    }
}
```

### What exists today

- A simple **contract** (`IEndpoint`) for defining endpoint modules  
- A **discovery mechanism** that scans assemblies for endpoint types  
- A **registration engine** that instantiates and maps endpoints  
- A **fluent extension** (`app.MapEndpoints()`) for convenience  
- A **modular endpoint model** (each endpoint is its own class)  
- No DI required for endpoints (constructed via `Activator.CreateInstance`)  
- Deterministic, convention‑based endpoint registration  

### What does *not* exist today

- No dependency injection support for endpoints  
- No constructor injection  
- No endpoint ordering  
- No grouping or versioning conventions  
- No filtering by namespace or attribute  
- No automatic assembly scanning  
- No startup module integration  
- No diagnostics or logging  
- No endpoint metadata conventions  

### Developer implications today

- Endpoints must have **parameterless constructors**  
- Endpoints must implement `IEndpoint`  
- Developers must call `EndpointRegistrationEngine.AddEndpoints(assembly)` during startup  
- Developers must call `app.MapEndpoints()` to map discovered endpoints  
- All endpoint logic must be inside the `Map` method  
- Endpoints must be stateless  

The current capability is intentionally minimal and focused on convention‑based discovery.

---

# 2. Future Intent (After Capability Expansion)

As the platform evolves, the Endpoint Registration Engine will expand to support richer API composition and engine‑level conventions.

### 2.1 Dependency Injection Support

Future enhancements may include:

- Constructor injection for endpoints  
- Scoped and transient endpoint instances  
- DI‑aware endpoint activation  

### 2.2 Endpoint Grouping and Versioning

Potential improvements:

- Automatic grouping by namespace  
- Versioning conventions  
- Route prefix conventions  
- Tag conventions  

### 2.3 Assembly Scanning Enhancements

Future support may include:

- Automatic scanning of all loaded assemblies  
- Attribute‑based filtering  
- Module‑based endpoint registration  

### 2.4 Metadata and Conventions

Potential additions:

- Automatic OpenAPI metadata  
- Automatic authorization conventions  
- Automatic validation conventions  
- Automatic logging conventions  

### 2.5 Diagnostics and Observability

Future capabilities may include:

- Endpoint discovery diagnostics  
- Mapping logs  
- Endpoint registration metrics  

These enhancements will transform the subsystem from a simple discovery helper into a full‑featured **Endpoint Registration Engine**.

---

# 3. Developer Responsibilities (Current vs Future)

## Current Responsibilities

Developers must:

- Implement `IEndpoint` for each endpoint  
- Provide a parameterless constructor  
- Implement the `Map` method  
- Call `EndpointRegistrationEngine.AddEndpoints(assembly)` during startup  
- Call `app.MapEndpoints()` to map endpoints  
- Ensure endpoints do not require DI  
- Ensure endpoint mapping is idempotent  
- Keep endpoints stateless  

## Future Responsibilities

Once the capability expands, developers will:

- Use constructor injection in endpoints  
- Rely on automatic assembly scanning  
- Use grouping and versioning conventions  
- Use metadata conventions for OpenAPI and authorization  
- Use diagnostics to validate endpoint registration  
- Follow engine‑level conventions for endpoint structure  

---

# 4. Architecture and Invariants

### 4.1 Endpoint Invariants

- Endpoints must implement `IEndpoint`  
- Endpoints must have a parameterless constructor  
- Endpoints must define all routing inside `Map`  
- Endpoints must be stateless (no instance fields)  

### 4.2 Discovery Invariants

- Only non‑abstract types implementing `IEndpoint` are discovered  
- Discovery is explicit (developer calls `AddEndpoints`)  
- Discovery is assembly‑based  
- Discovery is cached in a thread‑safe dictionary  

### 4.3 Mapping Invariants

- Endpoints are instantiated via `Activator.CreateInstance`  
- Mapping is performed once per endpoint type  
- Mapping order is not guaranteed  
- Mapping is synchronous  

---

# 5. Example Usage (Developer Perspective)

### 5.1 Define an endpoint

```csharp
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
```

### 5.2 Register endpoints during startup

```csharp
var builder = WebApplication.CreateBuilder(args);

EndpointRegistrationEngine.AddEndpoints(typeof(GetCustomerEndpoint).Assembly);

var app = builder.Build();

app.MapEndpoints();

app.Run();
```

This demonstrates:

- Defining an endpoint  
- Discovering endpoints  
- Mapping endpoints  
- Running the application  

---

# 6. Summary

**Current State:**  
The Endpoint Registration Engine provides:

- A simple contract (`IEndpoint`)  
- Assembly‑based endpoint discovery  
- Convention‑based endpoint registration  
- A clean, modular endpoint model  

**Future Intent:**  
The engine will evolve to support:

- Dependency injection  
- Grouping and versioning  
- Automatic assembly scanning  
- Metadata conventions  
- Diagnostics and observability  

As a developer:

- Today, you implement `IEndpoint` and rely on discovery  
- In the future, Frank will automate more of the endpoint lifecycle  

