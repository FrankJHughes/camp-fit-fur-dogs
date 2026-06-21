# Entity Framework Core Aggregate Root Configuration — Tester Guide

This guide documents the **current state** of the Entity Framework Core Aggregate Root Configuration capability in Frank and the **intended future state** as the platform evolves.

This capability defines how **domain aggregates** are mapped to EF Core using a reusable, convention‑enforcing base class.  
Testers validate the *mapping behavior*, not the domain behavior.

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

### What exists today (testable behavior)

- Aggregate roots map to a specific table  
- Aggregate IDs are mapped as **non‑generated** keys  
- Domain events are **ignored** by EF Core  
- Derived configurations must define:
  - A table name  
  - Additional property/relationship/index mappings  

### What does *not* exist today (not testable)

- No automatic value object conversion  
- No automatic ID conversion  
- No global conventions  
- No relationship conventions  
- No schema conventions  
- No outbox integration  
- No soft delete or audit conventions  

### What testers validate today

- The base configuration enforces required conventions  
- Derived configurations correctly map properties and relationships  
- Value objects are mapped correctly (manual conversions)  
- IDs are mapped correctly  
- Domain events are not persisted  
- Table names are correct  
- EF Core model builds successfully  

The current capability is intentionally minimal but enforces critical architectural rules.

---

# 2. Intended Future State (After Capability Expansion)

As the platform evolves, testers will validate richer mapping conventions.

### 2.1 Automatic Value Object Mapping

- Built‑in converters  
- Automatic owned‑type mapping  
- Convention‑based value object persistence  

### 2.2 Automatic ID Mapping

- Convention‑based ID conversion  
- Automatic mapping of `AggregateId` to `Guid`  

### 2.3 Table and Schema Conventions

- Automatic table naming  
- Automatic schema assignment  

### 2.4 Relationship Conventions

- Automatic one‑to‑many and many‑to‑one conventions  
- Automatic cascade rules  
- Automatic foreign key naming  

### 2.5 Global Configuration Pipeline

- Pre‑configuration hooks  
- Post‑configuration hooks  
- Outbox integration  
- Soft delete and audit conventions  

These features will significantly expand the testing surface.

---

# 3. Test Responsibilities (Current vs Future)

## Current Responsibilities

Testers must validate:

### **Base Class Behavior**
- Table name is applied  
- Primary key is configured  
- ID is marked as `ValueGeneratedNever`  
- Domain events are ignored  
- Derived configuration method is invoked  

### **Derived Configuration Behavior**
- All properties are mapped correctly  
- All relationships are mapped correctly  
- All indexes are mapped correctly  
- Value objects are mapped correctly  
- IDs are mapped correctly  
- EF Core model builds without errors  

### **No Additional Behavior**
- No automatic conventions  
- No automatic value object mapping  
- No automatic ID conversion  
- No lifecycle hooks  
- No outbox or audit behavior  

## Future Responsibilities

Once the capability expands, testers will validate:

- Automatic value object mapping  
- Automatic ID conversion  
- Table and schema conventions  
- Relationship conventions  
- Global configuration pipelines  
- Outbox and audit integration  

---

# 4. Required Test Types (Current State)

## 4.1 Table Mapping Tests

Validate:

- Table name matches the derived configuration  
- Table exists in the EF Core model  

### Example

```csharp
var entity = model.FindEntityType(typeof(Customer));
Assert.Equal("Customers", entity.GetTableName());
```

---

## 4.2 Key Mapping Tests

Validate:

- Primary key is configured  
- ID is not database‑generated  

### Example

```csharp
var id = entity.FindProperty("Id");
Assert.False(id.ValueGenerated != ValueGenerated.Never);
```

---

## 4.3 Domain Event Ignoring Tests

Validate:

- `DomainEvents` is ignored by EF Core  

### Example

```csharp
Assert.Null(entity.FindProperty("DomainEvents"));
```

---

## 4.4 Value Object Mapping Tests

Validate:

- Value objects are mapped using conversions or owned types  
- Conversion logic is correct  

### Example

```csharp
var email = entity.FindProperty("Email");
Assert.NotNull(email.GetValueConverter());
```

---

## 4.5 Relationship Mapping Tests

Validate:

- Navigation properties are mapped correctly  
- Foreign keys exist  
- Cascade rules are correct  

---

## 4.6 Model Build Tests

Validate:

- The EF Core model builds successfully  
- No missing configurations  
- No ambiguous relationships  

### Example

```csharp
var context = new TestDbContext(options);
var model = context.Model; // Should not throw
```

---

# 5. Anti‑Patterns (Tests Must Reject)

- Tests that assume automatic value object mapping  
- Tests that assume automatic ID conversion  
- Tests that assume global conventions  
- Tests that assume outbox or audit behavior  
- Tests that rely on database‑generated IDs  
- Tests that assume domain events are persisted  
- Tests that assume lifecycle hooks exist  

These features do **not** exist today.

---

# 6. Summary

**Current State:**  
Testers validate:

- Table mapping  
- Key mapping  
- Domain event ignoring  
- Value object mapping  
- Relationship mapping  
- Model build correctness  

**Future State:**  
Testers will validate:

- Automatic conventions  
- Value object conversion  
- ID conversion  
- Schema conventions  
- Relationship conventions  
- Outbox and audit integration  

This Tester Guide prepares testers for both the current minimal EF Core mapping abstraction and the richer future capability.

