
# Frank — Guides — Tester — Domain Event Dispatcher Guide

This guide documents how to test the **current, concrete** Domain Event Dispatcher implementation in Frank.

Frank’s Domain Event pipeline is now a **real capability**, consisting of:

- `IEvent`  
- `IEventHandler<TEvent>`  
- `IEventDispatcher`  
- `EventDispatcher` (concrete implementation)  
- Registration via the **Registration Engine** (`DiscoveryOptions`, `Orchestrator`)  

Testers validate the **observable runtime behavior** of the dispatcher and the **correctness of DI‑driven handler discovery**.

---

# 1. What the Dispatcher Actually Does

Based on the real implementation:

```csharp
public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
    where TEvent : IEvent
{
    var handlers = _provider.GetServices<IEventHandler<TEvent>>().ToList();

    if (handlers.Count == 0)
        return;

    foreach (var handler in handlers)
    {
        await handler.HandleAsync(domainEvent, ct);
    }
}
```

### Observable behavior

- All handlers for the event type are resolved from DI  
- Handlers are invoked **sequentially**, in DI order  
- Zero handlers → **no‑op**  
- Exceptions propagate  
- Cancellation tokens propagate  
- No validation  
- No return values  
- No parallelism  

This is the behavior testers must validate.

---

# 2. Required Test Types

## 2.1 Handler Discovery Tests

Validate:

- All handlers for a given event type are resolved  
- Handlers are resolved in **DI order**  
- Zero handlers → no exception  

### Scenarios

- Event with 0 handlers  
- Event with 1 handler  
- Event with multiple handlers  

---

## 2.2 Handler Invocation Tests

Validate:

- Handlers are invoked **sequentially**  
- Each handler receives the same event instance  
- No handler is skipped  

### Scenarios

- Multiple handlers → verify call order  
- Handlers with side effects → verify sequencing  

---

## 2.3 Exception Propagation Tests

Validate:

- If a handler throws, the dispatcher surfaces the exception  
- No subsequent handlers are invoked after an exception  
- No exceptions are swallowed  

### Scenarios

- First handler throws  
- Middle handler throws  
- Last handler throws  

---

## 2.4 Cancellation Token Tests

Validate:

- Cancellation token is passed to handlers  
- Dispatcher stops invoking handlers when cancellation is requested  

### Scenarios

- Cancellation requested before dispatch  
- Cancellation requested during handler execution  

---

## 2.5 DI Lifetime Tests

Validate:

- Dispatcher is scoped  
- Handlers are scoped  
- New scope → new handler instances  
- Same scope → same handler instances  

---

# 3. Registration Engine Tests (Event‑Specific)

Because `AddFrankEvent` uses the **current Registration Engine**, testers must validate:

### Interface discovery

`IEventHandler<TEvent>` must:

- be generic  
- be decorated with `[Registration]`  
- be included via `IncludeInterface`  

### Implementation discovery

Any class implementing `IEventHandler<TEvent>` must:

- be included via `IncludeImplementation`  
- be registered with the correct lifetime  
- be registered once  

### Orchestrator behavior

Validate:

- Valid shapes → success  
- Invalid shapes → `InvalidOperationException`  
- Exception message matches formatted violations  

---

# 4. Recommended Testing Patterns

## 4.1 Fake Events

```csharp
public sealed record TestEvent(string Value) : IEvent;
```

## 4.2 Tracking Handlers

```csharp
public sealed class TrackingHandler : IEventHandler<TestEvent>
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

## 4.3 Exception‑Throwing Handlers

```csharp
public sealed class ThrowingHandler : IEventHandler<TestEvent>
{
    public Task HandleAsync(TestEvent domainEvent, CancellationToken ct)
        => throw new InvalidOperationException("boom");
}
```

Use to verify:

- Exception propagation  
- Handler short‑circuiting  

---

## 4.4 Multiple Handler Tests

Ensure:

- All handlers are invoked in DI order  
- No handler is skipped  
- Exceptions stop the pipeline  

---

# 5. Anti‑Patterns (Tests Must Reject)

- Tests that assume parallel execution  
- Tests that assume validation occurs  
- Tests that assume return values  
- Tests that swallow exceptions  
- Tests that rely on static/global state  
- Tests that assume handler order outside DI ordering  
- Tests that assume handlers are optional when `[Registration]` requires them  

---

# 6. Summary

Frank’s **current** Domain Event Dispatcher:

- Resolves handlers via DI  
- Invokes them sequentially  
- Supports multiple handlers  
- Propagates exceptions  
- Propagates cancellation  
- Performs no validation  
- Returns no values  

Testers must validate:

- Handler discovery  
- Handler ordering  
- Handler invocation  
- Exception propagation  
- Cancellation behavior  
- DI lifetime behavior  
- Registration Engine correctness  

This guide reflects the **actual, concrete implementation** of the Domain Event Dispatcher in Frank.

