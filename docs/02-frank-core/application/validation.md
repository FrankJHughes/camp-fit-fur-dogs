# Frank.Core.Application — Validation

The validation subsystem in `Frank.Core.Application` provides the platform‑level mechanism for enforcing correctness, protecting domain invariants, and ensuring that commands and queries entering the system meet all structural and business‑rule requirements before any domain logic executes.

This document describes the responsibilities of the validation subsystem and maps the documentation folder:

```
docs/02-frank-core/application
```

back to the implementation under:

```
src/Frank/Core
```

---

## Purpose

The validation subsystem exists to:

- ensure commands and queries are structurally valid  
- prevent invalid data from reaching application handlers  
- enforce domain invariants before domain logic executes  
- provide consistent error reporting through `Result<T>`  
- integrate FluentValidation into the CQRS pipeline  
- keep handlers focused on orchestration rather than input checking  

Validation is a critical part of Frank’s deterministic and safe request‑processing model.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Application/Validation`

- **Documentation folder:**  
  `docs/02-frank-core/application`

This documentation must remain aligned with the actual validation pipeline and FluentValidation integration.

---

## Responsibilities of the Validation Subsystem

### [Validator Discovery](ca://s?q=Frank_Core_Application_Validator_Discovery)
Validators are automatically discovered and registered:

```csharp
services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
```

This ensures:

- every command/query validator is picked up  
- no manual registration is required  
- validation coverage remains complete as the system grows  

### [Pipeline Behavior Integration](ca://s?q=Frank_Core_Application_Pipeline_Behaviors)
Validation runs as a pipeline behavior before handler execution:

```
API Endpoint
    ↓
Dispatcher
    ↓
Validation Pipeline Behavior
    ↓
Handler
```

The validation behavior:

1. resolves all validators for the incoming command/query  
2. executes them  
3. aggregates errors  
4. returns a failure `Result<T>` if validation fails  
5. prevents handler execution when input is invalid  

### [Error Modeling](ca://s?q=Frank_Core_Application_Result_Modeling)
Validation errors are mapped into:

- `Result.Failure`  
- structured error objects  
- problem‑details‑compatible API responses  

This ensures consistent error shapes across all products.

### [Domain Invariant Protection](ca://s?q=Frank_Core_Domain_Invariants)
Validation protects domain invariants by:

- rejecting malformed commands  
- enforcing business rules before domain logic runs  
- preventing invalid state transitions  

This keeps domain aggregates pure and focused on invariant enforcement.

---

## How Validation Connects to the Broader Platform

Validation collaborates with multiple Frank subsystems:

- **Frank.Core.Api**  
  API endpoints rely on validation to reject bad input early.

- **Frank.Identity**  
  Validators may enforce identity‑based rules (e.g., ownership).

- **Frank.Core.Application**  
  Validation is part of the pipeline behavior chain.

- **Frank.Core.Domain**  
  Domain invariants are protected by pre‑handler validation.

- **Frank.Core.Infrastructure**  
  Observation context logs validation failures with correlation IDs.

This ensures validation participates fully in the vertical slice lifecycle.

---

## Runtime Collaboration Points

Validation interacts with the runtime by:

- running before any handler logic  
- enriching logs with validation error details  
- preventing domain event emission when input is invalid  
- mapping errors into consistent API responses  
- ensuring transactional boundaries are not opened unnecessarily  

This keeps request processing safe and predictable.

---

## Composition Flow (API → Application → Domain → Persistence)

Validation participates in the vertical slice flow:

```
API Endpoint
    ↓
ICommandDispatcher / IQueryDispatcher
    ↓
Validation Pipeline Behavior
    ↓
Application Handler
    ↓
Domain Aggregate
    ↓
Unit of Work Commit
    ↓
HTTP Response
```

Validation is the gatekeeper for the entire slice.

---

## What Belongs in This Document

This page should describe:

- validation responsibilities  
- how validators are discovered and registered  
- how validation fits into pipeline behaviors  
- how validation collaborates with dispatchers, domain logic, and persistence  
- how validation errors are modeled and returned  

It should **not** include:

- product‑specific validators  
- domain‑specific business rules  
- persistence‑level validation  

Those belong in product or domain documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Application validation implementation.  
Whenever validator discovery, pipeline behavior ordering, or error modeling evolves, update this section to reflect the current platform architecture.

