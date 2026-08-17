# Queries

The **Queries** folder contains the abstractions that define the read‑side of the CQRS (Command Query Responsibility Segregation) pattern. Queries represent requests for information, projections, or computed results. Unlike commands, queries never modify application state.

This folder provides the core interfaces used throughout the query pipeline: queries, query handlers, and the query dispatcher.

---

## Purpose

Queries model *information retrieval*. They are:

- **descriptive** — they ask for data (“give me this”)
- **side‑effect‑free** — they do not change system state
- **explicit** — each query represents a single, well‑defined read operation

By separating queries from commands, the application maintains a clear distinction between:

- **read operations** (queries)
- **write operations** (commands)

This separation improves clarity, scalability, and testability.

---

## Components

### IQuery<TResponse>
Represents a query that returns a typed response.

```csharp
public interface IQuery<TResponse> { }
```

Used for operations where the caller expects a result, such as:

- fetching an entity  
- retrieving a projection  
- computing a read‑side model  

---

### IQueryHandler<TQuery, TResponse>
Handles queries and returns typed results.

```csharp
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken ct);
}
```

Each handler is automatically registered via the `Registration` attribute with:

- scoped lifetime  
- concrete type registration  
- exactly one implementation  

---

### IQueryDispatcher
Coordinates query execution by locating and invoking the correct handler.

```csharp
public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
```

The dispatcher decouples query invocation from handler resolution, improving testability and inversion of control.

---

## Design Principles

- **Strong typing**  
  Queries and handlers enforce type‑safe request/response flows.

- **Separation of concerns**  
  Queries express intent; handlers perform read‑side logic; the dispatcher coordinates execution.

- **Automatic registration**  
  Handlers are discovered and registered via the `Registration` subsystem.

- **Side‑effect‑free**  
  Queries never modify application state, ensuring predictable behavior.
