# UnitOfWork

The **UnitOfWork** folder contains the application‑layer abstraction and
Entity Framework Core implementation of the Unit of Work pattern.  
This subsystem defines the commit boundary for persistence operations and
provides a consistent way for vertical slices to save changes without exposing
EF Core directly.

The folder contains the application contract (`IUnitOfWork`) in Abstractions and
the EF Core implementation in this folder.

---

## Components

### EntityFrameworkCoreUnitOfWorkBase\<TContext\>

The `EntityFrameworkCoreUnitOfWorkBase` class provides a base implementation of
the Unit of Work pattern for EF Core `DbContext` instances.

#### Responsibilities

- **Commit boundary**  
  Wraps `DbContext.SaveChangesAsync` to provide a single atomic commit operation.

- **Abstraction over EF Core**  
  Vertical slices depend on `IUnitOfWork`, not on EF Core directly.

- **Extensibility**  
  Derived implementations may add:
  - domain event dispatching  
  - audit logging  
  - pre‑commit validation  
  - transaction management  

This keeps the base implementation simple while allowing slices or infrastructure
layers to extend behavior as needed.

---

## Design Principles

- **Separation of concerns**  
  EF Core details stay in infrastructure; slices depend only on `IUnitOfWork`.

- **Atomicity**  
  All tracked changes are committed together.

- **Minimalism**  
  The base class performs only the commit; additional behavior is opt‑in.

- **Testability**  
  `IUnitOfWork` can be mocked or replaced in tests.

---

## How This Folder Fits Into the Architecture

The Unit of Work subsystem provides the persistence commit boundary for:

- vertical slice command handlers  
- domain services  
- application workflows  
- background jobs  

Slices do not call `DbContext.SaveChangesAsync` directly — they call
`IUnitOfWork.CommitAsync`, keeping persistence concerns isolated.

---

## Typical Usage

```csharp
public sealed class OwnerUnitOfWork : EntityFrameworkCoreUnitOfWorkBase<AppDbContext>
{
    public OwnerUnitOfWork(AppDbContext dbContext) : base(dbContext) { }
}
```

And inside a command handler:

```csharp
await _unitOfWork.CommitAsync(ct);
```

---

## Notes

- This folder contains **only** the EF Core implementation — not the abstraction.
- The abstraction (`IUnitOfWork`) lives in `Application/Abstractions/UnitOfWork`.
- Additional behaviors should be implemented in derived classes, not in the base.

---
