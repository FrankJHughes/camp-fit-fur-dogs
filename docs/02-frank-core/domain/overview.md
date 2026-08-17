# Frank.Core.Domain — Domain Layer

The Frank.Core domain layer provides the foundational building blocks used by all product and subsystem domains. It defines the core abstractions for aggregates, entities, value objects, domain events, and domain exceptions — forming the pure, invariant‑enforcing center of every vertical slice.

This document maps the domain layer under:

```
docs/02-frank-core/domain
```

back to its implementation in:

```
src/Frank/Core/Domain
```

---

## Key Abstractions

The domain layer provides the following primitives:

- **[AggregateRoot<TId>](ca://s?q=Frank_Core_Domain_AggregateRoot)** — base class for aggregates with strongly typed identity  
- **[Entity<TId>](ca://s?q=Frank_Core_Domain_Entity)** — base class for entities with identity but without consistency boundaries  
- **[ValueObject](ca://s?q=Frank_Core_Domain_ValueObject)** — base class for immutable, equality‑by‑value objects  
- **[AggregateId](ca://s?q=Frank_Core_Domain_AggregateId)** — base class for strongly typed identifiers  
- **[IDomainEvent](ca://s?q=Frank_Core_Domain_IDomainEvent)** — marker interface for domain events  
- **[DomainException](ca://s?q=Frank_Core_Domain_DomainException)** — base class for domain‑specific invariant violations  

These abstractions ensure domain logic remains pure, expressive, and isolated from application and infrastructure concerns.

---

## Strongly Typed Identity

Each aggregate defines its own identity type:

```csharp
public sealed class DogId : AggregateId
{
    public DogId(Guid value) : base(value) { }
    public static DogId New() => new(Guid.NewGuid());
}
```

Strongly typed IDs:

- prevent accidental cross‑assignment (e.g., `DogId` → `UserId`)  
- make code self‑documenting  
- reinforce aggregate boundaries  
- improve correctness and readability  

---

## Value Objects

Value objects are immutable and compared by value, not identity. They encapsulate domain concepts that have no lifecycle of their own.

Example:

```csharp
public sealed class DogName : ValueObject
{
    public string Value { get; }
    
    private DogName(string value)
    {
        Value = value;
    }
    
    public static Result<DogName> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<DogName>("Name is required");
        
        return Result.Success(new DogName(input));
    }
}
```

Value objects:

- enforce invariants at creation  
- guarantee immutability  
- provide semantic clarity  
- reduce primitive obsession  

---

## Domain Events

Domain events signal meaningful occurrences inside aggregates:

```csharp
public interface IDomainEvent
{
    // Marker for domain events
}

public sealed class DogRegisteredEvent : IDomainEvent
{
    public DogId DogId { get; }
    public UserId OwnerId { get; }
    // ...
}
```

Domain events:

- are raised by aggregates  
- are collected by the application layer  
- are dispatched to event handlers  
- run inside the same transactional boundary  

They allow the domain to react to changes without coupling aggregates together.

---

## How the Domain Layer Connects to the Broader Platform

The domain layer collaborates with:

- **Frank.Core.Application** — handlers invoke domain logic and dispatch events  
- **Frank.Core.EntityFrameworkCore** — aggregates are persisted and materialized  
- **Frank.Core.Infrastructure** — observation context logs domain behavior  
- **Frank.Identity** — identity flows into domain decisions via immutable context  

The domain layer is the center of the vertical slice architecture.

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Endpoint
    ↓
Application Handler
    ↓
Domain Aggregate / Value Objects
    ↓
Domain Events Raised
    ↓
Unit of Work Commit
    ↓
HTTP Response
```

The domain layer defines the rules; the rest of the platform orchestrates around them.

---

## Notes

Keep this document grounded in the actual Frank.Core.Domain implementation.  
Whenever aggregate modeling, value object patterns, or domain event flows evolve, update this section to reflect the current platform architecture.

