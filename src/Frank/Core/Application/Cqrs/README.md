# Cqrs

The **Cqrs** folder contains the application‑layer infrastructure for the
Command–Query Responsibility Segregation (CQRS) pattern.  
This folder does **not** define CQRS abstractions — those live in the
Abstractions layer. Instead, it provides the **runtime mechanics** that execute
commands and queries:

- validation orchestration  
- handler resolution  
- dependency‑injection registration  
- assembly‑based handler discovery  

The Cqrs subsystem ensures that both commands (write operations) and queries
(read operations) follow a consistent, predictable, slice‑aligned execution
pipeline.

---

## Folder Structure

```
Cqrs/
 ├── Commands/
 │    ├── CommandDispatcher.cs
 │    └── ServiceCollectionExtensions.cs
 └── Queries/
      ├── QueryDispatcher.cs
      └── ServiceCollectionExtensions.cs
```

Each subfolder contains the dispatcher and DI registration logic for its side of
the CQRS pattern.

---

## Commands

The **Commands** subsystem handles *write‑side* operations.

### CommandDispatcher
Orchestrates command execution:

1. Discovers and runs validators  
2. Resolves the correct `ICommandHandler<>` or `ICommandHandler<,>`  
3. Executes the handler  

Supports both response‑returning and fire‑and‑forget commands.

### ServiceCollectionExtensions
Registers:

- the `CommandDispatcher`
- command handlers discovered via:
  - `[Registration]` attribute
  - `ICommandHandler<>` / `ICommandHandler<,>` interfaces

This enables automatic handler discovery across vertical slices.

---

## Queries

The **Queries** subsystem handles *read‑side* operations.

### QueryDispatcher
Orchestrates query execution:

1. Discovers and runs validators  
2. Resolves the correct `IQueryHandler<,>`  
3. Executes the handler and returns the result  

Queries must be read‑only and side‑effect‑free.

### ServiceCollectionExtensions
Registers:

- the `QueryDispatcher`
- query handlers discovered via:
  - `[Registration]` attribute
  - `IQueryHandler<,>` interfaces

Ensures consistent read‑side behavior across slices.

---

## Design Principles

- **Validation-first**  
  Both commands and queries run validators before handler execution.

- **Explicit contracts**  
  Handlers are resolved strictly by their interface type.

- **Attribute-driven discovery**  
  Only interfaces marked with `[Registration]` are included.

- **Vertical-slice alignment**  
  Each slice defines its own commands, queries, and handlers.

- **Infrastructure-owned mechanics**  
  Dispatchers and DI registration live here, not in slices.

- **Testability**  
  Handlers are isolated and easy to test independently.

---

## How This Folder Fits Into the Application

The Cqrs folder provides the *execution engine* for:

- API endpoints  
- domain orchestration  
- background jobs  
- automation flows  

Slices define the commands, queries, and handlers.  
This folder ensures they are executed consistently and predictably.

---

## Typical Usage

```csharp
var result = await _queryDispatcher.DispatchAsync(new GetOrderQuery(id), ct);
await _commandDispatcher.DispatchAsync(new UpdateOrderStatusCommand(id), ct);
```

---

## Notes

- Validators are optional; absence of validators does not block execution.
- Handlers must be registered via discovery or manual DI.
- This folder contains **only** dispatchers + registration logic — not handler
  implementations or abstractions.

---
