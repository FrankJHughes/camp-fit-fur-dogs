
# CampFitFurDogs

The `CampFitFurDogs` namespace represents the **root of the CampFitFurDogs application**, defining the overall architecture, boundaries, and conventions used throughout the system.  
It is the entry point for understanding how the solution is organized into **Domain**, **Application**, **Infrastructure**, and **API** layers, each following strict vertical‑slice and DDD‑inspired principles.

This namespace itself contains minimal code — its purpose is conceptual.  
It defines the *system*, not the implementation.

---

## 🎯 Architectural Overview

CampFitFurDogs follows a **vertical slice architecture** with **clean layering**:

- **Domain Layer** — business rules, aggregates, invariants  
- **Application Layer** — commands, queries, orchestrators, unit of work  
- **Infrastructure Layer** — EF Core persistence, readers/writers, DbContexts  
- **API Layer** — HTTP endpoints, DTOs, request/response models  

Each slice (e.g., Dogs, Owners, Authentication) flows downward:

**API → Application → Domain → Infrastructure**

Explore the architecture:  
**[Vertical slice overview](ca://s?q=Explain_vertical_slice_architecture)**

---

## 🧩 Major Sub‑Namespaces

### `CampFitFurDogs.Domain`
The core of the business model.

Contains:

- Aggregates (`Dog`, `Owner`, etc.)  
- Value objects (`DogName`, `Breed`, `OwnerId`)  
- Invariants and business rules  
- Domain events  

Explore the domain:  
**[Domain modeling](ca://s?q=Explain_domain_modeling_in_CampFitFurDogs)**

---

### `CampFitFurDogs.Application`
The orchestration layer.

Contains:

- Commands and queries  
- Handlers  
- Application services  
- Unit of Work abstractions  
- Vertical slice boundaries  

Explore the application layer:  
**[Application layer design](ca://s?q=Explain_application_layer_design)**

---

### `CampFitFurDogs.Infrastructure`
The persistence and mechanical layer.

Contains:

- EF Core DbContexts  
- Entity configurations  
- Readers and writers  
- Unit of Work implementation  
- DI registration modules  

Explore infrastructure:  
**[Infrastructure overview](ca://s?q=Explain_CampFitFurDogs_Infrastructure)**

---

### `CampFitFurDogs.Api`
The HTTP interface layer.

Contains:

- Controllers or minimal API endpoints  
- Request/response DTOs  
- Authentication and authorization boundaries  
- API‑specific validation  

Explore the API layer:  
**[API layer design](ca://s?q=Explain_API_layer_design)**

---

## 🧭 Core Principles

The CampFitFurDogs architecture follows strict rules:

- **Vertical slices over horizontal layers**  
- **Domain purity** — no EF Core or infrastructure concerns in domain types  
- **Application orchestration** — commands/queries drive workflows  
- **Infrastructure isolation** — persistence is mechanical, not logical  
- **Explicit boundaries** — each slice owns its own readers/writers  
- **Strong typing** — IDs, names, and value objects enforce correctness  

Explore the philosophy:  
**[Clean architecture principles](ca://s?q=Explain_clean_architecture_principles)**

---

## 🚫 What Does *Not* Belong in the Root Namespace

The root namespace must **not** contain:

- Business logic  
- Persistence logic  
- Application orchestration  
- API endpoints  
- EF Core configurations  
- DI modules  

It is purely organizational.

---

## 📚 Related Documentation

- `CampFitFurDogs.Infrastructure` — persistence, DbContexts, unit of work  
- `CampFitFurDogs.Domain` — aggregates and invariants  
- `CampFitFurDogs.Application` — commands, queries, orchestrators  
- `CampFitFurDogs.Api` — HTTP endpoints and DTOs  

---

