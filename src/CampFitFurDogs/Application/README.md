# CampFitFurDogs.Application

The `CampFitFurDogs.Application` namespace contains the full application layer for the CampFitFurDogs system.  
It implements the vertical‑slice CQRS architecture, orchestrates domain workflows, enforces structural validation, and coordinates persistence through abstractions.  
This layer contains **no domain entities**, **no infrastructure**, and **no API concerns** — it is pure application logic.

---

## 🎯 Architectural Responsibilities

The application layer provides:

- **Vertical slices** for each functional area (e.g., Dogs)
- **CQRS command and query handlers**
- **FluentValidation validators**
- **Application‑level exceptions**
- **Identity consistency enforcement** via `ICurrentUser`
- **Resource‑level authorization** inside handlers
- **Orchestration of domain value objects and aggregates**
- **Delegation to persistence abstractions**
- **Transactional consistency** via `IAppUnitOfWork`

This layer is intentionally thin: it coordinates workflows but does not implement business rules (domain layer) or persistence (infrastructure layer).

---

## 📦 Key Sub‑Namespaces

### `CampFitFurDogs.Application.Dogs`
Contains all vertical slices related to dog management:

- RegisterDog  
- EditDog  
- RemoveDog  
- GetDog  
- ListDogsByOwner  

Each slice includes:

- Commands / Queries  
- Validators  
- Handlers  
- Abstractions for readers/writers  

Identity consistency and ownership checks are enforced here.

---

### `CampFitFurDogs.Application.Exceptions`
Contains application‑layer exceptions for:

- Authentication failures  
- Identity consistency failures  
- Standardized error codes  

Includes:

- `ErrorCode`  
- `UserNotAuthenticatedException`  
- `UserIdClaimNotFoundException`

These exceptions represent **structural application failures**, not domain or infrastructure errors.

---

### `CampFitFurDogs.Application.Abstractions`
Defines interfaces for:

- Readers / Writers  
- Unit of Work  
- Application‑level contracts  

These abstractions decouple the application layer from infrastructure.

---

## 🔧 Dependency Injection

All application‑layer services are registered via:

```csharp
services.AddCampFitFurDogsApplication();
```

This extension:

- Registers all Dogs vertical slices  
- Registers all validators in the application assembly  
- Provides a single entry point for application‑layer setup

See:

- `ServiceCollectionExtensions.AddCampFitFurDogsApplication`
- `AssemblyMarker` (used for assembly scanning)

---

## 🚫 What Does *Not* Belong Here

The application layer must **not** contain:

- Domain entities (`Dog`, `Owner`, etc.)
- EF Core DbContexts, readers, or writers
- API controllers or HTTP DTOs
- Business rules or domain invariants
- Infrastructure concerns (email, storage, external APIs)

Those belong in the Domain, Infrastructure, or API layers.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Domain` — aggregates, value objects, invariants  
- `CampFitFurDogs.Infrastructure` — EF Core, external systems  
- `CampFitFurDogs.Api` — HTTP endpoints, request/response models  
- `CampFitFurDogs.Application.Abstractions` — contracts for persistence and services  

---
