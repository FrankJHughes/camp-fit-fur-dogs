# Domain Events Architecture

This guide describes how domain events are modeled, raised, collected, and dispatched in Camp Fit Fur Dogs.  
Domain events enforce a clean separation between **business rules** (Domain) and **side effects** (Application and Infrastructure).

Domain events are a core architectural mechanism that ensures the Domain remains pure, testable, and independent of external concerns.

---

# 1. Purpose

Domain events exist to:

- Represent meaningful business facts  
- Decouple domain logic from side effects  
- Centralize event dispatching in the Application layer  
- Ensure all side effects occur within a controlled transaction boundary  
- Keep the Domain layer free of infrastructure dependencies  
- Provide a deterministic, testable event lifecycle  

Domain events describe **what happened**, not **what to do**.

---

# 2. Domain Responsibilities

The Domain layer is responsible for **raising** domain events, not dispatching them.

## 2.1 Raising Events

Aggregates raise events inside their methods when a business fact occurs.

Characteristics:

- Events are immutable classes  
- Events describe something that has already happened  
- Events contain only the data needed to describe the fact  
- Events contain no behavior  

Example shape:

```csharp
public sealed record DogRegisteredDomainEvent(DogId DogId, CustomerId OwnerId);
```

Events are raised inside aggregate methods, not handlers.

## 2.2 Event Storage on Aggregates

Aggregates maintain an internal list of pending events.

Each aggregate provides:

- A protected method to add events  
- A public method for Application to retrieve pending events  
- A method for Application to clear events after dispatch  

The Domain layer must not:

- Dispatch events  
- Depend on Application or Infrastructure  
- Perform side effects  

Domain events are pure business facts.

---

# 3. Application Responsibilities

The Application layer owns the **entire event dispatching lifecycle**.

## 3.1 IDomainEventDispatcher

```csharp
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<object> domainEvents, CancellationToken cancellationToken = default);
}
```

Responsibilities:

- Accept a batch of domain events  
- Resolve handlers via Frank auto‑registration  
- Invoke handlers in deterministic order  
- Run inside the Unit of Work boundary  

The dispatcher is part of the Application layer.

## 3.2 IDomainEventHandler

```csharp
public interface IDomainEventHandler<in TDomainEvent>
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
```

Event handlers:

- Live in Application  
- Are auto‑registered via `[AutoRegister]`  
- Contain no business rules  
- Implement side effects (email, logging, projections, etc.)  
- May depend on repositories, readers, or domain services  
- Must not raise domain events directly  

Handlers respond to events; they do not create new ones.

## 3.3 Unit of Work Integration

The Unit of Work coordinates:

1. Persistence  
2. Event collection  
3. Event dispatch  

Flow:

1. Handler performs domain operations  
2. Handler calls `CommitAsync()`  
3. Commit persists changes  
4. Commit collects domain events from aggregates  
5. Commit clears events from aggregates  
6. Commit invokes `IDomainEventDispatcher`  

This ensures events are dispatched **only after successful persistence**.

---

# 4. Infrastructure Responsibilities

Infrastructure participates in domain event flow only through:

- EF Core change tracking  
- Transaction boundaries  
- Unit of Work implementation  

Infrastructure may support side effects **triggered by Application handlers**, such as:

- Email delivery  
- Logging  
- External API calls  
- Projections  

Infrastructure must not:

- Raise domain events  
- Dispatch domain events  
- Contain domain logic  
- Depend on Application or API  

Infrastructure is a consumer of side effects, not a producer of domain facts.

---

# 5. Event Lifecycle

The complete lifecycle of a domain event:

```
Aggregate Method
    ↓
Domain Event Raised
    ↓
Aggregate Stores Event
    ↓
Command Handler Calls CommitAsync
    ↓
Unit of Work Persists Changes
    ↓
Unit of Work Collects Events
    ↓
Unit of Work Clears Events
    ↓
DomainEventDispatcher Dispatches Events
    ↓
Event Handlers Execute Side Effects
```

This ensures:

- Domain logic runs first  
- Persistence succeeds  
- Side effects run last  
- No side effects occur if persistence fails  

---

# 6. Purity Rules

Domain events enforce strict purity boundaries:

## Domain Layer
- Defines events  
- Raises events  
- Stores events  
- Contains no side effects  
- Does not dispatch events  
- Does not depend on Application or Infrastructure  

## Application Layer
- Dispatches events  
- Handles events  
- Performs side effects  
- Coordinates persistence  

## API Layer
- Never dispatches domain events  
- Never returns domain events  
- Never serializes domain events  

## Infrastructure Layer
- Never raises domain events  
- Never dispatches domain events  
- Never contains domain logic  

These rules ensure the Domain remains isolated and testable.

---

# 7. Contributor Guidelines

When adding a new domain event:

1. **Define the event** in the Domain layer  
2. **Raise the event** inside aggregate methods  
3. **Add event handlers** in the Application layer  
4. **Mark handlers with `[AutoRegister]`**  
5. **Do not dispatch events manually**  
6. **Do not raise events from handlers**  
7. **Keep handlers free of business logic**  
8. **Keep handlers free of Infrastructure references**  
9. **Test handlers in isolation**  
10. **Test the full slice via API tests**  

If a domain event handler grows beyond ~20–30 lines, logic is leaking into the wrong layer.

---

# Related Documents

- Dispatcher Pipeline Guide  
- Dependency Injection Architecture  
- API Endpoint Purity Guide  
- Vertical Slice Guide  
