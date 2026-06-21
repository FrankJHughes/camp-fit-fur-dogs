# Frank ExceptionHandling — Developer Guide

The ExceptionHandling capability in Frank is a **contract‑driven, deterministic, capability‑governed error pipeline**.  
It spans three layers:

1. **Core Abstractions** — how exceptions are classified and shaped  
2. **API Runtime** — how handlers are resolved and responses are produced  
3. **Capability Governance** — how the capability is activated or disabled  

This guide provides everything a developer needs to implement, extend, or integrate with Frank’s exception handling system.

---

# 1. Core Abstractions (Frank.Abstractions.ExceptionHandling)

These abstractions define the *contract* for exception handling across all transports (API, workers, CLI, etc.).  
They contain **no ASP.NET Core dependencies**.

---

## 1.1 IExceptionHandler

```csharp
[AutoRegister(ServiceLifetime.Singleton)]
public interface IExceptionHandler
{
    bool CanHandle(Exception exception);

    IErrorCode GetErrorCode(Exception exception);

    ProblemDetails CreateProblemDetails(Exception exception);
}
```

### Responsibilities

- **CanHandle**  
  Determines whether this handler is responsible for the exception.  
  Must be:
  - pure  
  - fast  
  - deterministic  
  - side‑effect free  

- **GetErrorCode**  
  Maps the exception to a stable, domain‑meaningful `IErrorCode`.

- **CreateProblemDetails**  
  Produces a **transport‑agnostic** `ProblemDetails` object.

### What handlers do *not* do

- They do **not** write HTTP responses.  
- They do **not** log.  
- They do **not** depend on ASP.NET Core.  
- They do **not** mutate state.  

Handlers classify and shape errors — nothing more.

---

## 1.2 ExceptionHandlerAttribute

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExceptionHandlerAttribute : Attribute
{
    public int Order { get; }
    public ExceptionHandlerAttribute(int order) => Order = order;
}
```

### Purpose

Defines deterministic ordering of handlers.

### Rules

- Lower `Order` runs first.
- Missing attribute defaults to `Order = 1000`.
- A catch‑all handler must exist at a high order.

---

## 1.3 ProblemDetails (Transport‑Agnostic)

```csharp
public class ProblemDetails
{
    public string Title { get; set; } = default!;
    public string Detail { get; set; } = default!;
    public int? Status { get; set; }
    public string Type { get; set; } = default!;
    public Dictionary<string, string[]>? Errors { get; set; }
}
```

### Purpose

A consistent error payload for:

- HTTP APIs  
- background workers  
- message processors  
- CLI tools  

The API layer later converts this into ASP.NET Core’s `ProblemDetails`.

---

# 2. API Runtime Layer (Frank.Api.ExceptionHandling)

This layer integrates the abstractions into the HTTP pipeline.

---

## 2.1 ExceptionHandlingRegistry

```csharp
public sealed class ExceptionHandlingRegistry
{
    private readonly IReadOnlyList<IExceptionHandler> _handlers;

    public ExceptionHandlingRegistry(IEnumerable<IExceptionHandler> handlers)
    {
        _handlers = handlers
            .OrderBy(GetOrder)
            .ToArray();
    }

    public IExceptionHandler Resolve(Exception exception)
        => _handlers.First(h => h.CanHandle(exception));

    private static int GetOrder(IExceptionHandler handler)
        => handler.GetType()
            .GetCustomAttribute<ExceptionHandlerAttribute>()?.Order
            ?? 1000;
}
```

### Responsibilities

- Sort handlers by `Order`
- Select the first handler whose `CanHandle` returns true
- Guarantee deterministic resolution

### Developer implications

- You must ensure a **catch‑all handler** exists.
- Ordering determines which handler wins when multiple can handle the same exception.

---

## 2.2 ExceptionHandlingOptions

```csharp
public sealed class ExceptionHandlingOptions
{
    public bool IncludeExceptionDetails { get; set; } = false;
    public bool IncludeErrorCode { get; set; } = true;
    public bool LogUnhandledExceptions { get; set; } = true;
}
```

### Behavior

- **IncludeExceptionDetails**  
  - true → include message + stack trace  
  - false → hide sensitive details  

- **IncludeErrorCode**  
  - true → include stable error code  
  - false → omit error code  

- **LogUnhandledExceptions**  
  - true → log exceptions that reach the top‑level handler  
  - false → suppress logging  

---

## 2.3 API Middleware (Conceptual)

The API layer typically wraps the request pipeline:

```csharp
try
{
    await next(context);
}
catch (Exception ex)
{
    var handler = registry.Resolve(ex);
    var problem = handler.CreateProblemDetails(ex);
    var code = handler.GetErrorCode(ex);

    // apply options
    // convert to ASP.NET Core ProblemDetails
    // write HTTP response
    // log if required
}
```

---

# 3. Capability Governance Layer

To support capability‑level opt‑out, the registration engine must treat ExceptionHandling as a **capability module**, not just a set of types.

### Why governance matters

- Some deployments may want to disable exception handling entirely.
- Some may want to replace it with a custom implementation.
- Some may want to disable specific handlers.

### Required governance behaviors

- Ability to disable the entire capability  
- Ability to disable specific handlers  
- Ability to override default handlers  
- Ability to replace the registry  
- Ability to replace the middleware  

This requires the registration engine to evolve into a **capability‑aware module loader**.

---

# 4. Implementing a New Exception Handler

Steps:

1. Implement `IExceptionHandler`
2. Add `[ExceptionHandler(order)]`
3. Register it (auto‑registration handles this)
4. Ensure `CanHandle` is precise and cheap
5. Ensure `CreateProblemDetails` produces safe output
6. Ensure `GetErrorCode` returns a stable code

Example:

```csharp
[ExceptionHandler(100)]
public sealed class DomainExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
        => exception is DomainException;

    public IErrorCode GetErrorCode(Exception exception)
        => DomainErrorCodes.InvalidOperation;

    public ProblemDetails CreateProblemDetails(Exception exception)
        => new()
        {
            Title = "Domain Error",
            Detail = exception.Message,
            Status = 400,
            Type = "https://errors.frank.dev/domain-error"
        };
}
```

---

# 5. Invariants

Developers must enforce:

- `CanHandle` must never throw  
- `CreateProblemDetails` must never return null  
- `GetErrorCode` must return a stable code  
- Handlers must be deterministic  
- Handlers must not mutate shared state  
- Registry must always resolve a handler  
- Options must be respected  

---

# 6. Anti‑Patterns

Never:

- throw inside `CanHandle`  
- use randomness or timestamps in handlers  
- leak sensitive details when `IncludeExceptionDetails` is false  
- rely on handler registration order instead of `Order`  
- omit a fallback handler  
- log inside handlers  
- write HTTP responses inside handlers  

---

# 7. Summary

The ExceptionHandling capability in Frank is:

- **Contract‑driven** — handlers classify and shape errors  
- **Deterministic** — ordering and resolution are predictable  
- **Transport‑agnostic** — ProblemDetails works everywhere  
- **Governed** — capability can be enabled/disabled  
- **Extensible** — new handlers are easy to add  
- **Safe** — no mutation, no side effects, no surprises  

This unified Developer Guide covers everything needed to implement, extend, or integrate with Frank’s exception handling system.
