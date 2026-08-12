
# CampFitFurDogs.Infrastructure.DbContexts

The `CampFitFurDogs.Infrastructure.DbContexts` namespace contains the database‑context bootstrapping layer for the CampFitFurDogs system.  
Its purpose is to configure, instantiate, and register EF Core DbContexts used by the infrastructure layer—primarily `AppDbContext`.

This namespace does **not** contain domain logic, application orchestration, or EF Core entity configurations.  
It focuses exclusively on **DbContext creation and dependency‑injection wiring**.

---

## 🎯 Architectural Role

This namespace provides:

- Design‑time DbContext creation for EF Core tooling  
- Runtime DbContext registration for dependency injection  
- Centralized configuration of database providers and connection strings  
- A clean separation between DbContext setup and entity configuration  

It ensures that both **EF Core migrations** and **runtime database access** behave consistently across environments.

---

## 📦 Included Components

### `AppDbContext`
The primary EF Core database context for the application.

Responsibilities:

- Serves as the unit‑of‑work boundary for infrastructure operations  
- Applies all entity configurations via `ApplyConfigurationsFromAssembly`  
- Materializes and persists domain aggregates  
- Provides a strongly typed entry point for EF Core queries  

Explore the persistence layer:  
**[Persistence overview](ca://s?q=Explain_the_persistence_layer)**

---

### `DesignTimeDbContextFactory`
Provides a design‑time factory for EF Core tooling.

Responsibilities:

- Creates `AppDbContext` instances for migrations  
- Loads configuration from:
  - `appsettings.json`  
  - `appsettings.Development.json`  
  - Environment variables (required for CI/CD)  
- Configures EF Core to use PostgreSQL via `UseNpgsql`  

Explore EF tooling:  
**[EF Core design‑time context](ca://s?q=Explain_design_time_DbContext)**

---

### `ServiceCollectionExtensions`
Registers `AppDbContext` with the dependency‑injection container.

Responsibilities:

- Adds `AppDbContext` with a scoped lifetime  
- Configures EF Core to use PostgreSQL  
- Reads the `DefaultConnection` connection string from configuration  
- Ensures consistent DbContext creation across the application  

Explore DI patterns:  
**[Dependency injection in infrastructure](ca://s?q=Explain_infrastructure_DI_patterns)**

---

## 🧭 DbContext Principles

The DbContext layer follows several core principles:

- **Single Source of Truth**  
  All database access flows through `AppDbContext`.

- **Configuration by Assembly Scanning**  
  Entity configurations live in the Infrastructure project and are auto‑applied.

- **Environment‑Aware Bootstrapping**  
  Design‑time and runtime contexts load configuration differently but consistently.

- **Provider Isolation**  
  PostgreSQL is configured centrally, not scattered across the codebase.

---

## 🚫 What Does *Not* Belong Here

This namespace must **not** contain:

- Entity configurations  
- Domain logic  
- Application commands or queries  
- Readers/writers for aggregates  
- Connection‑string manipulation logic  
- Migration scripts  

Only **DbContext creation and registration** belong here.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Infrastructure.Persistence` — EF Core configurations and DbContext implementation  
- `CampFitFurDogs.Infrastructure.Dogs` — readers/writers for the Dogs vertical slice  
- `CampFitFurDogs.Domain` — aggregates, value objects, invariants  
- `CampFitFurDogs.Application` — vertical slices orchestrating workflows  

---

