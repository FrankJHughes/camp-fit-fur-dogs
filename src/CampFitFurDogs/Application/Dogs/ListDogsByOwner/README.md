# ListDogsByOwner — Application Layer

The **CampFitFurDogs.Application.Dogs.ListDogsByOwner** namespace contains the application‑layer logic for retrieving all dogs owned by a specific user.  
This slice performs ownership‑validated retrieval and returns a stable, projection‑safe response DTO.

This namespace contains **no domain entities**, **no infrastructure code**, and **no abstractions**.  
Its purpose is to orchestrate the read workflow, enforce structural validation, and delegate persistence to the abstractions defined in `Application.Abstractions.Dogs.ListDogsByOwner`.

---

## Files in This Namespace

### ListDogsByOwnerQueryHandler

Handles the `ListDogsByOwnerQuery` by retrieving all dogs belonging to a specific owner.

This handler:

- Delegates retrieval to `IListDogsByOwnerReader`
- Performs no domain logic
- Returns a `ListDogsByOwnerResponse` containing all dogs owned by the user
- Operates purely as a read‑side orchestrator

**Workflow:**

1. Accept the `ListDogsByOwnerQuery`
2. Pass the `OwnerId` to the reader
3. Return the projected `ListDogsByOwnerResponse`

Ownership validation and projection are handled by the reader implementation in the infrastructure layer.

---

### ListDogsByOwnerQueryValidator

Provides structural validation for the `ListDogsByOwnerQuery`.

This validator:

- Ensures `OwnerId` is present  
- Ensures the caller is requesting dogs they actually own (`OwnerId == currentUser.Id`)  
- Performs *structural* validation only  

Domain‑level validation (e.g., verifying the owner exists or dogs are present) is handled by the reader.

---

## Architectural Role

The ListDogsByOwner application slice is responsible for:

- Enforcing structural validation  
- Ensuring the caller is authorized to list the requested dogs  
- Delegating retrieval to the read‑side abstraction  
- Returning a safe DTO to the API layer  

It does **not** contain domain logic or persistence logic.

---

## What Does *Not* Belong Here

Do **not** place the following in this namespace:

- Domain entities (`Dog`, `DogName`, `Breed`, `Sex`)  
- Infrastructure implementations (EF Core readers)  
- DTOs or abstractions  
- API endpoint definitions  
- Business rules  

Those belong in their respective layers.

---

## Related Components

- `Application.Abstractions.Dogs.ListDogsByOwner` — query, DTO, and reader contract  
- `IListDogsByOwnerReader` — infrastructure‑implemented read‑side abstraction  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`)  
- API endpoint for listing dogs  

---

This namespace defines the **application‑layer behavior** for listing all dogs owned by a user.  
Other layers implement the underlying domain and persistence logic.
