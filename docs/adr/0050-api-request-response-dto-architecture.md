# ADR‑0050 — API Request/Response DTO Architecture

## Status  
Accepted

## Context  
As the API surface expanded, inconsistencies emerged in how request and response DTOs were structured:

- Some endpoints returned domain entities directly.  
- Some responses mixed internal and external representations.  
- Validation rules were duplicated across DTOs and domain models.  
- Error shapes varied between endpoints.  
- Frontend forms required predictable DTO structures for FormCommand integration.  

A unified DTO architecture was required to ensure consistency, stability, and clear separation between API contracts and internal domain models.

## Decision  
The system now uses a **strict API DTO architecture** with the following characteristics:

### 1. Request DTOs  
- Represent **external input only**.  
- Contain no domain logic.  
- Are validated using FluentValidation.  
- Map into commands or value objects in the Application layer.  
- Never expose domain entities or internal IDs.

### 2. Response DTOs  
- Represent **external output only**.  
- Contain no domain logic.  
- Are shaped for frontend consumption.  
- Never expose internal domain structures or invariants.  
- Are stable across backend refactors.

### 3. Mapping Layer  
- Application layer maps between DTOs and domain models.  
- Mapping is explicit and testable.  
- No implicit or reflection‑based mapping.

### 4. Version Stability  
- DTOs are treated as part of the public API contract.  
- Breaking changes require explicit versioning.

### 5. Error DTOs  
- All errors conform to the unified error boundary (ADR‑0047).  
- Validation errors include field‑level details.

## Consequences  

### Positive  
- Predictable API contracts for frontend and external consumers.  
- Clear separation between domain models and external representations.  
- Reduced accidental exposure of internal structures.  
- Easier refactoring of domain and application layers.  
- Improved testability of DTO validation and mapping.

### Negative  
- Requires maintenance of mapping code.  
- DTOs must be versioned carefully to avoid breaking clients.
