# Entity Framework Core Aggregate Root Configuration — User Guide

This guide explains how users of the Frank platform should work with the **Entity Framework Core Aggregate Root Configuration** capability today, and how the experience will evolve in the future.

This capability provides a **base EF Core configuration class** that enforces consistent mapping rules for all aggregate roots.  
It ensures that aggregates are mapped correctly, domain events are ignored, and IDs are never database‑generated.

This guide focuses on how to *use* the configuration base class — not how to implement the underlying capability.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides a reusable EF Core configuration base class:

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

### What this means for you today

As a user of this capability:

- You **create a configuration class per aggregate root**  
- You **inherit from the base class**  
- You **provide a table name**  
- You **map properties, relationships, and indexes** inside `ConfigureAggregateRoot`  
- You **do not map domain events** (they are ignored automatically)  
- You **do not allow EF Core to generate IDs** (IDs must be created in the domain)  

### What the configuration base class does today

- Maps the aggregate to a table  
- Configures the primary key  
- Ensures IDs are never database‑generated  
- Ensures domain events are ignored  
- Provides a hook for additional configuration  

### What the configuration base class does *not* do today

- No automatic value object mapping  
- No automatic ID conversion  
- No automatic table naming conventions  
- No automatic relationship conventions  
- No schema conventions  
- No outbox or audit integration  

The current capability is intentionally minimal but enforces critical architectural rules.

---

# 2. Intended Future State (After Capability Expansion)

As the platform evolves, this capability will gain richer EF Core mapping support.

### Future enhancements may include:

- **Automatic value object mapping**  
- **Automatic ID conversion**  
- **Table and schema naming conventions**  
- **Relationship conventions**  
- **Global configuration pipelines**  
- **Outbox and audit integration**  
- **Soft delete conventions**  
- **Owned type conventions**  

These features will reduce boilerplate and improve consistency across all aggregate mappings.

---

# 3. How You Use the Configuration Base Class Today

## 3.1 Create a configuration class

```csharp
public sealed class CustomerConfiguration
    : AggregateRootConfiguration<Customer, CustomerId>
{
    protected override string TableName => "Customers";

    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Customer> builder)
    {
        // Additional mapping goes here
    }
}
```

## 3.2 Map value objects manually

```csharp
builder.Property(c => c.Email)
    .HasConversion(
        v => v.Value,
        v => new Email(v));
```

## 3.3 Map relationships manually

```csharp
builder.HasMany(c => c.Orders)
    .WithOne()
    .HasForeignKey(o => o.CustomerId);
```

## 3.4 Map indexes manually

```csharp
builder.HasIndex(c => c.Email)
    .IsUnique();
```

## 3.5 Register the configuration with EF Core

EF Core automatically discovers configurations via:

- `ModelBuilder.ApplyConfigurationsFromAssembly(...)`  
- Or explicit registration  

---

# 4. What the Configuration Base Class Guarantees

### **Consistency**
All aggregates follow the same mapping pattern.

### **Identity**
Aggregate IDs are:

- Strongly typed  
- Never database‑generated  
- Always mapped explicitly  

### **Domain Event Safety**
Domain events are:

- Never persisted  
- Always ignored by EF Core  

### **Extensibility**
You can add:

- Properties  
- Relationships  
- Indexes  
- Conversions  
- Constraints  

Inside `ConfigureAggregateRoot`.

---

# 5. What the Configuration Base Class Does *Not* Guarantee

### **No automatic mapping**
You must map:

- Value objects  
- Relationships  
- Indexes  
- Owned types  

### **No conventions**
There are no:

- Table naming conventions  
- Schema conventions  
- Relationship conventions  
- Cascade conventions  

### **No persistence behavior**
This capability does not:

- Save aggregates  
- Dispatch domain events  
- Handle transactions  
- Integrate with Unit of Work  

### **No lifecycle hooks**
There is no:

- Pre‑configure hook  
- Post‑configure hook  

Yet.

---

# 6. Example: Full Aggregate Configuration

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

        builder.HasMany(c => c.Orders)
            .WithOne()
            .HasForeignKey(o => o.CustomerId);

        builder.HasIndex(c => c.Email)
            .IsUnique();
    }
}
```

This demonstrates:

- Table naming  
- Value object conversion  
- Relationship mapping  
- Index creation  

---

# 7. Summary

**Current State:**  
You use the configuration base class to:

- Map aggregates to tables  
- Configure primary keys  
- Prevent database‑generated IDs  
- Ignore domain events  
- Add custom mapping logic  

**Future State:**  
The capability will expand to support:

- Automatic conventions  
- Value object mapping  
- ID conversion  
- Schema and table naming  
- Relationship conventions  
- Outbox and audit integration  

As a user of this capability:

- Today, you manually configure aggregate mappings  
- In the future, Frank will automate more of the EF Core mapping lifecycle  

