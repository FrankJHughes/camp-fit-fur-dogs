# Frank.Core.Api — Routing

The routing subsystem in Frank.Core.Api defines how HTTP endpoints are discovered, grouped, and mapped into the ASP.NET Core pipeline. It provides a consistent, platform‑level mechanism for assembling API surfaces across all products, including CampFitFurDogs.

This document describes the responsibilities of the routing subsystem and how it aligns with the implementation under `src/Frank/Core`.

---

## Purpose

The routing subsystem exists to:

- provide a unified routing model for all Frank‑based applications  
- automatically discover and register endpoints via `IEndpoint`  
- group routes under consistent base paths (e.g., `/api`)  
- ensure predictable ordering and composition of endpoint mappings  
- integrate routing with middleware, identity, and dispatching layers  

Routing is a foundational part of the platform and must remain stable and predictable for all products.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core`

- **Documentation folder:**  
  `docs/02-frank-core/api`

This documentation must remain aligned with the actual routing implementation and updated as the platform evolves.

---

## Responsibilities of the Routing Subsystem

### [Endpoint Discovery](ca://s?q=Frank_Core_Api_Endpoint_Discovery)
Frank automatically discovers endpoints by scanning assemblies for implementations of `IEndpoint`. This eliminates manual route registration and ensures consistent behavior across products.

### [Route Grouping](ca://s?q=Frank_Core_Api_Route_Groups)
All endpoints are grouped under a base path (typically `/api`) to provide a clean, predictable URL structure.

### [Mapping Lifecycle](ca://s?q=Frank_Core_Api_Mapping_Lifecycle)
Endpoints are mapped *after* middleware is applied, ensuring:

- security headers  
- CORS  
- exception handling  
- observation context  

are already in place before any endpoint executes.

### [Integration with Dispatchers](ca://s?q=Frank_Core_Application_Dispatchers)
Routing integrates seamlessly with:

- `ICommandDispatcher`  
- `IQueryDispatcher`  
- `ICurrentUser`  

allowing endpoints to remain thin orchestration layers.

---

## How Routing Connects to the Broader Platform

Routing collaborates with multiple Frank subsystems:

- **Frank.Core.Api.Middleware**  
  Ensures routing occurs after security, CORS, and observation setup.

- **Frank.Identity.Api**  
  Provides identity context (`ICurrentUser`) for authenticated endpoints.

- **Frank.Core.Application**  
  Dispatches commands and queries from endpoint handlers.

- **Frank.Core.Infrastructure**  
  Supplies logging, correlation IDs, and request context.

Routing is the bridge between HTTP and the rest of the platform.

---

## Runtime Collaboration Points

Routing interacts with the runtime in several ways:

- **Endpoint discovery** — reflection-based scanning at startup  
- **Route grouping** — consistent base paths for all products  
- **Dependency injection** — endpoints resolve services from DI  
- **Middleware ordering** — routing must occur after platform middleware  
- **Exception handling** — routing relies on platform-level error mapping  

This ensures that routing is deterministic and platform‑consistent.

---

## Composition Flow (API → Application → Domain → Persistence)

Routing is the first step in the vertical slice execution flow:

```
HTTP Request
    ↓
Frank.Core.Api Middleware
    ↓
Frank.Core.Api Routing (IEndpoint)
    ↓
Frank.Core.Application Dispatchers
    ↓
Product Application Handlers
    ↓
Product Domain Aggregates
    ↓
Frank.Core.EntityFrameworkCore Persistence
    ↓
HTTP Response
```

Routing provides the entry point for the entire slice.

---

## What Belongs in This Document

This page should describe:

- routing responsibilities  
- endpoint discovery behavior  
- route grouping conventions  
- collaboration with middleware and identity  
- how routing composes with dispatchers and persistence  

It should **not** include:

- product-specific endpoints  
- domain logic  
- persistence details  

Those belong in product documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Api routing implementation.  
Whenever endpoint discovery, grouping, or mapping behavior changes, update this page to reflect the current platform architecture.

