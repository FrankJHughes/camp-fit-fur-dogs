# Frank.Core.Domain — Domain Layer

The **Domain** folder contains the pure, business‑logic core of the Frank architecture.  
It defines the building blocks used to model aggregates, value objects, domain events, and domain‑level exceptions across all verticals.

This layer is intentionally free of infrastructure concerns, serialization details, HTTP concepts, or persistence logic.  
It represents the **canonical truth** of the system.

---

## Purpose

The Domain layer provides:

- **Aggregate roots** — authoritative entities that enforce invariants and control state transitions.
- **Value objects** — immutable, validated representations of domain concepts.
- **Domain exceptions** — explicit failures when invariants are violated.
- **Domain events** — pure signals emitted by aggregates to describe meaningful business occurrences.
- **Base abstractions** — shared primitives such as `AggregateId`, `AggregateRoot<T>`, and `ValueObject`.

Everything in this folder exists to model *business meaning*, not technical implementation.

---

## Architectural Principles

The Domain layer follows strict rules:

### **1. Purity**
No infrastructure, no HTTP, no EF Core, no DI, no logging.  
Only business logic and invariants.

### **2. Strong Typing**
Primitive strings and GUIDs are wrapped in value objects.  
This prevents invalid data from entering the model.

### **3. Invariant Enforcement**
All invariants are checked at construction time.  
Invalid domain data cannot exist.

### **4. Immutability**
Value objects are immutable.  
Aggregates mutate only through controlled operations.

### **5. Explicit Failure**
Domain exceptions clearly describe *why* a rule was violated.

### **6. Event‑Driven**
Aggregates raise domain events to describe meaningful changes.  
Events are pure and contain no behavior.

---

## Folder Structure

### **Aggregates**
Classes inheriting from `AggregateRoot<TId>` that represent authoritative domain entities.

Examples:
- `User`
- `Dog`
- `Reservation`
- `Customer`

### **Value Objects**
Immutable, validated domain concepts.

Examples:
- `Email`
- `PhoneNumber`
- `FirstName`
- `LastName`
- `ExternalId`
- `PasswordHash`
- `UserId`

### **Domain Events**
Pure business signals raised by aggregates.

Examples:
- `CustomerCreated`
- `EmailVerified`
- `PasswordResetRequested`

### **Exceptions**
Domain‑level validation errors.

Examples:
- `InvalidEmailException`
- `InvalidPhoneNumberException`
- `InvalidUserIdException`
- `MissingIdentitySourceException`

### **Base Types**
Shared primitives used across all aggregates and value objects:

- `AggregateId`
- `AggregateRoot<TId>`
- `ValueObject`
- `DomainException`

---

## Responsibilities of the Domain Layer

The Domain layer is responsible for:

- Modeling business concepts
- Enforcing invariants
- Normalizing and validating data
- Raising domain events
- Providing strongly‑typed identities
- Remaining independent of all infrastructure

It is **not** responsible for:

- Persistence
- Serialization
- HTTP endpoints
- Authentication middleware
- Logging
- Dependency injection

Those concerns belong to Application, Infrastructure, or API layers.

---

## How Other Layers Use the Domain

- **Application Layer**  
  Coordinates aggregates, commands, queries, and domain events.

- **Infrastructure Layer**  
  Persists aggregates, publishes events, and integrates external systems.

- **API Layer**  
  Exposes application behaviors via HTTP endpoints.

The Domain layer remains stable even as other layers evolve.

---

## Summary

The **Domain** folder is the heart of the Frank architecture.  
It defines:

- Aggregates  
- Value objects  
- Domain events  
- Domain exceptions  
- Base primitives  

All modeled with purity, strong typing, and strict invariant enforcement.

This layer is the foundation upon which the entire system is built.

