# Frank.Core.Domain — Value Objects

Value objects in `Frank.Core.Domain` represent immutable domain concepts that have **no identity** and are **compared purely by value**. They encapsulate validation, enforce invariants, and eliminate primitive obsession by giving meaningful structure to domain data.

This document maps the value object subsystem under:

```
docs/02-frank-core/domain
```

back to its implementation in:

```
src/Frank/Core/Domain
```

---

## Purpose

Value objects exist to:

- model domain concepts that do not require identity  
- enforce invariants at creation time  
- guarantee immutability  
- provide semantic clarity over raw primitives  
- ensure equality is based on value, not reference  

They are foundational to expressive, safe domain modeling.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Domain/ValueObject.cs`

- **Documentation folder:**  
  `docs/02-frank-core/domain`

This documentation must remain aligned with the actual value object base class and creation patterns.

---

## Responsibilities of the Value Object Subsystem

### [Immutability](ca://s?q=Frank_Core_Domain_ValueObject_Immutability)
Value objects are immutable by design:

- all fields are readonly  
- no setters exist  
- mutation requires creating a new instance  

This ensures domain state cannot be changed accidentally.

### [Invariant Enforcement](ca://s?q=Frank_Core_Domain_ValueObject_Invariants)
Value objects validate input at creation time:

```csharp
public static Result<DogName> Create(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return Result.Failure<DogName>("Name is required");

    return Result.Success(new DogName(input));
}
```

This prevents invalid data from entering aggregates.

### [Equality by Value](ca://s?q=Frank_Core_Domain_ValueObject_Equality)
Value objects override equality members so that:

- two value objects with the same values are equal  
- identity does not matter  
- reference equality is irrelevant  

This supports correct domain comparisons.

### [Semantic Modeling](ca://s?q=Frank_Core_Domain_ValueObject_Semantics)
Value objects replace primitive types such as:

- `string` → `DogName`  
- `decimal` → `Price`  
- `int` → `Age`  

This makes domain code self‑documenting and reduces ambiguity.

---

## Example: DogName

```csharp
public sealed class DogName : ValueObject
{
    public string Value { get; }

    private DogName(string value)
    {
        Value = value;
    }

    public static Result<DogName> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<DogName>("Name is required");

        return Result.Success(new DogName(input));
    }
}
```

Key characteristics:

- immutable  
- validated at creation  
- compared by value  
- semantically meaningful  

---

## How Value Objects Connect to the Broader Platform

Value objects collaborate with:

- **Frank.Core.Domain Aggregates**  
  Aggregates use value objects to enforce invariants and avoid primitive misuse.

- **Frank.Core.Application**  
  Handlers receive value objects as part of commands and queries.

- **Frank.Core.EntityFrameworkCore**  
  Value objects are persisted via EF Core backing fields.

- **Frank.Core.Infrastructure**  
  Logging and observation use value object values for clarity.

Value objects strengthen every layer of the vertical slice.

---

## Runtime Collaboration Points

Value objects interact with the runtime by:

- validating input before domain logic runs  
- preventing invalid aggregate state  
- ensuring consistent equality checks  
- enriching logs with meaningful domain values  

They are essential for safe, expressive domain modeling.

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Endpoint
    ↓
Command/Query Validation
    ↓
Value Object Creation (invariants enforced)
    ↓
Aggregate Method
    ↓
Domain Events Raised
    ↓
Unit of Work Commit
```

Value objects act as the first line of defense for domain correctness.

---

## What Belongs in This Document

This page should describe:

- value object responsibilities  
- immutability and equality semantics  
- invariant enforcement patterns  
- how value objects integrate with aggregates and persistence  
- how value objects fit into the vertical slice lifecycle  

It should **not** include:

- product‑specific value objects  
- infrastructure‑specific persistence details  
- application‑level validation rules  

Those belong in product or application documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Domain value object implementation.  
Whenever equality patterns, creation semantics, or invariant enforcement evolve, update this section to reflect the current platform architecture.

