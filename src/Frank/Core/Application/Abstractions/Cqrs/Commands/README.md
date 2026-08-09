# Commands

The **Commands** folder contains the abstractions that define the write‑side of the CQRS (Command Query Responsibility Segregation) pattern. Commands represent intentions to change application state, and command handlers encapsulate the logic required to perform those changes.

This folder provides the core interfaces used throughout the command pipeline: commands, command handlers, and the command dispatcher.

---

## Purpose

Commands model *actions* the system performs. They are:

- **imperative** — they express intent (“do this”)
- **state‑changing** — they modify the system in some way
- **explicit** — each command represents a single, well‑defined operation

By separating commands from queries, the application maintains a clear distinction between:

- **write operations** (commands)
- **read operations** (queries)

This separation improves clarity, testability, and scalability.

---

## Components

### ICommand
Represents a command that performs an action but does not return a value.

```csharp
public interface ICommand { }
```

Used for operations where the caller does not need a result beyond completion.

---

### ICommand<TResponse>
Represents a command that produces a typed response.

```csharp
public interface ICommand<TResponse> { }
```

Used when the caller needs a result, such as an identifier, a status object, or a computed value.

---

### ICommandHandler<TCommand>
Handles commands that do not return a value.

```csharp
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken ct);
}
```

Each handler is automatically registered via the `Registration` attribute with:

- scoped lifetime  
- concrete type registration  
- exactly one implementation  

---

### ICommandHandler<TCommand, TResponse>
Handles commands that return a typed response.

```csharp
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
}
```

Also automatically registered with the same DI rules.

---

### ICommandDispatcher
Coordinates command execution by locating and invoking the correct handler.

```csharp
public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct);
    Task DispatchAsync(ICommand command, CancellationToken ct);
}
```

The dispatcher decouples command invocation from handler resolution, improving testability and inversion of control.

---

## Design Principles

- **Strong typing**  
  Commands and handlers enforce type‑safe request/response flows.

- **Separation of concerns**  
  Commands express intent; handlers perform work; the dispatcher coordinates execution.

- **Automatic registration**  
  Handlers are discovered and registered via the `Registration` subsystem.

- **Testability**  
  Commands and handlers are easy to mock, isolate, and validate.
