# vertical-slice-boundaries.md

# Vertical Slice Boundaries

Vertical slices define how CampFitFurDogs organizes business capabilities.
Each slice is a self‑contained unit of behavior that spans the Domain,
Application, Infrastructure, and Api layers.

Slices exist to:

- isolate business concerns  
- reduce cross‑slice coupling  
- improve testability  
- enable parallel development  
- enforce clean boundaries  

## Slice Composition

Each slice contains:

- Application handlers  
- Validators  
- Domain aggregates, entities, and value objects  
- Infrastructure repositories/readers  
- API endpoints  

Slices must remain **self‑contained** and **independent**.

## Boundary Rules

- A slice **must not** reference another slice’s Application or Domain types.  
- Shared logic must be moved into **Frank** or a **shared abstraction**, not another slice.  
- Cross‑slice communication occurs only through:
  - Domain events  
  - Shared abstractions  
  - Frank primitives  

## Enforcement

Boundary rules are enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

Vertical slices are the primary unit of modularity in CampFitFurDogs.
