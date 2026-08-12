# EditDog — Application Abstractions

The **EditDog** folder contains the application‑layer abstractions required to update an existing dog in the Camp Fit Fur Dogs platform.  
These types define the contract between the API layer, the application layer, and the infrastructure layer, forming the write‑side of the Dogs vertical slice.

This folder contains **no domain logic** and **no infrastructure concerns**.  
Its purpose is to describe *what* the application needs to perform an edit operation, not *how* it is executed.

---

## Files in This Folder

### `EditDogCommand.cs`
Represents the command issued when a user edits a dog they own.

This command:

- Is part of the Dogs vertical slice  
- Is handled by `EditDogCommandHandler`  
- Contains only the data required to perform the update  
- Relies on the domain model and handler for validation and business rules  

**Fields include:**

- `DogId` — the dog being edited  
- `OwnerId` — the owner performing the edit  
- `Name` — updated dog name  
- `Breed` — updated breed  
- `DateOfBirth` — updated date of birth  
- `Sex` — updated sex  

The command implements `ICommand`, making it part of the CQRS write pipeline.

---

### `IEditDogWriter.cs`
Defines the write‑side persistence contract for updating a dog.

This interface:

- Is implemented in the infrastructure layer  
- Is invoked by `EditDogCommandHandler`  
- Persists changes to the dog aggregate  
- Enforces ownership and domain invariants through the domain model  

**Method:**

```csharp
Task WriteAsync(
    UserId ownerId,
    DogId id,
    DogName name,
    Breed breed,
    DateOnly dateOfBirth,
    Sex sex,
    CancellationToken cancellationToken);
```

The writer is responsible for:

- Locating the dog aggregate  
- Applying updated values  
- Committing changes to the persistence store  

---

## Vertical Slice Responsibilities

The EditDog slice is responsible for:

- Accepting an edit request from the API  
- Validating ownership and permissions  
- Applying domain rules (via the domain model)  
- Persisting the updated dog  
- Returning success or failure to the caller  

This folder provides the **application‑layer contract** for those operations.

---

## What Does *Not* Belong Here

Do **not** place the following in this folder:

- Domain entities (`Dog`, `DogName`, `Breed`, etc.)  
- Infrastructure implementations (EF Core writers, repositories)  
- API endpoint definitions  
- Validation logic  
- Business rules  

Those belong in their respective layers.

---

## Related Components

- `EditDogCommandHandler` — orchestrates the edit operation  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`) — enforces invariants  
- Infrastructure writer — implements `IEditDogWriter`  
- API endpoint — receives the HTTP request and issues the command  

---

This folder defines the **contract** for editing a dog.  
Other layers implement the behavior.
