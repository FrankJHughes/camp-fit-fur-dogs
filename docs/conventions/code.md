# Code Conventions

This document defines the coding‑level rules for backend and frontend code.  
It complements the architectural and workflow conventions and ensures code remains consistent, testable, and aligned with the system’s guardrails.

---

# Backend Conventions

## Layer Responsibilities

- **Api** — endpoints, DTO binding, authorization, dispatching commands/queries.  
- **Application** — use‑case orchestration, validation, domain interaction.  
- **Domain** — business rules, aggregates, value objects, domain events.  
- **Infrastructure** — persistence, external integrations, EF Core, repositories, unit of work.  
- **SharedKernel** — cross‑cutting primitives, CQRS abstractions, endpoint discovery, DI conventions, guardrails.

### Layering Rules

- Domain must not depend on Application, Infrastructure, or Api.  
- Application must not depend on Infrastructure or Api.  
- Infrastructure must not depend on Api.  
- Api must not contain business logic.  
- SharedKernel is the only allowed source of cross‑cutting primitives — do not reimplement.

---

# CQRS Handler Conventions

## Placement

- Commands, queries, and handlers live in Application slices.

## Responsibilities

Handlers must:

- Validate inputs via validators.  
- Load aggregates or required state.  
- Invoke domain behavior (aggregate methods, domain services).  
- Persist changes via repositories and `IUnitOfWork`.  
- Publish domain events via the dispatcher.

## Prohibitions

- No business rules in handlers.  
- No direct persistence logic (no `DbContext` access).  
- No bypassing the dispatcher pipeline.  
- No returning domain entities — handlers return DTOs or primitives.

---

# Endpoint Conventions

## Discovery & Structure

- Endpoints implement `IEndpoint`.  
- Each endpoint exposes `void Map(IEndpointRouteBuilder app)`.  
- Endpoints live alongside slice commands, queries, and DTOs.

## Allowed Responsibilities

- DTO binding and shape validation.  
- Authorization checks.  
- Dispatching commands/queries.  
- Returning DTOs or appropriate HTTP results.

## Forbidden Responsibilities

- No business logic.  
- No returning domain entities.  
- No Infrastructure references.  
- No accepting identity from request bodies — use `ICurrentUserService`.

## Routing Rules

- Routes map one‑to‑one to commands or queries.  
- Use explicit mapping inside the endpoint class.  
- Avoid attribute routing and monolithic endpoint files.

---

# Identity & Security

## Identity

- Resolve identity via `ICurrentUserService` only.  
- Application depends on the abstraction; Infrastructure provides the implementation; Api tests provide a test implementation.

## Passwords & Hashing

- Use the domain `PasswordHash` value object.  
- No plaintext passwords persisted or logged.  
- Use BCrypt with SharedKernel defaults.

## Authorization

- Authorization checks belong in endpoints and, when necessary, handlers.  
- Centralize policies; avoid duplication.

---

# EF Core & Persistence Conventions

## Mapping

- Aggregate root maps to a table; Id is the key and not value‑generated.  
- Domain events are ignored by EF Core and not persisted.  
- Use derived configurations for properties, relationships, and indexes.

## Unit of Work

- `DbContext` is the only layer that talks directly to the database.  
- Unit of Work coordinates `SaveChangesAsync` and domain event dispatch.  
- Unit of Work contains no business logic.

## Migrations

- Migrations are applied by CI workflows only; tests must not apply migrations.  
- Code must support clean migration runs against empty preview branches.  
- Migrations must be idempotent and preview‑safe.

---

# PR Preview Safety Rules

Code must be safe to run in ephemeral preview environments (Neon + Render).

## Requirements

- Expose `/health` for teardown readiness checks.  
- Do not assume persistent DB state; tolerate empty databases.  
- Read connection strings from `ConnectionStrings__DefaultConnection`.  
- Avoid environment‑specific branching.  
- Do not hardcode URLs, credentials, or secrets.

## Behavioral Expectations

- Code must tolerate cold starts and transient connection failures.  
- Migrations must be idempotent and safe to run in CI against preview branches.  
- No reliance on local file system state.

---

# Frontend Conventions

