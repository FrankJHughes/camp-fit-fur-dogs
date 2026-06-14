# Folder Structure & Slice Anatomy

This guide describes the high‑level folder structure of the Camp Fit Fur Dogs solution and the anatomy of a vertical slice.  
It ensures contributors understand where code belongs, why it belongs there, and how the layers interact.

This guide belongs to the **architecture** category because it governs cross‑cutting structure across all vertical slices.

---

# 1. Top‑Level Solution Structure

The solution follows a clean, layered architecture with vertical slices cutting through the layers.

````text
src/
  CampFitFurDogs.Api/
  CampFitFurDogs.Application/
  CampFitFurDogs.Domain/
  CampFitFurDogs.Infrastructure/
  Frank/

tests/
  CampFitFurDogs.Api.Tests/
  CampFitFurDogs.Application.Tests/
  CampFitFurDogs.Architecture.Tests/
  CampFitFurDogs.Domain.Tests/
  CampFitFurDogs.Infrastructure.Tests/
````

Each project has a clear responsibility:

- **Api** — HTTP endpoints, request/response mapping, routing  
- **Application** — use cases, handlers, validators, dispatchers, domain event dispatch  
- **Domain** — entities, value objects, domain events, invariants  
- **Infrastructure** — persistence, external systems, repository + reader implementations  
- **Frank** — cross‑cutting primitives, DI auto‑registration, endpoint discovery, guardrails, HostingEngine, StartupEngine  

