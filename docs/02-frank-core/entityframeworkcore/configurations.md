# Frank.Core.EntityFrameworkCore — Configurations

The configuration subsystem in `Frank.Core.EntityFrameworkCore` defines how aggregates, entities, value objects, and strongly typed identifiers are mapped into the database. It ensures that domain objects are persisted correctly without leaking domain logic into the persistence layer.

This document maps the configuration subsystem under:

```
docs/02-frank-core/entityframeworkcore
```

back to its implementation in:

```
src/Frank/Core/EntityFrameworkCore
```

---

## Purpose

The configuration subsystem exists to:

- map aggregates and entities to database tables  
- configure backing fields for domain properties  
- support strongly typed identifiers through value converters  
- ensure value objects are persisted safely and consistently  
- keep domain logic pure and free of ORM concerns  
- provide a predictable persistence model across all Frank products  

Configurations are the bridge between the domain model and the database schema.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/EntityFrameworkCore/Configurations`

- **Documentation folder:**  
  `docs/02-frank-core/entityframeworkcore`

This documentation must remain aligned with the actual EF Core configuration classes and mapping conventions.

---

## Responsibilities of the Configuration Subsystem

### [Aggregate Mapping](ca://s?q=Frank_Core_EFCore_Aggregate_Mapping)
Aggregates are mapped using `IEntityTypeConfiguration<T>` classes:

- table names  
- primary keys  
- backing fields  
- navigation properties  
- owned value objects  

Aggregates are materialized using private constructors, ensuring domain logic is not executed during hydration.

### [Strongly Typed ID Converters](ca://s?q=Frank_Core_EFCore_StronglyTypedId_Converters)
Strongly typed IDs (e.g., `DogId`, `UserId`) are mapped using EF Core value converters:

```csharp
builder.Property(x => x.Id)
    .HasConversion(
        id => id.Value,
        value => new DogId(value));
```

This ensures:

- IDs remain strongly typed in the domain  
- the database stores raw primitives (e.g., `Guid`)  
- conversions are handled automatically  

### [Value Object Mapping](ca://s?q=Frank_Core_EFCore_ValueObject_Mapping)
Value objects are mapped via:

- backing fields  
- owned entity types  
- custom converters (when needed)  

Example:

```csharp
builder.OwnsOne(x => x.Name, name =>
{
    name.Property(n => n.Value)
        .HasColumnName("Name")
        .IsRequired();
});
```

This preserves immutability while allowing EF Core to persist value object data.

### [Backing Fields](ca://s?q=Frank_Core_EFCore_BackingFields)
Domain objects often expose read‑only properties. EF Core maps these using backing fields:

```csharp
builder.Property<string>("_name")
    .HasColumnName("Name");
```

Backing fields ensure:

- domain invariants are enforced through constructors  
- EF Core can materialize objects without invoking domain logic  

### [Navigation and Relationship Configuration](ca://s?q=Frank_Core_EFCore_Relationships)
Configurations define:

- one‑to‑many relationships  
- many‑to‑many relationships  
- owned collections  
- cascade behaviors  

All relationships respect aggregate boundaries.

---

## How Configurations Connect to the Broader Platform

Configurations collaborate with:

- **Frank.Core.Domain**  
  Domain objects define invariants; configurations define persistence.

- **Frank.Core.Application**  
  Handlers rely on EF Core to materialize aggregates.

- **Frank.Core.Infrastructure**  
  Observation context logs database operations.

- **Unit of Work**  
  Configurations ensure aggregates are tracked correctly for commit.

Configurations ensure domain purity while enabling reliable persistence.

---

## Runtime Collaboration Points

Configurations interact with the runtime by:

- mapping domain objects to database schema  
- converting strongly typed IDs  
- materializing aggregates via private constructors  
- persisting value objects through owned types  
- ensuring domain events are dispatched before commit  

This keeps persistence predictable and domain‑safe.

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Endpoint
    ↓
Application Handler
    ↓
Domain Aggregate (state mutated)
    ↓
EF Core Tracking (configured mappings)
    ↓
Unit of Work Commit
    ↓
Database
```

Configurations define how domain objects reach the database.

---

## What Belongs in This Document

This page should describe:

- mapping responsibilities  
- how aggregates, entities, and value objects are configured  
- how strongly typed IDs are persisted  
- how configurations collaborate with domain and application layers  
- how configurations fit into the vertical slice lifecycle  

It should **not** include:

- product‑specific mappings  
- raw SQL migrations  
- infrastructure‑specific database tuning  

Those belong in product or infrastructure documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.EntityFrameworkCore configuration implementation.  
Whenever mapping conventions, ID converters, or value object patterns evolve, update this section to reflect the current platform architecture.

