# RegisterDog — Application Abstractions

The **RegisterDog** folder contains the application‑layer abstractions required to register a new dog under a specific owner.  
These types define the write‑side contract between the API layer, the application layer, and the infrastructure layer, forming the creation portion of the Dogs vertical slice.

This folder contains **no domain logic** and **no infrastructure concerns**.  
Its purpose is to describe *what* the application needs to create and persist a new dog, not *how* the operation is executed.

---

## Files in This Folder

### `RegisterDogCommand.cs`

Represents the command used to register a new dog.

This command:

- Implements `ICommand<Guid>`  
- Is handled by `RegisterDogCommandHandler`  
- Carries the data required to construct a new `Dog` aggregate  
- Returns the newly created dog’s identifier upon success  

**Fields include:**

- `OwnerId` — the owner registering the dog  
- `Name` — the dog’s name  
- `Breed` — the dog’s breed  
- `DateOfBirth` — the dog’s date of birth  
- `Sex` — the dog’s sex  

The command is a simple data carrier.  
All validation, authorization, and domain rule enforcement occur in the handler and domain model.

---

### `IRegisterDogWriter.cs`

Defines the write‑side persistence contract for registering a new dog.

This interface:

- Belongs to the Dogs vertical slice  
- Is implemented in the infrastructure layer  
- Persists a fully‑constructed `Dog` aggregate  
- Is invoked by `RegisterDogCommandHandler`  

**Method:**

```csharp
Task WriteAsync(Dog dog, CancellationToken cancellationToken = default);
```

The writer is responsible for:

- Inserting the dog into the persistence store  
- Ensuring the aggregate is saved atomically  
- Relying on the application layer to supply a valid domain object  

---

## Vertical Slice Responsibilities

The RegisterDog slice is responsible for:

- Accepting a registration request from the API  
- Validating ownership and input data  
- Constructing a new `Dog` aggregate  
- Persisting the aggregate  
- Returning the new dog’s identifier  

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

- `RegisterDogCommandHandler` — orchestrates the registration workflow  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`) — enforces invariants  
- Infrastructure writer — implements `IRegisterDogWriter`  
- API endpoint — receives the HTTP request and issues the command  

---

This folder defines the **contract** for registering a new dog.  
Other layers implement the behavior.
