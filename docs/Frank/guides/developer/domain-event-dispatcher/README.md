# Domain Event Dispatcher — Developer Guide

This guide documents the **current state** of Domain Event dispatching in Frank, and the **intended future state** defined by **US‑222 — Domain Event Dispatcher Implementation**.

Domain Events are a core architectural mechanism for expressing that “something happened” inside the domain.  
Frank currently provides only the *contracts* for domain event dispatching, but not the *implementation*.  
This guide explains how developers should work with the existing contracts today, and what will change once US‑222 is implemented.

---

# 1. Current State (Before US‑222)

Frank currently defines the following contracts:

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

- **Domain event types** (`IDomainEvent`)
- **Domain event handlers** (`IDomainEventHandler<TEvent>`)
- **Dispatcher contract** (`IDomainEventDispatcher`)
- **Auto‑registration attributes** for handlers and dispatcher interface

### What does *not* exist today

- **No concrete `DomainEventDispatcher` implementation**
- **No unified dispatching pipeline**
- **No deterministic handler ordering**
- **No built‑in error propagation rules**
- **No documentation for handler behavior**
- **No test suite for event dispatching**

### What developers must do today

- Each application or module must implement its own dispatcher  
- Handler resolution is inconsistent across modules  
- Event dispatching behavior varies between contexts  
- No shared guarantees about ordering, error handling, or cancellation  

This creates architectural drift and violates Frank’s principle of:

> “The platform provides the pipeline. The application provides the behavior.”

---

# 2. Intended Future State (After US‑222)

US‑222 introduces a **first‑class DomainEventDispatcher implementation** into Frank.

### The new dispatcher will:

- Resolve **all** `IDomainEventHandler<TEvent>` instances from DI  
- Invoke handlers **sequentially**, in DI order  
- Support **multiple handlers per event**  
- Gracefully handle the case where **no handlers exist**  
- Propagate exceptions (no swallowing)  
- Honor cancellation tokens  
- Be registered via AutoRegistration with scoped lifetime  
- Provide a consistent, deterministic dispatching pipeline  
- Mirror the architecture of CommandDispatcher and QueryDispatcher  

### What will *not* be added

- **No validation**  
  Domain events are *facts*, not *requests*.  
  They must never be validated.

- **No return values**  
  Domain events do not produce results.

- **No parallel dispatching**  
  Handlers run sequentially to preserve determinism.

---

# 3. Developer Responsibilities (Current vs Future)

## Current Responsibilities

- Implement your own dispatcher  
- Resolve handlers manually  
- Decide ordering rules  
- Decide error propagation rules  
- Maintain your own tests  
- Ensure handlers are registered correctly  

## Future Responsibilities (After US‑222)

- Define domain event types  
- Implement domain event handlers  
- Register handlers (AutoRegister already handles this)  
- Call the dispatcher when events occur  

Everything else is handled by Frank.

---

# 4. Example of the Intended Dispatcher (Future State)

This is the implementation that US‑222 will introduce:

```csharp
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;

    public DomainEventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync<TEvent>(
        TEvent domainEvent,
        CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        var handlers = _provider
            .GetServices<IDomainEventHandler<TEvent>>()
            .ToList();

        if (handlers.Count == 0)
            return;

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(domainEvent, cancellationToken);
        }
    }
}
```

This implementation:

- Matches the patterns used by CommandDispatcher and QueryDispatcher  
- Is simple, deterministic, and predictable  
- Provides the missing architectural symmetry  

---

# 5. Impact on Frank Architecture

### Before US‑222

- Event dispatching is inconsistent  
- Modules reinvent dispatching logic  
- No shared guarantees  
- Harder to test  
- Harder to reason about  
- Harder to extend (outbox, projections, sagas)

### After US‑222

- Unified dispatching pipeline  
- Deterministic handler execution  
- Predictable error behavior  
- Easier to test  
- Easier to reason about  
- Enables future event‑driven capabilities  

This story completes the triad:

- **CommandDispatcher**  
- **QueryDispatcher**  
- **DomainEventDispatcher** ← (US‑222)

---

# 6. Summary

The Domain Event Dispatcher is currently **contract‑only**, with no implementation.  
US‑222 introduces a **first‑class dispatcher** that:

- Resolves handlers from DI  
- Executes them sequentially  
- Propagates errors  
- Honors cancellation  
- Requires no validation  
- Requires no return values  
- Provides architectural consistency across Frank  

Developers should continue using the existing contracts today, and prepare for the unified dispatcher that will arrive with US‑222.

