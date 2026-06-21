# Entity Framework Core Unit of Work — User Guide

This guide explains how users of the Frank platform should work with the **Unit of Work** capability today, and how the experience will evolve in the future.

The Unit of Work (UoW) pattern provides a **transactional boundary** for persistence operations.  
In Frank, the only implementation today is based on **Entity Framework Core**, but the contract is intentionally persistence‑agnostic.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides:

```csharp
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
```

And a single implementation:

```csharp
public sealed class EfUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _dbContext;

    public EfUnitOfWork(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
```

### What this means for you today

- **You can inject `IUnitOfWork` anywhere you need a transactional boundary.**
- **You can call `CommitAsync` to persist changes.**
- **All repositories and DbContext instances must share the same DI scope.**
- **The UoW is a thin wrapper around EF Core’s `SaveChangesAsync`.**

### What the Unit of Work does today

- Saves all pending changes in the DbContext  
- Ensures atomicity within the DbContext transaction  
- Honors cancellation tokens  
- Propagates EF Core exceptions  

### What the Unit of Work does *not* do today

- No domain event dispatching  
- No outbox pattern  
- No retries or resilience  
- No distributed transactions  
- No pre‑commit or post‑commit hooks  
- No orchestration across multiple DbContexts  
- No alternative persistence providers  

The current capability is intentionally minimal.

---

# 2. Intended Future State (After Capability Expansion)

As the platform evolves, the Unit of Work capability will expand beyond EF Core.

### Future enhancements may include:

- **Multiple persistence providers**
  - Dapper  
  - MongoDB  
  - Cosmos DB  
  - Redis  
  - EventStore  
  - File‑based stores  

- **Transaction orchestration**
  - Pre‑commit hooks  
  - Post‑commit hooks  
  - Domain event dispatching after commit  
  - Outbox message persistence  
  - Cross‑aggregate consistency  

- **Observability**
  - Logging commit attempts  
  - Metrics for commit duration  
  - Failure tracking  
  - Tracing integration  

- **Resilience**
  - Retry policies  
  - Circuit breakers  
  - Provider‑specific exception translation  

- **Ambient UoW scopes**
  - Nested scopes  
  - Automatic enlistment of repositories  
  - Scope propagation  

These features will make UoW a full transactional orchestration capability rather than a thin wrapper.

---

# 3. How You Use Unit of Work Today

### 3.1 Inject the Unit of Work

```csharp
public class CreateCustomerHandler
{
    private readonly IUnitOfWork _uow;
    private readonly ICustomerRepository _repo;

    public CreateCustomerHandler(IUnitOfWork uow, ICustomerRepository repo)
    {
        _uow = uow;
        _repo = repo;
    }
}
```

### 3.2 Perform operations using repositories

```csharp
_repo.Add(new Customer(cmd.Id, cmd.Name));
```

### 3.3 Commit the transaction

```csharp
await _uow.CommitAsync(ct);
```

### What happens under the hood

- Repository operations modify the DbContext  
- `CommitAsync` calls `SaveChangesAsync`  
- EF Core commits the transaction  

### What you must ensure

- All repositories and the UoW share the same DI scope  
- DbContext is scoped  
- You call `CommitAsync` explicitly  
- You handle exceptions thrown by EF Core  

---

# 4. What Will Become Easier in the Future

### Today
- You must manually manage transactional boundaries  
- You must handle exceptions yourself  
- You must coordinate repositories manually  
- You must handle domain events separately  
- You must implement outbox behavior yourself  

### After future enhancements
- UoW will orchestrate transactions automatically  
- Domain events will be dispatched after commit  
- Outbox messages will be persisted automatically  
- Observability and resilience will be built‑in  
- Multiple persistence providers will be supported  

The user experience will become more powerful and more consistent.

---

# 5. Summary

**Current State:**  
Frank provides a minimal EF Core–based Unit of Work implementation.  
You use it to:

- Inject `IUnitOfWork`  
- Perform repository operations  
- Call `CommitAsync` to persist changes  

**Future State:**  
The capability will expand to support:

- Multiple providers  
- Transaction orchestration  
- Domain event dispatching  
- Outbox integration  
- Observability  
- Resilience  
- Ambient scopes  

As a user of this capability:

- Today, you use UoW as a thin EF Core wrapper  
- In the future, UoW will become a full transactional orchestration mechanism  

