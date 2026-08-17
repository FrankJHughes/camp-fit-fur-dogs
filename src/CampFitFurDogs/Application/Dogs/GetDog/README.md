# GetDog — Application Layer

The **CampFitFurDogs.Application.Dogs.GetDog** namespace contains the application‑layer logic for retrieving a single dog owned by a specific user.  
This slice handles ownership‑validated retrieval and projection into a presentation‑safe DTO.

This folder contains **no domain entities**, **no infrastructure code**, and **no abstractions**.  
Its purpose is to orchestrate the read workflow, enforce structural validation, and delegate persistence to the abstractions defined in `Application.Abstractions.Dogs.GetDog`.

---

## Files in This Namespace

### `GetDogQueryHandler.cs`

Handles the `GetDogQuery` by retrieving a dog and projecting it into a `GetDogResponse`.

This handler:

- Delegates retrieval to `IGetDogReader`
- Performs no domain logic
- Returns `null` if the dog does not exist or does not belong to the owner
- Operates purely as a read‑side orchestrator

**Workflow:**

1. Accept the `GetDogQuery`
2. Pass identifiers to the reader
3. Return the projected `GetDogResponse` or `null`

Ownership validation and projection are handled by the reader implementation in the infrastructure layer.

---

### `GetDogQueryValidator.cs`

Provides structural validation for the `GetDogQuery`.

This validator:

- Ensures `OwnerId` is present
- Ensures the caller is requesting a dog they own (`OwnerId == currentUser.Id`)
- Ensures `DogId` is present
- Performs *structural* validation only

Domain‑level validation (such as verifying the dog exists) is handled by the reader.

---

## Architectural Role

The GetDog application slice is responsible for:

- Enforcing structural validation
- Ensuring the caller is authorized to retrieve the dog
- Delegating retrieval to the read‑side abstraction
- Returning a safe DTO to the API layer

It does **not** contain domain logic or persistence logic.

---

## What Does *Not* Belong Here

Do **not** place the following in this namespace:

- Domain entities (`Dog`, `DogName`, `Breed`, etc.)
- Infrastructure implementations (EF Core readers)
- DTOs or abstractions
- API endpoint definitions
- Business rules

Those belong in their respective layers.

---

## Related Components

- `Application.Abstractions.Dogs.GetDog` — query, DTO, and reader contract
- `IGetDogReader` — infrastructure‑implemented read‑side abstraction
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`)
- API endpoint for retrieving a dog

---

This namespace defines the **application‑layer behavior** for retrieving a dog.  
Other layers implement the underlying domain and persistence logic.
