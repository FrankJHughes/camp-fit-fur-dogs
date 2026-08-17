# Domain Exceptions

This folder contains **domain‑specific exceptions** used to express violations of
business rules, invariants, and domain assumptions within the **Frank.Core.Domain**
layer.

Domain exceptions are part of the **ubiquitous language**. They communicate
meaningful domain failures rather than technical or infrastructure errors.

---

## Purpose

Domain exceptions:

- Represent **invalid states or operations** within the domain model  
- Communicate **broken invariants** or **failed business rules**  
- Are thrown **only** from domain logic (entities, value objects, aggregates)  
- Are **not** used for infrastructure, application, or UI concerns  
- Are **not** tied to HTTP semantics (even if names resemble them)

They help ensure the domain remains **explicit**, **intention‑revealing**, and
**self‑validating**.

---

## Included Exceptions

### `BadRequestException`
Indicates that a caller attempted an operation that violates a domain rule or
precondition.  
Examples:

- Creating an entity with invalid data  
- Executing a command that breaks a business rule  
- Providing syntactically valid but semantically invalid input  

This is **not** an HTTP 400 exception — it is a domain‑level validation failure.

### `BadConfigurationException`
Indicates that the system is misconfigured in a way that prevents correct domain
behavior.  
Examples:

- Missing required domain configuration values  
- Invalid domain configuration that breaks invariants  
- Incorrect wiring of domain services or factories  

This is **not** an infrastructure configuration exception — it is thrown only
when domain assumptions are violated by configuration.

---

## Architectural Notes

- All domain exceptions derive from `DomainException`.
- Domain exceptions must remain **pure** and **free of infrastructure concerns**.
- They should contain **only domain‑relevant information**.
- They should be thrown **only** from domain logic.
- They should be caught and translated by the **application layer**, not inside
  the domain.

---

## When to Add New Exceptions

Add a new domain exception when:

- A domain invariant needs a named, intention‑revealing failure mode  
- A business rule violation should be explicitly represented  
- A domain concept requires a dedicated error type  

Do **not** add exceptions for:

- HTTP status codes  
- Infrastructure failures  
- Logging or telemetry  
- Serialization or persistence errors  

Those belong in their respective layers.

---

## Example Usage

```csharp
if (!Email.IsValid(address))
    throw new BadRequestException("Email address is invalid.");
```

```csharp
if (config.DefaultDogBreed is null)
    throw new BadConfigurationException("Default dog breed must be configured.");
```

---

## Summary

This folder defines the **domain’s vocabulary for exceptional states**.  
These exceptions help keep the domain model expressive, explicit, and aligned
with business rules — while remaining isolated from infrastructure and
application concerns.

