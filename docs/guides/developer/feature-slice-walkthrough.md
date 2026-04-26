# Adding a New Feature Slice

Step-by-step walkthrough for adding a vertical slice to Camp Fit Fur Dogs. Covers aggregate setup, command (write) slices, and query (read) slices using TDD red-green-refactor across all layers — backend through frontend.

> **What is a slice?** One command or one query, vertical through all layers, that produces a visible result. One verb, one noun. Not a user journey.
>
> ✅ `RegisterDog` (command) · ✅ `GetDogProfile` (query) · ❌ "Dog Management" (feature area)

---

## Related Documents

| Document | Relevance |
|----------|-----------|
| [ADR Index](../../adr/README.md) | Architecture decision records |
| [Folder Structure](folder-structure.md) | Where files go |
| [Abstractions Contract](abstractions-contract.md) | Public Application API shape |
| [DI Conventions](di-conventions.md) | Auto-registration rules |
| [Purity Rules](purity-rules.md) | Cross-layer constraints |
| [Frontend Testing](frontend-testing.md) | Vitest + React Testing Library conventions |

Key ADRs: **ADR-0002** (DDD layer boundaries), **ADR-0011** (CQRS pipeline separation), **ADR-0015** (convention-based auto-registration), **ADR-0020** (endpoint auto-discovery), **ADR-0021** (query-side reader isolation).

---

## Establishing a New Aggregate (Domain Layer)

The Domain layer belongs to the **aggregate**, not to any individual slice. When the aggregate already exists, skip this section — jump directly to the command or query slice.

Perform this step only when introducing a **new aggregate root** for the first time.

### RED — write the entity test first

```
tests/CampFitFurDogs.Domain.Tests/<Feature>/<Entity>Tests.cs
```

Test creation with valid inputs, invariant violations, and domain event emission.

### GREEN — add production code

```
src/CampFitFurDogs.Domain/<Feature>/
    <Entity>.cs                        # aggregate root
    <ValueObject>.cs                   # one file per value object
    <Entity><Action>DomainEvent.cs     # domain event
```

### Rules

- Entities enforce invariants — no invalid state is constructible.
- Value objects validate in their factory method and are immutable.
- Domain events are raised inside the entity's mutation method.
- Subsequent command slices on the same aggregate may add mutation methods and domain events to the existing entity — they do not recreate it.

---

## Command Slice (Write Path)

A command slice mutates state. The full-stack runtime flow:

```
Page → API Client → Endpoint → CommandDispatcher → Validator → CommandHandler → Repository → Database
```

**Prerequisite:** The aggregate root must already exist in the Domain layer (see above).

**TDD order: Application → Infrastructure → API → Frontend.**

### 1. Application (RED → GREEN)

**RED** — write the handler test:

```
tests/CampFitFurDogs.Application.Tests/<Feature>/<UseCase>/<UseCase>CommandHandlerTests.cs
```

Mock the repository. Assert the handler persists through the repository, dispatches domain events, and calls `IUnitOfWork.CommitAsync()`.

**GREEN** — add Abstractions (public API):

```
src/CampFitFurDogs.Application/Abstractions/<Feature>/
    <UseCase>Command.cs        # implements ICommand
    <UseCase>Result.cs         # return type
```

Then the handler and validator:

```
src/CampFitFurDogs.Application/<Feature>/<UseCase>/
    <UseCase>CommandHandler.cs       # implements ICommandHandler<TCommand>
    <UseCase>CommandValidator.cs     # implements IValidator<TCommand>
```

**Rules:**

- Handlers inject `IUnitOfWork` and call `CommitAsync()` after repository operations.
- Handlers dispatch domain events via `IDomainEventDispatcher`.
- Validators run before the handler — the dispatcher pipeline guarantees ordering.
- All types auto-register by naming convention (ADR-0015).
- If this is the first command slice for the aggregate, add the repository interface to Abstractions now:

```
src/CampFitFurDogs.Application/Abstractions/<Feature>/
    I<Entity>Repository.cs     # repository contract
```

### 2. Infrastructure (RED → GREEN)

**RED** — write the repository integration test:

```
tests/CampFitFurDogs.Infrastructure.Tests/<Feature>/<Entity>RepositoryTests.cs
```

Uses `PostgresFixture` (Testcontainers) for real EF Core against Postgres.

**GREEN** — add production code:

```
src/CampFitFurDogs.Infrastructure/<Feature>/
    <Entity>Repository.cs          # implements I<Entity>Repository
    <Entity>Configuration.cs       # implements IEntityTypeConfiguration<T>
```

**Rules:**

- Access entities via `_db.Set<T>()` — never add `DbSet<T>` properties to `AppDbContext`.
- Entity configurations auto-register via `ApplyConfigurationsFromAssembly`.
- Repositories auto-register by naming convention (ADR-0015).
- If the repository and configuration already exist (subsequent command slice on the same aggregate), skip this step or extend the existing repository with new methods.

