# Shared Kernel

This guide explains what belongs in the Shared Kernel, how it interacts with Domain and Application, and how to avoid misusing it.  
The Shared Kernel is a critical part of maintaining clean boundaries and predictable dependencies across the system.

The Shared Kernel is **not** a “common utilities” folder.  
It is a **strategic domain + technical foundation** shared across all slices.

---

# 1. Purpose

The Shared Kernel contains **cross‑cutting domain concepts** and **cross‑cutting technical infrastructure** that are:

- Used by multiple bounded contexts or aggregates  
- Stable and unlikely to change frequently  
- Safe to reference from Domain and Application  
- Required by all slices  
- Layer‑agnostic and dependency‑safe  

It exists to prevent duplication, enforce consistency, and provide a stable foundation for all vertical slices.

---

# 2. Typical Contents

The Shared Kernel may contain both **domain primitives** and **cross‑cutting technical infrastructure**.

## 2.1 Domain Primitives  
Types that represent core domain concepts shared across multiple aggregates:

- `DogId`  
- `OwnerId`  
- `Email`  
- `PhoneNumber`  

These are **value objects** with invariants and validation.

## 2.2 Base Domain Types  
If shared across aggregates:

- `Entity`  
- `AggregateRoot`  
- `IDomainEvent`  

These types enforce consistent domain modeling across slices.

## 2.3 Cross‑Cutting Domain Interfaces  
Interfaces that represent domain‑level behavior used across multiple aggregates:

- `IDomainEvent`  
- Domain‑level abstractions that do not belong to a single bounded context  

Handlers, dispatchers, and pipelines belong in **Application**, not Shared Kernel.

## 2.4 Domain Events (Sometimes)  
If an event is used across multiple aggregates or contexts, it may live here.

Example:

- `OwnerNotifiedDomainEvent`

If an event is specific to a single aggregate, it belongs in that aggregate’s Domain folder.

---

# 2.5 Cross‑Cutting Technical Infrastructure

The Shared Kernel also contains **technical infrastructure** that is:

- Required by all layers  
- Layer‑agnostic  
- Stable  
- Safe for Domain and Application to reference  

This includes the Frank infrastructure that enforces architectural guardrails.

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
- No manual registration for slice‑level services  
- Strict enforcement of architectural rules  
- No Scrutor or suffix scanning  

## FluentValidation Integration  
Frank registers validators from all participating assemblies:

```
services.AddValidatorsFromAssembly(assembly);
```

Validators remain in Application, but the integration lives in Shared Kernel.

## EF Core Configuration Auto‑Discovery  
Frank provides helpers for:

- Applying all `IEntityTypeConfiguration<T>` implementations  
- Enforcing DbContext guardrails  
- Ensuring consistent EF Core configuration across slices  
- Preventing Infrastructure leakage into Domain or Application  

## Hosting Abstractions  
Frank includes:

- Hosting provider interfaces  
- Environment abstraction  
- API hosting helpers  
- Startup validation helpers  

These abstractions:

- Are cross‑cutting  
- Are safe for Domain and Application  
- Prevent direct environment access  
- Prevent direct HTTP/JSON/ZIP usage  

---

# 3. What Does NOT Belong in Shared Kernel

The Shared Kernel must **never** contain anything that introduces upward or lateral dependencies.

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
- Hosting provider implementations  

## 3.3 API Concerns  
- Endpoints  
- Request/response DTOs  
- Routing  
- Authorization attributes  
- Middleware  

## 3.4 “Common” Utilities  
Avoid dumping:

- String helpers  
- Date helpers  
- Random utilities  
- Extension methods  

These belong in:

- Domain (if domain‑specific)  
- Application (if use‑case‑specific)  
- Infrastructure (if technical)  
- A dedicated utilities project (if truly cross‑layer and non‑domain)  

The Shared Kernel is **not** a junk drawer.

---

# 4. Dependency Rules

## 4.1 Allowed Dependencies

| Layer | May depend on Shared Kernel? |
|-------|------------------------------|
| Domain | ✅ Yes |
| Application | ✅ Yes |
| Infrastructure | ⚠️ Yes — only for domain primitives + Frank infrastructure |
| API | ⚠️ Yes — only for domain primitives + Frank infrastructure |
| Shared Kernel | ❌ Must not depend on any other layer |

## 4.2 Forbidden Dependencies

Shared Kernel must **never** depend on:

- Domain  
- Application  
- Infrastructure  
- API  

This keeps the dependency graph **acyclic** and enforces **Architecture Governance**.

---

# 5. Relationship to Domain

The Shared Kernel is a **peer** to Domain, not a parent or child.

Use Shared Kernel when:

- A type is used across multiple aggregates  
- A type is used across multiple bounded contexts  
- A type is stable and unlikely to churn  
- A type expresses cross‑cutting domain semantics  

Use Domain when:

- A type is specific to a single aggregate  
- A type is part of a single bounded context  
- A type is likely to evolve with that context  

Shared Kernel types must be **stable**.  
Domain types may evolve rapidly.

---

# 6. Relationship to Application

Application may reference Shared Kernel types:

- Domain primitives  
- Domain events  
- Domain interfaces  
- Frank technical infrastructure (e.g., `[AutoRegister]`)  
- EF Core configuration helpers  
- Hosting abstractions  

Application must not:

- Add business logic to Shared Kernel  
- Add handlers or validators to Shared Kernel  
- Add DTOs to Shared Kernel  
- Add pipeline behaviors to Shared Kernel  

Shared Kernel is **referenced by Application**, not extended by it.

---

# 7. Contributor Guidelines

When adding a new type:

## Put it in **Domain** if:
- It belongs to a single aggregate  
- It is part of a single bounded context  
- It expresses domain rules or invariants  
- It is not used outside that context  

## Put it in **Shared Kernel** if:
- It is used across multiple aggregates  
- It is used across multiple bounded contexts  
- It is stable and domain‑specific  
- It is a domain primitive or cross‑cutting domain abstraction  
- It is cross‑cutting technical infrastructure (DI, EF Core config, hosting)  

## Do NOT put it in Shared Kernel if:
- It is technical (Infrastructure)  
- It is HTTP‑related (API)  
- It is a use‑case abstraction (Application)  
- It is a convenience helper  
- It is unstable or likely to churn  

If you’re unsure, default to **Domain**, not Shared Kernel.

---

# Related Documents

- **Dependency Injection Architecture**  
- **Domain Events Architecture**  
- **Dispatcher Pipeline**  
- **API Endpoint Purity**  
- **Architecture Governance**  
- **Security Governance**  
- **Operations Governance**  
