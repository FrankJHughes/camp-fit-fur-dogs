
# CampFitFurDogs.Infrastructure

The `CampFitFurDogs.Infrastructure` namespace contains the full infrastructure layer for the CampFitFurDogs application.  
It provides the concrete implementations that support persistence, transactional boundaries, and vertical‑slice data access.  
This layer is intentionally **mechanical**, **stateless**, and **free of domain logic**.

Infrastructure is responsible for *how* data is stored, retrieved, and committed — not *what* the business rules are.

---

## 🎯 Architectural Role

The Infrastructure layer provides:

- EF Core database contexts  
- Entity configurations  
- Readers and writers for vertical slices  
- Unit of Work implementation  
- Dependency‑injection registration  
- Assembly markers for scanning and bootstrapping  

It acts as the bridge between the **Application layer** and the **database**, ensuring clean separation of concerns and strict layering.

Explore the architecture:  
**[Vertical slice infrastructure](ca://s?q=Explain_vertical_slice_infrastructure)**

---

## 📦 Included Sub‑Namespaces

### `CampFitFurDogs.Infrastructure.Persistence`
Provides EF Core database context and entity configuration scanning.

Includes:

- `AppDbContext`  
- `DesignTimeDbContextFactory`  
- EF Core configuration discovery via `ApplyConfigurationsFromAssembly`

Explore DbContext design:  
**[AppDbContext](ca://s?q=Explain_AppDbContext)**

---

### `CampFitFurDogs.Infrastructure.DbContexts`
Provides DI registration for EF Core DbContexts.

Includes:

- `ServiceCollectionExtensions` for registering `AppDbContext`  
- PostgreSQL provider configuration  
- Environment‑aware connection string loading  

Explore DI patterns:  
**[Infrastructure DI patterns](ca://s?q=Explain_infrastructure_DI_patterns)**

---

### `CampFitFurDogs.Infrastructure.Dogs`
Provides persistence readers and writers for the Dogs vertical slice.

Includes:

- `EditDogWriter`  
- `RegisterDogWriter`  
- `RemoveDogWriter`  
- `GetDogReader`  
- `GetDogByIdReader`  
- `ListDogsByOwnerReader`  
- DI registration for all dog‑related infrastructure services  

Explore the Dogs slice:  
**[Dogs vertical slice](ca://s?q=Explain_Dogs_vertical_slice)**

---

### `CampFitFurDogs.Infrastructure.UnitOfWork`
Provides the EF Core–backed Unit of Work implementation.

Includes:

- `AppUnitOfWork`  
- DI registration for `IAppUnitOfWork`  

Explore the Unit of Work:  
**[Unit of Work pattern](ca://s?q=Explain_the_Unit_of_Work_pattern)**

---

### `CampFitFurDogs.Infrastructure.AssemblyMarker`
Provides a marker type for assembly scanning.

Includes:

- `AssemblyMarker` — a zero‑behavior type used for referencing the Infrastructure assembly via `typeof()`  

Explore marker types:  
**[Assembly marker pattern](ca://s?q=Explain_assembly_marker_pattern)**

---

## 🧭 Infrastructure Principles

The Infrastructure layer follows strict architectural rules:

- **No domain logic**  
- **No business rules**  
- **No application orchestration**  
- **No cross‑aggregate workflows**  
- **No EF Core leakage into domain types**  
- **No direct controller/API dependencies**

Infrastructure is purely mechanical and persistence‑focused.

---

## 🚫 What Does *Not* Belong Here

This namespace must **not** contain:

- Domain aggregates or value objects  
- Application commands, queries, or handlers  
- API controllers or DTOs  
- Business validation  
- Cross‑cutting policies (auth, rate limiting, etc.)  

Only **persistence**, **transaction management**, and **DI wiring** belong here.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Domain` — aggregates, invariants, value objects  
- `CampFitFurDogs.Application` — vertical slices, orchestration, business workflows  
- `CampFitFurDogs.Api` — HTTP endpoints and request/response DTOs  

---

