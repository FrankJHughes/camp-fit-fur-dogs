# Adding a New Feature Slice

Step-by-step walkthrough for adding a vertical slice to Camp Fit Fur Dogs.  
Covers aggregate setup, command (write) slices, and query (read) slices using TDD red–green–refactor across all layers — backend through frontend.

> **What is a slice?**  
> One command or one query, vertical through all layers, that produces a visible result.  
> One verb, one noun. Not a user journey.  
>
> ✅ `RegisterDog` (command) · ✅ `GetDogProfile` (query) · ❌ “Dog Management” (feature area)

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

Key ADRs: **ADR‑0002** (DDD layer boundaries), **ADR‑0011** (CQRS pipeline separation),  
**ADR‑0015** (convention-based auto-registration), **ADR‑0020** (endpoint auto-discovery),  
**ADR‑0021** (query-side reader isolation).

---

# Establishing a New Aggregate (Domain Layer)

Only required when introducing a **new aggregate root**.  
If the aggregate already exists, skip to the command or query slice.

### RED — write the entity test first

```
tests/CampFitFurDogs.Domain.Tests/<Feature>/<Entity>Tests.cs
```

Test creation, invariants, and domain event emission.

### GREEN — add production code

```
src/CampFitFurDogs.Domain/<Feature>/
    <Entity>.cs
    <ValueObject>.cs
    <Entity><Action>DomainEvent.cs
```

### Rules

- Entities enforce invariants — invalid state is unrepresentable.  
- Value objects validate in their factory and are immutable.  
- Domain events are raised inside aggregate mutation methods.  
- Subsequent slices extend the aggregate — they do not recreate it.

---

# Command Slice (Write Path)

A command slice **mutates state**.

Runtime flow:

```
Page → API Client → Endpoint → CommandDispatcher → Validator → CommandHandler → Repository → Database
```

**TDD order: Application → Infrastructure → API → Frontend**

---

## 1. Application (RED → GREEN)

### RED — handler test

```
tests/CampFitFurDogs.Application.Tests/<Feature>/<UseCase>/<UseCase>CommandHandlerTests.cs
```

Mock the repository. Assert:

- Repository is called  
- Domain events are dispatched  
- `IUnitOfWork.CommitAsync()` is invoked  

### GREEN — abstractions + handler

```
src/CampFitFurDogs.Application/Abstractions/<Feature>/
    <UseCase>Command.cs
    <UseCase>Result.cs
    I<Entity>Repository.cs   # only if first slice for this aggregate
```

Then:

```
src/CampFitFurDogs.Application/<Feature>/<UseCase>/
    <UseCase>CommandHandler.cs
    <UseCase>CommandValidator.cs
```

### Rules

- Handlers inject `IUnitOfWork` and call `CommitAsync()`.  
- Validators run before handlers via the dispatcher pipeline.  
- All types auto-register by naming convention (ADR‑0015).  
- Repository interface added only once per aggregate.

---

## 2. Infrastructure (RED → GREEN)

### RED — repository integration test

```
tests/CampFitFurDogs.Infrastructure.Tests/<Feature>/<Entity>RepositoryTests.cs
```

Uses `PostgresFixture` (Testcontainers).

### GREEN — repository + configuration

```
src/CampFitFurDogs.Infrastructure/<Feature>/
    <Entity>Repository.cs
    <Entity>Configuration.cs
```

### Rules

- Use `_db.Set<T>()` — never add `DbSet<T>` to `AppDbContext`.  
- Configurations auto-register via `ApplyConfigurationsFromAssembly`.  
- Repository auto-registers by naming convention.

---

## 3. API (RED → GREEN)

### RED — endpoint integration test

```
tests/CampFitFurDogs.Api.Tests/<Feature>/<UseCase>EndpointTests.cs
```

### GREEN — endpoint + request DTO

```
src/CampFitFurDogs.Api/<Feature>/
    <UseCase>Endpoint.cs
    <UseCase>Request.cs
```

### Rules

- Endpoints implement `IEndpoint` (ADR‑0020).  
- Endpoints are thin: map HTTP → command → result.  
- Identity comes from `ICurrentUserService`.

---

## 4. Frontend (RED → GREEN)

Frontend uses the **new FormCommand architecture**:

- `FormCommand.run` is the only submit API  
- `useFormStateMachine` manages validation + error merging  
- `FormField` handles accessibility  
- API clients return `CommandResult`  
- Success response for create-account is **only `{ customerId }`**

### 4a. Component (RED → GREEN)

```
frontend/test/components/<UseCase>Form.test.tsx
```

Then:

```
frontend/src/components/<UseCase>Form.tsx
```

### 4b. API Client (RED → GREEN)

```
frontend/test/api/<useCase>.test.ts
```

Then:

```
frontend/src/api/<useCase>.ts
```

### Rules

- API clients return `CommandResult` via `toCommandResult()`.  
- Never throw — all outcomes are typed.  
- Form components import types from the API slice.  
- Phone numbers must be normalized to digits-only before submit.

### 4c. Page (RED → GREEN)

```
frontend/test/app/<feature>/<route>/page.test.tsx
```

Then:

```
frontend/src/app/<feature>/<route>/page.tsx
```

### Rules

- Pages are `'use client'` thin orchestrators.  
- Use `useCommand(apiFn, onSuccess)` to wire the form.  
- Navigation on success uses `router.push()` inside `onSuccess`.

