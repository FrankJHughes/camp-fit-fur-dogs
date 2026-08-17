# EF Core Conventions

The EF Core conventions in CampFitFurDogs define how domain aggregates, value objects, and strongly typed identifiers are mapped to the underlying relational database. This document describes the responsibilities of the infrastructure subsystem, how it aligns with the source code under `src/CampFitFurDogs`, and how it composes with the broader platform.

---

## Purpose

This section documents:

- how EF Core maps domain types to database tables  
- how value objects and strongly typed IDs are persisted  
- how conventions ensure consistency across vertical slices  
- how the infrastructure layer collaborates with the application and domain layers  

The goal is to keep the architecture readable and predictable for future contributors.

---

## Source Alignment

- **Primary implementation area:**  
  `src/CampFitFurDogs/Infrastructure`

- **Documentation folder:**  
  `docs/01-campfitfurdogs/infrastructure`

This document must remain aligned with the actual EF Core configuration and updated as the persistence model evolves.

---

## What Belongs Here

This page should describe:

### 1. Responsibilities of the EF Core subsystem

- mapping aggregates (`Dog`) to tables  
- converting value objects (`DogName`, `Breed`) to primitives  
- converting strongly typed IDs (`DogId`, `UserId`) to GUIDs  
- enforcing relational constraints (keys, foreign keys, required fields)  
- defining table naming conventions and column naming conventions  

### 2. How EF Core connects to the broader platform

- DbContext registration in DI  
- connection string configuration  
- environment‑specific behavior (Development vs Production)  
- integration with Unit of Work  
- logging and diagnostics  

### 3. Runtime and infrastructure collaboration points

- how readers/writers use the DbContext  
- how migrations evolve the schema  
- how EF Core interacts with domain invariants  
- how persistence errors surface through exception handlers  

### 4. How the code is composed from API → Application → Persistence

- API constructs commands/queries  
- Application handlers use persistence abstractions  
- Infrastructure implements abstractions using EF Core  
- DbContext executes SQL and commits transactions  

This ensures strict separation of concerns and vertical‑slice clarity.

---

## EF Core Conventions in CampFitFurDogs

### Table Naming

- Aggregate roots map to pluralized table names (`dogs`, `users`)  
- Join tables follow EF Core conventions unless explicitly configured  

### Primary Keys

- Strongly typed IDs (`DogId`, `UserId`) are stored as GUIDs  
- Conversion is handled via `HasConversion()`  

### Value Objects

Value objects are stored as scalar columns:

- `DogName.Value` → `dog_name`  
- `Breed.Value` → `breed`  

Conversions ensure domain types remain intact while stored as primitives.

### Foreign Keys

Ownership relationships are enforced via foreign keys:

```csharp
builder.HasOne<User>()
    .WithMany()
    .HasForeignKey(d => d.OwnerId);
```

### Enum Storage

`Sex` is stored as either:

- a string (`"Male"`, `"Female"`)  
- or an integer (`0`, `1`)  

depending on configuration.

### Required Fields

Domain invariants are reinforced at the database level:

- `Name` → required  
- `Breed` → required  
- `OwnerId` → required  
- `DateOfBirth` → required  

### Migrations

Schema evolution is tracked via EF Core migrations under:

```
src/CampFitFurDogs/Infrastructure/Migrations/
```

---

## Notes

This document must remain grounded in the actual source code.  
Whenever EF Core mappings, conventions, or persistence behavior change, update this page to reflect the current architecture.

