
# CampFitFurDogs.Domain.Dogs

The `CampFitFurDogs.Domain.Dogs` namespace contains the full domain model for dog‑related concepts in the CampFitFurDogs system.  
These types represent **pure domain logic**, including aggregates, value objects, and invariants that define the behavior and identity of dogs within the system.

This layer contains **no application logic**, **no persistence concerns**, and **no infrastructure dependencies**.  
It is responsible solely for enforcing domain rules and maintaining consistency of dog‑related entities.

---

## 🎯 Architectural Role

The domain layer provides:

- **Aggregates** that enforce invariants and lifecycle rules  
- **Value objects** that encapsulate validation and equality semantics  
- **Strongly‑typed identifiers** for safety and clarity  
- **Immutable domain state** except where controlled mutation is explicitly allowed  
- **Pure business logic**, independent of application or infrastructure layers  

This namespace is consumed by the application layer, which orchestrates workflows around these domain types.

---

## 📦 Included Domain Types

### `Dog` (Aggregate Root)
Represents a dog owned by a user.

Responsibilities:

- Maintains ownership (`OwnerId`)  
- Holds immutable identity (`DogId`)  
- Stores name, breed, date of birth, and sex  
- Enforces invariants through factory and update methods  
- Provides controlled mutation via `Update(...)`  

The aggregate ensures that ownership never changes after creation.

---

### `DogId` (Strongly‑Typed Identifier)
A wrapper around `Guid` providing:

- Type safety  
- Domain‑specific validation  
- Clear separation from other identifiers  

Created via:

- `DogId.New()`  
- `DogId.From(Guid)`  

---

### `DogName` (Value Object)
Encapsulates the dog’s name.

Responsibilities:

- Enforces non‑empty, trimmed values  
- Provides value‑based equality  
- Prevents invalid or malformed names from entering the domain  

---

### `Breed` (Value Object)
Represents the dog’s breed.

Responsibilities:

- Enforces non‑empty, trimmed values  
- Provides value‑based equality  
- Treats breed as an immutable domain concept  

---

### `Sex` (Enum)
Represents the biological sex of the dog.

Values:

- `Male`  
- `Female`  

Used by the `Dog` aggregate to capture immutable biological characteristics.

---

## 🔐 Domain Invariants

The domain enforces several key invariants:

- A dog **must** have an owner.  
- A dog’s **name**, **breed**, and **sex** must be valid at creation.  
- A dog’s **identity** (`DogId`) must never be empty.  
- Ownership **cannot change** after creation.  
- Value objects must always be in a valid state.  

These invariants ensure that the domain remains consistent regardless of how the application layer orchestrates workflows.

---

## 🚫 What Does *Not* Belong Here

The domain layer must **not** contain:

- Application logic (commands, queries, handlers)  
- Persistence logic (EF Core, repositories, DbContexts)  
- Infrastructure concerns (email, storage, external APIs)  
- HTTP or API DTOs  
- Validation frameworks (FluentValidation)  

Only **pure domain logic** belongs here.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Application.Dogs` — vertical slices orchestrating workflows  
- `CampFitFurDogs.Application.Abstractions` — persistence contracts  
- `CampFitFurDogs.Infrastructure.Dogs` — EF Core implementations  
- `CampFitFurDogs.Api.Dogs` — HTTP endpoints and DTOs  

---

