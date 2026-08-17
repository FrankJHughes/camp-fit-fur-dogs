# Frank.Core.Domain — Domain Exceptions

Domain exceptions in `Frank.Core.Domain` provide a structured, explicit way for aggregates and value objects to signal invariant violations. They ensure that domain rules are enforced consistently and that invalid state transitions are never silently ignored. Domain exceptions are part of the core defensive layer of the domain model.

This document maps the domain exception subsystem under:

```
docs/02-frank-core/domain
```

back to its implementation in:

```
src/Frank/Core/Domain
```

---

## Purpose

Domain exceptions exist to:

- enforce domain invariants  
- prevent invalid state transitions inside aggregates  
- provide meaningful error information to the application layer  
- ensure domain logic remains pure and expressive  
- avoid leaking infrastructure or application concerns into the domain  

They are thrown only when a domain rule is violated — never for technical or infrastructure errors.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Domain/Exceptions`

- **Documentation folder:**  
  `docs/02-frank-core/domain`

This documentation must remain aligned with the actual domain exception hierarchy and usage patterns.

---

## Responsibilities of the Domain Exception Subsystem

### [Invariant Enforcement](ca://s?q=Frank_Core_Domain_Invariant_Enforcement)
Domain exceptions are thrown when invariants are violated, such as:

- attempting to register a dog with an invalid name  
- assigning a dog to a non‑existent owner  
- performing an action that contradicts business rules  

Aggregates use domain exceptions to protect their internal consistency.

### [Explicit Failure Modeling](ca://s?q=Frank_Core_Domain_Exception_Modeling)
Domain exceptions:

- clearly communicate *why* a rule was violated  
- prevent partial or inconsistent state changes  
- ensure handlers cannot ignore domain failures  
- map cleanly into `Result<T>` failures at the application layer  

This keeps error handling predictable and expressive.

### [Isolation From Infrastructure](ca://s?q=Frank_Core_Domain_Isolation)
Domain exceptions:

- do **not** depend on EF Core  
- do **not** depend on ASP.NET Core  
- do **not** depend on logging or middleware  
- do **not** include HTTP status codes  

They belong strictly to the domain layer.

---

## Typical Structure

Domain exceptions usually inherit from a shared base type:

```csharp
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
```

Concrete exceptions express specific rule violations:

```csharp
public sealed class DogNameInvalidException : DomainException
{
    public DogNameInvalidException(string name)
        : base($"Dog name '{name}' is invalid.") { }
}
```

This keeps domain errors explicit and self‑documenting.

---

## How Domain Exceptions Connect to the Broader Platform

Domain exceptions collaborate with:

- **Frank.Core.Application**  
  Pipeline behaviors catch domain exceptions and convert them into `Result<T>` failures.

- **Frank.Core.Api**  
  Exception middleware maps domain failures into problem‑details responses.

- **Frank.Core.Infrastructure**  
  Observation context logs domain failures with correlation IDs.

- **Frank.Core.EntityFrameworkCore**  
  Domain exceptions prevent persistence of invalid aggregates.

This ensures domain failures are handled consistently across the vertical slice.

---

## Runtime Collaboration Points

Domain exceptions interact with the runtime by:

- halting invalid aggregate operations  
- preventing domain event emission when invariants fail  
- propagating meaningful error messages to handlers  
- mapping into structured API responses  
- enriching logs with contextual metadata  

This keeps domain behavior safe and observable.

---

## Composition Flow (API → Application → Domain → Persistence)

Domain exceptions participate in the vertical slice flow:

```
API Endpoint
    ↓
Application Handler
    ↓
Aggregate Method (exception thrown if invariant violated)
    ↓
Pipeline Behavior (maps exception → Result.Failure)
    ↓
HTTP Response (problem details)
```

Domain exceptions ensure that invalid operations never reach persistence.

---

## What Belongs in This Document

This page should describe:

- domain exception responsibilities  
- how exceptions enforce invariants  
- how exceptions propagate through application and API layers  
- how exceptions fit into the vertical slice lifecycle  

It should **not** include:

- infrastructure exceptions  
- HTTP‑specific error handling  
- product‑specific domain rules  

Those belong in other documentation areas.

---

## Notes

Keep this document grounded in the actual Frank.Core.Domain exception implementation.  
Whenever invariant enforcement, exception hierarchy, or error propagation evolves, update this section to reflect the current platform architecture.

