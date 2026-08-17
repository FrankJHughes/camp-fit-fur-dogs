
# CampFitFurDogs.Application.Dogs

The `CampFitFurDogs.Application.Dogs` namespace contains all **application‑layer vertical slices** related to dog management.  
Each slice encapsulates its own commands, queries, validators, handlers, and supporting abstractions.

This namespace contains **no domain entities**, **no infrastructure**, and **no API endpoints**.  
Its responsibility is to orchestrate workflows, enforce structural validation, perform resource‑level authorization, and delegate persistence to abstractions.

---

## 🎯 Architectural Role

The Dogs application layer provides:

- **Owner‑scoped workflows** (register, edit, remove, list, get)
- **Structural validation** via FluentValidation
- **Identity consistency** (OwnerId must match `ICurrentUser.Id`)
- **Resource‑level authorization** (dog must exist and belong to the caller)
- **Orchestration** of domain value objects and aggregates
- **Delegation** to read/write persistence abstractions
- **Transactional consistency** via `IAppUnitOfWork`

Domain invariants are enforced by the domain model (`Dog`, `DogName`, `Breed`, `Sex`), not by the application layer.

---

## 📦 Vertical Slices Included

Each slice lives in its own folder under:

```
CampFitFurDogs.Application.Dogs
```

### RegisterDog
Allows an owner to register a new dog.

Includes:

- `RegisterDogCommand`
- `RegisterDogCommandValidator`
- `RegisterDogHandler`

Validator responsibilities:

- Required fields  
- Valid sex  
- Identity consistency (`OwnerId == currentUser.Id`)

Handler responsibilities:

- Construct domain value objects  
- Delegate to `IRegisterDogWriter`  
- Commit via `IAppUnitOfWork`

---

### EditDog
Allows an owner to edit an existing dog.

Includes:

- `EditDogCommand`
- `EditDogCommandValidator`
- `EditDogHandler`

Validator responsibilities:

- Required fields  
- Valid sex  
- Identity consistency

Handler responsibilities:

- Retrieve dog via `IGetDogByIdReader`  
- Ensure dog exists and belongs to owner  
- Apply updates via `IEditDogWriter`  
- Commit via `IAppUnitOfWork`

---

### RemoveDog
Allows an owner to remove a dog they own.

Includes:

- `RemoveDogCommand`
- `RemoveDogCommandValidator`
- `RemoveDogHandler`

Validator responsibilities:

- Required identifiers  
- Identity consistency

Handler responsibilities:

- Retrieve dog  
- Ensure ownership  
- Delegate deletion  
- Commit transaction

---

### GetDog
Retrieves a single dog owned by the authenticated user.

Includes:

- `GetDogQuery`
- `GetDogQueryValidator`
- `GetDogHandler`

Validator responsibilities:

- Required identifiers  
- Identity consistency

Handler responsibilities:

- Retrieve dog  
- Ensure ownership  
- Return DTO

---

### ListDogsByOwner
Lists all dogs belonging to the authenticated owner.

Includes:

- `ListDogsByOwnerQuery`
- `ListDogsByOwnerQueryValidator`
- `ListDogsByOwnerHandler`

Validator responsibilities:

- Required identifiers  
- Identity consistency

Handler responsibilities:

- Retrieve all dogs for owner  
- Return DTO list

---

## 🔐 Owner‑Scoped vs Admin‑Scoped Slices

This namespace contains **only owner‑scoped slices**.

Owner‑scoped slices enforce:

- Identity consistency in validators  
- Resource‑level authorization in handlers  

Admin/operator slices live in:

```
CampFitFurDogs.Application.Admin.Dogs
```

Admin slices:

- Do **not** include OwnerId in DTOs  
- Do **not** enforce identity consistency  
- Do **not** perform ownership checks  
- Rely on role‑based authorization policies

This separation keeps vertical slices clean and predictable.

---

## 🔧 Dependency Injection

All Dogs CQRS components are registered via:

```csharp
services.AddApplicationDogs();
```

This extension:

- Scans the Dogs assembly  
- Registers command handlers  
- Registers query handlers  
- Registers validators  
- Restricts discovery to the Dogs namespace

See:

- `ServiceCollectionExtensions.AddApplicationDogs`

---

## 🚫 What Does *Not* Belong Here

The Dogs application layer **must not** contain:

- Domain entities (`Dog`, `DogName`, `Breed`, `Sex`)
- EF Core readers/writers
- API endpoints
- DTOs for HTTP transport
- Business rules or domain invariants

Those belong in their respective layers.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Domain.Dogs` — domain model  
- `CampFitFurDogs.Application.Abstractions.Dogs` — read/write contracts  
- `CampFitFurDogs.Infrastructure.Dogs` — EF Core implementations  
- `CampFitFurDogs.Api.Dogs` — HTTP endpoints  

---
