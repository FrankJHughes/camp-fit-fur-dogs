# Frank – Guides – Developer – Command Dispatcher

The Command Dispatcher capability provides a **centralized, validated, DI‑driven command execution pipeline**.  
It ensures that:

- commands are validated before execution  
- handlers are resolved deterministically  
- commands execute through a consistent, predictable mechanism  
- validation failures surface as exceptions  
- handler resolution is type‑safe and DI‑driven  

This guide documents the architecture, invariants, and extension points for developers implementing or integrating with the Command Dispatcher.

---

# 1. Core Purpose

The Command Dispatcher provides:

- a single entry point for executing commands  
- automatic FluentValidation integration  
- automatic handler resolution  
- support for commands with and without return values  
- scoped lifetime semantics  

This capability enforces a clean separation between:

- **command intent**  
- **validation**  
- **execution**  

---

# 2. Command Abstractions

## 2.1 `ICommand` (no response)

```csharp
public interface ICommand { }
```

Represents a fire‑and‑forget command.

---

## 2.2 `ICommand<TResponse>`

```csharp
public interface ICommand<TResponse> { }
```

Represents a command that returns a value.

---

# 3. Command Handlers

Handlers are registered through **Frank’s Registration Engine**, using `RegistrationAttribute` applied to the handler interfaces.

## 3.1 `ICommandHandler<TCommand>`

```csharp
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken ct);
}
```

For commands with **no return value**.

---

## 3.2 `ICommandHandler<TCommand, TResponse>`

```csharp
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
}
```

For commands that **produce a result**.

---

# 4. Command Dispatcher

```csharp
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _provider;

    public CommandDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    ...
}
```

The dispatcher is responsible for:

1. Running validators  
2. Resolving the correct handler  
3. Executing the handler  

---

# 5. Validation Pipeline

The dispatcher automatically discovers and executes all validators for a command.

### How validators are resolved

```csharp
var validatorType = typeof(IValidator<>).MakeGenericType(commandType);
var validators = _provider.GetServices(validatorType);
```

### Validation behavior

- All validators run  
- A `ValidationContext` is created dynamically  
- If any validator fails → `ValidationException` is thrown  
- Command execution stops immediately  

### Developer requirements

- Validators must be registered in DI  
- Validators must target the exact command type  

---

# 6. Handler Resolution

Handlers are resolved using DI:

```csharp
var handlerType = typeof(ICommandHandler<,>)
    .MakeGenericType(command.GetType(), typeof(TResponse));

var handler = _provider.GetRequiredService(handlerType);
```

### Invariants

- Exactly one handler must exist  
- Handlers must be registered via Frank.Registration  
- Handlers must be scoped  

---

# 7. Execution Pipeline

### Commands with return values

```csharp
return await ((dynamic)handler).HandleAsync((dynamic)command, ct);
```

### Commands without return values

```csharp
await ((dynamic)handler).HandleAsync((dynamic)command, ct);
```

### Notes

- Dynamic dispatch avoids reflection‑heavy invocation  
- Exceptions thrown by handlers propagate naturally  
- Cancellation tokens are passed through end‑to‑end  

---

# 8. DI Registration

The dispatcher is registered via:

```csharp
[Registration(ServiceLifetime.Scoped)]
public interface ICommandDispatcher
```

Handlers are registered via:

```csharp
[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ICommandHandler<...>
```

### Implications

- Dispatcher is scoped  
- Handlers are scoped  
- Only one handler per command is allowed  
- Concrete handler types are registered automatically  

---

# 9. Developer Invariants

Developers must ensure:

- Commands are **POCOs** with no side effects  
- Validators are pure and deterministic  
- Handlers do not perform validation  
- Handlers do not depend on transient state outside DI  
- Handlers do not throw validation exceptions  
- Handlers return the correct type  

---

# 10. Anti‑Patterns

Avoid:

- Registering multiple handlers for the same command  
- Performing validation inside handlers  
- Throwing non‑`ValidationException` for validation failures  
- Using static/global state inside handlers  
- Creating commands with mutable public fields  
- Returning `null` from handlers that produce a result  

---

# 11. Summary

The Command Dispatcher provides:

- a unified command execution pipeline  
- automatic validation  
- automatic handler resolution  
- deterministic behavior  
- clean separation of concerns  

As a developer:

- You define commands  
- You implement handlers  
- You optionally add validators  
- The dispatcher handles everything else  

This Developer Guide documents everything needed to extend or integrate with the Command Dispatcher capability.
