# RemoveDog — Application Layer

The **CampFitFurDogs.Application.Dogs.RemoveDog** namespace contains the application‑layer logic for removing a dog owned by the authenticated user.  
This slice coordinates structural validation, resource‑level authorization, existence checks, deletion, and transactional persistence.

This namespace contains **no domain entities**, **no infrastructure code**, and **no abstractions**.  
Its purpose is to orchestrate the removal workflow and delegate persistence to the abstractions defined in `Application.Abstractions.Dogs.RemoveDog`.

---

## Files in This Namespace

### RemoveDogCommandValidator

Provides structural validation for the `RemoveDogCommand`.

This validator:

- Ensures `DogId` is present  
- Ensures `OwnerId` is present  
- Ensures `OwnerId` matches the authenticated user (`ICurrentUser`)  

Because the API endpoint constructs the command using the authenticated user’s ID, this validator ensures the command is structurally correct and consistent with the caller’s identity.  
It does **not** perform any database lookups or resource‑level authorization.

Domain‑level validation and existence checks are handled by the handler.

---

### RemoveDogCommandHandler

Handles the `RemoveDogCommand` by performing resource‑level authorization and orchestrating the removal workflow.

This handler:

- Retrieves the dog using `IGetDogByIdReader`  
- Ensures the dog exists  
- Ensures the dog actually belongs to the authenticated user  
- Delegates deletion to `IRemoveDogWriter`  
- Commits the transaction through `IAppUnitOfWork`  

The handler performs **resource‑level authorization**, which requires a database lookup and therefore cannot be done in the validator.  
This ensures that only the rightful owner can remove a dog and that the dog is in a valid state for removal.

---

## Architectural Role

The RemoveDog application slice is responsible for:

- Structural validation (validator)  
- Identity consistency (validator)  
- Existence checks (handler)  
- Resource‑level authorization (handler)  
- Delegating deletion to the write‑side abstraction  
- Committing changes through the unit of work  

It does **not** contain domain logic or persistence logic.

---

## What Does *Not* Belong Here

Do **not** place the following in this namespace:

- Domain entities (`Dog`, `DogName`, `Breed`, `Sex`)  
- Infrastructure implementations (EF Core readers/writers)  
- DTOs or abstractions  
- API endpoint definitions  
- Business rules or domain invariants  

Those belong in their respective layers.

---

## Related Components

- `Application.Abstractions.Dogs.RemoveDog` — command and writer contract  
- `Application.Abstractions.Dogs.GetDogById` — read‑side abstraction  
- `IGetDogByIdReader` — infrastructure‑implemented reader  
- `IRemoveDogWriter` — infrastructure‑implemented writer  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`)  
- API endpoint for removing dogs  

---

This namespace defines the **application‑layer behavior** for removing a dog owned by the authenticated user.  
Other layers implement the underlying domain and persistence logic.
