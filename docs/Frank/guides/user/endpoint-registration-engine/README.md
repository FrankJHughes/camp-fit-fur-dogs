# Frank — Guides — User — Endpoint Registration Engine

This guide explains how users of the Frank platform should work with the **Endpoint Registration Engine** today, and how the experience will evolve in the future.

The Endpoint Registration Engine provides a **convention‑based mechanism** for discovering, instantiating, and mapping API endpoints.  
It removes the need for manual endpoint registration and enables a clean, modular API structure.

This guide focuses on how to *use* the engine — not how to implement or extend it.

---

# 1. Current State (Before Future Enhancements)

The Endpoint Registration Engine currently provides:

- A simple contract for defining endpoints: `IEndpoint`
- A discovery mechanism that scans assemblies for endpoint types
- A registration engine that instantiates and maps endpoints
- A convenience extension method: `app.MapEndpoints()`

### What this means for you today

As a user of this capability:

- You define endpoints by implementing `IEndpoint`
- You place all routing logic inside the `Map` method
- You ensure endpoints have parameterless constructors
- You call `EndpointRegistrationEngine.AddEndpoints(assembly)` during startup
- You call `app.MapEndpoints()` to register all discovered endpoints

### What the engine does today

- Discovers endpoint classes automatically  
- Instantiates each endpoint  
- Calls `Map` on each endpoint to register routes  
- Keeps Startup clean and declarative  
- Supports modular API design (one endpoint per class)

### What the engine does *not* do today

- No dependency injection for endpoints  
- No constructor injection  
- No endpoint ordering  
- No grouping or versioning conventions  
- No automatic assembly scanning  
- No metadata conventions (OpenAPI, authorization, validation)  
- No diagnostics or logging  

The current capability is intentionally minimal and predictable.

---

# 2. Future Intent (After Capability Expansion)

As the platform evolves, the Endpoint Registration Engine will become more powerful and more automated.

### **2.1 Dependency Injection Support**
- Constructor injection for endpoints  
- Scoped and transient endpoint lifetimes  
- DI‑aware activation  

### **2.2 Endpoint Grouping and Versioning**
- Namespace‑based grouping  
- Versioning conventions  
- Route prefix conventions  
- Tag conventions  

### **2.3 Automatic Assembly Scanning**
- Scan all loaded assemblies  
- Attribute‑based filtering  
- Module‑based endpoint registration  

### **2.4 Metadata and Conventions**
- Automatic OpenAPI metadata  
- Automatic authorization conventions  
- Automatic validation conventions  
- Automatic logging conventions  

### **2.5 Diagnostics and Observability**
- Discovery diagnostics  
- Mapping logs  
- Endpoint registration metrics  

These enhancements will transform the subsystem into a full‑featured **Endpoint Registration Engine** aligned with the rest of Frank’s engine architecture.

---

# 3. How You Use the Engine Today

## 3.1 Define an endpoint

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

## 3.2 Register endpoints during startup

```csharp
var builder = WebApplication.CreateBuilder(args);

EndpointRegistrationEngine.AddEndpoints(typeof(GetCustomerEndpoint).Assembly);

var app = builder.Build();

app.MapEndpoints();

app.Run();
```

### What you get

- Automatic discovery  
- Automatic mapping  
- Clean Startup  
- Modular endpoint structure  

---

# 4. What the Engine Guarantees

### **Consistency**
All endpoints follow the same registration pattern.

### **Modularity**
Each endpoint is its own class, making APIs easier to maintain.

### **Deterministic Behavior**
- Discovery is explicit  
- Mapping is predictable  
- Endpoints are stateless  

### **Safety**
- Domain events are not involved  
- No DI complexity  
- No hidden behavior  

---

# 5. What the Engine Does *Not* Guarantee

### **No automatic persistence**
The engine does not save anything.

### **No DI activation**
Endpoints cannot receive services via constructor injection yet.

### **No ordering**
Endpoints may map in any order.

### **No grouping or versioning**
You must implement these manually if needed.

### **No metadata conventions**
OpenAPI, authorization, and validation must be added inside the endpoint.

### **No diagnostics**
The engine does not log or report discovery/mapping behavior.

---

# 6. Example: A Modular API Using the Engine

```
/Endpoints
    GetCustomerEndpoint.cs
    CreateCustomerEndpoint.cs
    DeleteCustomerEndpoint.cs
    ListCustomersEndpoint.cs
```

Startup:

```csharp
EndpointRegistrationEngine.AddEndpoints(typeof(GetCustomerEndpoint).Assembly);
app.MapEndpoints();
```

This gives you:

- A clean folder structure  
- A clean Startup  
- Automatic registration  
- A scalable endpoint model  

---

# 7. Summary

**Current State:**  
You use the Endpoint Registration Engine to:

- Define endpoints via `IEndpoint`  
- Discover endpoints via `AddEndpoints`  
- Register endpoints via `MapEndpoints`  
- Keep Startup clean and modular  

**Future Intent:**  
The engine will evolve to support:

- Dependency injection  
- Grouping and versioning  
- Automatic assembly scanning  
- Metadata conventions  
- Diagnostics and observability  

As a user of this capability:

- Today, you define endpoints and let the engine register them  
- In the future, Frank will automate more of the endpoint lifecycle
