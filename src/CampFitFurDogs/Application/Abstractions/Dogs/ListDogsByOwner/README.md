# ListDogsByOwner — Application Abstractions

The **ListDogsByOwner** folder contains the application‑layer abstractions required to retrieve all dogs owned by a specific user.  
These types define the read‑side contract between the API layer, the application layer, and the infrastructure layer, forming the collection‑query portion of the Dogs vertical slice.

This folder contains **no domain logic** and **no infrastructure concerns**.  
Its purpose is to describe *what* the application needs to read a user’s dog list, not *how* the data is retrieved.

---

## Files in This Folder

### `IListDogsByOwnerReader.cs`

Defines the read‑side abstraction for retrieving all dogs owned by a specific user.

This interface:

- Belongs to the Dogs vertical slice  
- Is implemented in the infrastructure layer  
- Projects domain entities into presentation‑safe DTOs  
- Returns a `ListDogsByOwnerResponse` containing all dogs for the owner  

**Method:**

```csharp
Task<ListDogsByOwnerResponse> ReadAsync(Guid ownerId, CancellationToken ct);
```

The reader is responsible for:

- Locating all dogs belonging to the owner  
- Projecting them into lightweight summaries  
- Returning a stable, immutable response DTO  

---

### `ListDogsByOwnerQuery.cs`

Represents the query used to retrieve all dogs owned by a specific user.

This query:

- Implements `IQuery<ListDogsByOwnerResponse>`  
- Is handled by `ListDogsByOwnerQueryHandler`  
- Performs ownership‑scoped retrieval  
- Returns a collection of dog summaries  

**Field:**

- `OwnerId` — the owner whose dogs should be retrieved  

The query carries only the identifier required to locate and authorize access to the dog collection.

---

### `DogSummary.cs` and `ListDogsByOwnerResponse.cs`

These DTOs define the shape of the data returned to the API.

#### `DogSummary`

A lightweight representation of a dog, containing:

- `Id` — dog identifier  
- `Name` — dog name  
- `Breed` — dog breed  

Used when returning collections of dogs.

#### `ListDogsByOwnerResponse`

Wraps a read‑only list of `DogSummary` records.

This DTO:

- Is produced by the query handler  
- Is consumed by the API endpoint  
- Contains only presentation‑safe fields  
- Does not expose domain entities directly  

---

## Vertical Slice Responsibilities

The ListDogsByOwner slice is responsible for:

- Accepting a read request from the API  
- Loading all dogs belonging to a specific owner  
- Projecting domain entities into safe DTOs  
- Returning a stable, immutable response  

This folder provides the **application‑layer contract** for those operations.

---

## What Does *Not* Belong Here

Do **not** place the following in this folder:

- Domain entities (`Dog`, `DogName`, `Breed`, etc.)  
- Infrastructure implementations (EF Core readers, repositories)  
- API endpoint definitions  
- Validation logic  
- Business rules  

Those belong in their respective layers.

---

## Related Components

- `ListDogsByOwnerQueryHandler` — orchestrates the read operation  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`) — enforces invariants  
- Infrastructure reader — implements `IListDogsByOwnerReader`  
- API endpoint — receives the HTTP request and issues the query  

---

This folder defines the **contract** for listing all dogs owned by a user.  
Other layers implement the behavior.
