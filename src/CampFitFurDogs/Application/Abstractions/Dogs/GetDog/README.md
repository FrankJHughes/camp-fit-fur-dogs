# GetDog — Application Abstractions

The **GetDog** folder contains the application‑layer abstractions required to retrieve a single dog owned by a specific user.  
These types define the read‑side contract between the API layer, the application layer, and the infrastructure layer, forming the query portion of the Dogs vertical slice.

This folder contains **no domain logic** and **no infrastructure concerns**.  
Its purpose is to describe *what* the application needs to read dog data, not *how* the data is retrieved.

---

## Files in This Folder

### `GetDogResponse.cs`

Represents the data returned when retrieving a single dog.

This DTO:

- Is part of the Dogs vertical slice  
- Is returned by the `GetDogQueryHandler`  
- Is consumed by the `GetDogEndpoint`  
- Contains only presentation‑safe fields  
- Does not expose domain objects directly  

**Fields include:**

- `Id` — the dog’s unique identifier  
- `OwnerId` — the owner who registered the dog  
- `Name` — the dog’s name  
- `Breed` — the dog’s breed  
- `DateOfBirth` — the dog’s date of birth  
- `Sex` — the dog’s sex  

Domain invariants and authorization checks occur earlier in the pipeline.

---

### `GetDogQuery.cs`

Represents the query used to retrieve a single dog owned by a specific user.

This query:

- Implements `IQuery<GetDogResponse?>`  
- Is handled by `GetDogQueryHandler`  
- Performs ownership validation  
- Returns `GetDogResponse` or `null`  

**Fields include:**

- `DogId` — the dog being retrieved  
- `OwnerId` — the owner requesting the dog  

The query carries only the identifiers required to locate and authorize access to the dog aggregate.

---

### `IGetDogReader.cs`

Defines the read‑side abstraction for retrieving a single dog.

This interface:

- Is implemented in the infrastructure layer  
- Is invoked by `GetDogQueryHandler`  
- Projects domain entities into `GetDogResponse`  
- Returns `null` when no matching dog is found  

**Method:**

```csharp
Task<GetDogResponse?> ReadAsync(
    Guid dogId,
    Guid ownerId,
    CancellationToken ct);
```

The reader is responsible for:

- Locating the dog  
- Ensuring the dog belongs to the requesting owner  
- Returning a presentation‑ready DTO  

---

## Vertical Slice Responsibilities

The GetDog slice is responsible for:

- Accepting a read request from the API  
- Validating ownership and permissions  
- Loading the dog from persistence  
- Projecting the domain entity into a safe DTO  
- Returning the dog or `null`  

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

- `GetDogQueryHandler` — orchestrates the read operation  
- Domain model (`Dog`, `DogName`, `Breed`, `Sex`) — enforces invariants  
- Infrastructure reader — implements `IGetDogReader`  
- API endpoint — receives the HTTP request and issues the query  

---

This folder defines the **contract** for retrieving a dog.  
Other layers implement the behavior.
