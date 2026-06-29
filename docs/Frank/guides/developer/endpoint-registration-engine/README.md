# Frank — Developer Guide — Endpoint Registration Engine  
*Aligned with the unified discovery + registration system.*

The Endpoint Registration Engine provides a **DI‑driven, convention‑based mechanism** for discovering, registering, and mapping API endpoints.  
It is a first‑class Frank engine, aligned with:

- Unified Registration Engine  
- Startup Engine  
- Module System  
- Command / Query / Event Dispatchers  

This guide documents the **current, correct architecture** and **developer responsibilities**.

---

# 1. Endpoint Contract

Frank defines a single governed endpoint contract:

```csharp
[Registration(ServiceLifetime.Transient)]
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
```

### Invariants

- `IEndpoint` **must** be decorated with `[Registration]`  
- All implementations are discovered and registered via the Registration Engine  
- Endpoints are **transient**  
- Constructor injection is fully supported  

---

# 2. Registration Pipeline (Unified Discovery)

Endpoints are registered via:

```csharp
services.AddFrankEndpoints([
    typeof(SomeEndpoint).Assembly
]);
```

This triggers the unified pipeline:

### 1. DiscoveryOptions

```csharp
options.IncludeInterface(iface =>
    iface.AsType() == typeof(IEndpoint) &&
    iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0);

options.IncludeImplementation(impl =>
    impl.ImplementedInterfaces.Any(i => i == typeof(IEndpoint)));
```

Meaning:

- Only the `IEndpoint` interface (with `[Registration]`) is governed  
- All concrete classes implementing `IEndpoint` are included  

### 2. Scanner

- Finds all non‑abstract endpoint types  
- Matches them to the governed interface  

### 3. Planner

- Builds a registration plan for `IEndpoint`  
- Enforces min/max rules from `[Registration]`  

### 4. Validator

- Ensures at least one endpoint exists (if required)  
- Ensures no violations  

### 5. Registrar

- Registers each endpoint as **transient**  
- Registers concrete types if `RegisterConcreteType = true`  

### 6. DI Container

- Endpoints become resolvable with full constructor injection  

---

# 3. Mapping Pipeline (DI‑Driven)

Mapping is performed through DI:

```csharp
app.MapFrankEndpoints();
```

Implementation:

```csharp
var endpoints = app.ServiceProvider.GetServices<IEndpoint>();

foreach (var endpoint in endpoints)
    endpoint.Map(app);
```

### Mapping invariants

- Endpoints are resolved from DI  
- Mapping order is deterministic (DI enumeration order)  
- Constructor injection is fully supported  
- No static lists, no Activator, no reflection instantiation  

---

# 4. Developer Responsibilities

## 4.1 Implement an endpoint

```csharp
public sealed class GetCustomerEndpoint : IEndpoint
{
    private readonly ICustomerService _service;

    public GetCustomerEndpoint(ICustomerService service)
    {
        _service = service;
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/customers/{id}", async (Guid id) =>
        {
            var customer = await _service.GetAsync(id);
            return Results.Ok(customer);
        });
    }
}
```

## 4.2 Register endpoints

```csharp
services.AddFrankEndpoints([
    typeof(GetCustomerEndpoint).Assembly
]);
```

## 4.3 Map endpoints

```csharp
app.MapFrankEndpoints();
```

---

# 5. Architecture and Invariants

## 5.1 Endpoint Invariants

- Endpoints must implement `IEndpoint`  
- Endpoints must be stateless  
- Endpoints may use constructor injection  
- Endpoints must define routing inside `Map`  

## 5.2 Discovery Invariants

- Discovery is performed by the unified Registration Engine  
- Only `IEndpoint` (with `[Registration]`) is governed  
- Only concrete classes implementing `IEndpoint` are included  
- Discovery is assembly‑based and explicit  

## 5.3 Mapping Invariants

- Endpoints are resolved from DI  
- Mapping is deterministic  
- Mapping is performed once per endpoint instance  
- Mapping is synchronous  

---

# 6. Example (Full Flow)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFrankEndpoints([
    typeof(GetCustomerEndpoint).Assembly
]);

var app = builder.Build();

app.MapFrankEndpoints();

app.Run();
```

---

# 7. Summary

The Endpoint Registration Engine provides:

- unified discovery + registration  
- DI‑driven endpoint activation  
- constructor injection support  
- deterministic mapping  
- clean, modular endpoint definitions  

As a developer:

- You implement `IEndpoint`  
- You register assemblies via `AddFrankEndpoints()`  
- You map endpoints via `MapFrankEndpoints()`  

Frank handles discovery, DI registration, and mapping.

This guide reflects the **actual, current, unified architecture**.
