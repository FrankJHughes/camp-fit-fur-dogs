# RemoveDog — Application Abstractions

The **RemoveDog** folder contains the application‑layer abstractions required to remove an existing dog owned by a specific user.  
These types define the write‑side contract between the API layer, the application layer, and the infrastructure layer, forming the deletion portion of the Dogs vertical slice.

This folder contains **no domain logic** and **no infrastructure concerns**.  
Its purpose is to describe *what* the application needs to delete a dog, not *how* the deletion is performed.

---

## Files in This Folder

### `RemoveDogCommand.cs`

Represents the command used to remove a dog.

This command:

- Implements `ICommand`  
- Is handled by `RemoveDogCommandHandler`  
- Carries the identifiers required to authorize and perform the removal  
- Does not contain domain logic  

**Fields include:**

- `DogId` — the dog being removed  
- `OwnerId` — the owner performing the removal  

The command is a simple data carrier.  
All validation, authorization, and domain rule enforcement occur in the handler and domain model.

---

### `IRemoveDogWriter.cs`

Defines the write‑side persistence contract for removing a dog.

This interface:

- Belongs to the Dogs vertical slice  
- Is implemented in the infrastructure layer  
- Performs the actual delete operation  
- Is invoked by `RemoveDogCommandHandler`  

**Method:**

```csharp
Task WriteAsync(Guid dogId, CancellationToken cancellationToken = default);
```

The writer is responsible for:

- Locating the dog aggregate  
- Performing a permanent delete operation  
- Relying on the application layer to ensure all invariants and authorization checks are already satisfied  

---

## Vertical Slice Responsibilities

The RemoveDog slice is responsible for:

- Accepting a delete request from the API  
- Validating ownership and permissions  
- Ensuring the dog exists and can be removed  
- Deleting the dog from persistence  
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

- `RemoveDogCommandHandler` — orchestrates the removal workflow  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`) — enforces invariants  
- Infrastructure writer — implements `IRemoveDogWriter`  
- API endpoint — receives the HTTP request and issues the command  

---

This folder defines the **contract** for removing a dog.  
Other layers implement the behavior.
