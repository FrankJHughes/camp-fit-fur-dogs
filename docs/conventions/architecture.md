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

`````text
Api → Application → Domain
Application → Infrastructure (persistence, external concerns)
`````

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

---

# Frontend Form Architecture (Updated)

All forms follow a unified architecture built on:

- **FormCommand** — the canonical submit interface  
- **useFormStateMachine** — deterministic state, validation, and error merging  
- **FormField** — accessibility‑correct field rendering  
- **Zod** — schema‑driven validation

This replaces all legacy `submit()` patterns.

## FormCommand Contract

`````ts
export interface FormCommand<T> {
  run: (values: T) => Promise<void>;
  errors?: Record<string, string>;
  error?: string;
  isSubmitting: boolean;
}
`````

### Rules

- `run` is the only allowed submit API  
- `errors` maps backend field errors  
- `error` maps backend form‑level errors  
- `isSubmitting` disables UI and prevents double submits  
- Commands must not mutate values or reshape backend responses  

---

## Form State Machine

All forms must use `useFormStateMachine`.

Responsibilities:

- Run client‑side validation  
- Merge backend errors into `displayErrors`  
- Track submission state  
- Provide `update`, `handleSubmit`, and `values`  
- Ensure deterministic behavior across slices  

Forms must not manage their own error or submission state.

---

## Field Rendering

All fields must use `FormField`:

`````tsx
<FormField label="Name" name="name" error={displayErrors.name}>
  {(fieldProps) => (
    <input
      {...fieldProps}
      value={values.name}
      onChange={update('name')}
      disabled={isSubmitting}
    />
  )}
</FormField>
`````

### Rules

- Always spread `fieldProps`  
- Never override `id` without checking for duplicates  
- All fields must support `aria-invalid`, `aria-describedby`, and `role="alert"` via `FormField`  

---

## Validation

- All validation schemas live in `src/lib/<domain>/<feature>Schema.ts`  
- Validation must use Zod  
- Validation messages must originate from the schema  
- Validation must return `{ field: message }` maps  
- Forms must not implement validation logic  

---

## Backend Contract Alignment

Backend error envelopes:

`````json
{ "errors": { "field": "message" } }
{ "error": "form-level message" }
`````

Backend success envelope for create-account:

`````json
{ "customerId": "guid" }
`````

### Rules

- Frontend must not expect additional fields  
- Frontend must normalize phone numbers to digits‑only  
- Frontend must treat success as “account created” and handle navigation accordingly  

---

# Hosting & Deployment Architecture

(unchanged)

---

# Summary

This document codifies:

- Strict backend layering  
- Consistent CQRS patterns  
- Deterministic frontend form architecture  
- Canonical FormCommand contract  
- Schema‑driven validation  
- Unified error envelope handling  
- Contract‑aligned success behavior  
- Deterministic PR Preview pipeline  
- Script‑first, test‑enforced guardrails

All code, scripts, and documentation must follow these conventions.
