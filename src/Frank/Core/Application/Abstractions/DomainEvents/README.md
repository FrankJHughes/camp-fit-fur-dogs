# Domain Events

The **DomainEvents** folder contains the abstractions that define how domain events are represented, handled, and dispatched within a domain‑driven design (DDD) architecture. Domain events capture meaningful occurrences inside the domain model and allow other parts of the system to react to those occurrences in a decoupled, explicit, and testable way.

This folder provides the foundational interfaces for domain events, event handlers, and the event dispatcher.

---

## Purpose

Domain events model *significant occurrences* within the domain. They are:

- **descriptive** — they express something that *has happened*  
- **immutable** — events represent facts, not commands  
- **decoupled** — publishers do not know which handlers will react  
- **explicit** — each event represents a single, meaningful domain change  

Domain events enable:

- side‑effects without tight coupling  
- reactive workflows  
- integration between aggregates  
- eventual consistency patterns  
- clear modeling of domain behavior  

---

## Components

### IDomainEvent
Represents a domain event — a fact that something meaningful occurred.

```csharp
public interface IDomainEvent { }
```

Domain events are typically raised by aggregates or domain services.

---

### IDomainEventHandler<TEvent>
Handles a specific type of domain event.

```csharp
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
```

Handlers are automatically registered via the `Registration` attribute with:

- scoped lifetime  
- concrete type registration  

Multiple handlers may exist for the same event type.

---

### IDomainEventDispatcher
Coordinates event dispatching by invoking all handlers for a given event.

```csharp
public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
```

The dispatcher ensures:

- all handlers for an event type are invoked  
- handlers run asynchronously  
- cancellation is respected  
- publishers remain decoupled from subscribers  

---

## Design Principles

- **Explicit modeling**  
  Domain events represent meaningful domain facts.

- **Decoupling**  
  Publishers do not know which handlers will react.

- **Multiple handlers**  
  Any number of handlers may respond to the same event.

- **Automatic registration**  
  Handlers are discovered and registered via the `Registration` subsystem.

- **Testability**  
  Events, handlers, and dispatchers are easy to mock and validate.

---

## How Domain Events Fit Into the Application

Domain events integrate naturally with:

- aggregates raising events during state changes  
- application services dispatching events after transactions  
- read‑model updates  
- integration workflows  
- eventual consistency patterns  
- vertical slice architecture  

They form the backbone of reactive domain behavior, enabling the system to respond to domain changes in a clean, modular, and maintainable way.

---
