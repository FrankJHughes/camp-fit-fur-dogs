# Domain Event Dispatcher — User Guide

This guide explains how users of the Frank platform should work with **Domain Events** today, and how the experience will improve once **US‑222 — Domain Event Dispatcher Implementation** ships.

Domain Events represent **facts** about something that happened inside the domain.  
They are not commands, not queries, and not requests — they are notifications that other parts of the system may react to.

---

# 1. Current State (Before US‑222)

Frank currently provides **only the contracts** for domain events:

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

### What this means for you today

- You **can define domain events** (`IDomainEvent`)
- You **can implement handlers** (`IDomainEventHandler<TEvent>`)
- You **can depend on the dispatcher interface**, but…

There is **no built‑in dispatcher implementation** yet.

### What you must do today

- Each module or application must implement its own dispatcher  
- You must manually resolve handlers from DI  
- You must decide how to invoke them  
- You must decide how to handle exceptions  
- You must decide whether to run handlers sequentially or in parallel  

This leads to inconsistent behavior across modules.

---

# 2. Intended Future State (After US‑222)

US‑222 introduces a **first‑class DomainEventDispatcher** into Frank.

### What the dispatcher will do for you

- Automatically resolve all handlers for a domain event  
- Invoke handlers **sequentially**, in DI order  
- Support multiple handlers per event  
- Gracefully handle the case where no handlers exist  
- Propagate exceptions (no swallowing)  
- Honor cancellation tokens  
- Require no configuration  
- Require no boilerplate  

### What the dispatcher will *not* do

- **No validation**  
  Domain events are *facts*, not requests.

- **No return values**  
  Handlers perform side effects only.

- **No parallel execution**  
  Determinism is more important than speed.

### What this means for you after US‑222

You will:

- Define domain events  
- Implement handlers  
- Inject and call the dispatcher  

Frank will handle everything else.

---

# 3. How You Use Domain Events Today

### 3.1 Define a domain event

```csharp
public sealed record CustomerCreated(Guid CustomerId) : IDomainEvent;
```

### 3.2 Implement a handler

```csharp
public sealed class SendWelcomeEmailHandler
    : IDomainEventHandler<CustomerCreated>
{
    public Task HandleAsync(CustomerCreated evt, CancellationToken ct)
    {
        // send email
        return Task.CompletedTask;
    }
}
```

### 3.3 Dispatch the event (your own implementation)

Because Frank does not yet provide a dispatcher, you must write your own:

```csharp
await myDispatcher.DispatchAsync(new CustomerCreated(id), ct);
```

Each module may do this differently.

---

# 4. How You Will Use Domain Events After US‑222

### 4.1 Define a domain event  
*(same as today)*

### 4.2 Implement handlers  
*(same as today)*

### 4.3 Dispatch the event  
*(Frank will provide the dispatcher)*

```csharp
await _domainEventDispatcher.DispatchAsync(
    new CustomerCreated(customerId),
    ct);
```

No custom dispatcher required.  
No handler resolution required.  
No ordering logic required.  
No exception handling boilerplate required.

---

# 5. What Will Become Easier

### Today
- You must implement dispatching yourself  
- You must decide how to resolve handlers  
- You must decide how to handle exceptions  
- You must decide how to sequence handlers  
- You must maintain your own tests  

### After US‑222
- Frank provides the dispatcher  
- Frank resolves handlers  
- Frank invokes handlers sequentially  
- Frank propagates exceptions  
- Frank provides a unified testable behavior  

You focus only on **domain behavior**, not infrastructure.

---

# 6. Summary

**Current State:**  
Frank provides only the contracts for domain events.  
You must implement your own dispatcher.

**Future State (US‑222):**  
Frank will include a full DomainEventDispatcher that:

- Resolves handlers  
- Invokes them sequentially  
- Propagates exceptions  
- Honors cancellation  
- Requires no validation  
- Requires no return values  

As a user of this capability:

- You define domain events  
- You implement handlers  
- You call the dispatcher  

Frank will handle everything else.

