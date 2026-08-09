# Configurations

The **Configurations** folder contains Entity Framework Core mapping
infrastructure for domain aggregate roots. This folder provides the base
configuration class used by vertical slices to define how their aggregate roots
are mapped to the database.

This subsystem ensures consistent EF Core conventions across all aggregates,
including identifier mapping, domain event suppression, and table naming.

---

## Components

### AggregateRootConfiguration\<TAggregateRoot, TId\>

The `AggregateRootConfiguration` class provides a shared EF Core configuration
template for all aggregate roots in the domain. It enforces the following rules:

#### Identifier Mapping

- The aggregate root’s `Id` property is mapped as the primary key.
- Identifiers are **never database‑generated** (`ValueGeneratedNever()`).
- This supports strongly‑typed IDs and private setters.

#### Domain Event Suppression

- The `DomainEvents` collection is ignored.
- Domain events are **not persisted** and never appear in the database schema.

#### Table Naming

- Derived configurations must specify the table name via the `TableName`
  property.
- This ensures explicit, slice‑controlled naming conventions.

#### Extensibility

Derived configurations implement:

```csharp
protected abstract void ConfigureAggregateRoot(EntityTypeBuilder<TAggregateRoot> builder);
```

This allows slices to define:

- additional properties  
- relationships  
- indexes  
- constraints  
- owned types  
- value object mappings  

without duplicating the shared rules.

---

## Design Principles

- **Consistency**  
  All aggregate roots follow the same EF Core conventions.

- **Explicitness**  
  Table names and additional mappings are defined by slices, not inferred.

- **Domain correctness**  
  Domain events are not persisted; identifiers are immutable.

- **Separation of concerns**  
  Shared EF Core rules live here; slice‑specific rules live in slice
  configurations.

---

## How This Folder Fits Into the Architecture

This folder provides the EF Core foundation for mapping domain aggregate roots.
It is used by:

- vertical slice persistence layers  
- DbContext configurations  
- migrations  
- domain‑driven design aggregate modeling  

Slices define concrete aggregate configurations; this folder provides the base
class that ensures correctness and consistency.

---

## Typical Usage

```csharp
public sealed class OwnerConfiguration
    : AggregateRootConfiguration<Owner, OwnerId>
{
    protected override string TableName => "Owners";

    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Owner> builder)
    {
        builder.Property(o => o.Email).IsRequired();
        builder.HasIndex(o => o.Email).IsUnique();
    }
}
```

---

## Notes

- This folder contains **only** the base configuration — not concrete mappings.
- All aggregate roots should have a corresponding configuration class in their
  vertical slice.
- The base class ensures domain‑driven design rules are respected at the
  persistence layer.

---
