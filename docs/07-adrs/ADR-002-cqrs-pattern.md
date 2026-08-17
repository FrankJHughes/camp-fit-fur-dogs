# ADR 002 — CQRS Pattern

## Status
Accepted

## Context

Traditional CRUD operations mix read and write semantics:

- Read operations do not require transactions  
- Write operations benefit from validation and domain events  
- Query optimization differs from command optimization  
- Testing is harder when concerns are mixed  

Separating reads and writes improves clarity, performance, and maintainability.

## Decision

Use CQRS (Command Query Responsibility Segregation) to separate read and write paths.

### Commands

```csharp
public sealed record RegisterDogCommand(
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    Sex Sex);

public sealed class RegisterDogCommandHandler 
    : ICommandHandler<RegisterDogCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(
        RegisterDogCommand command,
        CancellationToken ct)
    {
        // Validate input
        // Create domain model
        // Apply domain rules
        // Persist changes
        // Publish events
        // Return result
    }
}
```

### Queries

```csharp
public sealed record GetDogsQuery(Guid OwnerId);

public sealed class GetDogsQueryHandler 
    : IQueryHandler<GetDogsQuery, IEnumerable<DogDto>>
{
    public async Task<IEnumerable<DogDto>> HandleAsync(
        GetDogsQuery query,
        CancellationToken ct)
    {
        // No validation needed (caller approved)
        // Read from optimized query model/view
        // No domain rules applied (just data fetch)
        // Return DTO
    }
}
```

## Consequences

### Positive

- Clear intent — commands modify, queries read (no side effects)  
- Optimized paths — reads can be independently optimized  
- Testable — handlers are easy to mock  
- Async-friendly — handlers can await external services  
- Observable — all state changes go through command handlers  

### Negative

- More files — one handler per command/query  
- Boilerplate — requires infrastructure setup  
- Data synchronization — read model must stay in sync with write model  

## Implementation

- `ICommandDispatcher` routes commands to handlers  
- `IQueryDispatcher` routes queries to handlers  
- Handlers are discovered via assembly scanning  
- Results are strongly typed (`Result<T>` or `IEnumerable<T>`)  
