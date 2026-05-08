# Code Conventions

This document defines the coding‑level rules for backend and frontend code.  
It complements the architectural and workflow conventions and ensures code remains consistent, testable, and aligned with the system’s guardrails.

---

## Backend Conventions

### Layer responsibilities
- **Api** — endpoints, DTO binding, authorization, dispatching commands/queries.  
- **Application** — use‑case orchestration, validation, domain interaction.  
- **Domain** — business rules, aggregates, value objects, domain events.  
- **Infrastructure** — persistence, external integrations, EF Core, repositories, unit of work.  
- **SharedKernel** — cross‑cutting primitives, CQRS abstractions, endpoint discovery, DI conventions.

**Rules**
- Domain must not depend on Application, Infrastructure, or Api.  
- Application must not depend on Infrastructure or Api.  
- Infrastructure must not depend on Api.  
- Api must not contain business logic.  
- Prefer SharedKernel for cross‑cutting primitives; do not reimplement.

---

## CQRS Handler Conventions

**Placement**
- Commands, queries, and handlers live in Application slices.

**Handler responsibilities**
- Validate inputs via validators.  
- Load aggregates or required state.  
- Invoke domain behavior (aggregate methods, domain services).  
- Persist changes via repositories and `IUnitOfWork`.  
- Publish domain events via the dispatcher.

**Prohibitions**
- Handlers must not contain business rules.  
- Handlers must not perform low‑level persistence logic directly.  
- Do not bypass the dispatcher pipeline.

---

## Endpoint Conventions

**Discovery and structure**
- Endpoints are slice‑scoped and implement `IEndpoint`.  
- Each endpoint exposes `void Map(IEndpointRouteBuilder app)`.  
- Endpoints live alongside slice commands, queries, and DTOs.

**Allowed responsibilities**
- DTO binding and shape validation.  
- Authorization checks.  
- Dispatching commands/queries.  
- Returning DTOs or appropriate HTTP results.

**Forbidden responsibilities**
- No business logic in endpoints.  
- Do not return domain entities.  
- Do not reference Infrastructure directly.  
- Do not accept identity from request bodies — use `ICurrentUserService`.

**Routing**
- Routes map one‑to‑one to commands or queries.  
- Use explicit mapping inside the endpoint class.  
- Avoid attribute routing and monolithic endpoint files.

---

## Identity and Security

**Identity**
- Resolve identity via `ICurrentUserService` only.  
- Application depends on the abstraction; Infrastructure provides the implementation; Api tests provide a test implementation.

**Passwords and hashing**
- Use the domain `PasswordHash` value object for hashing and verification.  
- No plaintext passwords persisted or logged.  
- Use BCrypt with a work factor consistent with SharedKernel defaults.

**Authorization**
- Authorization checks belong in endpoints and, when necessary, in handlers.  
- Centralize policies; avoid duplicating security logic.

---

## EF Core and Persistence Conventions

**Mapping**
- Aggregate root maps to a table; Id is the key and not value‑generated.  
- Domain events are ignored by EF Core and not persisted.  
- Use derived configurations to map properties, relationships, and indexes.

**Unit of Work**
- `DbContext` is the only layer that talks directly to the database.  
- Unit of Work coordinates `SaveChangesAsync` and domain event dispatch after persistence.  
- Unit of Work contains no business logic.

**Migrations**
- Migrations are applied by CI workflows only; tests must not apply migrations.  
- Code must support clean migration runs against empty preview branches.

---

## PR Preview Safety Rules

Code must be safe to run in ephemeral preview environments (Neon + Render).

**Requirements**
- Expose `/health` for teardown readiness checks.  
- Do not assume persistent DB state; tolerate empty databases.  
- Read connection strings from `ConnectionStrings__DefaultConnection`.  
- Avoid environment‑specific branching in code.  
- Do not hardcode URLs, credentials, or secrets.

**Behavioral expectations**
- Code must tolerate cold starts and transient connection failures.  
- Migrations must be idempotent and safe to run in CI against preview branches.

---

## Frontend Conventions

**Structure**
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

**Layer responsibilities**
- `api/` — server‑call functions (one per slice).  
- `components/` — presentational React components.  
- `lib/` — pure logic and action functions.  
- `hooks/` — behavioral hooks for UI state and API calls.  
- `app/` — Next.js routing layer.

**Naming**
- `api/`: `camelCaseVerb.ts` (e.g., `getDogProfile.ts`).  
- `components/`: `PascalCase.tsx` (e.g., `DogProfileCard.tsx`).  
- `lib/`: `camelCase.ts` (e.g., `dogProfileActions.ts`).  
- `app/`: follow Next.js conventions (`page.tsx`, `layout.tsx`).

**Shared infra**
- `lib/api/` — API client, `CommandResult`, `QueryResult`.  
- `lib/hooks/` — shared hooks (`useApiQuery`, `useCommand`).  
- `lib/components/` — cross‑aggregate components.

**Dynamic route safety**
- When scripting file operations on dynamic route folders (e.g., `[id]`), use literal path semantics (`-LiteralPath` in PowerShell) to avoid globbing issues.

---

## Testing Conventions

**Principles**
- Tests must be fast, deterministic, and isolated.  
- Follow red–green–refactor discipline.

**Backend**
- Unit tests: domain logic, handlers, small infra pieces.  
- Integration tests: persistence, endpoint behavior, cross‑layer flows.  
- Infrastructure tests run against the fresh preview DB and must not apply migrations.

**Frontend**
- Use Vitest v3, React Testing Library, and `@testing-library/jest-dom`.  
- Component tests focus on behavior, not implementation details.  
- `test/` mirrors `src/`.

**Naming**
- Tests should describe behavior and read like specifications.

---

## Tooling and Scripts

**Shell and encoding**
- Primary shell: PowerShell.  
- Generated files: `utf8NoBOM`.  
- Scripts must be copy‑pasteable and idempotent.

**File generation**
- Use single‑quoted here‑strings for file generation.  
- Avoid nested here‑strings, backticks, and problematic quoting sequences.

---

## Enforcement and Guardrails

- Guardrail tests validate layering and dependency rules.  
- Document which guardrail tests exist and where they run.  
- When a convention is enforced by SharedKernel or tests, reference the exact type or test name in code comments or docs.

---

## Summary

These conventions ensure:

- Clear layering and separation of concerns.  
- Deterministic CI and preview behavior.  
- Preview‑safe code that tolerates ephemeral databases and cold starts.  
- Testable, maintainable backend and frontend slices.  
- Consistent use of SharedKernel for cross‑cutting concerns.

All contributors must follow these rules.
