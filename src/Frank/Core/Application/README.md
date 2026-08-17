# Application

The **Application** folder contains the high‑level application layer of the
Frank.Core architecture. This layer orchestrates behavior, defines contracts,
coordinates cross‑cutting concerns, and provides the execution mechanics for
CQRS, domain events, immutable context pipelines, registration/discovery, and
other application‑level services.

The Application layer does **not** contain business logic or persistence.  
Instead, it defines the *rules of interaction* between vertical slices,
infrastructure, and domain models.

---

## Folder Structure

```
Application/
 ├── Abstractions/
 ├── Cqrs/
 ├── DomainEvents/
 ├── ImmutableContexts/
 ├── Registration/
 ├── ServiceCollectionExtensions.cs
 └── AssemblyMarker.cs
```

Each subsystem has a clear responsibility and a dedicated README.

---

## Abstractions

The **Abstractions** folder defines the contracts used throughout the
application layer. These interfaces are intentionally lightweight and free of
implementation details.

Subdomains include:

- **Clock** — time abstraction (`IClock`)
- **CQRS** — command/query contracts
- **DomainEvents** — domain event contracts
- **Endpoints** — endpoint contract for vertical slices
- **EnvironmentVariables** — configuration abstraction
- **Exceptions** — structured exception handling contracts
- **Hosting** — hosting module contracts
- **ImmutableContexts** — contracts for immutable context pipelines
- **Observations** — observability contracts
- **UnitOfWork** — transactional boundary abstraction

These abstractions define the *shape* of the application layer.

---

## Cqrs

The **Cqrs** folder contains the *runtime mechanics* for executing commands and
queries:

- **CommandDispatcher** — validation + handler resolution + execution  
- **QueryDispatcher** — validation + handler resolution + execution  
- **ServiceCollectionExtensions** — attribute‑driven handler discovery  
- Sub‑READMEs for Commands and Queries

This subsystem ensures consistent, predictable execution of write‑side and
read‑side operations.

---

## DomainEvents

The **DomainEvents** folder provides the infrastructure for domain event
propagation:

- **DomainEventDispatcher** — fan‑out delivery to all handlers  
- **ServiceCollectionExtensions** — attribute‑driven handler discovery  

Domain events represent meaningful state changes inside the domain model.  
This subsystem ensures they are delivered reliably and observably.

---

## ImmutableContexts

The **ImmutableContexts** folder contains the execution engine for building
immutable context objects through declarative, step‑driven pipelines:

- **ImmutableContextBuilderBase** — orchestrates step selection, execution,
  transition validation, and observability

Slices define concrete contexts and steps; this subsystem provides the builder.

---

## Registration

The **Registration** folder contains the dynamic discovery and registration
pipeline used by CQRS, domain events, and other subsystems:

- **Scanner** — assembly scanning  
- **Planner** — dependency grouping  
- **Registrar** — DI registration  
- **Validator** — rule enforcement  
- **Orchestrator** — end‑to‑end discovery pipeline  
- **Shapes/** — internal structures for grouping interfaces/implementations  

This subsystem enables attribute‑driven, slice‑friendly automatic registration.

---

## ServiceCollectionExtensions.cs

Provides top‑level DI registration helpers for the entire Application layer.

This file ties the subsystems together so vertical slices can opt‑in to
application‑level services with minimal boilerplate.

---

## AssemblyMarker.cs

A simple marker type used to anchor assembly scanning.

---

## Design Principles

- **Vertical‑slice alignment**  
  Application services are designed to be consumed slice‑by‑slice.

- **Abstraction-first**  
  Contracts live in Abstractions; implementations live in Application.

- **Observability**  
  All major pipelines emit structured events.

- **Immutability**  
  Context builders enforce deterministic, traceable state evolution.

- **Automatic discovery**  
  Registration is driven by attributes and assembly scanning.

- **Testability**  
  Dispatchers, builders, and handlers are isolated and easy to test.

---

## How This Layer Fits Into the Architecture

The Application layer sits between:

- **Domain** (business rules)  
- **Infrastructure** (persistence, external systems)  
- **Vertical slices** (features)

It provides the glue that coordinates behavior across slices while keeping the
system modular, observable, and predictable.

---

## Notes

- This layer contains **no business logic**.  
- All behavior is orchestrational, not domain‑specific.  
- Every subsystem has its own README for deeper documentation.

---
