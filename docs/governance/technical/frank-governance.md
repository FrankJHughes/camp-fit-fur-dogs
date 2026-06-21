# Frank (Shared Kernel) Governance  
Authoritative companion to ADR‑0002 (Layered Architecture)

Frank is the **Shared Kernel** of Camp Fit Fur Dogs.  
It provides the **domain primitives**, **technical primitives**, and **cross‑cutting infrastructure** that every layer depends on.

Frank is **not** a utilities project.  
Frank is **not** a dumping ground.  
Frank is a **strategic, stable, dependency‑safe foundation** for the entire system.

Violations of Frank’s boundaries are architectural defects.

---

# 1. Purpose

Frank exists to provide:

- **Cross‑cutting domain primitives**  
- **Cross‑cutting technical primitives**  
- **Stable abstractions** used across all slices  
- **Layer‑agnostic infrastructure**  
- **Deterministic startup and hosting orchestration**  
- **Strict DI governance**  
- **Predictable dependency direction**

Frank prevents duplication, enforces consistency, and ensures that Domain and Application remain pure and stable.

---

# 2. What Frank Contains

Frank contains two categories of primitives:

1. **Domain primitives** (shared across bounded contexts)  
2. **Technical primitives** (shared across all layers)

Both categories must be:

- Stable  
- Layer‑agnostic  
- Dependency‑safe  
- Product‑agnostic  

---

# 2.1 Domain Primitives

Domain primitives shared across multiple aggregates or bounded contexts:

- `DogId`  
- `OwnerId`  
- `Email`  
- `PhoneNumber`  

These are **value objects** with invariants and validation.

Domain primitives in Frank must be:

- Immutable  
- Self‑validating  
- Free of Application or Infrastructure dependencies  

---

# 2.2 Base Domain Types

Frank provides the base types required for consistent domain modeling:

- `Entity<TId>`  
- `AggregateRoot<TId>`  
- `IDomainEvent`  

These types enforce:

- Aggregate boundaries  
- Domain event purity  
- Consistent modeling across slices  

---

# 2.3 Cross‑Cutting Domain Interfaces

Frank may contain domain‑level abstractions that span multiple aggregates:

- `IDomainEvent`  
- Domain‑level marker interfaces  
- Domain‑level invariants  

Frank must **not** contain:

- Handlers  
- Dispatchers  
- Application logic  

Those belong in Application.

---

# 2.4 Shared Domain Events (Rare)

A domain event belongs in Frank **only** if it is:

- Used across multiple aggregates  
- Used across multiple bounded contexts  
- Stable and cross‑cutting  

Example:

- `OwnerNotifiedDomainEvent`

If an event is specific to a single aggregate, it belongs in that aggregate’s Domain folder.

---

# 3. Technical Primitives (Frank Infrastructure)

Frank provides the **technical backbone** of the entire system.

These primitives are:

- Layer‑agnostic  
- Product‑agnostic  
- Stable  
- Safe for Domain and Application  
- Required by all slices  

Frank includes:

- DI auto‑registration engine  
- HostingEngine  
- StartupEngine  
- EF Core configuration helpers  
- Validator scanning  
- Hosting abstractions  
- Environment abstractions  
- Guardrail enforcement primitives  

CampFitFurDogs **implements modules**.  
Frank **implements engines**.

---

# 3.1 StartupEngine

StartupEngine is provided by Frank.

It:

- Accepts a list of startup modules  
- Invokes each module deterministically  
- Configures services  
- Configures application pipelines  
- Performs startup validation  
- Ensures consistent startup across environments  

CampFitFurDogs provides modules such as:

- `ApiStartupModule`  
- `ApplicationStartupModule`  
- `InfrastructureStartupModule`  

CampFitFurDogs **invokes** StartupEngine:

```csharp
StartupEngine.Run(builder, [
    new ApiStartupModule(),
    new ApplicationStartupModule(),
    new InfrastructureStartupModule()
]);
```

StartupEngine is **never modified** by CampFitFurDogs.

---

# 3.2 HostingEngine

HostingEngine is provided by Frank.

It:

- Selects the active hosting provider  
- Applies hosting‑specific configuration  
- Provides environment abstractions  
- Ensures consistent hosting behavior  

CampFitFurDogs provides hosting modules such as:

- `DevelopmentHostingModule`  
- `RenderHostingModule`  
- `TestHostingModule`  

CampFitFurDogs **invokes** HostingEngine:

```csharp
var hosting = HostingEngine.Select([
    new DevelopmentHostingModule(),
    new RenderHostingModule(),
    new TestHostingModule()
]);
```