## Structure

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

## Layer Responsibilities

- `api/` — server‑call functions (one per slice).  
- `components/` — presentational React components.  
- `lib/` — pure logic and action functions.  
- `hooks/` — behavioral hooks for UI state and API calls.  
- `app/` — Next.js routing layer.

## Naming

- `api/`: `camelCaseVerb.ts` (e.g., `getDogProfile.ts`).  
- `components/`: `PascalCase.tsx` (e.g., `DogProfileCard.tsx`).  
- `lib/`: `camelCase.ts` (e.g., `dogProfileActions.ts`).  
- `app/`: Next.js conventions (`page.tsx`, `layout.tsx`).

## Shared Infra

- `lib/api/` — API client, `CommandResult`, `QueryResult`.  
- `lib/hooks/` — shared hooks (`useApiQuery`, `useCommand`).  
- `lib/components/` — cross‑aggregate components.

## Dynamic Route Safety

- When scripting file operations on dynamic route folders (e.g., `[id]`), use literal path semantics (`-LiteralPath` in PowerShell) to avoid globbing issues.

## Form Naming & Style Conventions

### File Naming
Form‑related files must follow these naming conventions:

- Schema files:
  ```
  <feature>Schema.ts
  ```
  Example: `createAccountSchema.ts`

- Validator files:
  ```
  validate<Feature>Form.ts
  ```
  Example: `validateAccountForm.ts`

- Form components:
  ```
  <Feature>Form.tsx
  ```
  Example: `AccountForm.tsx`

- Wrapper components:
  ```
  <Feature>Form.tsx
  ```
  Example: `CreateAccountForm.tsx`

- Test files:
  ```
  <Feature>Form.test.tsx
  ```
  Example: `AccountForm.test.tsx`

### Import Style
- Always import schemas from `src/lib/<domain>/<feature>Schema.ts`.
- Always import inferred types from the schema file.
- Never define duplicate types in components or API clients.

### Component Style
- Form components must be functional components.
- Use RHF’s `useForm` with explicit generic types.
- Use controlled or uncontrolled inputs consistently per RHF guidelines.
- Use `<FormField>` and `<FieldError>` for all field rendering.

### Error Style
- Field‑level errors must use `role="alert"`.
- Form‑level errors must use a dedicated `<FieldError>` with a stable ID.
- Error IDs must be deterministic for `aria-describedby`.

### Test File Style
- Tests must use React Testing Library + userEvent.
- Tests must assert against schema‑defined messages.
- Tests must not duplicate validation logic.

---

# Testing Conventions

## Principles

- Tests must be fast, deterministic, and isolated.  
- Follow red–green–refactor discipline.

## Backend

- Unit tests: domain logic, handlers, small infra pieces.  
- Integration tests: persistence, endpoint behavior, cross‑layer flows.  
- Infrastructure tests run against the fresh preview DB and must not apply migrations.

## Frontend

- Use Vitest v3, React Testing Library, and `@testing-library/jest-dom`.  
- Component tests focus on behavior, not implementation details.  
- `test/` mirrors `src/`.

## Naming

- Tests should describe behavior and read like specifications.

---

# Tooling & Scripts

## Shell & Encoding

- Primary shell: PowerShell.  
- Generated files: `utf8NoBOM`.  
- Scripts must be copy‑pasteable and idempotent.

## File Generation

- Use single‑quoted here‑strings for file generation.  
- Avoid nested here‑strings, backticks, and problematic quoting sequences.

---

# Enforcement & Guardrails

- Guardrail tests validate layering and dependency rules.  
- Document which guardrail tests exist and where they run.  
- When a convention is enforced by SharedKernel or tests, reference the exact type or test name in code comments or docs.  
- CI dependency graph enforces correct test ordering and architectural consistency.

---

# Summary

These conventions ensure:

- Clear layering and separation of concerns.  
- Deterministic CI and preview behavior.  
- Preview‑safe code that tolerates ephemeral databases and cold starts.  
- Testable, maintainable backend and frontend slices.  
- Consistent use of SharedKernel for cross‑cutting concerns.  
- Enforcement through guardrails and CI validation.

All contributors must follow these rules.
