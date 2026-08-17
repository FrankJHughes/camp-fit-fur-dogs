# Frank.Core.Domain — Aggregates

Aggregates are the core building blocks of the Frank domain model. They define consistency boundaries, enforce invariants, and raise domain events when meaningful state changes occur. Every vertical slice ultimately operates on aggregates, making them the heart of the domain layer.

This document maps the aggregate subsystem under:

```
docs/02-frank-core/domain
```

back to its implementation in:

```
src/Frank/Core/Domain
```

---

## Purpose

Aggregates exist to:

- enforce business rules and invariants  
- encapsulate all state transitions  
- prevent invalid domain states  
- raise domain events when important changes occur  
- define the consistency boundary for a domain concept  

Aggregates ensure that domain logic remains pure, expressive, and protected from external concerns.

---

## Base Class

All aggregates inherit from `AggregateRoot<TId>`, which provides:

- strongly typed identity  
- domain event tracking  
- invariant enforcement helpers  
- protected mutation methods  

Example:

```csharp
public sealed class Dog : AggregateRoot<DogId>
{
    public UserId OwnerId { get; private set; }
    public DogName Name { get; private set; }
    // ... properties
    
    private Dog() { } // For ORM materialization
    
    public static Dog Create(UserId ownerId, DogName name, ...)
    {
        var dog = new Dog(DogId.New(), ownerId, name, ...);
        return dog;
    }
}
```

The private constructor ensures EF Core can materialize the aggregate without invoking domain logic.

---

## Responsibilities of an Aggregate

An aggregate:

- **owns all business rule enforcement**  
- **encapsulates all state mutations** behind well‑defined methods  
- **prevents invalid state transitions**  
- **raises domain events** when meaningful changes occur  
- **defines the consistency boundary** for its domain concept  

Aggregates are the only place where domain invariants may be broken or restored.

---

## Strongly Typed Identity

Each aggregate has its own identity type, preventing accidental cross‑assignment:

```csharp
public sealed class DogId : AggregateId
{
    public DogId(Guid value) : base(value) { }
    
    public static DogId New() => new(Guid.NewGuid());
}
```

This ensures:

- `DogId` cannot be used where `UserId` is expected  
- identity semantics remain explicit  
- aggregate boundaries remain clear  

---

## Domain Events

Aggregates raise domain events when important state changes occur:

```csharp
protected void RaiseDomainEvent(IDomainEvent domainEvent)
{
    // Internal tracking; dispatched by application layer
}
```

The application layer:

1. collects events from the aggregate  
2. dispatches them to event handlers  
3. ensures they run inside the same transactional boundary  

This keeps aggregates pure while enabling reactive behavior.

---

## Persistence

Aggregates are materialized via EF Core:

- EF calls the private parameterless constructor  
- properties are populated through backing fields  
- **no domain logic is executed during materialization**  
- invariants are assumed to be satisfied by persisted data  

This ensures persistence does not interfere with domain behavior.

---

## How Aggregates Connect to the Broader Platform

Aggregates collaborate with:

- **Frank.Core.Application**  
  Handlers invoke aggregate methods and dispatch domain events.

- **Frank.Core.EntityFrameworkCore**  
  Aggregates are persisted and materialized through the Unit of Work.

- **Frank.Core.Infrastructure**  
  Observation context logs aggregate changes and event propagation.

- **Frank.Identity**  
  Identity flows into aggregate decisions via immutable context.

Aggregates are the center of the vertical slice.

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Endpoint
    ↓
Application Handler
    ↓
Aggregate Method (invariants enforced)
    ↓
Domain Events Raised
    ↓
Unit of Work Commit
    ↓
HTTP Response
```

Aggregates define the rules; everything else orchestrates around them.

---

## Notes

Keep this document grounded in the actual Frank.Core.Domain aggregate implementation.  
Whenever aggregate modeling, identity patterns, or domain event flows evolve, update this section to reflect the current platform architecture.

