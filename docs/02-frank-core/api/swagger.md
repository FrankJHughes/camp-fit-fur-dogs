# Frank.Core.Api — Swagger

The Swagger subsystem in Frank.Core.Api provides automatic OpenAPI generation, UI hosting, and endpoint documentation for all products built on the Frank platform. It ensures that every `IEndpoint` implementation is discoverable, documented, and testable through a consistent OpenAPI surface.

This document describes the responsibilities of the Swagger subsystem and how it aligns with the implementation under `src/Frank/Core`.

---

## Purpose

The Swagger subsystem exists to:

- generate OpenAPI documentation for all platform and product endpoints  
- expose a consistent, interactive Swagger UI for development and testing  
- integrate endpoint metadata (tags, summaries, response types)  
- ensure that API surfaces remain discoverable and self‑documenting  
- provide tooling support for frontend and integration teams  

Swagger is a core part of the Frank platform’s developer experience.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core`

- **Documentation folder:**  
  `docs/02-frank-core/api`

This documentation must remain aligned with the actual Swagger configuration and updated as the platform evolves.

---

## Responsibilities of the Swagger Subsystem

### [OpenAPI Generation](ca://s?q=Frank_Core_Api_OpenAPI_Generation)
Frank registers OpenAPI services during platform composition:

```csharp
services.AddFrankCoreApiPlatformSwagger();
```

This extension method:

- registers Swagger/OpenAPI generators  
- configures schema generation for commands, queries, and DTOs  
- applies platform‑level conventions (e.g., grouping by namespace or tag)  
- integrates XML documentation when available  

### [Swagger UI Hosting](ca://s?q=Frank_Core_Api_Swagger_UI)
During startup, Frank hosts the Swagger UI:

```csharp
app.UseSwagger();
app.UseSwaggerUI();
```

The UI provides:

- interactive request testing  
- schema browsing  
- endpoint grouping  
- environment‑aware configuration  

### [Endpoint Metadata Integration](ca://s?q=Frank_Core_Api_Endpoint_Metadata)
Endpoints can provide metadata via:

- `.WithOpenApi()`  
- `.Produces()`  
- `.WithTags()`  
- `.WithSummary()`  
- `.WithDescription()`  

Frank automatically incorporates this metadata into the OpenAPI document.

### [Route Grouping](ca://s?q=Frank_Core_Api_Route_Groups)
All endpoints mapped under `/api` appear under a unified group in Swagger, making the API surface easy to navigate.

---

## How Swagger Connects to the Broader Platform

Swagger collaborates with multiple Frank subsystems:

- **Frank.Core.Api.Routing**  
  Discovers endpoints and exposes them to the OpenAPI generator.

- **Frank.Core.Application**  
  Documents command/query request/response shapes.

- **Frank.Identity.Api**  
  Adds security schemes (e.g., bearer tokens, OIDC flows).

- **Frank.Core.Infrastructure**  
  Provides logging and configuration for Swagger hosting.

Swagger is the documentation layer that ties the entire platform together.

---

## Runtime Collaboration Points

Swagger interacts with the runtime in several ways:

- **Startup configuration** — registers generators and UI  
- **Endpoint discovery** — inspects `IEndpoint` implementations  
- **Schema generation** — reflects DTOs, commands, queries, and results  
- **Security integration** — adds identity requirements to OpenAPI  
- **Environment behavior** — UI enabled in development, optional in production  

This ensures consistent documentation across all products.

---

## Composition Flow (API → Application → Domain → Persistence)

Swagger documents the API entry points in the vertical slice flow:

```
HTTP Request
    ↓
Frank.Core.Api Routing
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

Swagger exposes the *entry points* of this flow to developers and integrators.

---

## What Belongs in This Document

This page should describe:

- Swagger/OpenAPI responsibilities  
- how Swagger integrates with routing and endpoint metadata  
- how the UI is hosted and configured  
- how schemas are generated  
- how Swagger collaborates with identity and application layers  

It should **not** include:

- product‑specific endpoint documentation  
- domain logic  
- persistence details  

Those belong in product‑level documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Api Swagger implementation.  
Whenever OpenAPI generation, UI hosting, or endpoint metadata conventions change, update this page to reflect the current platform architecture.

