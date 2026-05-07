# ADR‑0014 — Application Dispatcher Pipeline

## Status
Accepted

## Context
The Application layer uses a CQRS pattern with:

- `ICommand<TResponse>` and `IQuery<TResponse>`
- `ICommandHandler<TCommand, TResponse>` and `IQueryHandler<TQuery, TResponse>`
- FluentValidation validators for commands and queries
- A custom dispatcher (`CommandDispatcher` and `QueryDispatcher`) that:
  - Resolves validators
  - Executes validation
  - Resolves the appropriate handler
  - Invokes the handler

Before this ADR, the dispatcher relied on dynamic invocation of `ValidateAsync` using the command/query instance directly. This caused:

- Runtime binder exceptions
- Nullability warnings
- Inconsistent validation behavior
- Tests expecting `ValidationException` to fail with `RuntimeBinderException`

A consistent, safe, and predictable validation + dispatch pipeline is required.

## Decision

### 1. Validators are resolved using `GetServices`
This allows zero or many validators per command/query.

```csharp
var validatorType = typeof(IValidator<>).MakeGenericType(message.GetType());
var validators = _provider.GetServices(validatorType);
```

### 2. Validation is executed using `ValidationContext<object>`
FluentValidation requires a `ValidationContext` for open‑generic dispatch.

```csharp
var context = new ValidationContext<object>(message);
var result = await ((IValidator)validator).ValidateAsync(context, ct);
```

### 3. Validation failures throw `ValidationException`
This ensures:

- API layer returns HTTP 400
- Tests can assert on validation behavior
- Domain invariants remain separate from validation rules

```csharp
if (!result.IsValid)
    throw new ValidationException(result.Errors);
```

### 4. Handlers are resolved using `GetRequiredService`
This enforces correct DI registration and eliminates nullability warnings.

```csharp
var handlerType = typeof(ICommandHandler<,>)
    .MakeGenericType(command.GetType(), typeof(TResponse));

var handler = _provider.GetRequiredService(handlerType);
```

### 5. Handler execution uses dynamic dispatch
This allows runtime binding to the correct generic handler without reflection.

```csharp
return await ((dynamic)handler).HandleAsync((dynamic)command, ct);
```

### 6. The same pipeline applies to both commands and queries
This ensures consistency across the entire Application layer.

## Consequences

### Positive
- Validation is predictable and consistent
- No runtime binder exceptions
- No nullability warnings
- DI failures surface immediately
- Tests for invalid commands/queries pass reliably
- Domain invariants remain in the domain layer
- API layer receives clean `ValidationException` → 400 Bad Request

### Negative
- Dynamic dispatch remains part of the pipeline (acceptable trade‑off)
- Validation requires an extra `ValidationContext<object>` allocation per request

### Neutral
- Mirrors MediatR’s pipeline behavior without introducing MediatR as a dependency

## Rationale
This design:

- Aligns with FluentValidation’s recommended usage for open‑generic dispatch
- Preserves the vertical slice architecture
- Keeps the Application layer free of framework dependencies
- Ensures handlers remain simple and focused
- Ensures validation is always executed before domain logic

## Code Reference (Final Form)

### CommandDispatcher

```csharp
foreach (var validator in validators)
{
    var context = new ValidationContext<object>(command);
    var result = await ((IValidator)validator).ValidateAsync(context, ct);

    if (!result.IsValid)
        throw new ValidationException(result.Errors);
}

var handlerType = typeof(ICommandHandler<,>)
    .MakeGenericType(command.GetType(), typeof(TResponse));

var handler = _provider.GetRequiredService(handlerType);

return await ((dynamic)handler).HandleAsync((dynamic)command, ct);
```

### QueryDispatcher

```csharp
foreach (var validator in validators)
{
    var context = new ValidationContext<object>(query);
    var result = await ((IValidator)validator).ValidateAsync(context, ct);

    if (!result.IsValid)
        throw new ValidationException(result.Errors);
}

var handlerType = typeof(IQueryHandler<,>)
    .MakeGenericType(query.GetType(), typeof(TResponse));

var handler = _provider.GetRequiredService(handlerType);

return await ((dynamic)handler).HandleAsync((dynamic)query, ct);
```

## Notes
This ADR formalizes the dispatcher pipeline so future slices, contributors, and refactors maintain:

- Validation before execution  
- Consistent exception semantics  
- Clean separation of concerns  
- Predictable behavior across commands and queries