### 3. API (RED → GREEN)

**RED** — write the endpoint integration test:

```
tests/CampFitFurDogs.Api.Tests/<Feature>/<UseCase>EndpointTests.cs
```

Uses `WebApplicationFactory<Program>` for full HTTP pipeline testing.

**GREEN** — add production code:

```
src/CampFitFurDogs.Api/<Feature>/
    <UseCase>Endpoint.cs       # implements IEndpoint
    <UseCase>Request.cs        # request DTO (if the endpoint accepts a body)
```

**Rules:**

- Endpoints implement `IEndpoint` with `static abstract void Map(IEndpointRouteBuilder app)` (ADR-0020).
- Endpoints are auto-discovered — no manual registration file.
- Endpoints are thin: map HTTP → command, dispatch, map result → response.
- Identity comes from `ICurrentUserService`, never from the request body.

### 4. Frontend (RED → GREEN)

With the backend API in place, build the UI layer. TDD order within the frontend: **Component → API Client → Page.**

#### 4a. Component (RED → GREEN)

**RED** — write the component test:

```
frontend/test/components/<UseCase>Form.test.tsx
```

Test rendering, user interactions, validation display, and submit callback invocation using React Testing Library.

**GREEN** — add the presentational component:

```
frontend/src/components/<UseCase>Form.tsx
```

#### 4b. API Client (RED → GREEN)

**RED** — write the API client test:

```
frontend/test/api/<useCase>.test.ts
```

Test success and error paths by mocking `fetch`. Assert typed result shape matches the discriminated union.

**GREEN** — add the API client function:

```
frontend/src/api/<useCase>.ts
```

**Rules:**

- Export a typed result interface with discriminated union for success/error states.
- Handle HTTP errors and network errors with typed results — never throw.
- Form data types (e.g., `DogFormData`) live in the API slice file — the component imports from the API slice.
- API functions return `CommandResult` via `toCommandResult()` from `lib/api/commandResult.ts`.

#### 4c. Page (RED → GREEN)

**RED** — write the page test:

```
frontend/test/app/<feature>/<route>/page.test.tsx
```

Test the page renders the component, wires up the API client on submit, and navigates on success.

**GREEN** — add the page:

```
frontend/src/app/<feature>/<route>/page.tsx
```

**Rules:**

- Pages are `'use client'` thin orchestrators — they import the component and the API client.
- State management uses the `useCommand` hook — the page calls `useCommand(commandFn, onSuccess)` and passes `{ errors, isSubmitting, handleSubmit }` to the form.
- Presentation lives in the component, not the page.
- Navigation on success uses Next.js `useRouter` via the `onSuccess` callback passed to `useCommand`.

---

## Destructive Command Slice (variant)

A destructive command removes or archives a resource. It follows the same backend TDD order as a standard command slice but differs on the frontend: no form component is needed. Instead the page wires a confirmation dialog.

### Frontend TDD order

| Step | What | Example file |
|------|------|-------------|
| 1 | API client | `api/dogs/removeDog.ts` |
| 2 | Aggregate behavioral hook | `hooks/dogs/useRemoveDog.ts` |
| 3 | Page integration | `app/dogs/[id]/page.tsx` |

Reuse existing shared components (`ConfirmDialog`, `ActionsCard` from `lib/components/`) — do not rebuild them per slice.

The behavioral hook owns the confirm-dialog lifecycle, the API call, error state, and post-success navigation. The page simply spreads `dialogProps` onto `ConfirmDialog` and renders the error.

## Query Slice (Read Path)

A query slice reads state without mutation. The full-stack runtime flow:

```
Page → API Client → Endpoint → QueryDispatcher → Validator → QueryHandler → Reader → Database
```

Query slices never touch the Domain layer — there are no invariants to enforce.

**TDD order: Application → Infrastructure → API → Frontend.**

### 1. Application (RED → GREEN)

**RED** — write the handler test:

```
tests/CampFitFurDogs.Application.Tests/<Feature>/<UseCase>/<UseCase>HandlerTests.cs
```

Mock the reader interface. Assert the handler delegates to the reader and returns its result.

**GREEN** — add Abstractions (public API):

```
src/CampFitFurDogs.Application/Abstractions/<Feature>/
    <UseCase>Query.cs          # implements IQuery<TResponse>
    <UseCase>Response.cs       # response DTO
    I<UseCase>Reader.cs        # reader interface
```

Then the handler and validator:

```
src/CampFitFurDogs.Application/<Feature>/<UseCase>/
    <UseCase>Handler.cs              # implements IQueryHandler<TQuery, TResponse>
    <UseCase>QueryValidator.cs       # implements IValidator<TQuery>
```

**Rules:**

- Query handlers depend on **reader interfaces**, never repositories (ADR-0021).
- The `QueryHandlerIsolationGuardrailTests` guardrail enforces this at compile time.
- Response DTOs live in Abstractions alongside the reader interface.

