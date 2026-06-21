# Query Dispatcher — Developer Guide

The Query Dispatcher capability provides a **centralized, validated, DI‑driven query execution pipeline**.  
It ensures that:

- queries are validated before execution  
- handlers are resolved deterministically  
- queries execute through a consistent, predictable mechanism  
- validation failures surface as exceptions  
- handler resolution is type‑safe and DI‑driven  

This guide documents the architecture, invariants, and extension points for developers implementing or integrating with the Query Dispatcher.

---

## 1. Core Purpose

The Query Dispatcher provides:

- a single entry point for executing queries  
- automatic FluentValidation integration  
- automatic handler resolution  
- support for typed responses  
- scoped lifetime semantics  

This capability enforces a clean separation between:

- **query intent**  
- **validation**  
- **execution**  

Queries represent **read‑only operations** and must not mutate state.

---

## 2. Query Abstractions

### 2.1 IQuery<TResponse>

```csharp
public interface IQuery<TResponse> { }
```

Represents a read‑only request that returns a value.

Queries must be:

- immutable  
- intention‑revealing  
- free of behavior  

---

## 3. Query Handlers

Handlers are auto‑registered via `AutoRegister`.

### 3.1 IQueryHandler<TQuery, TResponse>

```csharp
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken ct);
}
```

### Handler Requirements

- Must be pure (no side effects)  
- Must not mutate domain state  
- Must return the correct response type  
- Must be registered exactly once  

---

## 4. Query Dispatcher

```csharp
public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _provider;

    public QueryDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct)
    {
        ...
    }
}
```

The dispatcher is responsible for:

1. Running validators  
2. Resolving the correct handler  
3. Executing the handler  

---

## 5. Validation Pipeline

The dispatcher automatically discovers and executes all validators for a query.

### How validators are resolved

```csharp
var validatorType = typeof(IValidator<>).MakeGenericType(query.GetType());
var validators = _provider.GetServices(validatorType);
```

### Validation behavior

- All validators run  
- ValidationContext is created dynamically  
- If any validator fails → `ValidationException` is thrown  
- Query execution stops immediately  

### Developer requirements

- Validators must be registered in DI  
- Validators must target the exact query type  

---

## 6. Handler Resolution

Handlers are resolved using DI:

```csharp
var handlerType = typeof(IQueryHandler<,>)
    .MakeGenericType(query.GetType(), typeof(TResponse));

var handler = _provider.GetRequiredService(handlerType);
```

### Invariants

- Exactly one handler must exist  
- Handlers must be registered via AutoRegistration  
- Handlers must be scoped (default behavior)  

---

## 7. Execution Pipeline

```csharp
return await ((dynamic)handler).HandleAsync((dynamic)query, ct);
```

### Notes

- Dynamic dispatch is used to avoid reflection‑heavy invocation  
- Exceptions thrown by handlers propagate naturally  
- Cancellation tokens are passed through end‑to‑end  

---

## 8. DI Registration

The dispatcher is registered via:

```csharp
[AutoRegister(ServiceLifetime.Scoped)]
public interface IQueryDispatcher
```

Handlers are registered via:

```csharp
[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface IQueryHandler<...>
```

### Implications

- Dispatcher is scoped  
- Handlers are scoped  
- Only one handler per query is allowed  
- Concrete handler types are auto‑registered  

---

## 9. Developer Invariants

Developers must ensure:

- Queries are **pure** and **read‑only**  
- Validators are pure and deterministic  
- Handlers do not perform validation (dispatcher handles it)  
- Handlers do not mutate domain state  
- Handlers do not throw validation exceptions (validators handle it)  
- Handlers return the correct type  

---

## 10. Anti‑Patterns

Avoid:

- Registering multiple handlers for the same query  
- Performing validation inside handlers  
- Throwing non‑ValidationException for validation failures  
- Using static/global state inside handlers  
- Creating queries with mutable public fields  
- Returning null from handlers  

---

## 11. Summary

The Query Dispatcher provides:

- a unified query execution pipeline  
- automatic validation  
- automatic handler resolution  
- deterministic behavior  
- clean separation of concerns  

As a developer:

- You define queries  
- You implement handlers  
- You optionally add validators  
- The dispatcher handles everything else  

This Developer Guide documents everything needed to extend or integrate with the Query Dispatcher capability.
