# DomainEvents

The **DomainEvents** folder contains the application‑layer infrastructure for
dispatching domain events and registering domain event handlers.  
Unlike the abstractions layer, this folder provides **concrete behavior**:
resolving handlers, invoking them, and wiring them into dependency injection
through assembly‑based discovery.

Domain events represent meaningful state changes inside the domain model.
This subsystem ensures those events are delivered to all interested handlers in
a consistent, predictable, slice‑aligned pipeline.

---

## Components

### DomainEventDispatcher

The `DomainEventDispatcher` is responsible for orchestrating domain event
delivery:

1. **Resolve all handlers**  
   All `IDomainEventHandler<TDomainEvent>` instances registered in DI are
   retrieved and materialized.

2. **No‑op if no handlers exist**  
   If no handlers are registered for the event type, dispatch completes
   immediately.

3. **Invoke handlers sequentially**  
   Each handler’s `HandleAsync` method is invoked with the domain event.

Domain events are *fan‑out* operations — every handler receives the event.

---

### ServiceCollectionExtensions

Provides DI registration helpers for domain event infrastructure:

- `AddFrankCoreApplicationDomainEventDispatcher()`  
  Registers the `DomainEventDispatcher`.

- `AddFrankCoreApplicationDomainEvents()`  
  Performs assembly scanning and registers domain event handlers based on:
  - interfaces decorated with `[Registration]`
  - interfaces matching `IDomainEventHandler<>`
  - implementations that satisfy those interfaces

This enables automatic handler discovery across vertical slices.

---

## Design Principles

- **Fan‑out delivery**  
  All handlers for an event type are invoked.

- **Attribute-driven discovery**  
  Only handler interfaces marked with `[Registration]` are included.

- **Loose coupling**  
  Handlers are resolved via DI, not manually instantiated.

- **Vertical-slice alignment**  
  Domain events and handlers belong to slices; this folder provides the
  execution mechanics.

- **Read-only handlers**  
  Domain event handlers should not mutate aggregate state; they react to
  completed domain changes.

---

## How This Folder Fits Into the Application

This folder provides the *runtime mechanics* for domain event propagation.  
It is used by:

- aggregates raising domain events  
- application services emitting domain events  
- outbox pipelines  
- email, notification, and integration event workflows  

Slices define the domain events and handlers; this folder ensures they are
delivered consistently and predictably.

---

## Typical Usage

```csharp
await _domainEventDispatcher.DispatchAsync(new CustomerCreatedEvent(customer), ct);
```

All registered handlers for `CustomerCreatedEvent` will execute.

---

## Notes

- Handlers must be registered via discovery or manual DI.
- Dispatching is sequential; if parallelism is needed, handlers must manage it.
- This folder contains **only** dispatcher + registration logic — not domain
  event definitions or handler implementations.

---
