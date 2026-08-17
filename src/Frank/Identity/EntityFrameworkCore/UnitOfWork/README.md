# Identity EntityFrameworkCore — UnitOfWork

The **UnitOfWork** folder contains the transactional coordination layer for the
Identity EntityFrameworkCore subsystem.  
It ensures that multiple operations performed within a single vertical slice
(e.g., creating a session, revoking a session, updating identity data) are
committed atomically using EF Core’s DbContext.

This folder provides the Identity‑specific unit of work implementation and the
DI registration required to use it throughout the application.

---

## Purpose

The Unit of Work subsystem provides:

- A concrete EF Core–backed implementation of `IFrankIdentityUnitOfWork`
- A DI extension method for registering the unit of work
- A consistent transactional boundary for Identity vertical slices

It ensures that all Identity operations participate in a shared EF Core
transaction when appropriate.

---

## Files

### **FrankIdentityUnitOfWork**

Implements the Identity subsystem’s unit of work.

Responsibilities:

- Inherits from `EntityFrameworkCoreUnitOfWorkBase<FrankIdentityDbContext>`
- Provides commit and rollback behavior through the base class
- Coordinates EF Core transactions for Identity operations
- Implements `IFrankIdentityUnitOfWork` from the Application layer

Used during:

- Session creation
- Session revocation
- Identity‑related write operations
- Any vertical slice requiring atomic persistence

This class contains no additional logic because all transactional behavior is
provided by the shared base class.

---

### **ServiceCollectionExtensions**

Registers the Identity unit of work with the dependency injection container.

Responsibilities:

- Adds `IFrankIdentityUnitOfWork` as a scoped service
- Ensures the unit of work lifetime matches the DbContext lifetime
- Provides a single, consistent registration point for the Identity subsystem

Used during:

- Application startup (`Program.cs`)
- Host configuration

---

## Design Principles

The Unit of Work subsystem follows these architectural principles:

- **Transactional consistency**  
  All write operations within a vertical slice share the same DbContext and
  commit together.

- **Separation of concerns**  
  Transactional logic is isolated from domain and application logic.

- **Scoped lifetime alignment**  
  Unit of work lifetime matches EF Core DbContext lifetime.

- **Shared infrastructure**  
  Identity uses the same base unit of work abstraction as other subsystems,
  ensuring consistency across the codebase.

---

## How Unit of Work Is Used

1. **Application layer begins a vertical slice**  
   A handler receives `IFrankIdentityUnitOfWork`.

2. **Infrastructure operations occur**  
   Writers (e.g., `CreateSessionWriter`, `RevokeSessionWriter`) modify tracked
   entities.

3. **Commit**  
   The handler calls `unitOfWork.CommitAsync()` to persist all changes.

4. **Rollback (optional)**  
   If an exception occurs, the unit of work ensures the transaction is not
   committed.

---

## Summary

The **UnitOfWork** folder provides the transactional backbone for the Identity
EntityFrameworkCore subsystem:

- A concrete unit of work implementation  
- DI registration  
- Atomic commit and rollback behavior  

It ensures that Identity vertical slices operate reliably, consistently, and
transactionally across all environments.