Frank is a **product**, not a folder — see **[Multi‑Product Governance](ca://s?q=Open_multi_product_governance)**.

---

# 2. Vertical Slice Anatomy

A vertical slice spans all layers but keeps each layer pure.

Example slice: **Dogs**

````text
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
````

Each slice contains:

- **Abstractions** — commands, queries, results, reader interfaces  
- **Application** — handlers, validators, domain event handlers  
- **Domain** — entities, value objects, domain events  
- **Infrastructure** — repositories (commands), readers (queries), EF Core configuration  
- **Api** — endpoints  

All slice services are registered via **Frank auto‑registration**.

---

# 3. Layer Responsibilities

## 3.1 Api Layer

- Defines HTTP endpoints  
- Maps HTTP requests to commands/queries  
- Calls dispatchers  
- Maps results to HTTP responses  
- Contains no business logic  
- Must not depend on Infrastructure  
- Must not invoke handlers directly  

See **[API Endpoint Purity Guide](ca://s?q=Generate_API_Endpoint_Purity_Guide)**.

---

## 3.2 Application Layer

- Implements use cases  
- Contains handlers, validators, dispatchers  
- Dispatches domain events  
- Contains no HTTP or persistence logic  
- Command handlers call `IUnitOfWork.CommitAsync()`  
- All services are auto‑registered via `[AutoRegister]`  

See **[Dispatcher Pipeline](ca://s?q=Open_dispatcher_pipeline_guide)**.

---

## 3.3 Domain Layer

- Contains entities, value objects, domain events  
- Enforces invariants  
- Contains no Infrastructure or Application logic  
- Raises domain events internally  

See **[Domain Events Architecture](ca://s?q=Open_domain_events_guide)**.

---

## 3.4 Infrastructure Layer

- Implements repositories  
- Implements readers  
- Integrates with external systems  
- Contains EF Core, file I/O, messaging  
- Contains no domain logic  
- Contains no API references  
- All services are auto‑registered via `[AutoRegister]`  

---

## 3.5 Frank

Frank provides:

- Cross‑cutting primitives  
- DI auto‑registration engine  
- `[AutoRegister]` attribute  
- Endpoint discovery  
- Validation pipeline  
- EF Core configuration scanning  
- Hosting provider infrastructure  
- StartupEngine  
- HostingEngine  
- Architecture guardrails  

Frank must not depend on any product layer.

---

# 4. Folder Naming Conventions

## 4.1 Application Slice Structure

````text
Application/<Feature>/
  <UseCase>/
    <UseCase>Handler.cs
    <UseCase>Validator.cs
    <UseCase>DomainEventHandler.cs   # if applicable
````

## 4.2 Abstractions Structure

````text
Application/Abstractions/<Feature>/
  <UseCase>Command.cs
  <UseCase>Result.cs
  I<UseCase>Reader.cs      # query slices only
````

## 4.3 Domain Structure

````text
Domain/<Feature>/
  <Entity>.cs
  <ValueObject>.cs
  <DomainEvent>.cs
````

## 4.4 Infrastructure Structure

````text
Infrastructure/<Feature>/
  <Entity>Repository.cs
  <Entity>Configuration.cs
  <UseCase>Reader.cs       # query slices only
````

## 4.5 Api Structure

````text
Api/<Feature>/
  <UseCase>Endpoint.cs
  <UseCase>Request.cs
  <UseCase>Response.cs
````

---

# 5. Purity Rules by Layer

## Api
- No domain logic  
- No repository access  
- No handler instantiation  
- No Infrastructure references  
- Must use dispatchers  

## Application
- No HTTP  
- No EF Core  
- No Infrastructure references  
- Must use `[AutoRegister]`  

## Domain
- No Application references  
- No Infrastructure references  
- No Api references  

## Infrastructure
- No domain logic  
- No Api references  
- Must use `[AutoRegister]`  

## Frank
- No dependencies on any product layer  

These rules enforce **[Architecture Governance](ca://s?q=Open_architecture_governance)**.

---

# 6. Contributor Guidelines

When adding a new feature:

1. Add commands/queries/results and reader interfaces to **Abstractions**  
2. Add handlers/validators/domain event handlers to **Application**  
3. Add domain entities/events to **Domain**  
4. Add repositories (command slices), readers (query slices), and configurations to **Infrastructure**  
5. Inject `IUnitOfWork` in command handlers and call `CommitAsync()`  
6. Add endpoints to **Api** — each implements `IEndpoint`  
7. Add tests to the corresponding test project  
8. Guardrails:  
   - Architecture guardrails → `Architecture.Tests`  
   - DI guardrails → `Api.Tests/Guardrails`  
9. Follow naming conventions strictly  
10. Do not bypass the dispatcher pipeline  
11. Do not place code in Frank unless it is truly cross‑cutting  
12. Ensure all slice services use `[AutoRegister]`  

If unsure where something belongs, default to the **most restrictive** layer:  
**Domain → Application → Infrastructure → Api**

---

# 7. Frontend Folder Structure

The frontend lives in `frontend/` and follows a **layer + aggregate** convention that mirrors backend aggregate grouping without fighting Next.js routing.

## 7.1 Directory Layout

````text
frontend/src/
├── api/dogs/
│   ├── getDogProfile.ts
│   ├── registerDog.ts
│   └── editDogProfile.ts
├── components/dogs/
│   ├── DogProfileCard.tsx
│   ├── DogProfileActionsCard.tsx
│   ├── EditDogProfileForm.tsx
│   └── RegisterDogForm.tsx
├── lib/dogs/
│   └── dogProfileActions.ts
├── lib/api/
│   └── client.ts
└── app/
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

frontend/test/
├── api/dogs/
├── components/dogs/
├── lib/dogs/
└── app/dogs/
````

## 7.2 Convention Rules

| Rule | Detail |
|------|--------|
| **Structure** | `layer/aggregate/filename` — slice identity is encoded in the filename |
| **Slice subfolders** | Only when an aggregate accumulates **10+ files** in a layer |
| **`app/` layer** | Next.js owns routing — untouched by slice conventions |
| **`test/` mirror** | Test directory mirrors `src/` exactly |
| **Shared infra** | Cross‑aggregate utilities live in `lib/api/` |

## 7.3 Frontend Naming Conventions

| Layer | Pattern | Example |
|-------|---------|---------|
| `api/` | `camelCaseVerb.ts` | `getDogProfile.ts` |
| `components/` | `PascalCase.tsx` | `RegisterDogForm.tsx` |
| `lib/` | `camelCase.ts` | `dogProfileActions.ts` |
| `app/` | Next.js conventions | `app/dogs/[id]/page.tsx` |

## 7.4 Frontend Contributor Steps

When adding a new frontend feature:

1. Add server‑call functions to `api/<aggregate>/`  
2. Add pure logic to `lib/<aggregate>/` (if needed)  
3. Add presentational components to `components/<aggregate>/`  
4. Wire the page in `app/`  
5. Add tests mirroring `src/` structure  
6. Follow naming conventions strictly  

---

# Related Documents

- **[Architecture Governance](ca://s?q=Open_architecture_governance)**  
- **[Code Conventions](ca://s?q=Open_code_conventions)**  
- **[Dispatcher Pipeline](ca://s?q=Open_dispatcher_pipeline_guide)**  
- **[Domain Events Architecture](ca://s?q=Open_domain_events_guide)**  
- **[API Endpoint Purity](ca://s?q=Generate_API_Endpoint_Purity_Guide)**  
- **[Multi‑Product Governance](ca://s?q=Open_multi_product_governance)**  
