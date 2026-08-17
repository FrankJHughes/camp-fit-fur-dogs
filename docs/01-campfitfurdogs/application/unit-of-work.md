# Unit of Work

The unit of work pattern provides a consistent transactional boundary for application operations. It ensures that all state‑changing commands commit their changes atomically and that persistence remains predictable across the Dogs vertical slice.

## Pattern

Every command that modifies state uses the unit of work to commit changes in a single atomic operation:

```csharp
public sealed class RegisterDogCommandHandler : ICommandHandler<RegisterDogCommand, Guid>
{
    private readonly IRegisterDogWriter _dogWriter;
    private readonly IAppUnitOfWork _unitOfWork;
    
    public async Task<Guid> HandleAsync(RegisterDogCommand command, CancellationToken ct)
    {
        // Create the domain aggregate
        var dog = Dog.Create(command.OwnerId, ...);

        // Delegate persistence to infrastructure
        await _dogWriter.AddAsync(dog, ct);

        // Commit all changes atomically
        await _unitOfWork.CommitAsync(ct);

        return dog.Id.Value;
    }
}
```

Handlers never interact directly with EF Core or database transactions. Instead, they rely on the unit of work abstraction to ensure consistency.

## Implementation

`IAppUnitOfWork` is implemented by `AppUnitOfWork`, which wraps the EF Core `DbContext`:

```csharp
public sealed class AppUnitOfWork :
    EntityFrameworkCoreUnitOfWorkBase<AppDbContext>,
    IAppUnitOfWork
{
    // CommitAsync() saves all tracked changes
    // RollbackAsync() discards them
}
```

The base class provides:

- transaction management  
- change tracking coordination  
- rollback semantics  
- consistent commit behavior  

This keeps infrastructure concerns isolated from application logic.

## Scope

The unit of work is registered as **Scoped**, meaning a new instance is created for each HTTP request. This ensures:

- changes from one request never leak into another  
- the database context is disposed cleanly at the end of the request  
- EF Core’s change tracking remains consistent throughout the request lifetime  

Scoped lifetime aligns perfectly with the request‑based nature of CQRS command execution.

## Atomicity

All operations within a single `CommitAsync()` call are wrapped in a database transaction. This guarantees:

- **all changes persist**, or  
- **none do**  

There is no partial success. If any write operation fails, the entire transaction is rolled back, preserving domain consistency.

## Summary

The unit of work provides:

- a clean transactional boundary  
- atomic commit semantics  
- isolation between requests  
- decoupling between application logic and EF Core  

It is a foundational part of the Dogs vertical slice and ensures reliable, predictable persistence behavior across all write operations.