### 2. Infrastructure (RED → GREEN)

**RED** — write the reader integration test:

```
tests/CampFitFurDogs.Infrastructure.Tests/<Feature>/<UseCase>ReaderTests.cs
```

Uses `PostgresFixture` for real EF Core.

**GREEN** — add production code:

```
src/CampFitFurDogs.Infrastructure/<Feature>/
    <UseCase>Reader.cs       # implements I<UseCase>Reader
```

**Rules:**

- Readers return response DTOs — no domain logic, no aggregates.
- Access entities via `_db.Set<T>()`.
- Readers auto-register by naming convention (ADR-0015).

### 3. API (RED → GREEN)

Same pattern as command slices. The endpoint implements `IEndpoint`, dispatches the query, and maps the response.

### 4. Frontend (RED → GREEN)

TDD order within the frontend: **Component → API Client → Page.**

#### 4a. Component (RED → GREEN)

**RED** — write the component test:

```
frontend/test/components/<Entity><UseCase>Card.test.tsx
```

Test rendering of all display states (data, loading, not found, error) using React Testing Library.

**GREEN** — add the presentational component:

```
frontend/src/components/<Entity><UseCase>Card.tsx
```

#### 4b. API Client (RED → GREEN)

**RED** — write the API client test:

```
frontend/test/api/<useCase>.test.ts
```

Test success, not-found, and network error paths. Assert typed discriminated union result shape.

**GREEN** — add the API client function:

```
frontend/src/api/<useCase>.ts
```

**Rules:**

- API functions return `QueryResult<T>` from `lib/api/queryResult.ts` with a standard `.data` field.
- Handle 404 as a distinct result variant — not an error.
- Never throw — all outcomes are typed return values.

#### 4c. Page (RED → GREEN)

**RED** — write the page test:

```
frontend/test/app/<feature>/[id]/page.test.tsx
```

Test the page calls the API client on mount, passes data to the component, and handles loading/error/not-found states.

**GREEN** — add the page:

```
frontend/src/app/<feature>/[id]/page.tsx
```

**Rules:**

- Pages are `'use client'` thin orchestrators.
- Data fetching via the `useApiQuery` hook — call `useApiQuery(() => queryFn(id).then(toQueryState), [id])`.
- The hook returns a discriminated `QueryState<T>` — the page branches on `state.status` (loading / success / not-found / error). The component receives resolved data as props.
- Dynamic route segments use `useParams()`.

---

## Quick-Reference Checklists

### New Aggregate (only when introducing a new aggregate root)

| # | Layer | Test First | Then Production Code |
|---|-------|------------|----------------------|
| 1 | Domain | `<Entity>Tests.cs` | `<Entity>.cs`; `<ValueObject>.cs`; `<DomainEvent>.cs` |

### Command Slice (assumes aggregate exists)

| # | Layer | Test First | Then Production Code |
|---|-------|------------|----------------------|
| 1 | Application | `<UseCase>CommandHandlerTests.cs` | `<UseCase>Command.cs`; `<UseCase>Result.cs`; `I<Entity>Repository.cs`*; `<UseCase>CommandHandler.cs`; `<UseCase>CommandValidator.cs` |
| 2 | Infrastructure | `<Entity>RepositoryTests.cs` | `<Entity>Repository.cs`*; `<Entity>Configuration.cs`* |
| 3 | API | `<UseCase>EndpointTests.cs` | `<UseCase>Endpoint.cs`; `<UseCase>Request.cs` |
| 4 | Frontend | `<UseCase>Form.test.tsx`; `<useCase>.test.ts`; `page.test.tsx` | `<UseCase>Form.tsx`; `<useCase>.ts`; `page.tsx` |

*Only if this is the first command slice for the aggregate. Subsequent slices extend existing files.

### Query Slice

| # | Layer | Test First | Then Production Code |
|---|-------|------------|----------------------|
| 1 | Application | `<UseCase>HandlerTests.cs` | `<UseCase>Query.cs`; `<UseCase>Response.cs`; `I<UseCase>Reader.cs`; `<UseCase>Handler.cs`; `<UseCase>QueryValidator.cs` |
| 2 | Infrastructure | `<UseCase>ReaderTests.cs` | `<UseCase>Reader.cs` |
| 3 | API | `<UseCase>EndpointTests.cs` | `<UseCase>Endpoint.cs` |
| 4 | Frontend | `<Entity><UseCase>Card.test.tsx`; `<useCase>.test.ts`; `page.test.tsx` | `<Entity><UseCase>Card.tsx`; `<useCase>.ts`; `page.tsx` |

### After Every Slice

- [ ] All backend tests pass (`dotnet test`)
- [ ] All frontend tests pass (`cd frontend && npm test`)
- [ ] Guardrails pass (endpoint discovery, query handler isolation, auto-discovery)
- [ ] CHANGELOG updated under `[Unreleased]`
- [ ] PR opened with merge checklist

