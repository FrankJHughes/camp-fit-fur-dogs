# Endpoints

The **Endpoints** folder contains abstractions that define how HTTP endpoints are mapped into the ASP.NET Core routing pipeline. Endpoints represent the application’s public surface area — the routes, handlers, and configurations that expose functionality to clients.

This folder provides the foundational interface used to register endpoints in a modular, discoverable, and composable way.

---

## Purpose

Endpoints model *how the application is exposed over HTTP*. They are:

- **modular** — each endpoint encapsulates its own routing and configuration  
- **discoverable** — the application can locate all endpoints automatically  
- **explicit** — each endpoint defines exactly what it maps  
- **composable** — endpoints can be grouped, versioned, or layered  

By centralizing endpoint mapping behind a simple abstraction, the application gains:

- consistent routing behavior  
- predictable startup configuration  
- clean separation between routing and business logic  
- improved testability and maintainability  

---

## Components

### IEndpoint
Represents a single HTTP endpoint (or a group of related endpoints) that can be mapped into the ASP.NET Core routing pipeline.

```csharp
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
```

Each endpoint:

- defines its own routes  
- configures metadata (authorization, validation, filters, etc.)  
- attaches handlers or delegates  
- integrates cleanly with vertical slices  

Endpoints are automatically registered via the `Registration` attribute with:

- **singleton lifetime**  
- **concrete type registration**  

This ensures all endpoints are discovered and mapped during application startup.

---

## Design Principles

- **Separation of concerns**  
  Routing is defined in endpoints; business logic lives in commands, queries, or handlers.

- **Modularity**  
  Each endpoint is self‑contained and easy to reason about.

- **Automatic registration**  
  Endpoints are discovered through the `Registration` subsystem.

- **Consistency**  
  All endpoints follow the same mapping pattern, improving readability and maintainability.

- **Vertical slice alignment**  
  Endpoints naturally pair with command/query handlers, domain events, and slice‑specific logic.

---

## How Endpoints Fit Into the Application

Endpoints form the outermost layer of the application:

- They expose HTTP routes.  
- They delegate work to commands, queries, or domain services.  
- They integrate with middleware, filters, and pipeline behaviors.  
- They provide a clean entry point into vertical slices.  

This structure keeps the application’s HTTP surface area organized, explicit, and easy to evolve.

---
