# Dispatcher Pipeline

This guide describes the command and query dispatcher pipeline in Camp Fit Fur Dogs: how requests flow, where validation happens, and how handlers are invoked.

---

## 1. Goals

- Centralize command and query execution.
- Apply cross-cutting concerns consistently (validation, domain events, logging).
- Keep API endpoints thin and free of business logic.
- Make handlers easy to test in isolation.

---

## 2. Abstractions

### 2.1 ICommandDispatcher

```csharp
public interface ICommandDispatcher
{
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default);
}
```

### 2.2 IQueryDispatcher

```csharp
public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default);
}
```

These abstractions live in Application and are consumed by API endpoints and other application services.

---

## 3. Command Pipeline

The command pipeline performs:

1. **Validation**  
   - Resolve all `IValidator<TCommand>`.
   - Run validation.
   - Fail fast on validation errors.

2. **Handler Execution**  
   - Resolve `ICommandHandler<TCommand, TResult>`.
   - Invoke `HandleAsync`.

3. **Domain Events**  
   - Collect domain events from aggregates.
   - Dispatch via `IDomainEventDispatcher`.

4. **Result**  
   - Return the handler result to the caller.

---

## 4. Query Pipeline

The query pipeline performs:

1. **Validation**  
   - Resolve all `IValidator<TQuery>`.
   - Run validation.

2. **Handler Execution**  
   - Resolve `IQueryHandler<TQuery, TResult>`.
   - Invoke `HandleAsync`.

3. **Result**  
   - Return the handler result to the caller.

Queries generally do not raise domain events.

---

## 5. Handlers

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

Conventions:

- Class names end with `Handler`.
- Handlers live in Application slice folders.
- Handlers are registered by convention (no manual DI).

---

## 6. Validation

Validators implement:

```csharp
public interface IValidator<in T>
{
    Task ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
```

Conventions:

- Class names end with `Validator`.
- Validators live in Application slice folders.
- Validators are registered by convention.

---

## 7. Purity Rules

- API endpoints call dispatchers, not handlers directly.
- Handlers do not depend on API or UI types.
- Dispatchers live in Application and are tested in isolation.
- Validation is part of the pipeline, not embedded in handlers.

---

## 8. Contributor Guidelines

When adding a new command or query:

- Define the request type in Application Abstractions.
- Implement a handler in the corresponding slice.
- Optionally add validators.
- Use `ICommandDispatcher` / `IQueryDispatcher` from API endpoints.
- Do not bypass the dispatcher pipeline.
