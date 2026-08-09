# CQRS

The **CQRS** (Command Query Responsibility Segregation) folder contains the core abstractions that define how the application separates **write operations** (commands) from **read operations** (queries). This separation improves clarity, scalability, and testability by ensuring that each operation type has a single, well‑defined responsibility.

This folder provides the foundational interfaces for both sides of the CQRS pipeline, including commands, queries, handlers, and dispatchers.

---

## Why CQRS?

CQRS enforces a clear distinction between:

- **Commands** — imperative, state‑changing operations  
- **Queries** — descriptive, side‑effect‑free read operations  

This separation leads to:

- **Cleaner architecture** — each operation has a single purpose  
- **Better scalability** — read and write paths can evolve independently  
- **Improved testability** — handlers are isolated and deterministic  
- **Explicit intent** — every operation is modeled as a clear request  

CQRS is especially useful in systems with complex domain logic, multiple read models, or high performance requirements.

---

## Structure

The CQRS folder is organized into two subfolders:

### Commands
Contains abstractions for write‑side operations:

- `ICommand`
- `ICommand<TResponse>`
- `ICommandHandler<TCommand>`
- `ICommandHandler<TCommand, TResponse>`
- `ICommandDispatcher`

Commands express intent to change application state, and handlers perform the associated work.

### Queries
Contains abstractions for read‑side operations:

- `IQuery<TResponse>`
- `IQueryHandler<TQuery, TResponse>`
- `IQueryDispatcher`

Queries retrieve information without modifying state, and handlers compute or fetch the requested data.

---

## Design Principles

- **Strong typing**  
  Commands and queries enforce typed request/response flows.

- **Separation of concerns**  
  Commands mutate state; queries read state; dispatchers coordinate execution.

- **Automatic registration**  
  Handlers use the `Registration` attribute to ensure consistent DI configuration.

- **Predictable behavior**  
  Queries are side‑effect‑free; commands encapsulate all write logic.

- **Testability**  
  Each handler is isolated, deterministic, and easy to mock.

---

## How CQRS Fits Into the Application

CQRS forms the backbone of the application’s interaction model:

- Application services issue commands and queries.  
- Dispatchers locate and invoke the correct handlers.  
- Handlers perform the actual work (write or read).  
- The system remains modular, explicit, and easy to reason about.

CQRS also integrates cleanly with:

- vertical slice architecture  
- domain‑driven design  
- mediator‑style orchestration  
- event‑driven workflows  

---
