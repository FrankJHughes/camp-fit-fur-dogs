# Entity Framework Core Unit of Work — Developer Guide

This guide documents the **current state** of the Unit of Work capability in Frank and the **intended future state** as the platform evolves.

The Unit of Work (UoW) pattern coordinates transactional consistency across a persistence boundary.  
In Frank, the only implementation today is based on **Entity Framework Core**, but the contract is intentionally persistence‑agnostic.

---

# 1. Current State (Before Future Enhancements)

Frank currently defines:

```csharp
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
```

And provides a single implementation:

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

### What exists today

- A **contract** (`IUnitOfWork`)
- A **single EF Core implementation** (`EfUnitOfWork<TContext>`)
- Auto‑registration for the contract and implementation
- Scoped lifetime semantics
- A simple transactional boundary:  
  **CommitAsync → DbContext.SaveChangesAsync**

### What does *not* exist today

- No cross‑aggregate transaction orchestration  
- No distributed transaction support  
- No outbox pattern integration  
- No ambient UoW scopes  
- No retry policies or resilience layer  
- No non‑EF Core providers  
- No pipeline behaviors or pre/post commit hooks  

### Developer implications today

- UoW is effectively a thin wrapper around EF Core’s `SaveChangesAsync`
- Transaction boundaries are defined by DI scope
- All transactional consistency is delegated to EF Core
- No additional behavior is injected before or after commit

---

# 2. Intended Future State (After Capability Expansion)

The long‑term vision for the Unit of Work capability includes:

### 2.1 Multiple persistence providers

Future implementations may include:

- Dapper  
- MongoDB  
- Cosmos DB  
- Redis  
- EventStore  
- File‑based stores  
- Outbox‑enabled stores  

Each provider would implement `IUnitOfWork` with provider‑specific semantics.

### 2.2 Transaction orchestration

Future UoW may support:

- Pre‑commit hooks  
- Post‑commit hooks  
- Domain event dispatching after commit  
- Outbox message persistence  
- Retry policies  
- Transaction scopes across multiple repositories  

### 2.3 Ambient Unit of Work

Potential future support for:

- Nested scopes  
- Ambient context propagation  
- Automatic enlistment of repositories  

### 2.4 Observability

Future enhancements may include:

- Logging commit attempts  
- Metrics for commit duration  
- Failure tracking  
- Tracing integration  

### 2.5 Error handling and resilience

Potential future features:

- Retry policies  
- Circuit breakers  
- Provider‑specific exception translation  

---

# 3. Developer Responsibilities (Current vs Future)

## Current Responsibilities

Developers must:

- Inject `IUnitOfWork` where transactional boundaries are needed  
- Ensure DbContext is scoped correctly  
- Ensure repositories operate within the same DI scope  
- Call `CommitAsync` explicitly  
- Handle exceptions thrown by EF Core  
- Understand that UoW is a thin wrapper today  

## Future Responsibilities

Once the capability expands, developers will:

- Use UoW as the central transactional boundary  
- Rely on UoW for domain event dispatching  
- Use UoW for outbox message persistence  
- Use UoW for cross‑aggregate consistency  
- Gain observability and resilience features automatically  

---

# 4. Architecture and Invariants

### 4.1 Contract Invariants

`IUnitOfWork` guarantees:

- A single method: `CommitAsync`
- Commit is explicit, not implicit
- Commit returns the number of affected rows (EF Core behavior)
- Commit is atomic within the DbContext transaction boundary

### 4.2 Implementation Invariants (EF Core)

`EfUnitOfWork<TContext>` guarantees:

- One DbContext instance per scope  
- Commit delegates to `SaveChangesAsync`  
- No additional behavior is injected  
- No domain events are dispatched  
- No retries or resilience policies are applied  

### 4.3 DI Invariants

- UoW is **scoped**  
- DbContext is **scoped**  
- Repositories must be scoped  
- All must share the same DI scope to participate in the same transaction  

---

# 5. Example Usage (Developer Perspective)

### Injecting UoW

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

    public async Task HandleAsync(CreateCustomerCommand cmd, CancellationToken ct)
    {
        _repo.Add(new Customer(cmd.Id, cmd.Name));

        await _uow.CommitAsync(ct);
    }
}
```

### What happens under the hood

- Repository writes to DbContext  
- UoW calls `SaveChangesAsync`  
- EF Core commits the transaction  

---

# 6. Summary

**Current State:**  
Frank provides a simple, EF Core–based Unit of Work implementation.  
It wraps `DbContext.SaveChangesAsync` and defines a transactional boundary per DI scope.

**Future State:**  
The Unit of Work capability will expand to support:

- Multiple persistence providers  
- Transaction orchestration  
- Domain event dispatching  
- Outbox integration  
- Observability and resilience  
- Ambient scopes  

As a developer:

- Today, you use UoW as a thin EF Core wrapper  
- In the future, UoW will become a full transactional orchestration capability  

