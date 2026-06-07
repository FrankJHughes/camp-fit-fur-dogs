# ADR‑0052 — Vertical Slice Isolation Model

## Status  
Accepted

## Context  
As the codebase grew, cross‑slice coupling began to appear:

- Shared logic leaked between slices.  
- Some slices referenced other slices’ Application or Domain types.  
- Validation and mapping logic was duplicated inconsistently.  
- Slice boundaries were not always clear to new contributors.  

A formal isolation model was required to maintain architectural purity and long‑term maintainability.

## Decision  
The system now uses a **strict Vertical Slice Isolation Model** with the following characteristics:

### 1. Slice Structure  
Each slice contains:

- API endpoint(s)  
- Request/response DTOs  
- Application commands/queries  
- Handlers  
- Domain logic (entities, value objects, invariants)  
- Infrastructure implementations (repositories, readers, EF configs)

### 2. Isolation Rules  
- Slices may depend on **SharedKernel**.  
- Slices may not depend on each other.  
- No cross‑slice domain references.  
- No cross‑slice application references.  
- No shared mutable state.

### 3. Cross‑Cutting Logic  
- Lives in SharedKernel or Frank.  
- Never lives in a slice.  
- Includes:  
  - Value object primitives  
  - Domain abstractions  
  - DI auto‑registration  
  - Hosting/environment abstractions  
  - Validation boundaries

### 4. Testing Alignment  
- Tests mirror slice boundaries.  
- No cross‑slice test dependencies.

### 5. Refactoring Safety  
- Slices can be moved, renamed, or replaced independently.  
- No global coupling.

## Consequences  

### Positive  
- Strong modularity and maintainability.  
- Clear boundaries for new contributors.  
- Reduced accidental coupling.  
- Easier refactoring and slice evolution.  
- Predictable folder and architecture structure.

### Negative  
- Some duplication is intentional to preserve isolation.  
- Shared logic must be carefully evaluated before moving to SharedKernel.
