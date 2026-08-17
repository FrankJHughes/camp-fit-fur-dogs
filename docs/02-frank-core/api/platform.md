# Frank.Core.Api — API Platform

The Frank Core API platform acts as the **composition root** for all HTTP‑facing behavior in applications built on the Frank ecosystem. It wires middleware, routing, endpoint discovery, and cross‑cutting concerns into the ASP.NET Core pipeline, ensuring consistent behavior across all products.

This document explains how the API platform is registered, how the pipeline is composed, and how endpoints are discovered and executed.

---

## Service Registration

The API platform is added in `Program.cs`:

```csharp
services.AddFrankCoreApiPlatform(configuration);
```

This extension method registers all platform‑level services, including:

- core logging and observability infrastructure  
- exception handling and global error handlers  
- CORS policy configuration  
- security headers middleware  
- request context and observation services  
- command/query dispatching infrastructure  
- endpoint discovery and routing primitives  

Frank.Core.Api provides the foundation; products add their own services on top.

---

## Pipeline Configuration

After building the application, the platform middleware is applied:

```csharp
app.UseFrankCoreApiPlatform();
```

Middleware is added in a strict, intentional order:

1. **Exception handling** — wraps the entire pipeline  
2. **Request context setup** — establishes immutable request context  
3. **Logging & observability** — correlation ID, timing, structured logs  
4. **CORS headers** — cross‑origin policy enforcement  
5. **Security headers** — OWASP‑recommended hardening  
6. **Authorization** — via `UseFrankIdentityApiPlatform()`  

This ordering ensures predictable behavior across all environments and products.

---

## Endpoint Mapping

Endpoints are automatically discovered and mapped under `/api`:

```csharp
app.MapRegisteredApiEndpoints("/api");
```

This process:

1. Scans all loaded assemblies for types implementing `IEndpoint`  
2. Instantiates each endpoint  
3. Calls its `Map()` method  
4. Groups all routes under the `/api` route group  
5. Applies consistent tagging for documentation and tooling  

Products only need to implement `IEndpoint`; Frank handles the rest.

---

## Dependency Injection

Endpoints resolve their dependencies from the DI container:

```csharp
api.MapPost("/dogs", async (
    RegisterDogEndpointRequest request,
    [FromServices] ICurrentUser currentUser,
    [FromServices] ICommandDispatcher dispatcher) => ...)
```

Key platform services include:

- **[ICommandDispatcher](ca://s?q=Frank_Core_Application_ICommandDispatcher)** — routes commands to handlers  
- **[IQueryDispatcher](ca://s?q=Frank_Core_Application_IQueryDispatcher)** — routes queries to handlers  
- **[ICurrentUser](ca://s?q=Frank_Identity_ICurrentUser)** — identity information for the current request  
- **[IAppUnitOfWork](ca://s?q=Frank_Core_EntityFrameworkCore_UnitOfWork)** — transactional boundaries for persistence  

These abstractions ensure that endpoint logic remains thin and focused on orchestration.

---

## Summary

The Frank Core API platform provides:

- consistent hosting and middleware behavior  
- automatic endpoint discovery  
- structured logging and observability  
- hardened security defaults  
- unified command/query dispatching  
- seamless integration with identity and persistence layers  

Products like CampFitFurDogs build on this foundation to deliver business‑specific API surfaces while inheriting a robust, production‑ready platform.

