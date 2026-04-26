# Folder Structure & Slice Anatomy

This guide describes the high-level folder structure of the Camp Fit Fur Dogs solution and the anatomy of a vertical slice. It ensures contributors understand where code belongs, why it belongs there, and how the layers interact.

---

## 1. Top-Level Solution Structure

The solution follows a clean, layered architecture with vertical slices cutting through the layers.

```
src/
  CampFitFurDogs.Api/
  CampFitFurDogs.Application/
  CampFitFurDogs.Domain/
  CampFitFurDogs.Infrastructure/
  SharedKernel/

tests/
  CampFitFurDogs.Api.Tests/
  CampFitFurDogs.Application.Tests/
    CampFitFurDogs.Architecture.Tests/
  CampFitFurDogs.Domain.Tests/
  CampFitFurDogs.Infrastructure.Tests/
```

Each project has a clear responsibility:

- **Api** — HTTP endpoints, request/response mapping, routing.
- **Application** — use cases, handlers, validators, dispatchers, domain event dispatch.
- **Domain** — entities, value objects, domain events, invariants.
- **Infrastructure** — persistence, external systems, repository implementations.
- **SharedKernel** — cross-cutting domain primitives and abstractions.

---

## 2. Vertical Slice Anatomy

A vertical slice spans all layers but keeps each layer pure.

Example slice: **Dogs**

```
src/
  CampFitFurDogs.Application/
    Abstractions/
      Dogs/
        RegisterDogCommand.cs
        RegisterDogResult.cs
        GetDogProfileQuery.cs
        GetDogProfileResult.cs
        IGetDogProfileReader.cs

    Dogs/
      RegisterDog/
        RegisterDogCommandHandler.cs
        RegisterDogCommandValidator.cs
      GetDogProfile/
        GetDogProfileHandler.cs
        GetDogProfileQueryValidator.cs

  CampFitFurDogs.Domain/
    Dogs/
      Dog.cs
      DogRegisteredDomainEvent.cs

  CampFitFurDogs.Infrastructure/
    Dogs/
      DogRepository.cs
      DogConfiguration.cs
      GetDogProfileReader.cs

  CampFitFurDogs.Api/
    Dogs/
      RegisterDogEndpoint.cs
      GetDogProfileEndpoint.cs
```

Each slice contains:

- **Abstractions** — commands, queries, results, reader interfaces.
- **Application** — handlers, validators.
- **Domain** — entities, value objects, domain events.
- **Infrastructure** — repositories (commands), readers (queries), persistence.
- **Api** — endpoints.

---

## 3. Layer Responsibilities

### 3.1 Api Layer

- Defines HTTP endpoints.
- Maps HTTP requests to commands/queries.
- Calls dispatchers.
- Maps results to HTTP responses.
- Contains no business logic.

### 3.2 Application Layer

- Implements use cases.
- Contains handlers, validators, dispatchers.
- Dispatches domain events.
- Contains no HTTP or persistence logic.
- Calls `IUnitOfWork.CommitAsync` to persist changes (command handlers only).

### 3.3 Domain Layer

- Contains entities, value objects, domain events.
- Enforces invariants.
- Contains no infrastructure or application logic.

### 3.4 Infrastructure Layer

- Implements repositories.
- Integrates with external systems.
- Contains EF Core, file I/O, messaging, etc.
- Contains no domain logic.

### 3.5 SharedKernel

- Contains cross-cutting domain primitives.
- Contains no application or infrastructure concerns.

---

## 4. Folder Naming Conventions

### 4.1 Application Slice Structure

```
Application/<Feature>/
  <UseCase>/
    <UseCase>Handler.cs
    <UseCase>Validator.cs
```

### 4.2 Abstractions Structure

```
Application/Abstractions/<Feature>/
  <UseCase>Command.cs
  <UseCase>Result.cs
  I<UseCase>Reader.cs          # query slices only
```

### 4.3 Domain Structure

```
Domain/<Feature>/
  <Entity>.cs
  <ValueObject>.cs
  <DomainEvent>.cs
```

### 4.4 Infrastructure Structure

```
Infrastructure/<Feature>/
  <Entity>Repository.cs
  <Entity>Configuration.cs
  <UseCase>Reader.cs          # query slices only
```

### 4.5 Api Structure

