
# CampFitFurDogs.Infrastructure.Dogs

The `CampFitFurDogs.Infrastructure.Dogs` namespace contains the full persistence‑layer implementation for the Dogs vertical slice.  
These components provide EF Core–backed readers and writers that the application layer uses to perform dog‑related workflows such as registration, editing, deletion, and querying.

This namespace contains **no domain logic** and **no application orchestration**.  
Its sole responsibility is to translate domain aggregates into database representations and back.

---

## 🎯 Architectural Role

The infrastructure layer:

- Implements persistence for domain aggregates  
- Provides EF Core configurations  
- Supplies readers and writers consumed by the application layer  
- Ensures domain types remain pure and strongly typed  
- Avoids leaking EF Core concerns into the domain or application layers  

All services here are registered via dependency injection using scoped lifetimes.

---

## 📦 Included Components

### `DogConfiguration`
Configures EF Core mapping for the `Dog` aggregate.

Responsibilities:

- Maps `DogId` and `OwnerId` using strongly‑typed conversions  
- Configures owned value objects (`DogName`, `Breed`)  
- Maps primitive properties (`DateOfBirth`, `Sex`)  
- Defines the `"dogs"` table schema  

Explore the aggregate:  
**[Dog aggregate](ca://s?q=Explain_the_Dog_aggregate)**

---

### `EditDogWriter`
Updates an existing dog.

Responsibilities:

- Loads the dog by `DogId`  
- Ensures the dog belongs to the requesting owner  
- Applies domain‑approved updates via `Dog.Update`  
- Defers saving to the application layer  

---

### `GetDogByIdReader`
Reads a dog by its identifier.

Responsibilities:

- Converts raw GUID → `DogId`  
- Performs a no‑tracking query  
- Returns the domain aggregate or `null`  

---

### `GetDogReader`
Reads a dog profile for a specific owner.

Responsibilities:

- Ensures the dog belongs to the owner  
- Performs a no‑tracking query  
- Maps the aggregate into a `GetDogResponse` DTO  

Explore the DTO:  
**[GetDogResponse](ca://s?q=Explain_GetDogResponse)**

---

### `ListDogsByOwnerReader`
Lists all dogs belonging to an owner.

Responsibilities:

- Converts raw GUID → `UserId`  
- Performs a no‑tracking query  
- Projects aggregates into lightweight `DogSummary` DTOs  
- Wraps results in `ListDogsByOwnerResponse`  

---

### `RegisterDogWriter`
Registers a new dog.

Responsibilities:

- Adds the aggregate to the EF Core change tracker  
- Defers saving to the application layer  
- Ensures domain invariants are already satisfied  

Explore dog creation:  
**[Dog creation workflow](ca://s?q=Explain_Dog_creation_workflow)**

---

### `RemoveDogWriter`
Removes an existing dog.

Responsibilities:

- Loads the dog by `DogId`  
- Throws if not found  
- Marks the aggregate for deletion  

---

### `ServiceCollectionExtensions`
Registers all infrastructure services for the Dogs vertical slice.

Services registered:

- `IEditDogWriter` → `EditDogWriter`  
- `IRegisterDogWriter` → `RegisterDogWriter`  
- `IRemoveDogWriter` → `RemoveDogWriter`  
- `IGetDogByIdReader` → `GetDogByIdReader`  
- `IGetDogReader` → `GetDogReader`  
- `IListDogsByOwnerReader` → `ListDogsByOwnerReader`  

Explore DI patterns:  
**[Dependency injection in vertical slices](ca://s?q=Explain_DI_in_vertical_slices)**

---

## 🧭 Persistence Layer Principles

- **No domain logic**  
- **No application orchestration**  
- **No business validation**  
- **No cross‑aggregate workflows**  
- **No EF Core leakage into domain types**  

The infrastructure layer is intentionally thin and mechanical.

---

## 🚫 What Does *Not* Belong Here

This namespace must **not** contain:

- Domain invariants  
- Application commands/queries  
- API DTOs or controllers  
- Business rules  
- Cross‑aggregate logic  

Only persistence concerns belong here.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Domain.Dogs` — aggregates, value objects, invariants  
- `CampFitFurDogs.Application.Dogs` — vertical slices orchestrating workflows  
- `CampFitFurDogs.Api.Dogs` — HTTP endpoints and DTOs  

---

