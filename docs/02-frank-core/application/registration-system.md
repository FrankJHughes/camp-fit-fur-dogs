# Frank.Core.Application — Registration System

The registration system in `Frank.Core.Application` wires up CQRS handlers, validators, and application‑level services into the dependency injection container. It ensures that commands, queries, validators, and supporting services are automatically discovered and registered, keeping the application layer consistent and predictable across all products built on the Frank platform.

This document maps the registration subsystem under:

```
docs/02-frank-core/application
```

back to its implementation in:

```
src/Frank/Core
```

---

## Purpose

The registration system exists to:

- automatically discover command and query handlers  
- register handlers into the DI container  
- integrate FluentValidation validators  
- provide dispatcher services for orchestration  
- ensure consistent CQRS wiring across all assemblies  

This subsystem keeps the application layer maintainable and eliminates manual registration boilerplate.

---

## Command Dispatcher

The command dispatcher routes commands to their corresponding handlers:

```csharp
public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken ct);
    
    Task DispatchAsync(ICommand command, CancellationToken ct);
}
```

Responsibilities:

- resolve the correct handler from DI  
- execute pipeline behaviors (validation, logging, authorization)  
- invoke the handler  
- return a `Result<T>` or raw response  
- propagate immutable context  

---

## Handler Discovery

Command handlers implement:

```csharp
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
}
```

Handlers are discovered and registered automatically:

```csharp
services.AddCommandHandlers(typeof(AssemblyMarker).Assembly);
services.AddQueryHandlers(typeof(AssemblyMarker).Assembly);
```

This scanning process:

1. inspects the assembly for handler interfaces  
2. registers each handler with scoped lifetime  
3. ensures dispatchers can resolve handlers without manual wiring  

---

## Query Dispatcher

Queries follow the same pattern as commands:

```csharp
public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken ct);
}
```

Responsibilities:

- resolve the correct query handler  
- execute pipeline behaviors  
- return the query result  
- ensure read operations remain side‑effect‑free  

---

## Validator Integration

FluentValidation validators are automatically discovered:

```csharp
services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
```

The dispatcher:

- invokes validators before handler execution  
- aggregates validation errors  
- returns a failure `Result<T>` when validation fails  
- ensures domain invariants are protected before reaching handlers  

Validators apply to both commands and queries.

---

## How Registration Connects to the Broader Platform

The registration system collaborates with:

- **Frank.Core.Api**  
  Endpoints resolve dispatchers from DI.

- **Frank.Identity**  
  Dispatchers inject identity context into handlers.

- **Frank.Core.Domain**  
  Handlers invoke domain logic and raise domain events.

- **Frank.Core.EntityFrameworkCore**  
  Handlers commit changes through the Unit of Work.

This wiring ensures that every vertical slice is fully composed from API → Application → Domain → Persistence.

---

## Runtime Collaboration Points

Registration interacts with the runtime by:

- discovering handlers at startup  
- wiring validators into the pipeline  
- resolving dispatchers per request  
- ensuring handlers receive immutable context  
- enforcing transactional boundaries  

This keeps the CQRS model deterministic and consistent.

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Endpoint
    ↓
ICommandDispatcher / IQueryDispatcher
    ↓
Pipeline Behaviors (validation, logging, authorization)
    ↓
Application Handler
    ↓
Domain Aggregate
    ↓
Unit of Work Commit
    ↓
HTTP Response
```

The registration system ensures every step in this flow is correctly wired.

---

## Notes

Keep this document grounded in the actual Frank.Core.Application registration implementation.  
Whenever handler discovery, dispatcher behavior, or validator integration changes, update this section to reflect the current platform architecture.

