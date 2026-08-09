# Frank.Core.Domain

## Purpose
`Frank.Core.Domain` contains the foundational building blocks of the Frank platform’s domain model. These abstractions define how entities, value objects, aggregate roots, domain events, and domain exceptions behave. They provide a consistent, opinionated, dependency‑free domain layer aligned with Domain‑Driven Design (DDD).

## Architectural Role
This folder represents the **root of all domain modeling** in the Frank ecosystem. It is intentionally minimal, stable, and free of infrastructure concerns. All higher‑level domain models (CampFitFurDogs, Frank.Identity, etc.) build on these primitives.

`Frank.Core.Domain` establishes:

- Strongly typed identity semantics (`[AggregateId](ca://s?q=Explain_AggregateId)`)
- Entity semantics (`[Entity<TId>](ca://s?q=Explain_Entity_TId)`)
- Aggregate semantics (`[AggregateRoot<TId>](ca://s?q=Explain_AggregateRoot_TId)`)
- Value object semantics (`[ValueObject](ca://s?q=Explain_ValueObject)`)
- Domain event semantics (`[IDomainEvent](ca://s?q=Explain_IDomainEvent)`)
- Domain exception semantics (`[DomainException](ca://s?q=Explain_DomainException)`)

These types form the **core domain vocabulary** for all Frank‑based applications.

---

## Domain Philosophy

The `Frank.Core.Domain` layer is intentionally small, pure, and explicit. Its purpose is to define the **ubiquitous language** and **behavioral rules** that all Frank‑based systems rely on. The design follows key Domain‑Driven Design (DDD) principles:

### Explicitness Over Convenience
Domain types must be intention‑revealing. Strongly typed identifiers, immutable value objects, and explicit aggregate roots make domain behavior clear and predictable.

### Purity and Isolation
The domain layer has **no dependencies** on infrastructure, application services, or external libraries. This ensures:
- Deterministic behavior  
- Testability  
- Stability across refactors  
- Independence from frameworks  

### Strongly Typed Identity
Identifiers are never raw GUIDs. They are wrapped in `[AggregateId](ca://s?q=Explain_AggregateId)` types to prevent accidental misuse and to make domain intent explicit.

### Structural Equality for Value Objects
Value objects represent concepts, not entities. They use structural equality to ensure correctness and clarity.

### Behavioral Consistency Through Aggregates
Aggregates enforce invariants and raise `[domain events](ca://s?q=Explain_IDomainEvent)` to communicate meaningful changes. They are the consistency boundaries of the domain.

### Named Domain Failures
Domain exceptions express domain‑specific error conditions using meaningful names. They are part of the ubiquitous language and help ensure domain correctness.

This philosophy ensures that the domain layer remains expressive, stable, and aligned with the business rules it represents.

---

## Key Abstractions

### AggregateId
A strongly typed identifier for aggregates.  
Backed by a `Guid`, wrapped in a value object to enforce type safety and equality semantics.

### Entity<TId>
Base class for domain entities.  
Provides identity‑based equality and hash code behavior.

### AggregateRoot<TId>
Base class for aggregates.  
Supports domain event collection and lifecycle management via:

- `[RaiseDomainEvent](ca://s?q=Explain_RaiseDomainEvent)`
- `[ClearDomainEvents](ca://s?q=Explain_ClearDomainEvents)`
- `[DomainEvents](ca://s?q=Explain_DomainEvents)` (read‑only list)

### ValueObject
Base class for immutable value objects.  
Implements structural equality.

### IDomainEvent
Marker interface for domain events raised by aggregates.

### DomainException
Base class for domain‑specific failures.

### Derived Exceptions
- `[BadConfigurationException](ca://s?q=Explain_BadConfigurationException)`
- `[BadRequestException](ca://s?q=Explain_BadRequestException)`

These represent domain‑level error conditions.

### AssemblyMarker
A marker type used for assembly scanning and DI registration.

---

## Invariants

- Value objects must be immutable and define structural equality.
- Entities must have a non‑null ID of type `ValueObject`.
- Aggregate roots raise domain events **only** via `RaiseDomainEvent`.
- Domain events must be cleared after dispatch using `ClearDomainEvents`.
- Domain exceptions represent **domain failures**, not infrastructure or application errors.
- Aggregate IDs must be strongly typed — never raw GUIDs.

---

## Conventions

- All domain types live under the `Frank.Core.Domain` namespace.
- Domain exceptions follow the naming pattern `*Exception`.
- Aggregate IDs follow the naming pattern `*Id`.
- Value objects must override equality semantics.
- Domain events should be simple, immutable data carriers.

---

## Dependencies

### This folder depends on:
- **No external libraries**  
  (Domain primitives must remain pure.)

### Folders that depend on this folder:
- `Frank.Core.Application` (domain event dispatching)
- `Frank.Core.EntityFrameworkCore` (aggregate persistence)
- `Frank.Identity.Domain` (identity modeling)
- `CampFitFurDogs.Domain` (product domain modeling)

This folder is the **root dependency** for all domain layers.

---

## How to Extend This Folder

The `Frank.Core.Domain` folder is designed to be extended by downstream domain models. When adding new domain types, follow these patterns:

### Create a New Value Object
Use a value object when representing an immutable concept with structural equality.

Example:
```csharp
public sealed class EmailAddress : ValueObject<string>
{
    public EmailAddress(string value) : base(value)
    {
        // Add validation here
    }
}
```

### Create a New AggregateId
Use a strongly typed ID for every aggregate.

Example:
```csharp
public sealed class DogId : AggregateId
{
    public DogId(Guid value) : base(value) { }
}
```

### Create a New Entity<TId>
Use an entity when modeling something with identity and lifecycle.

Example:
```csharp
public sealed class Dog : Entity<DogId>
{
    // Domain behavior here
}
```

### Create a New AggregateRoot<TId>
Use an aggregate root when enforcing invariants and raising domain events.

Example:
```csharp
public sealed class Owner : AggregateRoot<OwnerId>
{
    public void RegisterDog(Dog dog)
    {
        RaiseDomainEvent(new DogRegistered(dog.Id, clock.UtcNow));
    }
}
```

### Create a New Domain Event
Domain events should be immutable data carriers.

Example:
```csharp
public sealed class DogRegistered : DomainEventBase
{
    public DogId DogId { get; }

    public DogRegistered(DogId dogId, DateTime occurredOn)
        : base(occurredOn)
    {
        DogId = dogId;
    }
}
```

### Create a New DomainException
Use domain exceptions to represent invariant violations.

Example:
```csharp
public sealed class InvalidDogNameException : DomainException
{
    public InvalidDogNameException(string message) : base(message) { }
}
```

### Follow the Invariants
All new types must follow the invariants defined in this folder:
- Value objects are immutable  
- Entities have strongly typed IDs  
- Aggregates raise domain events  
- Domain events are immutable  
- Domain exceptions represent domain failures  

By following these patterns, new domain types remain consistent with the Frank platform’s architecture.

---

## Summary
`Frank.Core.Domain` defines the **core primitives** that all Frank‑based domain models rely on.  
It is intentionally small, stable, and pure — forming the backbone of the Frank platform’s domain architecture.
