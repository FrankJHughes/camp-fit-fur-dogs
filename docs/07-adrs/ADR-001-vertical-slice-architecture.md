# ADR 001 — Vertical Slice Architecture

## Status
Accepted

## Context

A typical layered architecture (Controllers → Services → Repositories → DbContext) creates horizontal dependencies:

- Business logic scattered across layers  
- Hard to find all code for one feature  
- Difficult to understand feature requirements  
- Features depend on framework layers  

This makes the system harder to maintain, reason about, and evolve.

## Decision

Organize features as vertical slices with independent domain, application, API, and infrastructure concerns:

```
src/CampFitFurDogs/
  Domain/Dogs/
    Dog.cs (aggregate root)
    DogName.cs (value object)
    DogAggregateRepository.cs (interface)

  Application/Dogs/
    RegisterDogCommand.cs
    RegisterDogCommandHandler.cs
    GetDogsQuery.cs
    GetDogsQueryHandler.cs

  Infrastructure/Dogs/
    DogEntityMapper.cs
    DogRepository.cs (impl)
    RegisterDogWriter.cs
    GetDogsForOwnerReader.cs

  Api/Endpoints/Dogs/
    RegisterDogEndpoint.cs
    GetDogsEndpoint.cs
```

## Consequences

### Positive

- Feature understanding — all code for a feature is in one folder hierarchy  
- Independent testing — can test a feature in isolation  
- Clear dependencies — a feature doesn't depend on other features  
- Easier refactoring — moving or removing a feature is straightforward  
- Scales well — adding new features doesn’t complicate existing ones  

### Negative

- Requires discipline — easy to accidentally couple features  
- Learning curve — different from typical ASP.NET patterns  
- More files/folders — can feel verbose initially  

## Implementation Notes

- Each vertical slice is independent  
- Cross-cutting concerns (Auth, Logging, CORS) are in Frank.Core  
- Domain models don't reference other features' models  
- Commands/queries are feature-scoped  
- Use dependency injection to compose features  
