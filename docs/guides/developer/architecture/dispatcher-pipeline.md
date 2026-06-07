# Dispatcher Pipeline Guide

This guide explains how the **command and query dispatcher pipeline** works in Camp Fit Fur Dogs.  
It describes the runtime flow, how handlers are invoked, how validation works, and how domain events are dispatched.  
This is a **developer guide**, not governance or conventions.

---

# 1. Purpose of the Dispatcher Pipeline

The dispatcher pipeline exists to:

- Centralize command and query execution  
- Apply cross‑cutting concerns consistently  
- Keep API endpoints thin  
- Make handlers easy to test  
- Ensure all business logic flows through a single mechanism  
- Provide a predictable, uniform execution model across all slices  

The dispatcher is the backbone of the application layer.

---

# 2. Core Abstractions

## 2.1 ICommandDispatcher

```csharp
public interface ICommandDispatcher
{
    Task<TResult> DispatchAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default);
}
```

## 2.2 IQueryDispatcher

```csharp
public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default);
}
```

API endpoints call these dispatchers — never handlers directly.

Handlers, validators, and dispatchers are auto‑registered.

---

# 3. Command Pipeline (Deep‑Dive)

The command pipeline executes in the following strict order:

---

## 3.1 Validation

- All `IValidator<TCommand>` instances are resolved  
- Validation runs before the handler  
- Validation failures stop the pipeline immediately  
- Errors are returned to the API boundary for shaping  

Validation is a pipeline concern — not a handler concern.

---

## 3.2 Handler Resolution

- Exactly one `ICommandHandler<TCommand, TResult>` must exist  
- Resolved via auto‑registration  
- Handlers are never invoked directly by endpoints  

---

## 3.3 Handler Execution

- `HandleAsync` is invoked  
- Handlers orchestrate domain behavior  
- Handlers may call repositories, domain services, and aggregates  
- Handlers do not raise domain events directly — aggregates do  

---

## 3.4 Persistence (Unit of Work)

The Unit of Work coordinates:

- Change tracking  
- Commit  
- Domain event dispatch  

Flow:

1. Handler performs repository operations  
2. Handler calls `CommitAsync()`  
3. Commit persists changes  
4. Commit triggers domain event dispatch  

The handler never flushes DbContext directly.

---

## 3.5 Domain Events

Domain events flow as follows:

1. Aggregates raise events internally  
2. Unit of Work collects them  
3. After commit, `IDomainEventDispatcher` dispatches them  
4. Domain event handlers run in sequence  

Domain events never cross the API boundary.

---

## 3.6 Result

- Handler returns a DTO or primitive  
- Dispatcher returns it to the endpoint  
- Endpoint shapes the HTTP response  

Commands never return domain entities.

---

# 4. Query Pipeline (Deep‑Dive)

Queries follow a simplified pipeline:

---

## 4.1 Validation

- All `IValidator<TQuery>` instances are resolved  
- Validation runs before the handler  

Queries must not mutate state.

---

## 4.2 Handler Execution

- Exactly one `IQueryHandler<TQuery, TResult>` must exist  
- Handler executes read‑only logic  
- Queries may use readers or read‑only repositories  

---

## 4.3 Result

- Handler returns a DTO or primitive  
- Dispatcher returns it to the endpoint  

Queries do not raise domain events.

---

# 5. Handler Overview

Handlers implement:

```csharp
public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
```

Handlers are:

- Slice‑local  
- Auto‑registered  
- Stateless  
- Focused on orchestration  

---

# 6. Validation Overview

Validators implement:

```csharp
public interface IValidator<in T>
{
    Task ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
```

Validators:

- Run before handlers  
- Are auto‑registered  
- Live in the same slice as the handler  
- Contain no business logic  

---

# 7. Execution Flow Summary

### Command Flow

```
API Endpoint
    ↓
Command DTO
    ↓
Command Dispatcher
    ↓
Validators
    ↓
Handler
    ↓
Repositories / Domain Services
    ↓
Unit of Work Commit
    ↓
Domain Events Dispatched
    ↓
Result Returned
```

### Query Flow

```
API Endpoint
    ↓
Query DTO
    ↓
Query Dispatcher
    ↓
Validators
    ↓
Handler
    ↓
Readers / Read‑Only Repositories
    ↓
Result Returned
```

---

# 8. Contributor Guidance

When adding a new command or query:

1. Create the request type  
2. Add validators if needed  
3. Implement the handler  
4. Inject repositories or readers  
5. For commands, call `CommitAsync`  
6. Use dispatchers in endpoints  
7. Keep handlers small and focused  
8. Ensure domain events are raised inside aggregates  
9. Test handlers in isolation  
10. Test the slice end‑to‑end via API tests  

If a handler grows beyond ~30 lines, logic is leaking into the wrong layer.

---

# Related Guides

- API Endpoint Guide  
- Domain Events Guide  
- Authentication Architecture Guide  
- Session Management Guide  
- Vertical Slice Guide  
