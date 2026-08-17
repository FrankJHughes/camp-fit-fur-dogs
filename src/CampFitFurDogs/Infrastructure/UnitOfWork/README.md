
# CampFitFurDogs.Infrastructure.UnitOfWork

The `CampFitFurDogs.Infrastructure.UnitOfWork` namespace contains the EF Core–backed **Unit of Work** implementation used across the CampFitFurDogs application.  
Its purpose is to provide a **transactional boundary** for application‑layer workflows, ensuring that all changes tracked by `AppDbContext` are committed atomically and consistently.

This namespace does **not** contain domain logic, EF Core entity configurations, or persistence readers/writers.  
It focuses exclusively on **transaction management** and **dependency‑injection registration**.

---

## 🎯 Architectural Role

The Unit of Work layer provides:

- A consistent transactional boundary for application workflows  
- A single place to commit or roll back EF Core changes  
- A clean abstraction (`IAppUnitOfWork`) consumed by vertical slices  
- A bridge between application orchestration and EF Core persistence  

It ensures that multi‑operation workflows behave predictably and safely.

Explore the concept:  
**[Unit of Work pattern](ca://s?q=Explain_the_Unit_of_Work_pattern)**

---

## 📦 Included Components

### `AppUnitOfWork`
The EF Core–backed implementation of `IAppUnitOfWork`.

Responsibilities:

- Wraps `AppDbContext`  
- Delegates commit/rollback behavior to `EntityFrameworkCoreUnitOfWorkBase<TContext>`  
- Ensures atomic persistence of all tracked changes  
- Provides a stable abstraction for application‑layer orchestration  

Explore the DbContext:  
**[AppDbContext](ca://s?q=Explain_AppDbContext)**

---

### `ServiceCollectionExtensions`
Registers the Unit of Work with the dependency‑injection container.

Responsibilities:

- Binds `IAppUnitOfWork` → `AppUnitOfWork`  
- Uses a scoped lifetime appropriate for EF Core operations  
- Ensures each workflow receives its own transactional boundary  

Explore DI patterns:  
**[Infrastructure DI patterns](ca://s?q=Explain_infrastructure_DI_patterns)**

---

## 🧭 Unit of Work Principles

The Unit of Work layer follows several core principles:

- **Atomicity**  
  All changes tracked by the DbContext are committed together.

- **Isolation**  
  Each request or workflow receives its own scoped unit of work.

- **Abstraction**  
  Application code interacts with `IAppUnitOfWork`, not EF Core directly.

- **Consistency**  
  Commit/rollback behavior is centralized and predictable.

---

## 🚫 What Does *Not* Belong Here

This namespace must **not** contain:

- EF Core entity configurations  
- Readers/writers for aggregates  
- Domain logic or invariants  
- Application commands or queries  
- Connection‑string or provider configuration  

Only **transaction management** belongs here.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Infrastructure.Persistence` — DbContext and EF Core configurations  
- `CampFitFurDogs.Infrastructure.Dogs` — readers/writers for the Dogs vertical slice  
- `CampFitFurDogs.Application.UnitOfWork` — UoW abstraction consumed by vertical slices  
- `CampFitFurDogs.Domain` — aggregates, value objects, invariants  

---

