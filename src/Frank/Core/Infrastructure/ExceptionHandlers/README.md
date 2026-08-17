# ExceptionHandlers

The **ExceptionHandlers** folder contains the infrastructure‑level components
responsible for discovering, ordering, and resolving exception handlers across
the application.  
This subsystem provides deterministic exception‑handling behavior, unified
registration, and environment‑specific configuration options.

Exception handling is treated as an extensible pipeline: slices define handlers,
infrastructure discovers and registers them, and the registry resolves the
correct handler at runtime.

---

## Components

### ExceptionHandlerRegistry

`ExceptionHandlerRegistry` provides ordered resolution of
`IExceptionHandler` implementations.

#### Responsibilities

- Orders handlers using `ExceptionHandlerAttribute.Order`.
- Resolves the first handler whose `CanHandle` method returns `true`.
- Ensures deterministic exception‑handling behavior.
- Supports slice‑specific handlers without global coupling.

Handlers without an `ExceptionHandlerAttribute` default to order `1000`,
placing them at the end of the chain.

---

### ExceptionHandlingOptions

`ExceptionHandlingOptions` defines configuration flags that control how
exceptions are exposed and logged.

#### Options

- **IncludeExceptionDetails**  
  Whether to include message + stack trace in `ProblemDetails`.  
  Should be enabled only in Development.

- **IncludeErrorCode**  
  Whether to include the handler’s error code in `ProblemDetails`.

- **LogUnhandledExceptions**  
  Whether unhandled exceptions should be logged.

These options allow environments (Development, Staging, Production) to adjust
diagnostic verbosity.

---

### ServiceCollectionExtensions

Provides DI registration for the exception‑handling subsystem.

#### Responsibilities

- Registers `ExceptionHandlerRegistry` as a singleton.
- Discovers `IExceptionHandler` interfaces decorated with `[Registration]`.
- Discovers implementations that explicitly implement `IExceptionHandler`.
- Allows callers to customize discovery via `DiscoveryOptions`.
- Uses the unified `Orchestrator` pipeline for registration.

This ensures slice‑controlled, attribute‑driven discovery rather than
auto‑registering every handler in the assembly.

---

## Design Principles

- **Explicit discovery**  
  Only handlers decorated with `[Registration]` are included.

- **Deterministic ordering**  
  `ExceptionHandlerAttribute.Order` defines resolution priority.

- **Separation of concerns**  
  Slices define handlers; infrastructure discovers and registers them.

- **Environment‑aware behavior**  
  Options allow safe diagnostic output in Development and secure output in Production.

- **Unified registration**  
  All discovery flows through the `Orchestrator` for consistency.

---

## How This Folder Fits Into the Architecture

The ExceptionHandlers subsystem supports:

- API exception translation  
- domain exception mapping  
- infrastructure exception handling  
- consistent `ProblemDetails` output  
- operational logging of unhandled exceptions  

Vertical slices define their own handlers; this folder provides the mechanism
that discovers, orders, and resolves them.

---

## Typical Usage

Registering handlers:

```csharp
services.AddFrankCoreInfrastructureExceptionHandlers(
    assembliesToSearch: new[] { typeof(MySlice.AssemblyMarker).Assembly }
);
```

Resolving a handler:

```csharp
var handler = _registry.Resolve(exception);
var problemDetails = handler.Handle(exception);
```

---

## Notes

- This folder contains **only** the infrastructure components — not the handlers themselves.
- Slice‑specific handlers should live in their respective vertical slice folders.
- All handlers must implement `IExceptionHandler` and be decorated with `[Registration]`.
- Ordering is explicit; no implicit priority rules exist.

---
