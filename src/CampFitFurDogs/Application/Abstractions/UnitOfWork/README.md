# UnitOfWork — Application Abstractions

The **UnitOfWork** folder contains the application‑layer transactional abstraction used across all vertical slices in the Camp Fit Fur Dogs application.  
It defines the contract for coordinating atomic persistence operations without exposing infrastructure details such as EF Core, database contexts, or transaction scopes.

This folder contains **no domain logic**, **no persistence logic**, and **no API concerns**.  
Its purpose is to provide a stable, intention‑revealing interface for committing or rolling back changes within the application layer.

---

## File in This Folder

### `IAppUnitOfWork.cs`

Defines the application‑level unit of work abstraction.

This interface:

- Inherits from the shared `IUnitOfWork` contract  
- Serves as the transactional boundary for all vertical slices  
- Is implemented in the infrastructure layer  
- Is used by command handlers that require atomic operations (e.g., registering, editing, or removing dogs)

**Declaration:**

```csharp
public interface IAppUnitOfWork : IUnitOfWork { }
```

The alias provides clarity and avoids leaking infrastructure naming conventions into the application layer.

---

## Responsibilities

The application Unit of Work is responsible for:

- Coordinating atomic write operations  
- Ensuring consistency across multiple persistence actions  
- Allowing command handlers to commit or roll back changes  
- Providing a clean separation between application logic and infrastructure concerns  

Typical usage occurs in:

- `RegisterDogCommandHandler`  
- `EditDogCommandHandler`  
- `RemoveDogCommandHandler`  
- Any other slice that modifies application state  

---

## What Does *Not* Belong Here

Do **not** place the following in this folder:

- Domain entities (`Dog`, `DogName`, `Breed`, etc.)  
- Infrastructure implementations (EF Core DbContext, repositories)  
- API endpoint definitions  
- Business rules or validation logic  

Those belong in their respective layers.

---

## Related Components

- `IUnitOfWork` (shared abstraction from Frank.Core)  
- Infrastructure implementation (e.g., EF Core unit of work)  
- Application command handlers that require transactional consistency  

---

This folder defines the **transactional contract** for all application‑level operations.  
Other layers implement the behavior.
