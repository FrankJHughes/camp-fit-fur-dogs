# ADR-0011: CQRS Command/Query Pipelines

| Field     | Value                              |
|-----------|------------------------------------|
| Status    | Accepted                           |
| Date      | 2026-04-11                         |
| Deciders  | Frank Hughes                       |

## Context

Camp Fit Fur Dogs uses a DDD layered architecture (ADR-0002) where the
Application layer mediates between the API and Domain layers. As the
product grew from a single CreateCustomer command to multiple slices
(CreateCustomer, RegisterDog, GetDogProfile), the team needed a
consistent pattern for dispatching operations to their handlers.

The requirements for the dispatch pattern are:

- Enforce compile-time separation between commands (write) and queries
  (read) at the type level.
- Each operation is a self-contained unit: one command/query, one
  handler, one folder.
- The API layer must not reference concrete handler types directly.
- Pipeline behaviors (validation, logging, transactions) must be
  addable later without modifying existing handlers.
- The pattern must be simple enough that adding a new slice requires
  only adding files in the slice folder.

## Decision

We will use a **hand-rolled CQRS pattern** with explicit command/query
separation, generic handler interfaces, and dispatcher-based resolution.

### Pattern Structure

The pattern consists of six interfaces in `Application/Abstractions/`:

**Commands (write side):**

- `ICommand<TResponse>` — Marker interface. A command carries input
  data and declares its response type.
- `ICommandHandler<TCommand, TResponse>` — Handles one command type.
  Single method: `Task<TResponse> Handle(TCommand, CancellationToken)`.
- `ICommandDispatcher` — Resolves and invokes the correct handler for
  any `ICommand<TResponse>`.

**Queries (read side):**

- `IQuery<TResponse>` — Marker interface. A query carries filter/lookup
  criteria and declares its response type.
- `IQueryHandler<TQuery, TResponse>` — Handles one query type. Single
  method: `Task<TResponse> Handle(TQuery, CancellationToken)`.
- `IQueryDispatcher` — Resolves and invokes the correct handler for any
  `IQuery<TResponse>`.

The design is intentionally symmetric: commands and queries share the
same structural pattern but are distinct type hierarchies, enforcing
read/write separation at compile time.

### Handler Discovery and Registration

**Current state:** Handlers are registered manually in
`DependencyInjection.cs`. Each handler requires an explicit
`services.AddTransient<ICommandHandler<TCommand, TResponse>,
ConcreteHandler>()` call. Dispatchers are registered as scoped services.

**Known limitation:** This manual registration means every new slice
requires editing `DependencyInjection.cs` — violating the goal that
adding a slice requires only adding files in the slice folder. US-079
(Convention-Based Auto-Registration) will address this by introducing
assembly-scanning conventions that discover handlers automatically.

### Dispatcher Implementation

**Current state:** Both `CommandDispatcher` and `QueryDispatcher` use
reflection and `dynamic` to resolve handlers at runtime:

1. Build the closed generic handler type using `MakeGenericType`.
2. Resolve the handler from `IServiceProvider.GetRequiredService`.
3. Invoke `Handle` via `dynamic` cast.

**Known limitation:** The `dynamic` dispatch bypasses compile-time
safety and has a performance cost. Both dispatchers carry a TODO:
"Avoid using reflection and dynamic in production code." US-079 will
replace this mechanism with a compile-time-safe approach as part of
the convention-based registration refactor.

### Pipeline Behaviors

**Current state:** No pipeline behaviors exist. Handlers are invoked
directly by the dispatcher with no interception points.

**Future direction:** When cross-cutting concerns are needed (validation,
logging, transaction management), the dispatcher becomes the natural
insertion point. Behaviors will be added as decorator layers around
handler invocation — not as middleware in the API layer. This keeps
cross-cutting logic in the Application layer where it belongs.

The proving slice (US-084) will establish the first real handler flow
through the frontend. Pipeline behaviors will be introduced only when
a concrete need arises, not speculatively.

### Folder Conventions

Each operation lives in its own folder under its aggregate:

```
Application/
  {Aggregate}/
    {OperationName}/
      {OperationName}Command.cs   — or Query.cs
      {OperationName}Handler.cs
      {OperationName}Response.cs  — queries only
```

Example (current codebase):

```
Application/
  Customers/
    CreateCustomer/
      CreateCustomerCommand.cs
      CreateCustomerHandler.cs
  Dogs/
    RegisterDog/
      RegisterDogCommand.cs
      RegisterDogHandler.cs
    GetDogProfile/
      GetDogProfileQuery.cs
      GetDogProfileHandler.cs
      DogProfileResponse.cs
```

One folder = one slice = one command or one query.

### Alternatives Considered

| Alternative          | Strengths                                           | Why Not                                                              |
|----------------------|-----------------------------------------------------|----------------------------------------------------------------------|
| **MediatR**          | Mature; built-in pipeline behaviors; large community | Adds a dependency for what is currently six interfaces; uses reflection internally; "magic" registration conflicts with explicit-is-better philosophy |
| **Wolverine**        | Convention-based; high performance; no marker interfaces needed | Heavy framework with its own runtime; overkill for current scope; ties the Application layer to a specific library |
| **No CQRS (direct service calls)** | Simpler; no dispatcher indirection | No compile-time read/write separation; API layer coupled to concrete handlers; no natural insertion point for pipeline behaviors |

### Why Hand-Rolled

- **Explicit over implicit** — Six interfaces, two dispatchers, zero
  third-party dependencies. Every line is visible and debuggable.
- **Minimal surface area** — The pattern adds only the abstractions
  needed today. Pipeline behaviors, assembly scanning, and
  compile-time-safe dispatch are deferred to when they are needed
  (US-079), not built speculatively.
- **No framework lock-in** — The abstractions are owned by the project.
  If MediatR or Wolverine becomes the right choice later, the migration
  is mechanical: swap the dispatcher implementation and registration,
  keep the handlers.
- **Aligned with DDD layering** — The dispatcher lives in the
  Application layer. The API layer depends only on the dispatcher
  interface and the command/query types. Domain and Infrastructure
  layers are unaware of the dispatch mechanism.

## Consequences

### Positive

- Compile-time read/write separation. A handler that accepts
  `ICommand<T>` cannot accidentally handle a query, and vice versa.
- Each slice is self-contained: command/query + handler + response in
  one folder. New slices do not modify existing code (after US-079).
- The API layer depends only on `ICommandDispatcher` /
  `IQueryDispatcher` — never on concrete handlers. Coupling is minimal.
- Pipeline behaviors can be added later by decorating the dispatcher
  without changing any handler.
- Zero third-party dependencies for the dispatch pattern itself.

### Negative

- Manual handler registration in `DependencyInjection.cs` until US-079
  ships. Every new slice requires one edit to a shared file.
- Reflection + `dynamic` in dispatchers until US-079 replaces the
  mechanism. This is a known technical debt item with a planned fix.
- No built-in pipeline behaviors. Validation, logging, and transaction
  management must be built when needed. This is a conscious deferral,
  not an oversight.

### Neutral

- The pattern is simple enough that contributors unfamiliar with CQRS
  can follow it by reading one example slice.
- The six interfaces are stable — they have not changed since
  introduction and are unlikely to change. The dispatcher
  implementations will change (US-079), but the interfaces will not.
- The folder convention is enforced by example and code review today.
  US-079 will enforce it structurally via assembly scanning rules.
