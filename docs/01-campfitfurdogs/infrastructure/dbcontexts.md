# DbContexts

The CampFitFurDogs infrastructure layer provides the concrete persistence implementation for the domain model. At the center of this layer is the EF Core `DbContext`, which acts as the gateway between aggregates, value objects, and the underlying PostgreSQL database.

This document describes the responsibilities of the infrastructure subsystem, how it aligns with the source code under `src/CampFitFurDogs`, and how it composes with the broader platform.

---

## Purpose

The DbContext is responsible for:

- mapping domain aggregates and value objects to relational tables  
- enforcing persistence‑level constraints  
- coordinating EF Core change tracking  
- participating in the Unit of Work pattern  
- providing read/write access for application‑layer readers and writers  

It is the infrastructure boundary where domain purity meets real‑world storage.

---

## Source Alignment

- **Primary implementation:**  
  `src/CampFitFurDogs/Infrastructure/Database/AppDbContext.cs`

- **Documentation folder:**  
  `docs/01-campfitfurdogs/infrastructure`

This document must remain aligned with the actual EF Core configuration and updated whenever the persistence model evolves.

---

## Responsibilities of the Subsystem

### 1. Aggregate Persistence

The DbContext maps domain aggregates such as `Dog` into database tables:

- `Dog` → `dogs` table  
- strongly typed IDs → primary keys  
- value objects (`DogName`, `Breed`) → scalar columns  
- ownership (`OwnerId`) → foreign key to `users` table  

Mapping is performed using EF Core configuration classes or fluent API conventions.

### 2. Value Object Conversion

Value objects are persisted using EF Core value converters:

- `DogName.Value` → `dog_name` column  
- `Breed.Value` → `breed` column  
- `Sex` enum → string or integer column depending on configuration  

These converters ensure domain types remain intact while stored as primitives.

### 3. Unit of Work Integration

The DbContext participates in the Unit of Work pattern through:

- `AppUnitOfWork` (infrastructure implementation)  
- transactional commit via `SaveChangesAsync()`  
- rollback semantics when exceptions occur  

The application layer never interacts with EF Core directly — only through abstractions.

### 4. Reader/Writer Collaboration

The DbContext powers all persistence abstractions:

- `IRegisterDogWriter`  
- `IEditDogWriter`  
- `IRemoveDogWriter`  
- `IGetDogReader`  
- `IListDogsByOwnerReader`

These abstractions use the DbContext internally but expose domain‑friendly interfaces to the application layer.

### 5. Runtime and Platform Integration

The DbContext integrates with the broader platform:

- connection strings from `appsettings.json`  
- environment‑specific configuration (Development, Staging, Production)  
- DI registration via `AddInfrastructureDbContexts(configuration)`  
- logging and observability through EF Core diagnostics  

This ensures consistent behavior across environments.

---

## How the Code Is Composed (API → Application → Infrastructure)

1. **API Layer**  
   Endpoint receives request → constructs command/query.

2. **Application Layer**  
   Handler executes → uses persistence abstractions (readers/writers).

3. **Infrastructure Layer**  
   Reader/writer uses DbContext → EF Core executes SQL → Unit of Work commits.

4. **Database**  
   PostgreSQL stores and retrieves data.

This flow ensures strict separation of concerns and maintains vertical‑slice purity.

---

## What Belongs in This Document

This page should always describe:

- the responsibilities of the DbContext subsystem  
- how EF Core maps domain objects  
- how persistence integrates with the Unit of Work  
- how infrastructure connects to the rest of the platform  
- how the persistence model evolves over time  

It should **not** include:

- API‑level behavior  
- domain logic  
- application‑layer orchestration  
- frontend concerns  

---

## Notes

This document must remain grounded in the actual source code.  
Whenever the persistence model changes — new tables, new aggregates, new value objects — update this page to reflect the current architecture.

