# Developer Architecture Guides

This folder contains the **core architectural explanations** that define how the CampFitFurDogs backend is structured.  
These guides help developers understand the system’s layers, purity rules, pipelines, and cross‑cutting architectural patterns.

These documents are **explanatory**, not prescriptive.  
They do **not** define rules (see Conventions) and do **not** record decisions (see ADRs).  
Instead, they explain **how the architecture works** and **how to apply it correctly**.

---

# Purpose of This Folder

Use these guides to understand:

- How API, Application, and Domain layers interact  
- How purity rules constrain each layer  
- How commands and queries flow through the dispatcher pipeline  
- How domain events propagate  
- How vertical slices are structured  
- How validation boundaries are enforced  
- How authentication and session flows integrate with the architecture  
- How tests map to architectural layers  

These guides evolve as the system evolves.

---

# Architecture Guide Index

## Core Architecture

- **[API Endpoint Purity](../api-endpoint-purity.md)**  
  How to keep endpoints thin, pure, and free of domain logic.

- **[Dispatcher Pipeline](../dispatcher-pipeline.md)**  
  How commands and queries flow through the application layer.

- **[Domain Events](../domain-events.md)**  
  How domain events are raised, dispatched, and handled.

- **[Folder Structure](../folder-structure.md)**  
  How vertical slices and layers are organized in the codebase.

- **[Purity Rules](../purity-rules.md)**  
  Cross-layer architectural purity constraints.

- **[Shared Kernel](../shared-kernel.md)**  
  What belongs in the SharedKernel and why.

- **[Validation Boundaries](validation-boundaries.md)**  
  Clear separation of API, Application, and Domain validation responsibilities.

- **[Test Architecture](../test-architecture.md)**  
  How to structure tests across layers (API, Application, Domain).

---

## Authentication Architecture

- **[Authentication Overview](../authentication/overview.md)**  
  High-level explanation of the OIDC authentication flow.

- **[Login Endpoint](../authentication/login-endpoint.md)**  
  Details for `/api/auth/login`, the pure redirect endpoint.

- **[Callback Endpoint](../authentication/callback-endpoint.md)**  
  Details for `/api/auth/callback`, which completes the OIDC flow.

- **[Authentication Configuration](../authentication/configuration.md)**  
  Required Auth0 configuration keys and environment variables.

---

# When to Update These Guides

Update or add new architecture guides when:

- A new subsystem is introduced (e.g., messaging, workflows, caching)
- A new architectural pattern is adopted
- A purity rule changes
- A new pipeline or cross-cutting concern is added
- A feature introduces a reusable architectural concept
- A developer needs clarification that applies across slices

Do **not** update these guides for:

- One-off implementation details  
- Business logic changes  
- Story-specific behavior  
- Temporary constraints  
- Decisions that belong in ADRs  

---

# Related Documentation

- `docs/guides/developer/` — Developer handbook  
- `docs/conventions/` — Architectural and coding rules  
- `product/adrs/` — Architectural decision records  
- `docs/guides/developer/authentication/` — Authentication subsystem guides
