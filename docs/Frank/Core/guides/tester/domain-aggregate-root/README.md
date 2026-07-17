# Domain Aggregate Root — Tester Guide

This guide documents the **current state** of the Domain Aggregate Root capability in Frank and the **intended future state** as the platform evolves.

The Domain Aggregate Root capability defines the core domain modeling primitives used throughout Frank‑based systems.  
These types are persistence‑agnostic and represent the heart of the domain layer.  
Testers validate the *behavior* of aggregates, not their persistence.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides the following domain primitives:

- `AggregateRoot<TId>` — supports domain event lifecycle  
- `Entity<TId>` — identity‑based equality  
- `ValueObject` — structural equality  
- `AggregateId` — strongly‑typed identifier  
- Domain event raising and clearing  

### What exists today (testable behavior)

- Domain events can be raised  
- Domain events accumulate in memory  
- Domain events can be cleared  
- Value objects compare structurally  
- Entities compare by identity  
- Aggregate IDs compare by value  
- Aggregates enforce invariants manually (developer‑defined)

### What does *not* exist today (not testable)

- No automatic domain event dispatching  
- No lifecycle hooks  
- No invariant enforcement helpers  
- No versioning or snapshotting  
- No domain services  
- No consistency boundary helpers  

### What testers validate today

- Domain event lifecycle behavior  
- Equality semantics (entity vs value object)  
- Identity semantics  
- Aggregate invariants (developer‑defined)  
- Aggregate behavior under valid and invalid operations  

The current capability is intentionally minimal and pure.

---

# 2. Intended Future State (After Capability Expansion)

As the platform evolves, testers will validate richer domain behavior.

### 2.1 Domain event orchestration

- Automatic dispatching after commit  
- Integration with Unit of Work  
- Integration with Outbox pattern  
- Event clearing policies  

### 2.2 Aggregate lifecycle hooks

- `OnCreated`  
- `OnUpdated`  
- `OnDeleted`  
- Invariant enforcement helpers  

### 2.3 Identity and versioning

- Aggregate versioning  
- Concurrency tokens  
- Snapshotting  
- Event‑sourced identity patterns  

### 2.4 Value object enhancements

- Automatic EF Core conversion  
- Built‑in validation  
- Built‑in parsing  
- Built‑in serialization helpers  

### 2.5 Domain services and policies

- Domain service abstractions  
- Policy enforcement helpers  
- Consistency boundary helpers  

These features will significantly expand the testing surface.

---

# 3. Test Responsibilities (Current vs Future)

## Current Responsibilities

Testers must validate:

### **Domain Event Lifecycle**
- Domain events are raised when expected  
- Domain events accumulate in order  
- Domain events are not mutated  
- Domain events can be cleared  
- Clearing removes all events  

### **Aggregate Behavior**
- Aggregate invariants hold after each operation  
- Invalid operations throw exceptions  
- State transitions are correct  
- Aggregate methods enforce business rules  

### **Identity Semantics**
- Entities compare equal when IDs match  
- Entities compare unequal when IDs differ  
- IDs are immutable  

### **Value Object Semantics**
- Value objects compare structurally  
- Value objects are immutable  
- Equality components are correct  

### **No Additional Behavior**
- No automatic dispatching  
- No persistence behavior  
- No lifecycle hooks  
- No versioning  

## Future Responsibilities

Once the capability expands, testers will validate:

- Automatic domain event dispatching  
- Outbox integration  
- Aggregate lifecycle hooks  
- Versioning and concurrency  
- Snapshotting  
- Domain service behavior  
- Policy enforcement  
- Value object validation and conversion  

---

# 4. Required Test Types (Current State)

## 4.1 Domain Event Tests

### Validate raising events

```csharp
aggregate.RaiseSomething();
Assert.Contains(typeof(SomethingHappened), aggregate.DomainEvents);
```

### Validate event ordering

- Events appear in the order raised  
- No reordering occurs  

### Validate clearing events

```csharp
aggregate.ClearDomainEvents();
Assert.Empty(aggregate.DomainEvents);
```

---

## 4.2 Aggregate Invariant Tests

Testers validate:

- Aggregate remains valid after each operation  
- Invalid operations throw exceptions  
- State transitions follow business rules  

### Example

```csharp
Assert.Throws<InvalidOperationException>(() => aggregate.DoSomethingInvalid());
```

---

## 4.3 Entity Identity Tests

Validate:

- Entities compare equal when IDs match  
- Entities compare unequal when IDs differ  
- Hash codes match identity semantics  

---

## 4.4 Value Object Equality Tests

Validate:

- Structural equality  
- Immutability  
- Correct equality components  

### Example

```csharp
var a = new Email("test@example.com");
var b = new Email("test@example.com");

Assert.Equal(a, b);
```

---

## 4.5 Aggregate ID Tests

Validate:

- IDs wrap a Guid  
- IDs compare by value  
- IDs are immutable  

---

# 5. Anti‑Patterns (Tests Must Reject)

- Tests that assume domain events are dispatched automatically  
- Tests that assume persistence behavior  
- Tests that assume lifecycle hooks exist  
- Tests that assume versioning or concurrency tokens  
- Tests that rely on EF Core behavior  
- Tests that assume aggregates are rehydrated automatically  
- Tests that assume value objects are validated automatically  

These features do **not** exist today.

---

# 6. Summary

**Current State:**  
Testers validate:

- Domain event lifecycle  
- Aggregate invariants  
- Entity identity  
- Value object equality  
- Aggregate behavior  

**Future State:**  
Testers will validate:

- Domain event orchestration  
- Lifecycle hooks  
- Versioning  
- Snapshotting  
- Outbox integration  
- Domain services and policies  

This Tester Guide prepares testers for both the current minimal domain model and the richer future capability.

