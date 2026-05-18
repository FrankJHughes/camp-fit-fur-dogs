````markdown
# Architecture Conventions

This document defines the architectural boundaries, hosting model, layering rules, and cross‑cutting conventions for the Camp Fit Fur Dogs system.  
It is the canonical source of truth for architectural decisions and must be followed by all code, documentation, and automation.

## Related Documents

- `workflow.md` — CI/CD and preview lifecycle behavior  
- `Code.md` — implementation‑level rules, preview‑safe behavior, script‑first patterns  
- `Docs.md` — documentation structure and authoring rules  
- `adr/` — Architecture Decision Records (canonical decision history)

---

# DDD Layered Architecture

The backend follows a layered architecture with strict boundaries.

- **Api** depends on **Application** and **Infrastructure**  
- **Application** depends on **Domain**  
- **Infrastructure** depends on **Application** and **Domain**  
- All layers may depend on **SharedKernel**

**Rules**

- Domain must not depend on Application, Infrastructure, or Api  
- Application must not depend on Infrastructure or Api  
- Infrastructure must not depend on Api  
- Api must not contain business logic  
- SharedKernel is the only allowed cross‑layer dependency

**Primary flow**

```text
Api → Application → Domain
Application → Infrastructure (persistence, external concerns)
```

Guardrail tests enforce these rules.

---

# CQRS Pipelines

Commands and queries follow consistent pipeline behavior.

**Commands**

- Implement `ICommand<TResponse>`  
- Handled by `ICommandHandler<TCommand, TResponse>`  
- Dispatched via `ICommandDispatcher`

**Queries**

- Implement `IQuery<TResponse>`  
- Handled by `IQueryHandler<TQuery, TResponse>`  
- Dispatched via `IQueryDispatcher`

**Dispatcher behavior**

- Resolve validators  
- Run validation  
- Throw on validation failure  
- Invoke handler  
- Ensure consistent pipeline behavior

Direct handler invocation is not allowed.

---

# Domain Model

The domain model uses:

- Value Objects — immutable, equality by components  
- Entities — identity and equality by Id  
- Aggregate Roots — consistency boundaries, raise domain events  
- Aggregate Ids — value objects wrapping a Guid

**Rules**

- Domain code must not depend on Application, Infrastructure, or Api  
- Business rules live in aggregates, value objects, and domain services  
- Domain events are raised inside aggregates and dispatched after persistence  
- Domain types must not cross the Api boundary

---

# Repositories & Unit of Work

Repositories provide persistence operations for aggregates:

- Get by Id  
- Add  
- Update  
- Delete  

`IUnitOfWork` coordinates saving changes.

**Infrastructure responsibilities**

- Implement repositories using EF Core  
- Implement Unit of Work using `DbContext`  
- Ensure `CommitAsync` is called after successful command handling

Domain does not know about repositories or unit of work.

---

# EF Core Conventions

Infrastructure provides base configurations for aggregates.

- Aggregate root configuration maps the aggregate to a table  
- Id is configured as the key and is not value‑generated  
- Domain events are ignored and never persisted  
- Derived configurations map properties, relationships, and indexes

Migrations are applied only by CI; tests must not apply migrations.

---

# SharedKernel Architecture

SharedKernel contains cross‑cutting building blocks:

- CQRS abstractions and dispatchers  
- Validation pipeline integration  
- DI conventions and auto‑registration helpers  
- Domain primitives (`ValueObject`, `Entity`, `AggregateRoot`, `AggregateId`)  
- Domain event abstractions and dispatcher  
- EF Core base classes for aggregate configuration and unit of work  
- Endpoint discovery infrastructure  
- `SharedKernelOptions`  
- Architecture guardrails

**Rules**

- Product layers must not reimplement these building blocks  
- SharedKernel is the only allowed cross‑layer dependency

---

# SharedKernel Enforcement & Guardrail Tests

Guardrail tests enforce architecture boundaries:

- `tests/SharedKernel.Tests`  
- `tests/SharedKernel.Api.Tests`  
- `tests/SharedKernel.Infrastructure.EntityFrameworkCore.Tests`  
- `tests/CampFitFurDogs.Domain.Tests`  
- `tests/CampFitFurDogs.Application.Tests`

Any architectural change requires an ADR and updates to this document.

---

# Endpoint Discovery

Api endpoints implement `IEndpoint` and define a `Map` method.

**Behavior**

- SharedKernel.Api scans assemblies for `IEndpoint` implementations  
- Api assembly is registered for endpoint discovery  
- All discovered endpoints are instantiated and mapped at startup

Endpoints must:

- Use CQRS dispatchers  
- Resolve identity from the current user service  
- Avoid leaking domain entities

---

# Frontend Architecture

The frontend mirrors backend aggregate grouping.

**Structure**

- `layer/aggregate/filename`

**Layers**

- `api/` — server‑call functions  
- `components/` — presentational components  
- `lib/` — pure logic  
- `hooks/` — behavioral hooks  
- `app/` — Next.js routing

**Shared infrastructure**

