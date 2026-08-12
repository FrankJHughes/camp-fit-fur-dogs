# GetDogById — Application Abstractions

The **GetDogById** folder contains the application‑layer abstraction used to retrieve a dog by its unique identifier, without performing ownership validation.  
This is a low‑level read operation intended for internal workflows where the caller already has permission to access the dog, or where ownership checks occur elsewhere in the pipeline.

This folder contains **no domain logic**, **no DTOs**, and **no infrastructure implementations**.  
Its purpose is to define *what* the application needs to load a dog aggregate directly from persistence.

---

## Files in This Folder

### `IGetDogByIdReader.cs`

Defines the read‑side contract for retrieving a dog by its identifier.

This interface:

- Belongs to the Dogs vertical slice  
- Is implemented in the infrastructure layer  
- Returns a full domain entity (`Dog`)  
- Performs no projection into DTOs  
- Performs no ownership or authorization checks  

**Method:**

```csharp
Task<Dog?> ReadAsync(Guid dogId, CancellationToken ct);
```

The reader is responsible for:

- Locating the dog aggregate  
- Returning the domain entity directly  
- Returning `null` when no matching dog exists  

This abstraction is typically used by:

- Command handlers that need full domain behavior  
- Internal workflows that modify dog state  
- Operations where ownership has already been validated  

---

## Vertical Slice Responsibilities

The GetDogById slice is responsible for:

- Loading a dog aggregate by ID  
- Returning the domain model intact  
- Supporting write‑side operations that require domain invariants  
- Avoiding projection or transformation  

This slice is intentionally minimal and focused.

---

## What Does *Not* Belong Here

Do **not** place the following in this folder:

- DTOs (`GetDogResponse`, summaries, lists)  
- Ownership‑validated readers  
- Infrastructure implementations (EF Core, Dapper, etc.)  
- API endpoints  
- Business rules or validation logic  

Those belong in their respective layers or in other Dogs slices.

---

## Related Components

- Domain model (`Dog`, `DogName`, `Breed`, `Sex`)  
- Write‑side handlers (e.g., EditDogCommandHandler)  
- Ownership‑validated readers (`IGetDogReader`)  
- API endpoints that expose dog data  

---

This folder defines the **contract** for retrieving a dog aggregate by ID.  
Other layers implement the behavior.
