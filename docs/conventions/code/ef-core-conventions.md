# EF Core Conventions (CampFitFurDogs)

EF Core is used exclusively in the Infrastructure slice.  
Application and Api must not depend on EF Core types.

---

## Mapping Rules

- Aggregate root → table  
- Id is not value‑generated  
- Value objects mapped as owned types  
- Domain events ignored by EF Core  
- Navigation properties must be explicit  
- No lazy loading  

---

## DbContext Rules

DbContext must:

- live in Infrastructure  
- expose DbSets only for aggregate roots  
- configure all entities via `IEntityTypeConfiguration<>`  
- never expose IQueryable to Application  

DbContext must not:

- contain business logic  
- contain domain behavior  
- depend on Api  

---

## Migrations

Migrations must:

- be applied by CI only  
- be idempotent  
- tolerate empty databases  
- be preview‑safe  
- contain no environment‑specific logic  

---

## Query Rules

Read‑model queries must:

- use `AsNoTracking`  
- return DTOs only  
- never return domain entities  
- never mutate state  

Readers must not depend on repositories.

---

## Prohibitions

EF Core usage must not:

- leak into Application  
- leak into Api  
- expose EF Core types in public APIs  
- use lazy loading  
