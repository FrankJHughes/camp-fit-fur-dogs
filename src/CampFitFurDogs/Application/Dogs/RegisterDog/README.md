# RegisterDog — Application Layer

The **CampFitFurDogs.Application.Dogs.RegisterDog** namespace contains the application‑layer logic for registering a new dog.  
This slice coordinates structural validation, domain object construction, aggregate creation, and transactional persistence.

This namespace contains **no domain entities**, **no infrastructure code**, and **no abstractions**.  
Its purpose is to orchestrate the registration workflow and delegate persistence to the abstractions defined in `Application.Abstractions.Dogs.RegisterDog`.

---

## Files in This Namespace

### RegisterDogCommandValidator

Provides structural validation for the `RegisterDogCommand`.

This validator:

- Ensures `OwnerId`, `Name`, and `Breed` are present  
- Ensures `DateOfBirth` is in the past  
- Ensures `Sex` is `"Male"` or `"Female"`  
- Performs *structural* validation only  

Domain invariants (e.g., valid breed, name rules, aggregate consistency) are enforced by the `Dog` aggregate and its value objects.

---

### RegisterDogCommandHandler

Handles the `RegisterDogCommand` by creating a new `Dog` aggregate and committing it atomically.

This handler:

- Parses and validates the `Sex` enum  
- Constructs domain value objects (`DogName`, `Breed`, etc.)  
- Creates the `Dog` aggregate  
- Delegates persistence to `IRegisterDogWriter`  
- Commits the transaction through `IAppUnitOfWork`  
- Returns the newly created dog’s identifier  

The handler performs no infrastructure logic and no API concerns; it is purely an application‑layer orchestrator.

---

## Architectural Role

The RegisterDog application slice is responsible for:

- Enforcing structural validation  
- Constructing domain value objects  
- Creating the `Dog` aggregate  
- Delegating persistence to the write‑side abstraction  
- Committing changes through the unit of work  
- Returning a safe identifier to the API layer  

It does **not** contain domain logic or persistence logic.

---

## What Does *Not* Belong Here

Do **not** place the following in this namespace:

- Domain entities (`Dog`, `DogName`, `Breed`, `Sex`)  
- Infrastructure implementations (EF Core writers)  
- DTOs or abstractions  
- API endpoint definitions  
- Business rules  

Those belong in their respective layers.

---

## Related Components

- `Application.Abstractions.Dogs.RegisterDog` — command, DTO, and writer contract  
- `IRegisterDogWriter` — infrastructure‑implemented write‑side abstraction  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`)  
- API endpoint for registering dogs  

---

This namespace defines the **application‑layer behavior** for registering a new dog.  
Other layers implement the underlying domain and persistence logic.
