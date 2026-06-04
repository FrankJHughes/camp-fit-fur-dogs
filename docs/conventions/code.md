# Code Conventions

This document defines the coding‑level rules for backend and frontend code.  
It complements the architectural, security, and workflow conventions and ensures code remains consistent, testable, and aligned with system guardrails.

Code conventions describe **how code is written**, not architectural policy or governance.

---

# 1. Backend Conventions

## 1.1 Layer Responsibilities

- **Api** — endpoints, DTO binding, authorization, dispatching commands/queries.  
- **Application** — use‑case orchestration, validation, domain interaction.  
- **Domain** — business rules, aggregates, value objects, domain events.  
- **Infrastructure** — persistence, external integrations, EF Core, repositories, unit of work.  
- **Frank** — cross‑cutting primitives, CQRS abstractions, DI auto‑registration, endpoint discovery, guardrails.

### Layering Rules (Implementation-Level)

- Domain must not depend on Application, Infrastructure, or Api.  
- Application must not depend on Infrastructure or Api.  
- Infrastructure must not depend on Api.  
- Api must not contain business logic.  
- Api may depend on Application, Frank, and Domain primitives only.  
- Frank is the only allowed cross‑cutting dependency.

---

# 2. CQRS Handler Conventions

## 2.1 Placement

- Commands, queries, and handlers live in Application slices.

## 2.2 Responsibilities

Handlers must:

- Validate inputs via validators.  
- Load aggregates or required state.  
- Invoke domain behavior (aggregate methods, domain services).  
- Persist changes via repositories and `IUnitOfWork`.  
- Publish domain events via the dispatcher.

## 2.3 Registration

- Handlers are **auto‑registered** via `[AutoRegister]`.  
- Handlers must not be manually registered in DI.  
- Handlers must not be invoked directly — always use dispatchers.

## 2.4 Prohibitions

- No business rules in handlers.  
- No direct persistence logic (no `DbContext` access).  
- No bypassing the dispatcher pipeline.  
- No returning domain entities — handlers return DTOs or primitives.

---

# 3. Endpoint Conventions

## 3.1 Discovery & Structure

- Endpoints implement `IEndpoint`.  
- Each endpoint exposes `void Map(IEndpointRouteBuilder app)`.  
- Endpoints live alongside slice commands, queries, and DTOs.

## 3.2 Allowed Responsibilities

- DTO binding and shape validation.  
- Authorization checks.  
- Dispatching commands/queries via dispatchers.  
- Returning DTOs or appropriate HTTP results.

## 3.3 Forbidden Responsibilities

- No business logic.  
- No returning domain entities.  
- No Infrastructure references.  
- No accepting identity from request bodies — use `ICurrentUserService`.  
- No direct handler invocation — always use dispatchers.

## 3.4 Routing Rules

- Routes map one‑to‑one to commands or queries.  
- Use explicit mapping inside the endpoint class.  
- Avoid attribute routing and monolithic endpoint files.

---

# 4. Identity, Authentication & Security Conventions

## 4.1 Identity Resolution

- Resolve identity via `ICurrentUserService` only.  
- Application depends on the abstraction; Infrastructure provides the implementation; Api tests provide a fake implementation.

## 4.2 Authentication Flow (Implementation-Level)

- Login initiation endpoint redirects to OIDC provider.  
- Callback endpoint exchanges code, fetches userinfo, resolves identity, issues session cookie.  
- Authentication logic lives in Api + Infrastructure, not Application or Domain.

## 4.3 Session Cookies

- Cookies contain opaque session IDs only.  
- Cookies are HttpOnly.  
- Cookies are Secure in production.  
- Cookies are validated on each request.  
- Cookies must not contain JWTs, tokens, or PII.

## 4.4 Passwords & Hashing

- Use the domain `PasswordHash` value object.  
- No plaintext passwords persisted or logged.  
- Use BCrypt with Frank defaults.

---

# 5. EF Core & Persistence Conventions

## 5.1 Mapping

- Aggregate root maps to a table; Id is the key and not value‑generated.  
- Domain events are ignored by EF Core and not persisted.  
- Use derived configurations for properties, relationships, and indexes.  
- Value objects are mapped as owned types.

## 5.2 Unit of Work

- `DbContext` is the only layer that talks directly to the database.  
- Unit of Work coordinates `SaveChangesAsync` and domain event dispatch.  
- Unit of Work contains no business logic.

## 5.3 Repository Registration

- Repositories are **auto‑registered** via `[AutoRegister]`.  
- Repositories must not be manually registered in DI.  
- Repositories must not expose EF Core types.

## 5.4 Migrations

- Migrations are applied by CI workflows only; tests must not apply migrations.  
- Code must support clean migration runs against empty preview branches.  
- Migrations must be idempotent and preview‑safe.

---

# 6. PR Preview Safety Rules

Code must be safe to run in ephemeral preview environments (Neon + Render).

## 6.1 Requirements

- Expose `/health` for teardown readiness checks.  
- Do not assume persistent DB state; tolerate empty databases.  
- Read connection strings from `ConnectionStrings__DefaultConnection`.  
- Avoid environment‑specific branching.  
- Do not hardcode URLs, credentials, or secrets.

## 6.2 Behavioral Expectations

- Code must tolerate cold starts and transient connection failures.  
- Migrations must be idempotent and safe to run in CI against preview branches.  
- No reliance on local file system state.

---

# 7. Test Seam Conventions

## 7.1 HttpClient Test Seam

- All external HTTP calls must use named HttpClients.  
- Tests replace HttpClient with FakeHttpMessageHandler.  
- No real network calls are allowed in tests.

## 7.2 Hosting Provider Test Seam

- Environment variables are injected via test seam.  
- GitHub artifacts are injected via test seam.  
- Providers must be fully testable without real infrastructure.

## 7.3 Identity Resolver Test Seam

- Identity resolution is abstracted.  
- Tests use FakeIdentityResolver.

## 7.4 Audit Logger Test Seam

- Audit logging is abstracted.  
- Tests use FakeAuditLogger.

---

# 8. Frontend Conventions

## 8.1 Structure

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

---

# Summary

These conventions ensure:

- Clear layering and separation of concerns.  
- Deterministic CI and preview behavior.  
- Preview‑safe code that tolerates ephemeral databases and cold starts.  
- Testable, maintainable backend and frontend slices.  
- Consistent use of Frank for cross‑cutting concerns.  
- Enforcement through guardrails and CI validation.

All contributors must follow these rules.
