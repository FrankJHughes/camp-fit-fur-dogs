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
  CampFitFurDogs.SharedKernel/

tests/
  CampFitFurDogs.Api.Tests/
  CampFitFurDogs.Application.Tests/
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

  CampFitFurDogs.Api/
    Dogs/
      RegisterDogEndpoint.cs
      GetDogProfileEndpoint.cs
```

Each slice contains:

- **Abstractions** — commands, queries, results.
- **Application** — handlers, validators.
- **Domain** — entities, value objects, domain events.
- **Infrastructure** — repositories, persistence.
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

1. Add commands/queries/results to **Abstractions**.
2. Add handlers/validators to **Application**.
3. Add domain entities/events to **Domain**.
4. Add repositories to **Infrastructure**.
5. Inject `IUnitOfWork` in command handlers and call `CommitAsync` after repository operations.
6. Add endpoints to **Api**.
7. Add tests to the corresponding test project.
8. Follow naming conventions strictly.
9. Do not bypass the dispatcher pipeline.
10. Do not place code in SharedKernel unless it is truly cross-cutting.

If you’re unsure where something belongs, default to the **most restrictive** layer (Domain > Application > Infrastructure > Api).

