# Architecture Governance

This document defines the architectural boundaries, dependency rules, and enforcement mechanisms for all products in the repository.  
It complements (but does not duplicate) code conventions, security governance, and operational governance.

Architecture governance ensures:

- Stable, predictable system structure  
- Clear separation of responsibilities  
- Enforceable dependency boundaries  
- Long-term maintainability  
- Zero architectural drift  
- Consistent behavior across all products  

Architecture is a contract. Violations are defects.

---

# 1. Architectural Principles

All products must follow these principles:

- **Separation of Concerns** — each layer has a single purpose  
- **Explicit Dependencies** — no hidden or implicit coupling  
- **Inversion of Control** — high-level policies depend on abstractions  
- **Testability** — architecture must enable deterministic testing  
- **Replaceability** — components must be swappable without cascade changes  
- **Purity** — domain logic must remain free of infrastructure concerns  
- **Determinism** — architectural behavior must not depend on runtime accidents  

Architecture is intentional, not emergent.

---

# 2. Layered Architecture Governance

The system uses a strict layered architecture:

```
Api → Application → Domain
Infrastructure → Application → Domain
All layers → SharedKernel
```

## 2.1 Domain Layer

The Domain layer:

- Owns business rules  
- Owns aggregates, entities, value objects  
- Owns domain events  
- Must not depend on any other layer  
- Must not reference infrastructure, API, or external libraries except:  
  - BCL  
  - SharedKernel primitives  

Forbidden:

- HTTP concepts  
- Database concepts  
- Serialization  
- Logging  
- Configuration access  
- External service calls  

## 2.2 Application Layer

The Application layer:

- Coordinates use cases  
- Implements CQRS command/query handlers  
- Defines readers and application-level abstractions  
- Dispatches domain events  
- Depends only on Domain and SharedKernel  
- Participates in DI via `[AutoRegister]`  

Forbidden:

- Direct database access  
- HTTP clients  
- UI logic  
- Hosting logic  
- Authentication logic  
- Configuration access  
- Manual DI registration of slice services  

## 2.3 Infrastructure Layer

The Infrastructure layer:

- Implements repositories  
- Implements readers  
- Implements external service integrations  
- Implements database access  
- Implements hosting providers  
- Implements authentication providers  
- Depends on Application and Domain  
- Uses SharedKernel auto‑registration for DI participation  

Forbidden:

- Exposing infrastructure types to API  
- Implementing business rules  
- Creating domain entities outside factories  
- Manual DI registration of slice services  

## 2.4 API Layer

The API layer:

- Exposes HTTP endpoints  
- Performs request/response mapping  
- Performs authentication and authorization  
- Performs validation  
- Dispatches to Application layer  

Forbidden:

- Business logic  
- Database access  
- External service calls  
- Domain mutation outside Application layer  
- Direct handler invocation (must use dispatchers)  
- Infrastructure dependencies  

---

# 3. Dependency Direction Governance

Allowed dependencies:

- Api → Application  
- Api → Domain (primitives only)  
- Api → SharedKernel  
- Application → Domain  
- Application → SharedKernel  
- Infrastructure → Application  
- Infrastructure → Domain  
- Infrastructure → SharedKernel  

Forbidden dependencies:

- Domain → Application  
- Domain → Infrastructure  
- Domain → Api  
- Application → Api  
- Infrastructure → Api  

Any violation is a governance failure.

---

# 4. CQRS Governance

CQRS is mandatory.

Commands:

- Must mutate state  
- Must not return domain entities  
- Must not return arbitrary data  
- Must be deterministic  

Queries:

- Must not mutate state  
- Must return DTOs  
- Must be idempotent  

Forbidden:

- Commands returning data  
- Queries performing writes  
- Mixing command and query responsibilities  
- Direct handler invocation (must use dispatchers)  

---

# 5. Hosting Provider Governance

Hosting providers:

- Run before DI is built  
- Must be deterministic  
- Must validate required configuration  
- Must fail fast on misconfiguration  
- Must not silently skip required configuration  
- Must not depend on product-specific logic  

Only SharedKernel may define hosting provider infrastructure.

---

# 6. Authentication Architecture Governance

Authentication:

- Must be implemented in the API layer  
- Must use Application layer abstractions  
- Must not leak external identity provider details into Domain  
- Must issue opaque session identifiers only  
- Must not expose tokens to the frontend  
- Must not store tokens in Domain or Application  

Identity resolution:

- Must be implemented via an Application abstraction  
- Must not depend on infrastructure details  

---

# 7. Error Boundary Governance

Errors must:

- Be shaped at the API boundary  
- Never leak internal exceptions  
- Never expose stack traces  
- Never expose infrastructure details  
- Never expose domain internals  

Domain errors must be mapped to API errors through Application layer boundaries.

---

# 8. Forbidden Architectural Patterns

The following patterns are prohibited:

- Active Record  
- Anemic Domain Model  
- Service Locator  
- Static service access  
- Direct database access from API  
- Domain events triggering infrastructure calls  
- Infrastructure types leaking into Domain or Application  
- Circular dependencies  
- Runtime type scanning for business logic  
- **Manual DI registration of slice services**  
- **Scrutor or suffix-based DI scanning**  
- **Any DI mechanism other than SharedKernel auto‑registration**  

---

# 9. Architecture Enforcement

Architecture is enforced through:

- Automated architecture tests  
- CI dependency graph validation  
- Reviewer enforcement  
- SharedKernel guardrails  
- Static analysis  

No PR may merge if:

- It violates dependency rules  
- It introduces forbidden patterns  
- It weakens architectural boundaries  

Architecture governance is non-negotiable.
