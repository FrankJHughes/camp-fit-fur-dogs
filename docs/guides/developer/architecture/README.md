# Developer Architecture Guides

This folder contains the **core architectural explanations** that define how the CampFitFurDogs backend is structured.  
These guides help developers understand the system’s layers, purity rules, pipelines, and cross‑cutting architectural patterns.

These documents are **explanatory**, not prescriptive.  
They do **not** define rules (see Conventions) and do **not** record decisions (see ADRs).  
Instead, they explain **how the architecture works** and **how to apply it correctly**.

Frank provides the system’s cross‑cutting backbone — including the DI auto‑registration engine, endpoint discovery, validator scanning, environment abstraction, hosting provider abstractions, **HostingEngine**, and **StartupEngine**.  
Application, Domain, Infrastructure, and Api form the vertical slices of behavior.  
Authentication and session flows now use the **ImmutableContextBuilder** architecture.

---

# Purpose of This Folder

Use these guides to understand:

- how API, Application, and Domain layers interact  
- how purity rules constrain each layer  
- how commands and queries flow through the dispatcher pipeline  
- how domain events propagate  
- how vertical slices are structured  
- how validation boundaries are enforced  
- how the **builder‑based authentication callback architecture** works  
- how identity mapping, session creation, and cookie issuance integrate with the architecture  
- how Infrastructure integrates with Application and Domain  
- how **StartupEngine** composes startup modules  
- how **HostingEngine** selects and configures hosting providers  
- how environment seams and hosting seams work  
- how tests map to architectural layers and seams  
- how Frank provides cross‑cutting primitives and discovery mechanisms  
- how ImmutableContextBuilder enables pure, deterministic multi‑stage transformations  

These guides evolve as the system evolves.

---

# Architecture Guide Index

## Core Architecture

- **API Endpoint Purity**  
  How to keep endpoints thin, pure, and free of domain logic.

- **Dispatcher Pipeline**  
  How commands and queries flow through the Application layer.

- **Domain Events**  
  How domain events are raised, dispatched, and handled.

- **Folder Structure**  
  How vertical slices and layers are organized in the codebase.

- **Purity Rules**  
  Cross‑layer architectural purity constraints.

- **Shared Kernel (Frank)**  
  What belongs in Frank and why.

- **Validation Boundaries**  
  Clear separation of API, Application, and Domain validation responsibilities.

- **Test Architecture**  
  How to structure tests across layers (API, Application, Domain) and how to use Frank’s seams.

- **Infrastructure Architecture**  
  How EF Core, repositories, readers, hosting providers, and environment abstractions integrate with the system.

- **Vertical Slice Architecture**  
  How slices encapsulate API, Application, Domain, and Infrastructure behavior.

- **Immutable Context Builder Architecture**  
  How multi‑stage transformations are implemented using pure, deterministic, immutable pipelines.

---

## Authentication Architecture

- **Authentication Overview**  
  High‑level explanation of the OIDC authentication flow and the three‑layer callback architecture.

- **Login Endpoint**  
  Details for `/api/auth/login`, the pure redirect endpoint.

- **Callback Endpoint**  
  Details for `/api/auth/callback`, which orchestrates the Frank pipeline, Application pipeline, cookie issuance, and redirect.

- **Authentication Configuration**  
  Required Auth0 configuration keys and environment variables.

- **Frank Callback Pipeline**  
  Protocol‑level pipeline implemented using ImmutableContextBuilder.

- **Application Callback Pipeline**  
  Business‑level pipeline implemented using ImmutableContextBuilder.

- **Identity Mapping**  
  How external identities map to internal Owner identities.

- **Session Token Architecture**  
  How session tokens are generated, hashed, stored, and validated.

- **Session Cookie Specification**  
  Cookie format, flags, security properties, and lifetime.

- **Session Management**  
  How session records are created, persisted, validated, and expired.

- **Authentication Error Handling**  
  How errors are surfaced during login, callback, and session validation.

---

## Cross‑Cutting Architecture

- **Hosting Provider Architecture**  
  How hosting providers integrate with Frank’s hosting abstractions and how HostingEngine selects and configures them.

- **Environment Abstraction**  
  How environment variables are accessed safely and deterministically through Frank’s seams.

- **Security Headers Architecture**  
  How security headers are applied and enforced across environments.

- **Startup Engine Architecture**  
  How StartupEngine runs startup modules to assemble the application.

- **Hosting Engine Architecture**  
  How HostingEngine evaluates hosting modules and configures the environment.

---

# When to Update These Guides

Update or add new architecture guides when:

- a new subsystem is introduced (e.g., messaging, workflows, caching)  
- a new architectural pattern is adopted  
- a purity rule changes  
- a new pipeline or cross‑cutting concern is added  
- a feature introduces a reusable architectural concept  
- a developer needs clarification that applies across slices  
- a hosting provider abstraction changes  
- a new environment or preview behavior is introduced  
- StartupEngine or HostingEngine behavior changes  
- a new ImmutableContextBuilder pipeline is introduced  

Do **not** update these guides for:

- one‑off implementation details  
- business logic changes  
- story‑specific behavior  
- temporary constraints  
- decisions that belong in ADRs  
- conventions or governance rules  

---

# Related Documentation

- `docs/guides/developer/` — Developer handbook  
- `docs/conventions/` — Architectural and coding rules  
- `product/adrs/` — Architectural decision records  
- `docs/guides/developer/authentication/` — Authentication subsystem guides  
- `docs/guides/developer/testing/` — Testing architecture and patterns  
- `docs/guides/developer/operations/` — Operational behavior and hosting details
