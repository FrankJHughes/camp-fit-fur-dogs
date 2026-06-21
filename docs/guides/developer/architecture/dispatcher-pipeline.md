# Dispatcher Pipeline Guide

This guide describes how the command and query dispatcher pipeline works in Camp Fit Fur Dogs.  
It defines the runtime flow, validation rules, handler responsibilities, Unit of Work behavior, and domain event dispatching.  
This is a developer‑level guide focused on how slices execute through the Application layer.

---

# 1. Purpose of the Dispatcher Pipeline

The dispatcher pipeline provides a unified execution model for all commands and queries.  
It ensures:

- Consistent cross‑cutting behavior  
- Thin API endpoints  
- Predictable handler execution  
- Centralized validation  
- Centralized persistence  
- Centralized domain event dispatch  
- Isolation between read and write paths  
- Easy testability of handlers and slices  

All business logic flows through this pipeline.

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

API endpoints call dispatchers — never handlers directly.

Handlers, validators, and dispatchers are auto‑registered via Frank’s DI engine.

---

# 3. Command Pipeline

The command pipeline executes in a strict, ordered sequence.

```
Validation → Handler → Unit of Work → Domain Events → Result
```

## 3.1 Validation

- All `IValidator<TCommand>` instances are resolved  
- Validation runs before the handler  
- Validation failures stop execution  
- Errors are returned to the API boundary for shaping  

Validation contains no business logic.

## 3.2 Handler Resolution

- Exactly one `ICommandHandler<TCommand, TResult>` must exist  
- Resolved via auto‑registration  
- Endpoints never invoke handlers directly  

## 3.3 Handler Execution

Handlers:

- Orchestrate domain behavior  
- Call repositories  
- Load aggregates  
- Invoke aggregate methods  
- Never raise domain events directly (aggregates do)  
- Never call DbContext directly  
- Never perform persistence  

Handlers are pure orchestrators.

## 3.4 Persistence (Unit of Work)

The Unit of Work coordinates:

1. Change tracking  
2. Commit  
3. Domain event dispatch  

Flow:

1. Handler performs repository operations  
2. Handler calls `CommitAsync()`  
3. Commit persists changes  
4. Commit triggers domain event dispatch  

Handlers never flush DbContext directly.

## 3.5 Domain Events

Domain events flow through the system as follows:

1. Aggregates raise events internally  
2. Unit of Work collects them  
3. After commit, `IDomainEventDispatcher` dispatches them  
4. Domain event handlers run in sequence  

Domain events never cross the API boundary.

## 3.6 Result

- Handler returns a DTO or primitive  
- Dispatcher returns it to the endpoint  
- Endpoint shapes the HTTP response  

Commands never return domain entities.

---

# 4. Query Pipeline

Queries follow a simplified, read‑only pipeline.

```
Validation → Handler → Reader → Result
```

## 4.1 Validation

- All `IValidator<TQuery>` instances are resolved  
- Validation runs before the handler  

Queries must not mutate state.

## 4.2 Handler Execution

- Exactly one `IQueryHandler<TQuery, TResult>` must exist  
- Handler executes read‑only logic  
- Handlers depend on readers, not repositories (ADR‑0021)  
- Handlers never call `CommitAsync()`  

## 4.3 Result

- Handler returns a DTO or primitive  
- Dispatcher returns it to the endpoint  

Queries do not raise domain events.

---

# 5. Handler Responsibilities

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
- Stateless  
- Auto‑registered  
- Focused on orchestration  
- Bound to a single use case  
- Free of cross‑cutting concerns  

Handlers do not:

- Perform validation  
- Manage transactions  
- Dispatch domain events  
- Issue HTTP responses  
- Access HttpContext  
- Access EF Core directly  

---

# 6. Validation Responsibilities

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
- Contain no domain logic  
- Contain no persistence logic  
- Contain no cross‑cutting logic  

Validators enforce input correctness, not business rules.

---

# 7. Execution Flow Summary

## Command Flow

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

## Query Flow

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

# 8. Purity Rules

The dispatcher pipeline enforces strict purity rules:

- Handlers do not access HTTP, cookies, or headers  
- Handlers do not access EF Core directly  
- Handlers do not perform I/O  
- Handlers do not dispatch domain events  
- Validators do not perform domain logic  
- Queries do not mutate state  
- Commands do not return domain entities  
- Domain events do not cross the API boundary  

These rules ensure slices remain isolated, testable, and predictable.

---

# 9. Contributor Guidance

When adding a new command or query:

1. Create the request type in Abstractions  
2. Add validators if needed  
3. Implement the handler  
4. Inject repositories or readers  
5. For commands, call `CommitAsync()`  
6. Use dispatchers in endpoints  
7. Keep handlers small and focused  
8. Ensure aggregates raise domain events  
9. Test handlers in isolation  
10. Test the slice end‑to‑end via API tests  

If a handler grows beyond ~30 lines, logic is leaking into the wrong layer.

---

# Related Guides

- API Endpoint Guide  
- Domain Events Architecture  
- Dependency Injection Architecture  
- Session Management Guide  
- Vertical Slice Guide  
