# Dogs — Application Layer

The **Application/Dogs** folder contains the application‑layer logic for the Dogs vertical slice.  
This includes command handlers, validators, and orchestration logic that sits between the API layer and the domain model.

This folder contains **no domain entities**, **no infrastructure code**, and **no DTOs**.  
Its purpose is to coordinate workflows, enforce structural validation, construct domain value objects, and delegate persistence to abstractions defined in `Application/Abstractions/Dogs`.

---

## Files in This Folder

### `EditDogCommandValidator.cs`

Provides structural validation for the `EditDogCommand`.

This validator:

- Ensures required fields are present  
- Validates that `Sex` maps to a valid domain enum  
- Performs *structural* validation only  
- Leaves *domain* validation to the `Dog` aggregate and its value objects  

**Responsibilities:**

- Validate `DogId` and `OwnerId` are non‑empty  
- Validate `Name` and `Breed` are non‑empty  
- Validate `Sex` is `"Male"` or `"Female"`  
- Prevent malformed commands from reaching the handler  

This ensures the handler receives well‑formed input before constructing domain objects.

---

### `EditDogCommandHandler.cs`

Handles the `EditDogCommand` by applying updates to an existing dog.

This handler:

- Constructs domain value objects (`DogName`, `Breed`, `Sex`, etc.)  
- Delegates persistence to `IEditDogWriter`  
- Commits the transaction through `IAppUnitOfWork`  
- Enforces domain invariants through the `Dog` aggregate  

**Workflow:**

1. Convert identifiers into domain types (`UserId`, `DogId`)  
2. Convert raw strings into domain value objects  
3. Invoke the write‑side persistence abstraction  
4. Commit the unit of work atomically  

This ensures edits are applied consistently and safely.

---

## Architectural Role

The Dogs application layer is responsible for:

- Orchestrating dog‑related workflows  
- Performing structural validation  
- Constructing domain value objects  
- Delegating persistence to abstractions  
- Committing changes through the unit of work  

It does **not** contain domain logic or persistence logic.

---

## What Does *Not* Belong Here

Do **not** place the following in this folder:

- Domain entities (`Dog`, `DogName`, `Breed`, etc.)  
- Infrastructure implementations (EF Core writers/readers)  
- DTOs or abstractions  
- API endpoint definitions  
- Business rules  

Those belong in their respective layers.

---

## Related Components

- `Application/Abstractions/Dogs` — commands, queries, DTOs, and persistence contracts  
- `Application/Abstractions/UnitOfWork` — transactional boundary  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`)  
- Infrastructure implementations of readers/writers  
- API endpoints for dog management  

---

This folder defines the **application‑layer behavior** for editing dogs.  
Other layers implement the underlying domain and persistence logic.