---

# Destructive Command Slice (Variant)

No form component. Instead:

| Step | What | Example |
|------|------|---------|
| 1 | API client | `api/dogs/removeDog.ts` |
| 2 | Behavioral hook | `hooks/dogs/useRemoveDog.ts` |
| 3 | Page integration | `app/dogs/[id]/page.tsx` |

Use shared components (`ConfirmDialog`, `ActionsCard`).  
The hook owns dialog lifecycle + API call + navigation.

---

# Query Slice (Read Path)

A query slice **reads state** without mutation.

Runtime flow:

```
Page → API Client → Endpoint → QueryDispatcher → Validator → QueryHandler → Reader → Database
```

**TDD order: Application → Infrastructure → API → Frontend**

---

## 1. Application (RED → GREEN)

### RED — handler test

```
tests/CampFitFurDogs.Application.Tests/<Feature>/<UseCase>/<UseCase>HandlerTests.cs
```

### GREEN — abstractions + handler

```
src/CampFitFurDogs.Application/Abstractions/<Feature>/
    <UseCase>Query.cs
    <UseCase>Response.cs
    I<UseCase>Reader.cs
```

Then:

```
src/CampFitFurDogs.Application/<Feature>/<UseCase>/
    <UseCase>Handler.cs
    <UseCase>QueryValidator.cs
```

### Rules

- Query handlers depend on **readers**, not repositories (ADR‑0021).  
- Guardrail tests enforce isolation.  
- Response DTOs live in Abstractions.

---

## 2. Infrastructure (RED → GREEN)

### RED — reader integration test

```
tests/CampFitFurDogs.Infrastructure.Tests/<Feature>/<UseCase>ReaderTests.cs
```

### GREEN — reader implementation

```
src/CampFitFurDogs.Infrastructure/<Feature>/
    <UseCase>Reader.cs
```

### Rules

- Readers return DTOs — no domain logic.  
- Use `_db.Set<T>()`.  
- Auto-register by naming convention.

---

## 3. API (RED → GREEN)

Same pattern as command slices.

---

## 4. Frontend (RED → GREEN)

### 4a. Component (RED → GREEN)

```
frontend/test/components/<Entity><UseCase>Card.test.tsx
```

Then:

```
frontend/src/components/<Entity><UseCase>Card.tsx
```

### 4b. API Client (RED → GREEN)

```
frontend/test/api/<useCase>.test.ts
```

Then:

```
frontend/src/api/<useCase>.ts
```

### Rules

- API clients return `QueryResult<T>` via `toQueryState()`.  
- 404 is a **not-found** variant, not an error.  
- Never throw — all outcomes are typed.

### 4c. Page (RED → GREEN)

```
frontend/test/app/<feature>/[id]/page.test.tsx
```

Then:

```
frontend/src/app/<feature>/[id]/page.tsx
```

### Rules

- Pages use `useApiQuery(() => queryFn(id).then(toQueryState), [id])`.  
- Branch on `state.status` (loading / success / not-found / error).  
- Pass resolved data to the component.

---

# Quick-Reference Checklists

## New Aggregate

| # | Layer | Test First | Then Production Code |
|---|-------|------------|----------------------|
| 1 | Domain | `<Entity>Tests.cs` | `<Entity>.cs`; `<ValueObject>.cs`; `<DomainEvent>.cs` |

## Command Slice

| # | Layer | Test First | Then Production Code |
|---|-------|------------|----------------------|
| 1 | Application | `<UseCase>CommandHandlerTests.cs` | `<UseCase>Command.cs`; `<UseCase>Result.cs`; `I<Entity>Repository.cs`*; `<UseCase>CommandHandler.cs`; `<UseCase>CommandValidator.cs` |
| 2 | Infrastructure | `<Entity>RepositoryTests.cs` | `<Entity>Repository.cs`*; `<Entity>Configuration.cs`* |
| 3 | API | `<UseCase>EndpointTests.cs` | `<UseCase>Endpoint.cs`; `<UseCase>Request.cs` |
| 4 | Frontend | `<UseCase>Form.test.tsx`; `<useCase>.test.ts`; `page.test.tsx` | `<UseCase>Form.tsx`; `<useCase>.ts`; `page.tsx` |

\* Only for the first slice on an aggregate.

## Query Slice

| # | Layer | Test First | Then Production Code |
|---|-------|------------|----------------------|
| 1 | Application | `<UseCase>HandlerTests.cs` | `<UseCase>Query.cs`; `<UseCase>Response.cs`; `I<UseCase>Reader.cs`; `<UseCase>Handler.cs`; `<UseCase>QueryValidator.cs` |
| 2 | Infrastructure | `<UseCase>ReaderTests.cs` | `<UseCase>Reader.cs` |
| 3 | API | `<UseCase>EndpointTests.cs` | `<UseCase>Endpoint.cs` |
| 4 | Frontend | `<Entity><UseCase>Card.test.tsx`; `<useCase>.test.ts`; `page.test.tsx` | `<Entity><UseCase>Card.tsx`; `<useCase>.ts`; `page.tsx` |

## After Every Slice

- [ ] Backend tests pass  
- [ ] Frontend tests pass  
- [ ] Guardrails pass  
- [ ] CHANGELOG updated  
- [ ] PR opened with merge checklist