```
Api/<Feature>/
  <UseCase>Endpoint.cs
  <UseCase>Request.cs
  <UseCase>Response.cs
```

---

## 5. Purity Rules by Layer

### Api
- No domain logic.
- No repository access.
- No handler instantiation.

### Application
- No HTTP.
- No EF Core.
- No Infrastructure references.

### Domain
- No Application references.
- No Infrastructure references.
- No API references.

### Infrastructure
- No domain logic.
- No API references.

### SharedKernel
- No dependencies on any other layer.

---

## 6. Contributor Guidelines

When adding a new feature:

1. Add commands/queries/results and reader interfaces (query slices) to **Abstractions**.
2. Add handlers/validators to **Application**.
3. Add domain entities/events to **Domain**.
4. Add repositories (command slices), readers (query slices), and entity configurations to **Infrastructure**.
5. Inject `IUnitOfWork` in command handlers and call `CommitAsync` after repository operations.
6. Add endpoints to **Api** — each endpoint class implements `IEndpoint`.
7. Add tests to the corresponding test project. Pure-reflection guardrails go in Architecture.Tests; DI-dependent guardrails go in Api.Tests/Guardrails/.
8. Follow naming conventions strictly.
9. Do not bypass the dispatcher pipeline.
10. Do not place code in SharedKernel unless it is truly cross-cutting.

If you’re unsure where something belongs, default to the **most restrictive** layer (Domain > Application > Infrastructure > Api).


---

## 7. Frontend Folder Structure

The frontend lives in `frontend/` and follows a **layer + aggregate** convention
that mirrors the backend's aggregate grouping without fighting Next.js conventions.

### 7.1 Directory Layout

```
frontend/src/
├── api/dogs/                  ← Server-call functions grouped by aggregate
│   ├── getDogProfile.ts
│   ├── registerDog.ts
│   └── editDogProfile.ts
├── components/dogs/           ← Presentational React components grouped by aggregate
│   ├── DogProfileCard.tsx
│   ├── DogProfileActionsCard.tsx
│   ├── EditDogProfileForm.tsx
│   └── RegisterDogForm.tsx
├── lib/dogs/                  ← Pure logic / action functions grouped by aggregate
│   └── dogProfileActions.ts
├── lib/api/                   ← Shared infrastructure (API client, auth helpers)
│   └── client.ts
└── app/                       ← Next.js routing layer — UNTOUCHED by this convention
    ├── page.tsx
    └── dogs/
        ├── [id]/
        │   ├── page.tsx
        │   └── edit/
        │       └── page.tsx
        └── register/
            ├── page.tsx
            └── success/
                └── page.tsx

frontend/test/                 ← Mirrors src/ structure
├── api/dogs/
├── components/dogs/
├── lib/dogs/
└── app/dogs/
```

### 7.2 Convention Rules

| Rule | Detail |
|------|--------|
| **Structure** | `layer/aggregate/filename` — slice identity is encoded in the filename, not a subfolder |
| **Slice subfolders** | Introduced only when an aggregate accumulates **10+ files** in a single layer |
| **`app/` layer** | Left untouched — Next.js owns routing; the file-system convention already reads like slices |
| **`test/` mirror** | Test directory mirrors `src/` exactly (`test/api/dogs/`, `test/components/dogs/`, etc.) |
| **Shared infra** | Cross-aggregate utilities live in `lib/api/` (e.g., `client.ts`) — no aggregate subfolder |

### 7.3 Frontend Naming Conventions

| Layer | File pattern | Example |
|-------|-------------|---------|
| `api/` | `camelCaseVerb.ts` | `getDogProfile.ts`, `registerDog.ts` |
| `components/` | `PascalCase.tsx` | `DogProfileCard.tsx`, `RegisterDogForm.tsx` |
| `lib/` | `camelCase.ts` | `dogProfileActions.ts` |
| `app/` | `page.tsx` (Next.js convention) | `app/dogs/[id]/page.tsx` |

### 7.4 Frontend Contributor Steps

When adding a new frontend feature:

1. Add server-call function(s) to `api/<aggregate>/`.
2. Add pure logic / action functions to `lib/<aggregate>/` (if needed).
3. Add presentational component(s) to `components/<aggregate>/`.
4. Wire the page in `app/` — compose components, call server functions.
5. Add tests mirroring `src/` structure in `test/`.
6. Follow naming conventions strictly.
