# Entity Framework Core Aggregate Root Configuration — Developer Guide

This guide documents the **current state** of the Entity Framework Core Aggregate Root Configuration capability in Frank and the **intended future state** as the platform evolves.

This capability defines how **domain aggregates** are mapped to **EF Core** using a reusable, convention‑enforcing base class.  
It ensures consistency across all aggregate mappings and enforces architectural rules such as ignoring domain events and preventing database‑generated IDs.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides the following EF Core configuration abstraction:

```csharp
public abstract class AggregateRootConfiguration<TAggregateRoot, TId>
    : IEntityTypeConfiguration<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TId>
    where TId : AggregateId
{
    public void Configure(EntityTypeBuilder<TAggregateRoot> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Ignore(a => a.DomainEvents);

        ConfigureAggregateRoot(builder);
    }

    protected abstract string TableName { get; }

    protected abstract void ConfigureAggregateRoot(EntityTypeBuilder<TAggregateRoot> builder);
}
```

### What exists today

- A **base configuration class** for all aggregate roots  
- Explicit key mapping (`HasKey(a => a.Id)`)  
- ID configured as **never database‑generated**  
- Domain events explicitly ignored  
- A required `TableName` property  
- A required `ConfigureAggregateRoot` method for derived configurations  
- Consistent mapping conventions across all aggregates  

### What does *not* exist today

- No automatic value object conversion  
- No automatic ID conversion  
- No automatic table naming conventions  
- No automatic relationship conventions  
- No owned‑type conventions  
- No shadow property conventions  
- No outbox integration  
- No soft delete or audit conventions  
- No global configuration pipeline  

### Developer implications today

- Developers must define table names manually  
- Developers must configure properties, relationships, and indexes manually  
- Value objects must be mapped manually  
- ID conversion must be configured manually if needed  
- All aggregate mappings must derive from this base class  

The current capability is intentionally minimal but enforces critical architectural rules.

---

# 2. Intended Future State (After Capability Expansion)

As the platform evolves, the EF Core Aggregate Root Configuration capability will expand to support richer mapping conventions.

### 2.1 Automatic Value Object Mapping

Future enhancements may include:

- Automatic conversion for simple value objects  
- Automatic owned‑type mapping for complex value objects  
- Built‑in converters for common patterns (Email, Money, etc.)  

### 2.2 Automatic ID Mapping

Potential improvements:

- Automatic conversion between `AggregateId` and `Guid`  
- Convention‑based ID mapping  
- Optional database‑generated IDs for non‑aggregate entities  

### 2.3 Table and Schema Conventions

Future support may include:

- Automatic table naming conventions  
- Automatic schema assignment  
- Naming conventions for relationships and indexes  

### 2.4 Relationship Conventions

Potential additions:

- Automatic one‑to‑many and many‑to‑one conventions  
- Automatic cascade rules  
- Automatic foreign key naming  

### 2.5 Global Configuration Pipeline

Future capabilities may include:

- Pre‑configuration hooks  
- Post‑configuration hooks  
- Global conventions for all aggregates  
- Integration with outbox pattern  
- Integration with soft delete and audit conventions  

---

# 3. Developer Responsibilities (Current vs Future)

## Current Responsibilities

Developers must:

- Create a configuration class per aggregate  
- Inherit from `AggregateRootConfiguration<TAggregateRoot, TId>`  
- Provide a `TableName`  
- Implement `ConfigureAggregateRoot`  
- Map all properties manually  
- Map all relationships manually  
- Map all value objects manually  
- Ensure IDs are mapped correctly  
- Ensure domain events are not persisted  

## Future Responsibilities

Once the capability expands, developers will:

- Rely on automatic value object mapping  
- Rely on automatic ID conversion  
- Rely on global conventions for relationships and indexes  
- Use lifecycle hooks for advanced mapping scenarios  
- Use built‑in conventions for table naming and schema assignment  

---

# 4. Architecture and Invariants

### 4.1 AggregateRootConfiguration Invariants

- All aggregate mappings must derive from this base class  
- All aggregates must define a table name  
- All aggregates must have an explicit key  
- Aggregate IDs must never be database‑generated  
- Domain events must never be persisted  
- Derived configurations must define additional mapping  

### 4.2 Domain Model Invariants (Enforced Indirectly)

- Aggregate IDs are strongly typed  
- Domain events are ephemeral  
- Value objects are immutable  
- Entities compare by identity  

### 4.3 EF Core Invariants

- DbContext must be scoped  
- All aggregate mappings must be registered  
- All value object conversions must be explicit  

---

# 5. Example Usage (Developer Perspective)

### 5.1 Define an Aggregate

```csharp
public sealed class Customer : AggregateRoot<CustomerId>
{
    public Email Email { get; private set; }
}
```

### 5.2 Define the EF Core Configuration

```csharp
public sealed class CustomerConfiguration
    : AggregateRootConfiguration<Customer, CustomerId>
{
    protected override string TableName => "Customers";

    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(c => c.Email)
            .HasConversion(
                v => v.Value,
                v => new Email(v));
    }
}
```

This demonstrates:

- Table naming  
- Value object conversion  
- Inheriting from the base configuration  

---

# 6. Summary

**Current State:**  
Frank provides a minimal but powerful EF Core mapping abstraction that:

- Enforces explicit key mapping  
- Prevents database‑generated IDs  
- Ensures domain events are ignored  
- Provides a consistent mapping pattern  
- Requires developers to map properties and relationships manually  

**Future State:**  
The capability will expand to support:

- Automatic value object mapping  
- Automatic ID conversion  
- Table and schema conventions  
- Relationship conventions  
- Global configuration pipelines  
- Outbox and audit integration  

As a developer:

- Today, you manually configure aggregate mappings using the base class  
- In the future, Frank will automate more of the EF Core mapping lifecycle  

