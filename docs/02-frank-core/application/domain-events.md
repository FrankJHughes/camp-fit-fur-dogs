# Frank.Core.Application — Domain Events

The domain events subsystem in `Frank.Core.Application` provides the platform‑level primitives for publishing, handling, and propagating domain events across vertical slices. It ensures that domain‑level changes can trigger additional application behaviors without creating tight coupling between aggregates, handlers, or infrastructure.

This document describes the responsibilities of the domain event subsystem and maps the documentation folder:

```
docs/02-frank-core/application
```

back to the implementation under:

```
src/Frank/Core
```

---

## Purpose

The domain events subsystem exists to:

- allow domain aggregates to emit events when meaningful state changes occur  
- propagate those events through the application layer in a consistent, decoupled way  
- support cross‑slice reactions without violating aggregate boundaries  
- provide a unified event contract for all products built on Frank  
- enable future infrastructure (outbox, async processing, integration events)  

Domain events make the system reactive while keeping domain logic pure and isolated.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Application/DomainEvents`

- **Documentation folder:**  
  `docs/02-frank-core/application`

This documentation must remain aligned with the actual event dispatcher, event base types, and event propagation logic.

---

## Responsibilities of the Domain Events Subsystem

### [Event Definition](ca://s?q=Frank_Core_Application_DomainEvent_Definition)
Frank provides base types such as:

- `IDomainEvent`  
- `DomainEvent`  
- strongly‑typed event identifiers  

Aggregates raise events by adding them to an internal event collection.

### [Event Collection on Aggregates](ca://s?q=Frank_Core_Domain_AggregateRoot)
Aggregates store emitted events until the application layer processes them:

```csharp
public abstract class AggregateRoot<TId>
{
    private readonly List<IDomainEvent> _events = new();

    protected void RaiseEvent(IDomainEvent @event) => _events.Add(@event);

    public IReadOnlyList<IDomainEvent> DequeueEvents() => ...
}
```

This ensures events are emitted only when domain invariants are satisfied.

### [Event Dispatching](ca://s?q=Frank_Core_Application_DomainEvent_Dispatching)
After a command handler modifies an aggregate, the application layer:

1. dequeues domain events  
2. dispatches them to registered event handlers  
3. ensures handlers run within the same transactional boundary  

This keeps event propagation consistent and predictable.

### [Event Handlers](ca://s?q=Frank_Core_Application_DomainEventHandler)
Handlers implement:

```csharp
public interface IDomainEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken ct);
}
```

Handlers may:

- update other aggregates  
- trigger additional commands  
- write to infrastructure  
- publish integration events (future extension)  

---

## How Domain Events Connect to the Broader Platform

Domain events collaborate with multiple Frank subsystems:

- **Frank.Core.Domain**  
  Aggregates raise events when invariants change.

- **Frank.Core.Application**  
  Dispatchers process events after command execution.

- **Frank.Core.EntityFrameworkCore**  
  Events are dispatched before committing the Unit of Work.

- **Frank.Core.Infrastructure**  
  Observation context logs event propagation.

This ensures domain events participate fully in the vertical slice lifecycle.

---

## Runtime Collaboration Points

Domain events interact with the runtime in several ways:

- **During command execution** — events are raised inside aggregates  
- **During application dispatch** — events are dequeued and processed  
- **During persistence** — events run before the Unit of Work commits  
- **During logging** — correlation IDs track event propagation  
- **During error handling** — event handler failures map to `Result<T>`  

This keeps event processing deterministic and observable.

---

## Composition Flow (API → Application → Domain → Persistence)

Domain events participate in the vertical slice flow:

```
API Endpoint
    ↓
ICommandDispatcher
    ↓
Application Handler
    ↓
Domain Aggregate (events raised)
    ↓
Domain Event Dispatching
    ↓
Infrastructure Persistence (Unit of Work)
    ↓
HTTP Response
```

Events allow domain changes to trigger additional behavior without coupling slices together.

---

## What Belongs in This Document

This page should describe:

- domain event responsibilities  
- how aggregates raise events  
- how events are dispatched and handled  
- how events collaborate with application and persistence layers  
- how events fit into the vertical slice lifecycle  

It should **not** include:

- product‑specific events  
- infrastructure‑specific event storage  
- async/integration event patterns (future extensions)  

Those belong in product or infrastructure documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Application domain event implementation.  
Whenever event dispatching, handler contracts, or aggregate event flows evolve, update this section to reflect the current platform architecture.

