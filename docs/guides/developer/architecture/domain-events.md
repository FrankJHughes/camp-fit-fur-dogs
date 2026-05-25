# Domain Events Architecture

This guide describes how domain events are modeled, raised, and dispatched in Camp Fit Fur Dogs. It explains the responsibilities of the Domain, Application, and Infrastructure layers in the domain event pipeline.

---

## 1. Goals

- **Decouple** domain logic from side effects.
- **Centralize** event dispatching in the Application layer.
- **Keep** the Domain layer free of infrastructure concerns.
- **Make** domain events testable and observable.

---

## 2. Domain Responsibilities

### 2.1 Raising Events

Domain entities and aggregates may raise domain events to describe something that has happened.

- Domain events are simple classes (e.g., `DogRegisteredDomainEvent`).
- Events are raised inside domain methods (e.g., `RegisterDog`).
- The Domain layer does **not** know how events are dispatched or handled.

### 2.2 Event Storage on Aggregates

Aggregates typically expose:

- A collection of pending domain events.
- Methods to add events internally.
- A way for the Application layer to retrieve and clear events.

The Domain layer **does not**:

- Dispatch events.
- Depend on Application or Infrastructure.

---

## 3. Application Responsibilities

### 3.1 IDomainEventDispatcher

The Application layer defines:

```csharp
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<object> domainEvents, CancellationToken cancellationToken = default);
}
```

Responsibilities:

- Accept a collection of domain events.
- Dispatch them to the appropriate handlers.
- Run within the Application layer (no ASP.NET, no EF Core).

### 3.2 IDomainEventHandler

Domain event handlers live in Application and implement:

```csharp
public interface IDomainEventHandler<in TDomainEvent>
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
```

Conventions:

- Handlers are application services.
- Handlers are discovered and registered by convention.
- Handlers must not depend on API or UI types.

### 3.3 DomainEventDispatcher

`DomainEventDispatcher` is the concrete implementation of `IDomainEventDispatcher`:

- Resolves all `IDomainEventHandler<T>` for each event type.
- Invokes handlers asynchronously.
- Runs after the domain operation completes (e.g., after command handling).

The dispatcher is registered in Application DI and used by dispatchers or unit-of-work boundaries.

---

## 4. Infrastructure Responsibilities

Infrastructure may:

- Persist domain events (if needed).
- Integrate with external systems in event handlers (e.g., email, messaging).
- Provide transaction boundaries that determine when events are dispatched.

Infrastructure must **not**:

- Introduce domain logic.
- Depend on API or UI.

---

## 5. Event Flow

1. A command is handled by an Application handler.
2. The handler calls domain methods on aggregates.
3. Aggregates raise domain events and store them internally.
4. After the command completes, the Application layer:
   - Collects pending domain events.
   - Calls `IDomainEventDispatcher.DispatchAsync`.
5. `DomainEventDispatcher` resolves and invokes all matching `IDomainEventHandler<T>` implementations.

---

## 6. Purity Rules

- Domain events are defined in Domain or SharedKernel.
- `IDomainEventDispatcher` and `IDomainEventHandler<T>` live in Application Abstractions.
- Domain does **not** reference Application.
- Event handlers live in Application, not in API or Infrastructure.
- Infrastructure may implement side effects but not domain decisions.

---

## 7. Contributor Guidelines

When adding a new domain event:

- Define the event in Domain (or SharedKernel if cross-aggregate).
- Raise it from domain entities/aggregates.
- Add one or more `IDomainEventHandler<T>` implementations in Application.
- Do **not** dispatch events manually from API or Infrastructure.
- Let the existing dispatch pipeline handle them.
