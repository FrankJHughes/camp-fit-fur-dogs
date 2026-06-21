# Domain Event Dispatcher — Tester Guide

This guide documents the **current state** of Domain Event dispatching in Frank and the **intended future state** defined by **US‑222 — Domain Event Dispatcher Implementation**.

Testers validate the observable behavior of the domain event pipeline.  
Because Frank currently provides only contracts (and no implementation), the testing surface today is minimal.  
Once US‑222 is implemented, the dispatcher will become a first‑class capability with a well‑defined, testable behavior.

---

# 1. Current State (Before US‑222)

Frank currently defines:

```csharp
public interface IDomainEvent { }

public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
```

### What exists today

- Domain event **types**  
- Domain event **handlers**  
- Dispatcher **contract only**  
- Auto‑registration attributes for handlers and dispatcher interface  

### What does *not* exist today

- No concrete dispatcher implementation  
- No unified dispatching behavior  
- No ordering guarantees  
- No error propagation rules  
- No cancellation semantics  
- No test suite  

### What testers can validate today

- Handler interfaces compile and can be implemented  
- Auto‑registration attributes are present  
- Domain event types can be defined  

There is **no runtime behavior** to test because no dispatcher exists.

---

# 2. Intended Future State (After US‑222)

US‑222 introduces a **concrete DomainEventDispatcher** with deterministic, testable behavior.

### The dispatcher will:

- Resolve **all** `IDomainEventHandler<TEvent>` instances from DI  
- Invoke handlers **sequentially**, in DI order  
- Support **multiple handlers** per event  
- Gracefully handle **zero handlers**  
- Propagate exceptions (no swallowing)  
- Honor cancellation tokens  
- Be registered via AutoRegistration with scoped lifetime  

### What will *not* be added

- **No validation** (domain events are facts)  
- **No return values**  
- **No parallel dispatching**  

---

# 3. Test Responsibilities (Current vs Future)

## Current Responsibilities

- Minimal — only verify that domain event handlers can be implemented and registered  
- No dispatcher behavior to test  
- No ordering or error semantics to validate  

## Future Responsibilities (After US‑222)

Testers will validate:

- Handler discovery  
- Handler invocation order  
- Multiple handler execution  
- Zero‑handler behavior  
- Exception propagation  
- Cancellation token behavior  
- DI lifetime behavior  
- Deterministic sequential dispatch  

---

# 4. Required Test Types (Future State)

## 4.1 Handler Discovery Tests

Validate:

- All handlers for a given event type are resolved  
- Handlers are resolved in DI order  
- Zero handlers → no‑op (no exception)  

### Scenarios

- Event with 0 handlers  
- Event with 1 handler  
- Event with multiple handlers  

---

## 4.2 Handler Invocation Tests

Validate:

- Handlers are invoked **sequentially**  
- Each handler receives the same event instance  
- No handler is skipped  

### Scenarios

- Multiple handlers → verify call order  
- Handlers with side effects → verify sequencing  

---

## 4.3 Exception Propagation Tests

Validate:

- If a handler throws, the dispatcher surfaces the exception  
- No subsequent handlers are invoked after an exception  
- No exceptions are swallowed  

### Scenarios

- First handler throws  
- Middle handler throws  
- Last handler throws  

---

## 4.4 Cancellation Token Tests

Validate:

- Cancellation token is passed to handlers  
- Dispatcher stops invoking handlers when cancellation is requested  

### Scenarios

- Cancellation requested before dispatch  
- Cancellation requested during handler execution  

---

## 4.5 DI Lifetime Tests

Validate:

- Dispatcher is scoped  
- Handlers are scoped  
- New scope → new handler instances  
- Same scope → same handler instances  

---

# 5. Recommended Testing Patterns

## 5.1 Fake Domain Events

```csharp
public sealed record TestEvent(string Value) : IDomainEvent;
```

## 5.2 Tracking Handlers

```csharp
public sealed class TrackingHandler : IDomainEventHandler<TestEvent>
{
    public bool Called { get; private set; }

    public Task HandleAsync(TestEvent domainEvent, CancellationToken ct)
    {
        Called = true;
        return Task.CompletedTask;
    }
}
```

Use to verify:

- Handler invocation  
- Ordering  
- Cancellation behavior  

---

## 5.3 Exception‑Throwing Handlers

```csharp
public sealed class ThrowingHandler : IDomainEventHandler<TestEvent>
{
    public Task HandleAsync(TestEvent domainEvent, CancellationToken ct)
        => throw new InvalidOperationException("boom");
}
```

Use to verify:

- Exception propagation  
- Handler short‑circuiting  

---

## 5.4 Multiple Handler Tests

Ensure:

- All handlers are invoked in DI order  
- No handler is skipped  
- Exceptions stop the pipeline  

---

# 6. Anti‑Patterns (Tests Must Reject)

- Tests that assume parallel execution  
- Tests that assume validation occurs  
- Tests that assume return values  
- Tests that swallow exceptions  
- Tests that rely on static/global state  
- Tests that assume handler order outside DI ordering  

---

# 7. Summary

**Current State:**  
Frank provides only contracts for domain events. No dispatcher exists, so runtime behavior cannot be tested.

**Future State (US‑222):**  
Frank will include a first‑class DomainEventDispatcher with deterministic, testable behavior:

- Sequential handler invocation  
- Multiple handler support  
- Exception propagation  
- Cancellation support  
- Scoped DI semantics  

This Tester Guide prepares the testing strategy for both the current minimal state and the fully realized dispatcher that will arrive with US‑222.

