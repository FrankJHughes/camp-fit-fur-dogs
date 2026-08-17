# Dogs — Application Abstractions

The **Dogs** folder contains all application‑layer abstractions for the Camp Fit Fur Dogs platform’s dog‑management vertical.  
These abstractions define the contracts between the API layer, the application layer, and the infrastructure layer.  
They describe *what* the application needs to perform dog‑related operations, not *how* those operations are implemented.

This folder contains **no domain logic**, **no infrastructure code**, and **no API endpoint definitions**.  
It is purely the interface boundary for the Dogs vertical slice.

---

## Vertical Slice Overview

The Dogs subsystem supports the following capabilities:

- Registering a new dog  
- Editing an existing dog  
- Removing a dog  
- Retrieving a single dog  
- Listing all dogs owned by a specific user  
- Internal domain‑level retrieval by ID  

Each capability is implemented as a **vertical slice**, with its own command/query, DTOs, and persistence abstractions.

---

## Subfolders and Their Responsibilities

### `RegisterDog`
Handles creation of new dogs.

Includes:

- `RegisterDogCommand` — data required to create a dog  
- `IRegisterDogWriter` — write‑side persistence contract  

This slice constructs a new `Dog` aggregate and persists it.

---

### `EditDog`
Handles updates to an existing dog.

Includes:

- `EditDogCommand` — updated dog data  
- `IEditDogWriter` — write‑side persistence contract  

This slice enforces ownership and domain rules before applying updates.

---

### `RemoveDog`
Handles deletion of a dog.

Includes:

- `RemoveDogCommand` — identifies the dog and owner  
- `IRemoveDogWriter` — write‑side persistence contract  

This slice performs a permanent delete operation after validation.

---

### `GetDog`
Retrieves a single dog owned by a specific user.

Includes:

- `GetDogQuery` — identifies the dog and owner  
- `GetDogResponse` — presentation‑safe DTO  
- `IGetDogReader` — read‑side persistence contract  

This slice performs ownership‑validated retrieval.

---

### `GetDogById`
Retrieves a dog by ID without ownership validation.

Includes:

- `IGetDogByIdReader` — returns a full `Dog` domain entity  

Used internally by write‑side workflows that require domain behavior.

---

### `ListDogsByOwner`
Retrieves all dogs owned by a specific user.

Includes:

- `ListDogsByOwnerQuery` — identifies the owner  
- `DogSummary` — lightweight dog DTO  
- `ListDogsByOwnerResponse` — collection wrapper  
- `IListDogsByOwnerReader` — read‑side persistence contract  

This slice returns a stable, immutable list of dog summaries.

---

## Architectural Principles

The Dogs abstractions follow these conventions:

- **CQRS** — commands mutate state; queries read state  
- **Vertical slices** — each capability is self‑contained  
- **DTO projection** — domain entities are never exposed directly to the API  
- **Infrastructure inversion** — persistence is defined by interfaces, implemented elsewhere  
- **Purity** — abstractions contain no logic, only contracts  

---

## What Does *Not* Belong Here

Do **not** place the following in this folder:

- Domain entities (`Dog`, `DogName`, `Breed`, etc.)  
- EF Core or other persistence implementations  
- API endpoints  
- Validation logic  
- Business rules  
- Handlers  

Those belong in the domain, infrastructure, API, or application handler layers.

---

## Related Components

- Application handlers (`RegisterDogCommandHandler`, `GetDogQueryHandler`, etc.)  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`)  
- Infrastructure readers/writers (EF Core, Dapper, etc.)  
- API endpoints for dog management  

---

This folder defines the **contract boundary** for all dog‑related operations.  
Other layers implement the behavior.
