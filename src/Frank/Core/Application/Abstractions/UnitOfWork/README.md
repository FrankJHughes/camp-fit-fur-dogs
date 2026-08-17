# UnitOfWork

The **UnitOfWork** folder contains abstractions that define transactional
boundaries within the application. These interfaces ensure that changes made
through repositories or other persistence components are committed atomically,
providing consistency and reliability across write operations.

The unit of work pattern centralizes commit logic, prevents partial writes, and
provides a clear transactional checkpoint for application workflows.

---

## Purpose

The Unit of Work subsystem exists to:

- coordinate changes across multiple repositories  
- ensure atomic commits (all changes succeed or none do)  
- prevent partial or inconsistent writes  
- provide a single commit point for application workflows  
- abstract underlying transaction mechanisms (EF Core, Dapper, custom stores)  
- simplify error handling and rollback behavior  

This subsystem is foundational for maintaining data integrity across vertical
slices and infrastructure boundaries.

---

## Components

### IUnitOfWork

Represents a transactional boundary for committing changes.

```csharp
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
```

Responsibilities:

- wrap a database transaction or equivalent atomic operation  
- coordinate changes across repositories  
- commit all pending changes  
- return the number of affected records (implementation‑specific)  

---

## Design Principles

- **Atomicity**  
  All changes within a unit of work are committed together.

- **Consistency**  
  The system remains in a valid state before and after the commit.

- **Isolation**  
  Work performed inside a unit of work is isolated from other operations.

- **Abstraction**  
  Application code depends on the abstraction, not the underlying database or
  transaction mechanism.

- **Separation of concerns**  
  Repositories handle data access; the unit of work handles commit semantics.

---

## How Unit of Work Fits Into the Application

Unit of Work is typically used in:

- command handlers  
- domain orchestration  
- transactional workflows  
- multi‑repository operations  
- persistence boundaries  

It ensures that write operations across multiple components are coordinated and
committed safely.

---

## Typical Usage Pattern

```csharp
var result = await _handler.HandleAsync(request);

await _unitOfWork.CommitAsync();
```

The handler performs work, and the unit of work commits all changes at the end
of the workflow.

---

## Notes

- Implementations are provided by the infrastructure layer.  
- Application code should never directly manage transactions.  
- Unit of Work may be scoped per request, per command, or per operation,
  depending on architectural needs.

---
