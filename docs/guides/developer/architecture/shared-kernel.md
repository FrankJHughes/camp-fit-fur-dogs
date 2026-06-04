# Shared Kernel

This guide explains what belongs in the Shared Kernel, how it interacts with Domain and Application, and how to avoid misusing it. The Shared Kernel is a critical part of maintaining clean boundaries and predictable dependencies across the system.

---

# 1. Purpose

The Shared Kernel contains **cross-cutting domain concepts** and **cross-cutting technical infrastructure** that are:

- Used by multiple bounded contexts or aggregates  
- Stable and unlikely to change frequently  
- Safe to reference from Domain and Application  
- Required by all slices  

It is **not** a “common utilities” folder.  
It is a **strategic domain and infrastructure asset**.

---

# 2. Typical Contents

The Shared Kernel may contain:

## 2.1 Domain Primitives  
Types that represent core domain concepts shared across multiple aggregates:

- `DogId`  
- `OwnerId`  
- `Email`  
- `PhoneNumber`  

These are usually **value objects**.

## 2.2 Base Domain Types  
If shared across aggregates:

- `Entity`  
- `AggregateRoot`  
- `IDomainEvent`  

## 2.3 Cross-Cutting Domain Interfaces  
Interfaces that represent domain-level behavior:

- `IDomainEvent`  
- Domain-level abstractions used across multiple aggregates  

(Handlers and dispatchers belong in Application, not Shared Kernel.)

## 2.4 Domain Events (Sometimes)  
If an event is used across multiple aggregates or contexts, it may live here.

Example:

- `OwnerNotifiedDomainEvent`

If an event is specific to a single aggregate, it belongs in that aggregate’s Domain folder.

---

# 2.5 Technical Infrastructure (New)

The Shared Kernel also contains **cross-cutting technical infrastructure** required by all layers.

## Auto‑Registration System  
Frank provides the DI architecture used across the entire application:

- `[AutoRegister]` attribute  
- Assembly scanning for decorated interfaces  
- Implementation discovery  
- Min/max implementation validation  
- Concrete type registration (optional)  
- Startup failure on violations  

This system ensures:

- Predictable DI behavior  
- No manual registration for slice-level services  
- Strict enforcement of architectural rules  

## FluentValidation Integration  
Frank registers validators from all participating assemblies:

```
services.AddValidatorsFromAssembly(assembly);
```

## EF Core Configuration Auto‑Discovery  
Frank provides helpers for:

- Applying all `IEntityTypeConfiguration<T>` implementations  
- Enforcing DbContext guardrails  
- Ensuring consistent EF Core configuration across slices  

## Hosting Abstractions  
Frank includes:

- Hosting provider interfaces  
- API hosting helpers  
- Environment detection logic  

These are cross-cutting and layer-agnostic.

---

# 3. What Does NOT Belong in Shared Kernel

The Shared Kernel must **never** contain:

## 3.1 Application Concerns  
- Handlers  
- Validators  
- Dispatchers  
- Commands  
- Queries  
- DTOs  
- Pipeline behaviors  

## 3.2 Infrastructure Concerns  
- EF Core DbContexts  
- Repositories  
- Migrations  
- HTTP clients  
- Logging  
- File I/O  
- External service integrations  

## 3.3 API Concerns  
- Endpoints  
- Request/response DTOs  
- Routing  
- Authorization attributes  

## 3.4 “Common” Utilities  
Avoid dumping:

- String helpers  
- Date helpers  
- Random utilities  
- Extension methods  

These belong in:

- Domain (if domain-specific)  
- Application (if use-case-specific)  
- Infrastructure (if technical)  
- A dedicated utilities project (if truly cross-layer and non-domain)  

---

# 4. Dependency Rules

## 4.1 Allowed Dependencies

| Layer | May depend on Shared Kernel? |
|-------|------------------------------|
| Domain | ✅ Yes |
| Application | ✅ Yes |
| Infrastructure | ⚠️ Yes, but only for domain primitives and Frank infrastructure |
| API | ⚠️ Yes, but only for domain primitives and Frank infrastructure |
| Shared Kernel | ❌ Must not depend on any other layer |

## 4.2 Forbidden Dependencies

Shared Kernel must **never** depend on:

- Domain  
- Application  
- Infrastructure  
- API  

This keeps the dependency graph **acyclic** and predictable.

---

# 5. Relationship to Domain

The Shared Kernel is a **peer** to Domain, not a parent or child.

Use Shared Kernel when:

- A type is used across multiple aggregates  
- A type is used across multiple bounded contexts  
- A type is stable and unlikely to churn  

Use Domain when:

- A type is specific to a single aggregate  
- A type is part of a single bounded context  
- A type is likely to evolve with that context  

---

# 6. Relationship to Application

Application may reference Shared Kernel types:

- Domain primitives  
- Domain events  
- Domain interfaces  
- Frank technical infrastructure (e.g., `[AutoRegister]`)  

Application must not:

- Add business logic to Shared Kernel  
- Add handlers or validators to Shared Kernel  
- Add DTOs to Shared Kernel  

---

# 7. Contributor Guidelines

When adding a new type:

## Put it in **Domain** if:
- It belongs to a single aggregate  
- It is part of a single bounded context  
- It expresses domain rules or invariants  

## Put it in **Shared Kernel** if:
- It is used across multiple aggregates  
- It is used across multiple bounded contexts  
- It is stable and domain-specific  
- It is a domain primitive or cross-cutting domain abstraction  
- It is cross-cutting technical infrastructure (DI, EF Core config, hosting)  

## Do NOT put it in Shared Kernel if:
- It is technical (Infrastructure)  
- It is HTTP-related (API)  
- It is a use-case abstraction (Application)  
- It is a convenience helper  

If you’re unsure, default to **Domain**, not Shared Kernel.

---

# Related Documents

- **[Dependency Injection Architecture](ca://s?q=Generate_Dependency_Injection_Architecture_Guide)**  
- **[Domain Events Architecture](ca://s?q=Generate_Domain_Events_Architecture_Guide)**  
- **[Dispatcher Pipeline](ca://s?q=Generate_Dispatcher_Pipeline_Guide)**  
- **[API Endpoint Purity](ca://s?q=Generate_API_Endpoint_Purity_Guide)**  
