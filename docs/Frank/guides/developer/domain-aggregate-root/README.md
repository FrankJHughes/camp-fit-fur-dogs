# Domain Aggregate Root — Developer Guide

This guide documents the **current state** of the Domain Aggregate Root capability in Frank and the **intended future state** as the platform evolves.

The Domain Aggregate Root capability defines the **core domain modeling primitives** used throughout Frank‑based systems.  
These types are persistence‑agnostic and represent the heart of the domain layer.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides the following domain primitives:

```csharp
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : AggregateId
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

```csharp
public abstract class Entity<TId>
    where TId : ValueObject
{
    public TId Id { get; protected set; } = default!;
}
```

```csharp
public abstract class AggregateId : ValueObject
{
    public Guid Value { get; }
}
```

```csharp
public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();
}
```

### What exists today

- **AggregateRoot** with domain event lifecycle support  
- **Entity** with identity semantics  
- **ValueObject** with structural equality  
- **AggregateId** as a strongly‑typed identifier  
- Domain event collection and raising  
- Domain event clearing  
- Equality and identity rules  

### What does *not* exist today

- No automatic domain event dispatching  
- No invariant enforcement helpers  
- No aggregate factories  
- No domain services  
- No consistency boundary helpers  
- No snapshotting or versioning  
- No built‑in validation  
- No lifecycle hooks (OnCreate, OnUpdate, etc.)  

### Developer implications today

- Aggregates must manually enforce invariants  
- Domain events must be raised manually  
- Domain events must be dispatched externally (e.g., by the application layer)  
- Identity must be created manually  
- Value objects must define equality components explicitly  

The current capability is intentionally minimal and pure.

---

# 2. Intended Future State (After Capability Expansion)

As the platform evolves, the Domain Aggregate Root capability will expand to support richer domain modeling.

### 2.1 Domain event orchestration

Future enhancements may include:

- Automatic dispatching of domain events after commit  
- Integration with Unit of Work  
- Integration with Outbox pattern  
- Aggregate‑level event clearing policies  

### 2.2 Aggregate lifecycle helpers

Potential additions:

- `OnCreated` / `OnUpdated` / `OnDeleted` hooks  
- Invariant enforcement helpers  
- Aggregate factories  
- Aggregate rehydration helpers  

### 2.3 Identity and versioning

Future support may include:

- Aggregate versioning  
- Concurrency tokens  
- Snapshotting  
- Event‑sourced identity patterns  

### 2.4 Value object enhancements

Potential improvements:

- Automatic EF Core conversion  
- Built‑in validation  
- Built‑in parsing  
- Built‑in serialization helpers  

### 2.5 Domain services and policies

Future capabilities may include:

- Domain service abstractions  
- Policy enforcement helpers  
- Consistency boundary helpers  

---

# 3. Developer Responsibilities (Current vs Future)

## Current Responsibilities

Developers must:

- Define aggregate roots by inheriting from `AggregateRoot<TId>`  
- Define strongly‑typed IDs by inheriting from `AggregateId`  
- Define value objects by inheriting from `ValueObject`  
- Implement equality components for value objects  
- Raise domain events manually  
- Clear domain events manually  
- Enforce invariants inside aggregate methods  
- Ensure aggregates remain consistent after each operation  

## Future Responsibilities

Once the capability expands, developers will:

- Use built‑in lifecycle hooks  
- Rely on automatic domain event dispatching  
- Use aggregate factories for creation  
- Use built‑in invariant enforcement helpers  
- Use versioning and snapshotting features  
- Use domain services for cross‑aggregate logic  

---

# 4. Architecture and Invariants

### 4.1 AggregateRoot Invariants

- Aggregates own their invariants  
- Aggregates raise domain events when state changes  
- Domain events are stored temporarily, not persisted  
- Domain events must be cleared after dispatch  
- Aggregates must always remain in a valid state  

### 4.2 Entity Invariants

- Entities are compared by identity  
- Identity is immutable after creation  
- Entities cannot exist without an ID  

### 4.3 ValueObject Invariants

- Value objects are compared structurally  
- Value objects must be immutable  
- Equality is defined by `GetEqualityComponents`  

### 4.4 AggregateId Invariants

- IDs are strongly typed  
- IDs wrap a `Guid`  
- IDs participate in value object equality  

---

# 5. Example Usage (Developer Perspective)

### 5.1 Define an Aggregate ID

```csharp
public sealed class CustomerId : AggregateId
{
    public CustomerId(Guid value) : base(value) { }
}
```

### 5.2 Define a Value Object

```csharp
public sealed class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

### 5.3 Define an Aggregate Root

```csharp
public sealed class Customer : AggregateRoot<CustomerId>
{
    public Email Email { get; private set; }

    public Customer(CustomerId id, Email email)
        : base(id)
    {
        Email = email;
        RaiseDomainEvent(new CustomerCreated(id.Value));
    }

    public void ChangeEmail(Email newEmail)
    {
        Email = newEmail;
        RaiseDomainEvent(new CustomerEmailChanged(Id.Value, newEmail.Value));
    }
}
```

---

# 6. Summary

**Current State:**  
Frank provides a minimal but complete set of domain modeling primitives:

- AggregateRoot  
- Entity  
- ValueObject  
- AggregateId  
- Domain event lifecycle  

Developers use these to model aggregates, enforce invariants, and raise domain events.

**Future State:**  
The capability will expand to support:

- Domain event orchestration  
- Lifecycle hooks  
- Aggregate factories  
- Versioning and snapshotting  
- Value object enhancements  
- Domain services and policies  

As a developer:

- Today, you manually model aggregates and domain events  
- In the future, Frank will provide richer domain modeling support  

