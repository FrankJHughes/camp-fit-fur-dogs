# Architecture Governance

This document defines the architectural boundaries, dependency rules, and enforcement mechanisms for all products in the repository.  
It complements (but does not duplicate) code conventions, security governance, and operational governance.

Architecture governance ensures:

- Stable, predictable system structure  
- Clear separation of responsibilities  
- Enforceable dependency boundaries  
- Long‑term maintainability  
- Zero architectural drift  
- Consistent behavior across all products  

Architecture is a contract. Violations are defects.

---

# 1. Architectural Principles

All products must follow these principles:

- **Separation of Concerns** — each layer has a single purpose  
- **Explicit Dependencies** — no hidden or implicit coupling  
- **Inversion of Control** — high‑level policies depend on abstractions  
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
All layers → Frank
```

Frank is the **only allowed cross‑cutting dependency**.  
Frank provides primitives, DI auto‑registration, endpoint discovery, validator scanning, hosting provider abstractions, environment abstraction, security headers, and test seams.

## 2.1 Domain Layer

The Domain layer:

- Owns business rules  
- Owns aggregates, entities, value objects  
- Owns domain events  
- Must not depend on any other layer  
- Must not reference infrastructure, API, or external libraries except:  
  - BCL  
  - Frank primitives  

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
- Defines readers and application‑level abstractions  
- Dispatches domain events  
- Depends only on Domain and Frank  
- Participates in DI via `[AutoRegister]`  

Forbidden:

- Direct database access  
- HTTP clients  
- UI logic  
- Hosting logic  
- Authentication protocol logic  
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
- Uses Frank auto‑registration for DI participation  
- Uses Frank hosting provider abstractions:
  - `IEnvironment`  
  - `IRenderPrParser`  
  - `IGitHubArtifactClient`  
  - `IRenderConfigurationWriter`

Forbidden:

- Exposing infrastructure types to API  
- Implementing business rules  
- Creating domain entities outside factories  
- Manual DI registration of slice services  
- Direct environment variable access  
- Direct HTTP, JSON, or ZIP handling inside hosting providers  

## 2.4 API Layer

The API layer:

- Exposes HTTP endpoints  
- Performs request/response mapping  
- Performs authentication and authorization  
- Performs validation  
- Dispatches to Application layer  
- Uses Frank endpoint discovery  
- Applies Frank security headers middleware  
- Uses Frank error boundary helpers  

Forbidden:

- Business logic  
- Database access  
- External service calls  
- Domain mutation outside Application layer  
- Direct handler invocation (must use dispatchers)  
- Infrastructure dependencies  
- Bypassing Frank middleware (security headers, correlation, error boundary)

---

# 3. Dependency Direction Governance

Allowed dependencies:

- Api → Application  
- Api → Domain (primitives only)  
- Api → Frank  
- Application → Domain  
- Application → Frank  
- Infrastructure → Application  
- Infrastructure → Domain  
- Infrastructure → Frank  

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

Frank enforces:

- Handler auto‑registration  
- Dispatcher pipeline behavior  
- Validation integration  
- Error boundary integration  

---

# 5. Hosting Provider Governance

Hosting providers:

- Run before DI is built  
- Must be deterministic  
- Must validate required configuration  
- Must fail fast on misconfiguration  
- Must not silently skip required configuration  
- Must not depend on product‑specific logic  
- Must use injected abstractions only  
- Must not read environment variables directly  
- Must not perform HTTP directly  
- Must not parse JSON or ZIP directly  
- Must not write configuration directly  

Only Frank may define hosting provider infrastructure.

The Render hosting provider must use:

- `IEnvironment`  
- `IRenderPrParser`  
- `IGitHubArtifactClient`  
- `IRenderConfigurationWriter`

Provider selection rules:

- Providers are evaluated in order  
- First active provider wins  
- All others are skipped  
- Enforced by guardrails  

---

# 6. Authentication Architecture Governance

Authentication:

- Must be implemented in the API layer  
- Must use Application layer abstractions  
- Must not leak external identity provider details into Domain  
- Must issue opaque session identifiers only  
- Must not expose tokens to the frontend  
- Must not store tokens in Domain or Application  
- Must integrate with Frank’s authentication seams  

Identity resolution:

- Must be implemented via an Application abstraction  
- Must not depend on infrastructure details  
- Must not accept identity from request bodies  
- Must use Frank’s identity seam  

## 6.1 Authentication Callback Architecture Governance (NEW)

The authentication callback flow must follow a **three‑layer architecture**:

### **1. Frank Pipeline (Protocol Layer)**  
- Handles OIDC protocol logic  
- Exchanges authorization codes  
- Extracts claims  
- Normalizes provider output  
- Contains **no business logic**

### **2. Application Pipeline (Business Layer)**  
- Resolves identity  
- Creates or loads customer  
- Creates session  
- Produces redirect URL  
- Produces cookie value  
- Contains **no protocol logic**

### **3. API Callback Endpoint (Infrastructure Boundary)**  
The API endpoint must:

- Extract the `code` query parameter  
- Throw a shaped `400` error if missing  
- Invoke Frank pipeline  
- Invoke Application pipeline  
- Issue the session cookie  
- Redirect to the Application pipeline’s URL  

The API endpoint must not:

- Perform protocol logic  
- Perform business logic  
- Construct aggregates  
- Perform persistence  
- Compute redirect URLs  
- Compute cookie values  

This separation is **architecturally mandatory**.

---

# 7. Error Boundary Governance

Errors must:

- Be shaped at the API boundary  
- Never leak internal exceptions  
- Never expose stack traces  
- Never expose infrastructure details  
- Never expose domain internals  

Error shaping must use:

- Frank error boundary helpers  
- Frank correlation middleware  
- Frank security headers middleware  

Domain errors must be mapped to API errors through Application layer boundaries.

## 7.1 Callback Error Governance (NEW)

The callback endpoint must:

- Return `400` for missing/invalid `code`  
- Allow pipeline exceptions to flow into Frank’s error boundary  
- Never leak identity provider errors  
- Never leak tokens or claims  

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
- **Scrutor or suffix‑based DI scanning**  
- **Any DI mechanism other than Frank auto‑registration**  
- Hosting providers accessing environment variables directly  
- Hosting providers performing HTTP, JSON, or ZIP operations directly  
- API bypassing Frank middleware  

## 8.1 Callback‑Specific Forbidden Patterns (NEW)

Forbidden:

- Protocol logic in Application  
- Business logic in Frank  
- Cookie issuance outside API  
- Redirect computation outside Application pipeline  
- Identity provider calls in API or Application  
- Token handling outside Frank pipeline  
- Returning provider errors  
- Returning raw exceptions  

---

# 9. Architecture Enforcement

Architecture is enforced through:

- Automated architecture tests  
- CI dependency graph validation  
- Reviewer enforcement  
- Frank guardrails  
- Static analysis  
- Hosting provider seam tests  
- Endpoint discovery tests  
- Security header enforcement tests  
- Authentication callback pipeline tests (NEW)

No PR may merge if:

- It violates dependency rules  
- It introduces forbidden patterns  
- It weakens architectural boundaries  
- It bypasses Frank’s cross‑cutting infrastructure  
- It violates callback architecture rules (NEW)

Architecture governance is non‑negotiable.
