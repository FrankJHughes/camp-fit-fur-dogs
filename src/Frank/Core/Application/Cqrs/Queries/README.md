# Queries

The **Queries** folder contains the application‑layer infrastructure for the
read‑side of the CQRS pattern. Unlike the abstractions layer, this folder
provides **concrete behavior**: validation orchestration, handler resolution,
and dependency‑injection registration for query handlers.

Queries represent read‑only operations. This subsystem ensures they are executed
consistently, predictably, and with proper validation.

---

## Components

### QueryDispatcher

The `QueryDispatcher` is responsible for orchestrating query execution:

1. **Validator discovery and execution**  
   All `IValidator<TQuery>` instances registered in DI are resolved and run
   before the query handler executes.

2. **Handler resolution**  
   The dispatcher resolves the correct handler based on the query type:
   - `IQueryHandler<TQuery, TResponse>`

3. **Handler invocation**  
   The dispatcher executes the handler and returns the result.

The dispatcher enforces a consistent read‑side pipeline across vertical slices.

---

### ServiceCollectionExtensions

Provides DI registration helpers for the query pipeline:

- `AddFrankCoreApplicationCqrsQueryDispatcher()`  
  Registers the `QueryDispatcher`.

- `AddFrankCoreApplicationCqrsQueries()`  
  Performs assembly scanning and registers query handlers based on:
  - interfaces decorated with `[Registration]`
  - interfaces matching `IQueryHandler<,>`
  - implementations that satisfy those interfaces

This enables automatic handler discovery across slices without manual wiring.

---

## Design Principles

- **Validation-first**  
  Queries are validated before handler execution.

- **Read-only semantics**  
  Queries must not mutate state.

- **Attribute-driven discovery**  
  Only handler interfaces marked with `[Registration]` are included.

- **Loose coupling**  
  Handlers are resolved via DI, not manually instantiated.

- **Vertical-slice alignment**  
  Queries and handlers are discovered per slice.

- **Testability**  
  Handlers are isolated and easy to test independently.

---

## How This Folder Fits Into the Application

This folder provides the *runtime mechanics* of query execution. It is used by:

- API endpoints that retrieve data  
- domain read workflows  
- dashboards and reporting flows  
- background processes that perform read operations  

Slices define the queries and handlers; this folder ensures they are executed
consistently and predictably.

---

## Typical Usage

```csharp
var result = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id), ct);
```

---

## Notes

- Validators are optional; queries without validators simply execute.
- Handlers must be registered via discovery or manual DI.
- This folder contains **only** dispatcher + registration logic — not query
  definitions or handler implementations.

---
