# Unit of Work (Infrastructure)

The Unit of Work subsystem in the CampFitFurDogs infrastructure layer provides transactional consistency for all write operations. It ensures that changes made through EF Core are committed atomically and that persistence behavior remains predictable across vertical slices. This document describes how the infrastructure implementation aligns with the source code under `src/CampFitFurDogs`.

---

## Purpose

The Unit of Work:

- defines a transactional boundary for application commands  
- coordinates EF Core change tracking and database commits  
- ensures atomicity (all changes succeed or none do)  
- isolates persistence concerns from domain and application layers  
- provides rollback semantics when exceptions occur  

This subsystem keeps the architecture maintainable and predictable for future contributors.

---

## Source Alignment

- **Primary implementation area:**  
  `src/CampFitFurDogs/Infrastructure/UnitOfWork/AppUnitOfWork.cs`

- **Documentation folder:**  
  `docs/01-campfitfurdogs/infrastructure`

This document must remain aligned with the actual Unit of Work implementation and updated as the persistence model evolves.

---

## Responsibilities of the Subsystem

### 1. Transaction Management

The Unit of Work wraps EF Core’s `DbContext` and provides:

- `CommitAsync()` — persists all tracked changes  
- `RollbackAsync()` — discards tracked changes  
- transactional guarantees across multiple write operations  

### 2. Collaboration with DbContext

`AppUnitOfWork` delegates persistence to EF Core:

```csharp
public sealed class AppUnitOfWork :
    EntityFrameworkCoreUnitOfWorkBase<AppDbContext>,
    IAppUnitOfWork
{
    // CommitAsync() and RollbackAsync() inherited from base class
}
```

The base class handles:

- opening and closing transactions  
- coordinating EF Core’s change tracker  
- ensuring consistent commit behavior  

### 3. Integration with Application Layer

Handlers rely on the Unit of Work to commit changes:

```csharp
await _dogWriter.WriteAsync(dog, ct);
await _unitOfWork.CommitAsync(ct);
```

This ensures:

- domain invariants are enforced before persistence  
- persistence is performed only after successful domain operations  
- no partial writes occur  

### 4. Runtime Collaboration Points

The Unit of Work interacts with:

- **DbContext** — tracks and persists changes  
- **EF Core** — executes SQL and manages transactions  
- **Application handlers** — orchestrate write operations  
- **Exception handlers** — rollback on failure  

This keeps persistence concerns isolated and predictable.

---

## How the Code Is Composed (API → Application → Infrastructure)

1. **API Layer**  
   Endpoint receives request → constructs command.

2. **Application Layer**  
   Handler executes domain logic → calls writer → commits via Unit of Work.

3. **Infrastructure Layer**  
   Writer uses DbContext → Unit of Work commits → EF Core executes SQL.

4. **Database**  
   PostgreSQL stores updated state.

This flow maintains strict separation of concerns and vertical‑slice clarity.

---

## What Belongs in This Document

This page should describe:

- responsibilities of the Unit of Work subsystem  
- how it collaborates with DbContext and EF Core  
- how it integrates with application handlers  
- how transactional boundaries are enforced  
- how persistence flows from API → Application → Infrastructure  

It should **not** include:

- domain logic  
- application orchestration  
- API‑level behavior  

---

## Notes

This document must remain grounded in the actual source code.  
Whenever EF Core behavior, DbContext configuration, or persistence flows change, update this page to reflect the current architecture.

