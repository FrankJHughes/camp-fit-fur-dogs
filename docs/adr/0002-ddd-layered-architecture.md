# ADR-002: DDD layered architecture with clean separation

| Field     | Value          |
|-----------|----------------|
| Status    | Accepted       |
| Date      | 2026-03-30     |
| Deciders  | Frank Hughes   |

## Context

Camp Fit Fur Dogs needs a clear code organization strategy that separates
business logic from infrastructure concerns. The project will grow to
include persistence, authentication, and external integrations. Without
explicit boundaries, domain logic tends to leak into controllers and
data-access code, making the system fragile and hard to test.

## Decision

We adopt a four-layer Domain-Driven Design architecture. Each layer is a
separate .NET project with explicit dependencies:

```
src/
├── CampFitFurDogs.Api              → Presentation (controllers, middleware)
├── CampFitFurDogs.Application      → Use cases, orchestration, DTOs
├── CampFitFurDogs.Domain           → Entities, aggregates, value objects, domain events
├── CampFitFurDogs.Infrastructure   → EF Core, external services, repositories

tests/
├── CampFitFurDogs.Api.Tests
├── CampFitFurDogs.Application.Tests
├── CampFitFurDogs.Domain.Tests
├── CampFitFurDogs.Infrastructure.Tests
```

### Dependency rules

- **Domain** has zero project references — it depends on nothing
- **Application** references Domain only
- **Infrastructure** references Application and Domain
- **Api** references Application and Infrastructure (for DI wiring)
- No layer may reference a layer above it
- Cross-cutting building blocks (base entity types, value object base class,
  repository interfaces) will live in a SharedKernel project added in a
  follow-up story

### Key patterns

- **Aggregates** are the consistency boundary — all mutations go through
  aggregate root methods
- **Repositories** are defined as interfaces in Domain, implemented in
  Infrastructure
- **Domain events** are raised by aggregates and dispatched by
  Infrastructure
- **Application services** orchestrate use cases by coordinating aggregates
  and repositories

## Consequences

### Positive

- Domain logic is isolated and testable without infrastructure dependencies
- Each layer can be tested independently with clear boundaries
- Dependency direction is enforced by project references — the compiler
  catches violations
- New developers can locate code by responsibility (controller vs. use case
  vs. entity)

### Negative

- More projects to manage than a monolithic single-project layout
- Simple features require touching multiple layers (controller → service →
  entity → repository)
- Mapping between layers (DTOs ↔ entities) introduces boilerplate

### Neutral

- The four-layer structure is a well-known DDD pattern with broad community
  support and documentation
- SharedKernel will be introduced as a separate project in Sprint 1 to
  house reusable building blocks