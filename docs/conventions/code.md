# Code Conventions

Code conventions define **how code is written** across backend and frontend slices.  
They complement the architecture, workflow, and documentation conventions and ensure consistency, testability, and maintainability.

These conventions describe **implementation‑level rules**, not architectural governance.

Frank provides cross‑cutting primitives, DI auto‑registration for attributed interfaces, endpoint discovery, validator scanning, hosting provider abstractions, and security headers.  
Application, Infrastructure, and Api remain responsible for their own slice‑specific DI.

---

# 1. Backend Code Conventions

The backend follows a strict layering model enforced by guardrails:

- **Domain** — business rules  
- **Application** — use‑case orchestration  
- **Infrastructure** — persistence + external integrations  
- **Api** — endpoints + request/response shaping  
- **Frank** — cross‑cutting primitives and auto‑registration engine

## 1.1 Layer Responsibilities

### Domain
- Aggregates  
- Entities  
- Value objects  
- Domain events  
- Invariants  
- Business rules  

Domain must not depend on Application, Infrastructure, Api, or Frank.

### Application
- Commands, queries, handlers  
- Validation  
- Domain interaction  
- Transaction boundaries  
- Domain event publishing  

Application must not depend on Infrastructure or Api.

### Infrastructure
- EF Core persistence  
- Repositories  
- Unit of Work  
- External integrations  
- Hosting provider implementations  

Infrastructure must not depend on Api.

### Api
- Endpoints  
- DTO binding  
- Authorization  
- Dispatching commands/queries  
- Security headers  
- Endpoint discovery  

Api must not contain business logic.

### Frank
- DI auto‑registration for attributed interfaces  
- Core CQRS services (CommandDispatcher, QueryDispatcher, DomainEventDispatcher)  
- Validator scanning  
- Endpoint discovery  
- Security headers middleware  
- Hosting provider abstractions  
- Environment abstraction  
- Test seams  

Frank must not depend on product code.

---

# 2. CQRS Conventions

## 2.1 Commands and Queries

Commands and queries:

- Live in Application slices  
- Represent use‑case intent  
- Must be immutable  
- Must have validators when input exists  

## 2.2 Handlers

Handlers must:

- Validate inputs  
- Load aggregates or required state  
- Invoke domain behavior  
- Persist changes via repositories + Unit of Work  
- Publish domain events  

Handlers must not:

- Contain business rules  
- Access EF Core directly  
- Access HttpContext  
- Invoke other handlers directly  
- Return domain entities  

Handlers return DTOs or primitives.

## 2.3 Registration

Handlers are auto‑registered via Frank’s DI engine:

- Interfaces marked with `[AutoRegister]` are discovered  
- Handlers must not be manually registered  
- Handlers must not bypass the dispatcher pipeline

---

# 3. Endpoint Conventions

Endpoints implement `IEndpoint` and are discovered automatically by Frank.

## 3.1 Structure

Each endpoint:

- Lives in the Api slice  
- Implements `void Map(IEndpointRouteBuilder app)`  
- Defines routes explicitly  
- Dispatches commands/queries via dispatchers  
- Performs DTO binding and authorization  

## 3.2 Forbidden in Endpoints

Endpoints must not:

- Contain business logic  
- Access Infrastructure directly  
- Return domain entities  
- Read identity from request bodies  
- Invoke handlers directly  

## 3.3 Routing Rules

- One endpoint per command/query  
- No attribute routing  
- No monolithic endpoint files  
- Use explicit mapping inside the endpoint class

---

# 4. Security Headers

Security headers are mandatory for all environments.

Api must:

- Call `AddSecurityHeaders()`  
- Use `SecurityHeadersMiddleware`  

Frank provides the canonical implementation.

Headers include:

- X‑Content‑Type‑Options  
- X‑Frame‑Options  
- X‑XSS‑Protection  
- Referrer‑Policy  
- Permissions‑Policy  
- COOP / COEP / CORP  
- Strict CSP baseline

Guardrails enforce presence.

---

# 5. EF Core & Persistence Conventions

## 5.1 Mapping

- Aggregate root → table  
- Id is not value‑generated  
- Value objects mapped as owned types  
- Domain events ignored by EF Core  
- Configurations live in Infrastructure  

## 5.2 Unit of Work

- Coordinates SaveChangesAsync  
- Dispatches domain events  
- Contains no business logic  

## 5.3 Repositories

- Auto‑registered via `[AutoRegister]`  
- Must not expose EF Core types  
- Must not contain business logic  

## 5.4 Migrations

- Applied by CI only  
- Must be idempotent  
- Must tolerate empty databases  
- Must be preview‑safe  

---

# 6. Hosting Provider Conventions

Hosting providers configure environment‑specific behavior at startup.

## 6.1 Provider Rules

Providers must:

- Use injected abstractions only  
- Never read environment variables directly  
- Never perform HTTP calls directly  
- Never parse JSON or ZIP files directly  
- Never write configuration directly  
- Be fully testable  

## 6.2 Provider Selection

- Providers are evaluated in order  
- First active provider wins  
- Enforced by guardrails  

## 6.3 Render Hosting Provider

Render is the canonical provider for PR Previews.

It uses:

- `IEnvironment`  
- `IRenderPrParser`  
- `IGitHubArtifactClient`  
- `IRenderConfigurationWriter`

Required environment variables:

- `IS_PULL_REQUEST`  
- `RENDER_GIT_REPO_SLUG`  
- `RENDER_SERVICE_NAME`  
- `GITHUB_PAT`

Required artifacts:

- `pr-{n}-db/db-conn.txt`  
- `pr-{n}-frontend/frontend-url.txt`

---

# 7. Test Seam Conventions

## 7.1 HttpClient Seam

- All external HTTP calls use named HttpClients  
- Tests replace HttpClient with FakeHttpMessageHandler  
- No real network calls allowed in tests  

## 7.2 Hosting Provider Seam

- Providers use injected abstractions  
- Tests supply fakes for:
  - IEnvironment  
  - IRenderPrParser  
  - IGitHubArtifactClient  
  - IRenderConfigurationWriter  

## 7.3 Identity Seam

- Identity resolved via `ICurrentUserService`  
- Tests use FakeCurrentUser  

## 7.4 Audit Logging Seam

- Audit logging abstracted  
- Tests use FakeAuditLogger  

---

# 8. Frontend Conventions

The frontend mirrors backend slice structure.

## 8.1 Folder Structure

```
frontend/
  src/
    api/<aggregate>/...
    components/<aggregate>/...
    lib/<aggregate>/...
    hooks/<aggregate>/...
    lib/api/...
    lib/hooks/...
    lib/components/...
    app/...
  test/...
```

## 8.2 Rules

- One slice per aggregate  
- Hooks must not perform fetches directly  
- API layer owns all HTTP calls  
- Components must be pure and testable  
- No business logic in components  
- Tests must not hit real APIs  

---

# Summary

These conventions ensure:

- Strict layering  
- Deterministic hosting behavior  
- Strong security posture  
- Testable, maintainable slices  
- Consistent use of Frank for cross‑cutting concerns  
- Preview‑safe behavior  
- Predictable CI/CD behavior  

All contributors must follow these rules.