- `lib/api/`  
- `lib/hooks/`  
- `lib/components/`

**Rules**

- Slice subfolders appear only when an aggregate accumulates 10+ files  
- `test/` mirrors `src/` exactly

## Form Architecture (RHF + Zod)

All forms in the application follow a unified architecture built on **React Hook Form (RHF)** and **Zod**. This ensures deterministic validation, consistent error handling, and predictable behavior across all vertical slices.

### Schema Location
Every form must define its validation schema in:

```
src/lib/<domain>/<feature>Schema.ts
```

Examples:
- `src/lib/account/createAccountSchema.ts`
- `src/lib/dog/registerDogSchema.ts`

Schemas must use lowercase camelCase naming.

### Validation Rules
- All validation logic must live in the schema file.
- Form components must not implement validation logic.
- Validation messages must be defined exclusively in the schema.
- Validation uses `safeParse` and is flattened into a `{ field: message }` map.
- Types must be inferred from the schema using `z.infer`.

### Form Component Responsibilities
Form components must:
- Render fields and labels
- Call a validator (e.g., `validateAccountForm`) which wraps the schema
- Display field‑level and form‑level errors
- Accept `errors` and `isSubmitting` from the caller
- Never define their own types or validation rules

### Error Handling
- Client‑side and server‑side errors are merged using `useFormErrors`.
- Field‑level errors must support:
  - `aria-invalid`
  - `aria-describedby`
  - `role="alert"`
- Form‑level errors must be rendered via `<FieldError id="error-form" />`.

### Vertical Slice Integration
Each vertical slice must include:
- A schema (`<feature>Schema.ts`)
- A validator (`validate<Feature>Form.ts`)
- A form component (`<Feature>Form.tsx`)
- A wrapper component that adapts values → API command
- A page that wires the wrapper to `useApiCommand`

### Architectural Guarantees
- All forms follow the same structure.
- All validation is deterministic.
- All error messages originate from a single source of truth.
- Tests assert against schema‑defined messages.

---

# Hosting & Deployment Architecture

The system uses a cloud‑hosted model with independently deployable units.

---

## API Hosting (Render)

- Dockerized .NET 10 Web Service  
- HTTPS termination by Render  
- Teardown probe: `/api/health` → expect `404`  
- Readiness probe: `/api/dogs` → expect `200`, `400`, or `401`  
- Stateless; all state in Neon  
- Cold‑start behavior requires readiness polling

---

## Database Hosting (Neon)

- PostgreSQL  
- Persistent production branch  
- Ephemeral preview branches (`pr-<number>`)  
- SSL‑required connections  
- EF Core + SharedKernel abstractions only

---

## Frontend Hosting (Vercel)

- Next.js application  
- Uses `NEXT_PUBLIC_API_URL`  
- Production deploys on push to `main`  
- Preview deploys per PR

---

# PR Preview Architecture

Preview environments are **per‑PR**, fully ephemeral stacks.

Key principles:

- `render-preview` label controls API preview lifecycle  
- Neon branches and Vercel previews created/destroyed by CI  
- Preview DBs are ephemeral

---

## Preview Context

Computes:

- `common_prefix = "pr-<number>"`  
- Neon branch name  
- API base URL  
- Artifact names  
- Flags: `should_destroy`, `should_deploy`

---

## Destroy Phase (Architecture View)

Executed when `should_destroy == true`.

- Remove `render-preview` label  
- Wait for teardown (`/api/health` → `404`)  
- Delete artifacts  
- Delete Neon branch  
- Delete Vercel previews

---

## Deploy Phase (Architecture View)

Executed when `should_deploy == true`.

- Deploy fresh frontend  
- Deploy fresh DB  
- Deploy fresh API  
- Run integration tests

Readiness/teardown rules are defined in `workflow.md`.

---

# Security & Secrets

- Secrets stored in GitHub Secrets  
- Connection strings passed via environment variables  
- Artifacts containing secrets must be treated as sensitive  
- No secrets may be logged or committed

---

# Script‑First Compatibility

All operational behavior must be:

- Expressible as scripts or composite actions  
- Runnable locally  
- Free of environment‑specific assumptions

Inline shell is allowed only for trivial glue.

---

# Operational Guardrails

- Migrations applied only by CI  
- Preview DBs are ephemeral  
- Label controls Render preview lifecycle  
- Readiness checks must use configured endpoints  
- Infrastructure tests must be deterministic  
- CI path filters must remain aligned with solution structure  
- Any hosting/preview change must update:
  - `architecture.md`  
  - `workflow.md`  
  - ADRs

---

# ADR References

Significant architectural decisions must be captured under `adr/`.

---

# Summary

This document codifies:

- Strict layering and DDD guardrails  
- Consistent CQRS patterns  
- EF Core conventions  
- SharedKernel as the canonical cross‑cutting library  
- Deterministic PR Preview pipeline  
- Accurate readiness/teardown rules  
- CI workflows aligned with code changes  
- Script‑first, test‑enforced guardrails

All code, scripts, and documentation must follow these conventions.
````