HostingEngine is **never modified** by CampFitFurDogs.

---

# 3.3 DI Auto‑Registration System

Frank provides the DI architecture:

- `[AutoRegister]` attribute  
- Assembly scanning  
- Implementation discovery  
- Min/max implementation validation  
- Concrete type registration (optional)  
- Startup failure on violations  

This system ensures:

- Predictable DI behavior  
- No manual registration for slice services  
- No Scrutor  
- No suffix scanning  
- Strict architectural enforcement  

---

# 3.4 FluentValidation Integration

Frank registers validators from all participating assemblies:

```
services.AddValidatorsFromAssembly(assembly);
```

Validators remain in Application.  
Integration remains in Frank.

---

# 3.5 EF Core Configuration Auto‑Discovery

Frank provides helpers for:

- Applying all `IEntityTypeConfiguration<T>` implementations  
- Enforcing DbContext guardrails  
- Ensuring consistent EF Core configuration  
- Preventing Infrastructure leakage into Domain or Application  

---

# 3.6 Hosting Abstractions

Frank includes:

- Hosting provider interfaces  
- Environment abstractions  
- API hosting helpers  
- Startup validation helpers  

These abstractions:

- Are cross‑cutting  
- Are safe for Domain and Application  
- Prevent direct environment access  
- Prevent direct HTTP/JSON/ZIP usage  

---

# 4. What Frank Must Never Contain

Frank must **never** contain:

## 4.1 Application Concerns

- Handlers  
- Validators  
- Dispatchers  
- Commands  
- Queries  
- DTOs  
- Pipeline behaviors  

## 4.2 Infrastructure Concerns

- DbContexts  
- Repositories  
- Readers  
- Migrations  
- HTTP clients  
- Logging  
- File I/O  
- External service integrations  

## 4.3 API Concerns

- Endpoints  
- Request/response DTOs  
- Routing  
- Authorization attributes  
- Middleware  

## 4.4 “Common” Utilities

Frank is **not** a junk drawer.

Do not add:

- String helpers  
- Date helpers  
- Random utilities  
- Extension methods  

If it’s not domain‑specific or cross‑cutting infrastructure, it does not belong in Frank.

---

# 5. Dependency Rules

## 5.1 Allowed Dependencies

| Layer | May depend on Frank? |
|-------|-----------------------|
| Domain | ✅ Yes |
| Application | ✅ Yes |
| Infrastructure | ⚠️ Yes — only for domain primitives + Frank infrastructure |
| API | ⚠️ Yes — only for domain primitives + Frank infrastructure |
| Frank | ❌ Must not depend on any other layer |

## 5.2 Forbidden Dependencies

Frank must **never** depend on:

- Domain  
- Application  
- Infrastructure  
- API  

Frank is the **lowest‑level dependency** in the system.

---

# 6. Relationship to Domain

Frank is a **peer** to Domain.

Use Frank when:

- A type is shared across multiple aggregates  
- A type is shared across multiple bounded contexts  
- A type is stable and domain‑specific  
- A type expresses cross‑cutting domain semantics  

Use Domain when:

- A type belongs to a single aggregate  
- A type belongs to a single bounded context  
- A type is likely to evolve with that context  

Frank types must be **stable**.  
Domain types may evolve rapidly.

---

# 7. Relationship to Application

Application may reference Frank types:

- Domain primitives  
- Domain events  
- Domain interfaces  
- Frank technical infrastructure  
- EF Core configuration helpers  
- Hosting abstractions  

Application must not:

- Add business logic to Frank  
- Add handlers or validators to Frank  
- Add DTOs to Frank  
- Add pipeline behaviors to Frank  

Frank is **referenced**, not extended.

---

# 8. Contributor Guidelines

## Put it in **Domain** if:

- It belongs to a single aggregate  
- It belongs to a single bounded context  
- It expresses domain rules or invariants  
- It is not used outside that context  

## Put it in **Frank** if:

- It is used across multiple aggregates  
- It is used across multiple bounded contexts  
- It is stable and domain‑specific  
- It is a domain primitive  
- It is cross‑cutting technical infrastructure  

## Do NOT put it in Frank if:

- It is technical Infrastructure  
- It is HTTP‑related  
- It is a use‑case abstraction  
- It is a convenience helper  
- It is unstable or likely to churn  

When unsure, default to **Domain**, not Frank.

---

# Related Documents

- **Purity Rules Governance**  
- **Dependency Injection Architecture**  
- **Domain Events Architecture**  
- **Dispatcher Pipeline Guide**  
- **API Endpoint Purity Guide**  
- **Architecture Governance**  
- **Security Governance**  
- **Operations Governance**
