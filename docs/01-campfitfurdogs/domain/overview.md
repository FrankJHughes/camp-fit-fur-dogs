# Domain Layer Overview

The CampFitFurDogs domain layer contains the core business logic and rules governing dog management. It is organized around the `Dog` aggregate root, which encapsulates all state, behavior, and invariants related to dog entities. The domain layer is completely independent of the application, infrastructure, and API layers.

## Core Concepts

### Aggregate: Dog

The `Dog` aggregate is the consistency boundary for all dog‑related domain operations:

```csharp
public sealed class Dog : AggregateRoot<DogId>
{
    public UserId OwnerId { get; private set; }
    public DogName Name { get; private set; }
    public Breed Breed { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Sex Sex { get; private set; }

    public static Dog Create(
        UserId ownerId,
        DogName name,
        Breed breed,
        DateOnly dateOfBirth,
        Sex sex)
    {
        // Enforce domain invariants
        return new Dog(DogId.New(), ownerId, name, breed, dateOfBirth, sex);
    }
}
```

**Key Invariants:**

- Every dog has exactly one owner (`UserId`)  
- A dog must have a valid name (non‑empty, trimmed)  
- A dog must have a valid breed (non‑empty, trimmed)  
- A dog must have a date of birth  
- A dog must have a sex (`Male` or `Female`)  

The aggregate enforces these invariants internally, ensuring domain consistency.

---

## Value Objects

Value objects encapsulate domain primitives and enforce invariants at construction time.

### DogName

Represents a dog's name with validation:

- Cannot be null, empty, or whitespace  
- Automatically trimmed  
- Equality based on normalized value  

```csharp
public sealed class DogName : ValueObject
{
    public string Value { get; }

    public static DogName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dog name is required.");

        return new DogName(value.Trim());
    }
}
```

### Breed

Represents a dog's breed with the same validation pattern as `DogName`:

- Cannot be null, empty, or whitespace  
- Automatically trimmed  
- Supports any breed string (e.g., “Labrador Retriever”, “Poodle Mix”)  

### Sex

An enumeration representing biological sex:

```csharp
public enum Sex
{
    Male,
    Female
}
```

---

## Strongly Typed Identifiers

### DogId

A strongly typed identifier for dogs:

```csharp
public sealed class DogId : AggregateId { }
```

Usage:

```csharp
var dogId = DogId.New();            // Generate new ID
var existingId = DogId.From(guid);  // Wrap existing GUID
```

Strongly typed IDs prevent accidental misuse of identifiers across aggregates.

---

## Domain Rules and Invariants

1. **Ownership Immutability**  
   Once a dog is created for an owner, ownership cannot change.

2. **Name Validation**  
   Dog names must be non‑empty and properly formatted.

3. **Breed Validation**  
   Dog breeds must be non‑empty and properly formatted.

4. **Identity Uniqueness**  
   Each dog has a globally unique identifier.

These rules ensure that the domain model remains consistent regardless of how the application layer interacts with it.

---

## Persistence Mapping

The domain model is persisted using Entity Framework Core. Mapping ensures:

- `Dog` entities are stored in the `dogs` table  
- Value objects (`DogName`, `Breed`) are stored as columns  
- Ownership is maintained via foreign key to the `users` table  
- Identity is enforced via primary key constraints  

See **EF Core Conventions** for details.

---

## Integration with the Broader System

The domain layer remains independent of:

- **Application layer** — commands/queries orchestrating domain operations  
- **Infrastructure layer** — persistence implementation details  
- **API layer** — HTTP request/response mapping  

Domain exceptions bubble up to the application layer and are handled by the API exception pipeline.

---

## Best Practices

1. **Enforce Invariants in Domain Objects**  
   Never bypass validation in value objects or aggregates.

2. **Use Factory Methods**  
   Always use `Create()` rather than constructors.

3. **Keep Aggregates Focused**  
   The `Dog` aggregate should contain only dog‑related state and behavior.

4. **Test Domain Rules**  
   Unit test invariant enforcement without infrastructure dependencies.

---

## Source References

- `src/CampFitFurDogs/Domain/Dogs/Dog.cs` — Dog aggregate root  
- `src/CampFitFurDogs/Domain/Dogs/DogName.cs` — Dog name value object  
- `src/CampFitFurDogs/Domain/Dogs/Breed.cs` — Breed value object  
- `src/CampFitFurDogs/Domain/Dogs/Sex.cs` — Sex enumeration  
- `src/CampFitFurDogs/Domain/Dogs/DogId.cs` — Strongly typed dog identifier  
- `src/Frank/Core/Domain/*` — Base classes (`AggregateRoot`, `ValueObject`, `AggregateId`)  

