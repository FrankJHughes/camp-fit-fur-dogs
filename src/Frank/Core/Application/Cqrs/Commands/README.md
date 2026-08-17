# Commands

The **Commands** folder contains the application‑layer infrastructure that powers
the CQRS command execution pipeline. This folder does not define command or
handler abstractions — those live in the Abstractions layer. Instead, it
provides the concrete behavior required to:

- validate commands,
- resolve handlers,
- execute handlers,
- and register command‑related services into the DI container.

This subsystem forms the operational backbone of the write‑side CQRS workflow.

---

## Components

### CommandDispatcher

The `CommandDispatcher` is responsible for orchestrating command execution:

1. **Validator discovery and execution**  
   All `IValidator<TCommand>` instances registered in DI are resolved and run
   before the command is handled.

2. **Handler resolution**  
   The dispatcher resolves the correct handler based on the command type:
   - `ICommandHandler<TCommand, TResponse>`  
   - `ICommandHandler<TCommand>`

3. **Handler invocation**  
   The dispatcher executes the handler and returns (or awaits) the result.

The dispatcher supports both response‑returning and fire‑and‑forget commands.

---

### ServiceCollectionExtensions

Provides DI registration helpers for the command pipeline:

- `AddFrankCoreApplicationCqrsCommandDispatcher()`  
  Registers the `CommandDispatcher`.

- `AddFrankCoreApplicationCqrsCommands()`  
  Performs assembly scanning and registers command handlers based on:
  - interfaces decorated with `[Registration]`
  - interfaces matching `ICommandHandler<>` or `ICommandHandler<,>`
  - implementations that satisfy those interfaces

This enables automatic handler discovery across vertical slices.

---

## Design Principles

- **Validation-first**  
  Commands are validated before handler execution.

- **Explicit handler resolution**  
  Handlers are resolved by type, ensuring clear ownership of command behavior.

- **Attribute-driven discovery**  
  Only handler interfaces marked with `[Registration]` are included.

- **Infrastructure-owned**  
  The application layer provides the dispatcher and registration logic; slices
  provide the handlers.

- **Vertical-slice alignment**  
  Commands and handlers are discovered per slice, not globally.

---

## How This Folder Fits Into the Application

This folder provides the *runtime mechanics* of command execution. It is used by:

- API endpoints issuing commands  
- background jobs  
- domain orchestration  
- automation workflows  

Slices define the commands and handlers; this folder ensures they are executed
consistently and predictably.

---

## Typical Usage

```csharp
var result = await _dispatcher.DispatchAsync(new CreateOrderCommand(...), ct);
```

Fire‑and‑forget:

```csharp
await _dispatcher.DispatchAsync(new MarkOrderAsShippedCommand(...), ct);
```

---

## Notes

- Validators are optional; commands without validators simply execute.
- Handlers must be registered via discovery or manual DI.
- This folder contains **only** dispatcher + registration logic — not command
  definitions or handler implementations.

---
