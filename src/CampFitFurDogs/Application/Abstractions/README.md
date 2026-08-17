# Application Abstractions

The **Application/Abstractions** folder defines the contracts that shape how the application layer interacts with the domain and infrastructure layers.  
These abstractions describe *what* the application needs to perform its work, without specifying *how* that work is implemented.

This folder contains **no domain logic**, **no infrastructure code**, and **no API endpoint definitions**.  
It is purely the interface boundary for the application layer.

---

## Purpose

Application abstractions provide:

- Clear, intention‑revealing contracts  
- Stable boundaries between vertical slices  
- Infrastructure inversion (interfaces instead of implementations)  
- DTOs for safe data transfer  
- CQRS command/query definitions  
- Persistence contracts (readers/writers/unit of work)  

Each vertical slice defines its own abstractions here, ensuring isolation and clarity.

---

## Folder Structure

### `Dogs/`
Contains all abstractions for the Dogs vertical slice.

This includes:

- **Commands**  
  - `RegisterDogCommand`  
  - `EditDogCommand`  
  - `RemoveDogCommand`  

- **Queries**  
  - `GetDogQuery`  
  - `ListDogsByOwnerQuery`  

- **DTOs**  
  - `GetDogResponse`  
  - `DogSummary`  
  - `ListDogsByOwnerResponse`  

- **Persistence Contracts**  
  - `IRegisterDogWriter`  
  - `IEditDogWriter`  
  - `IRemoveDogWriter`  
  - `IGetDogReader`  
  - `IGetDogByIdReader`  
  - `IListDogsByOwnerReader`  

Each subfolder represents a **vertical slice**, containing only the abstractions required for that slice.

These abstractions:

- Do not contain domain logic  
- Do not contain infrastructure logic  
- Do not expose domain entities directly (except internal readers like `IGetDogByIdReader`)  
- Are consumed by handlers in the application layer  
- Are implemented by infrastructure (EF Core, Dapper, etc.)

---

### `UnitOfWork/`
Contains the application‑level transactional boundary.

Includes:

- `IAppUnitOfWork` — the application’s unit of work abstraction

This interface:

- Extends the shared `IUnitOfWork` contract  
- Provides a stable transactional boundary for all vertical slices  
- Is implemented in the infrastructure layer  
- Is used by command handlers that require atomic operations  

---

## Architectural Principles

The abstractions follow these conventions:

- **CQRS**  
  Commands mutate state; queries read state.

- **Vertical Slices**  
  Each capability is self‑contained and owns its abstractions.

- **Infrastructure Inversion**  
  Persistence is defined by interfaces, implemented elsewhere.

- **DTO Projection**  
  Domain entities are not exposed to the API layer.

- **Purity**  
  Abstractions contain no logic—only contracts.

---

## What Does *Not* Belong Here

Do **not** place the following in this folder:

- Domain entities (`Dog`, `DogName`, `Breed`, etc.)  
- Infrastructure implementations (EF Core DbContext, repositories)  
- API endpoint definitions  
- Business rules  
- Validation logic  
- Application handlers  

Those belong in their respective layers.

---

## Related Components

- Application handlers (commands/queries)  
- Domain model (Dog aggregate and value objects)  
- Infrastructure readers/writers  
- API endpoints for dog management  
- Shared `IUnitOfWork` abstraction  

---

This folder defines the **contract boundary** for the entire application layer.  
Other layers implement the behavior.
