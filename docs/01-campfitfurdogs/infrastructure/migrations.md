# Migrations

The migrations subsystem manages schema evolution for the CampFitFurDogs database. It ensures that changes to domain aggregates, value objects, and persistence conventions are consistently reflected in the underlying relational schema. This document describes how migrations fit into the infrastructure layer and how they align with the implementation under `src/CampFitFurDogs`.

---

## Purpose

Migrations provide:

- a versioned history of database schema changes  
- a reproducible way to apply schema updates across environments  
- alignment between EF Core model configuration and the actual database  
- safe evolution of tables, constraints, foreign keys, and indexes  

This subsystem keeps the architecture maintainable and predictable for future contributors.

---

## Source Alignment

- **Primary implementation area:**  
  `src/CampFitFurDogs/Infrastructure/Migrations/`

- **Documentation folder:**  
  `docs/01-campfitfurdogs/infrastructure`

The documentation must remain aligned with the actual EF Core migration files and updated whenever schema changes occur.

---

## What Belongs Here

This page should describe:

### 1. Responsibilities of the migrations subsystem

- generating migrations when domain or persistence models change  
- applying migrations during application startup or deployment  
- maintaining schema consistency across environments  
- ensuring backward‑compatible evolution when possible  

### 2. How migrations connect to the broader platform

- migrations are generated from EF Core model definitions  
- applied via CLI (`dotnet ef database update`) or automated deployment  
- integrated with the DbContext and EF Core conventions  
- used by CI/CD pipelines to ensure schema correctness  

### 3. Runtime and infrastructure collaboration points

- migrations depend on accurate EF Core model configuration  
- migrations interact with the Unit of Work through the DbContext  
- schema changes influence readers, writers, and aggregate mappings  
- errors during migration surface through infrastructure exception handlers  

### 4. How the code is composed from API → Application → Persistence

- API triggers commands/queries  
- Application uses persistence abstractions  
- Infrastructure uses EF Core to map aggregates  
- Migrations ensure the database schema matches the EF Core model  

This ensures vertical‑slice consistency from domain to persistence.

---

## Migration Structure

Each migration contains:

- **Up()** — operations to apply schema changes  
- **Down()** — operations to revert schema changes  
- **ModelSnapshot** — EF Core’s representation of the current schema  

Example migration structure:

```
20240115_InitialCreate.cs
20240210_AddDogBreedColumn.cs
20240301_AddOwnerForeignKey.cs
AppDbContextModelSnapshot.cs
```

---

## Typical Migration Responsibilities

- creating tables (`dogs`, `users`)  
- adding or modifying columns (e.g., `breed`, `sex`)  
- enforcing foreign keys (`OwnerId → users.Id`)  
- adding indexes for query performance  
- updating value object conversions  
- ensuring strongly typed IDs map correctly to GUIDs  

---

## Best Practices

1. **Generate migrations only after model changes**  
   Keep migrations aligned with domain and persistence updates.

2. **Review generated SQL**  
   Ensure EF Core produces the expected schema changes.

3. **Avoid destructive changes without planning**  
   Dropping columns or tables should be deliberate and documented.

4. **Use meaningful migration names**  
   Example: `AddDogSexColumn` instead of `Migration123`.

5. **Test migrations in a staging environment**  
   Validate schema changes before production deployment.

6. **Keep migrations in version control**  
   They are part of the architectural history.

---

## Notes

This document must remain grounded in the actual source code.  
Whenever EF Core mappings or domain aggregates change, update this page to reflect the new schema evolution path.

