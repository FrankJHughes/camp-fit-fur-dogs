# Guides – Developer – Domain Event Dispatcher

Frank provides a **first‑class Domain Event Dispatcher** that resolves and executes domain event handlers through the Registration Engine.  
This capability mirrors the Command and Query dispatchers and provides a consistent, deterministic event‑handling pipeline.

Domain Events represent “something that happened” inside the domain.  
Handlers react to these events without returning values.

---

# 1. Core Purpose

The Domain Event Dispatcher:

- resolves all handlers for a given event type  
- executes them **sequentially**  
- honors cancellation tokens  
- propagates exceptions  
- performs no validation  
- performs no return‑value aggregation  

Domain events are **facts**, not requests — they are not validated and do not produce results.

---

# 2. Domain Event Abstractions

## 2.1 `IEvent`

```csharp
public interface IEvent { }
```

Represents a domain event.

---

## 2.2 `IEventHandler<TEvent>`

```csharp
public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
```

Handlers react to events.  
Multiple handlers may exist for the same event.

---

## 2.3 `IEventDispatcher`

```csharp
public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : IEvent;
}
```

The dispatcher is the single entry point for raising events.

---

# 3. Dispatcher Implementation

Frank ships a concrete dispatcher:

```csharp
public sealed class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _provider;

    public EventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

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
}
```

### Dispatcher guarantees

- **Sequential execution**  
- **No‑op if no handlers exist**  
- **No swallowing of exceptions**  
- **DI‑driven handler resolution**  
- **Scoped lifetime**  

---

# 4. DI Registration (via Registration Engine)

Frank provides:

```csharp
public static IServiceCollection AddFrankEvent(
    this IServiceCollection services,
    IEnumerable<Assembly> assemblies,
    Action<DiscoveryOptions>? configure = null)
```

### What it registers

- `IEventDispatcher` → `EventDispatcher` (scoped)
- All `IEventHandler<TEvent>` implementations discovered via `DiscoveryOptions`

### Discovery rules used

```csharp
options.IncludeInterface(iface =>
    HasRegistrationAttribute(iface) &&
    iface.IsGenericType &&
    iface.GetGenericTypeDefinition() == typeof(IEventHandler<>));

options.IncludeImplementation(impl =>
    impl.ImplementedInterfaces.Any(i =>
        i.IsGenericType &&
        i.GetGenericTypeDefinition() == typeof(IEventHandler<>)));
```

Meaning:

- Only interfaces marked with `[Registration]` are governed  
- Only classes implementing `IEventHandler<>` are included  
- Concrete handlers are auto‑registered  

---

# 5. Developer Responsibilities

Developers must:

- Define event types (`IEvent`)  
- Implement handlers (`IEventHandler<TEvent>`)  
- Mark handler interfaces with `[Registration]`  
- Ensure assemblies are included in the orchestrator call  

Developers must **not**:

- Manually register event handlers  
- Invoke handlers directly  
- Implement their own dispatcher  

---

# 6. Handler Behavior Rules

Handlers must:

- be deterministic  
- be side‑effect‑free except for domain‑appropriate actions  
- honor cancellation tokens  
- not throw validation exceptions  
- not return values  

Handlers may:

- publish additional domain events  
- write to the domain model  
- enqueue outbox messages  
- trigger projections  

---

# 7. Summary

Frank provides a complete Domain Event Dispatcher:

- DI‑driven  
- Sequential  
- Deterministic  
- Registration‑Engine‑governed  
- Symmetric with Command and Query dispatchers  

Developers define events and handlers — Frank handles the pipeline.
