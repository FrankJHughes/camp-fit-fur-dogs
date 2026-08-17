# Dogs Aggregate

The `Dog` aggregate is the core domain model for representing dogs within Camp Fit Fur Dogs. It encapsulates all invariants, lifecycle rules, and mutation behavior. As an aggregate root, it ensures that all dog‑related business rules are enforced consistently and atomically.

## Structure

```csharp
public sealed class Dog : AggregateRoot<DogId>
{
    public UserId OwnerId { get; private set; }
    public DogName Name { get; private set; }
    public Breed Breed { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Sex Sex { get; private set; }

    // Private constructor for ORM materialization
    private Dog() { }

    // Private constructor for creation flow
    private Dog(DogId id, UserId ownerId, DogName name, Breed breed, DateOnly dob, Sex sex)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        Breed = breed;
        DateOfBirth = dob;
        Sex = sex;
    }

    // Factory method
    public static Dog Create(UserId ownerId, DogName name, Breed breed, DateOnly dob, Sex sex)
        => new(DogId.New(), ownerId, name, breed, dob, sex);

    // Mutation method
    public void Update(DogName newName, Breed newBreed, DateOnly newDob)
    {
        Name = newName;
        Breed = newBreed;
        DateOfBirth = newDob;
    }
}
```

The aggregate exposes only controlled creation and mutation methods, ensuring invariants are always upheld.

---

## Domain Invariants

The `Dog` aggregate enforces several business rules:

- **A dog always has an owner**  
  Ownership is immutable after creation.

- **Name, breed, and birth date are required**  
  These are validated through domain value objects.

- **Name and Breed are value objects**  
  They encapsulate formatting, validation, and normalization.

- **All business rules are enforced internally**  
  No external component may bypass invariants or mutate state directly.

These invariants ensure the domain model remains consistent regardless of how the application layer interacts with it.

---

## Value Objects

The aggregate depends on several domain value objects:

- **`DogId`** — strongly typed identifier  
- **`DogName`** — encapsulates name validation and formatting  
- **`Breed`** — encapsulates breed validation and representation  
- **`Sex`** — enum representing biological sex  

Value objects ensure that domain rules are enforced at the boundary of the aggregate.

---

## Creation and Mutation

### Creation

All creation flows through the `Create` factory method:

```csharp
var dog = Dog.Create(
    ownerId,
    DogName.Create("Buddy"),
    Breed.Create("Golden Retriever"),
    new DateOnly(2020, 1, 15),
    Sex.Male);
```

This ensures:

- invariants are validated  
- value objects are constructed correctly  
- the aggregate is always created in a valid state  

### Mutation

Mutations occur through well‑defined methods such as `Update`:

```csharp
dog.Update(
    DogName.Create("Buddy"),
    Breed.Create("Golden Retriever"),
    new DateOnly(2020, 1, 15));
```

Direct property assignment is never allowed. All state changes must pass through domain logic.

---

## Summary

The `Dog` aggregate:

- encapsulates all dog‑related business rules  
- ensures invariants through value objects and controlled mutation  
- provides a clean creation flow via factory methods  
- acts as the authoritative source of truth for dog state  

It is the foundation of the Dogs vertical slice and the core of the Camp Fit Fur Dogs domain model.

