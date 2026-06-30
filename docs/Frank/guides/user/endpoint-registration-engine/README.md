
# Frank — Guides — User — Endpoint Registration Guide  
*How to use the current Endpoint Registration Engine.*

The Endpoint Registration Engine provides a **governed, deterministic, DI‑driven mechanism** for discovering, instantiating, and mapping API endpoints.  
It removes manual endpoint registration and enables a clean, modular API structure.

This guide explains how to *use* the engine — not how to implement or extend it.

---

# 1. Current Architecture

The modern Endpoint Registration Engine is built on top of the **Registration Engine**, using:

- `IEndpoint` — the contract for endpoint modules  
- `[Registration]` — metadata describing lifetime and constraints  
- `DiscoveryOptions` — rules for selecting endpoint interfaces and implementations  
- `Orchestrator.Orchestrate` — the single entry point for scanning, validation, and DI registration  
- DI activation — endpoints are instantiated through the DI container  
- `MapFrankEndpoints(app)` — maps all registered endpoints

There is **no** `EndpointRegistrationEngine.AddEndpoints`.  
There is **no** `Activator.CreateInstance`.  
There is **no** manual endpoint scanning.

---

# 2. What This Means for You Today

As a user of the Endpoint Registration Engine:

### ✔ You define endpoints by implementing `IEndpoint`
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

### ✔ You mark the interface with `[Registration]`
```csharp
[Registration(ServiceLifetime.Scoped)]
public interface IEndpoint { }
```

### ✔ You register endpoints using the Registration Engine
```csharp
var options = new DiscoveryOptions();

options.IncludeInterface(iface =>
    iface == typeof(IEndpoint));

options.IncludeImplementation(impl =>
    typeof(IEndpoint).IsAssignableFrom(impl));

Orchestrator.Orchestrate(services, assemblies, options);
```

### ✔ You map endpoints using DI
```csharp
app.MapFrankEndpoints();
```

---

# 3. What the Engine Does Today

### **Discovery**
- Finds all classes implementing `IEndpoint`  
- Honors `DiscoveryOptions` filters  
- Ignores abstract classes and open generics  
- Discovery is deterministic

### **Instantiation**
- Endpoints are instantiated via DI  
- Constructor injection is supported  
- Endpoints must be resolvable by DI  
- Endpoints should be stateless

### **Mapping**
- Each endpoint’s `Map` method is invoked  
- Mapping order follows DI resolution order  
- Mapping is deterministic  
- Mapping is idempotent

### **Registration Engine Behavior**
- Min/max constraints are enforced  
- Violations throw `InvalidOperationException`  
- Lifetime is determined by `[Registration]`  
- Concrete type registration occurs when enabled

---

# 4. What the Engine Does *Not* Do Today

- No automatic assembly scanning  
- No grouping or versioning conventions  
- No metadata conventions (OpenAPI, authorization, validation)  
- No diagnostics or logging  
- No endpoint ordering beyond DI order  
- No implicit discovery without `DiscoveryOptions`

These features will arrive in future capability expansions.

---

# 5. How You Use the Engine Today

## 5.1 Define an endpoint

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

## 5.2 Register endpoints during startup

```csharp
var builder = WebApplication.CreateBuilder(args);

var options = new DiscoveryOptions();

options.IncludeInterface(iface => iface == typeof(IEndpoint));
options.IncludeImplementation(impl => typeof(IEndpoint).IsAssignableFrom(impl));

Orchestrator.Orchestrate(builder.Services, assemblies, options);

var app = builder.Build();

app.MapFrankEndpoints();

app.Run();
```

### What you get

- Automatic discovery  
- DI‑based instantiation  
- Automatic mapping  
- Clean Startup  
- Modular endpoint structure  

---

# 6. What the Engine Guarantees

### **Consistency**
All endpoints follow the same registration pattern.

### **Modularity**
Each endpoint is its own class, making APIs easier to maintain.

### **Deterministic Behavior**
- Discovery is explicit  
- Mapping is predictable  
- Endpoints are stateless  

### **Safety**
- No hidden behavior  
- No reflection magic  
- No implicit scanning  

---

# 7. What the Engine Does *Not* Guarantee

### **No automatic persistence**
The engine does not save anything.

### **No automatic grouping or versioning**
You must implement these manually if needed.

### **No metadata conventions**
OpenAPI, authorization, and validation must be added inside the endpoint.

### **No diagnostics**
The engine does not log or report discovery/mapping behavior.

---

# 8. Example: A Modular API Using the Engine

```
/Endpoints
    GetCustomerEndpoint.cs
    CreateCustomerEndpoint.cs
    DeleteCustomerEndpoint.cs
    ListCustomersEndpoint.cs
```

Startup:

```csharp
var options = new DiscoveryOptions();
options.IncludeInterface(iface => iface == typeof(IEndpoint));
options.IncludeImplementation(impl => typeof(IEndpoint).IsAssignableFrom(impl));

Orchestrator.Orchestrate(services, assemblies, options);
app.MapFrankEndpoints();
```

This gives you:

- A clean folder structure  
- A clean Startup  
- Automatic registration  
- A scalable endpoint model  

---

# 9. Summary

**Current State:**  
You use the Endpoint Registration Engine to:

- Define endpoints via `IEndpoint`  
- Govern them via `[Registration]`  
- Discover them via `DiscoveryOptions`  
- Register them via `Orchestrator.Orchestrate`  
- Map them via `MapFrankEndpoints`  
- Keep Startup clean and modular  

**Future Intent:**  
The engine will evolve to support:

- Dependency injection enhancements  
- Grouping and versioning  
- Automatic assembly scanning  
- Metadata conventions  
- Diagnostics and observability  

As a user of this capability:

- Today, you define endpoints and let the engine register them  
- In the future, Frank will automate more of the endpoint lifecycle

