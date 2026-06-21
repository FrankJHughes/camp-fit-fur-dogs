# Entity Framework Core Unit of Work — Tester Guide

This guide documents the **current state** of the Unit of Work capability in Frank and the **intended future state** as the capability evolves.

The Unit of Work (UoW) pattern provides a transactional boundary for persistence operations.  
Today, Frank includes only a **single EF Core implementation**, which means the testing surface is small but well‑defined.  
Future enhancements will expand the behavior and therefore the testing responsibilities.

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

### What exists today

- A contract (`IUnitOfWork`)
- A single EF Core implementation (`EfUnitOfWork<TContext>`)
- Scoped DI registration
- A single behavior: **CommitAsync → SaveChangesAsync**

### What does *not* exist today

- No pre‑commit or post‑commit hooks  
- No domain event dispatching  
- No outbox integration  
- No distributed transactions  
- No retry or resilience layer  
- No alternative providers  
- No orchestration across multiple DbContexts  

### What testers can validate today

- That `CommitAsync` calls `SaveChangesAsync` exactly once  
- That cancellation tokens are honored  
- That exceptions propagate  
- That DbContext lifetime rules are respected  
- That UoW is scoped correctly  
- That repositories and UoW share the same DbContext instance  

This is a **minimal testing surface**, but it must be validated thoroughly.

---

# 2. Intended Future State (After Capability Expansion)

As the Unit of Work capability evolves, testers will eventually validate:

### 2.1 Transaction orchestration

- Pre‑commit hooks  
- Post‑commit hooks  
- Domain event dispatching after commit  
- Outbox message persistence  
- Cross‑aggregate consistency  

### 2.2 Multiple persistence providers

- Dapper  
- MongoDB  
- Cosmos DB  
- Redis  
- EventStore  
- File‑based stores  

Each provider will require its own test suite.

### 2.3 Observability

- Logging commit attempts  
- Metrics for commit duration  
- Failure tracking  
- Tracing integration  

### 2.4 Resilience

- Retry policies  
- Circuit breakers  
- Provider‑specific exception translation  

### 2.5 Ambient UoW scopes

- Nested scopes  
- Automatic enlistment of repositories  
- Scope propagation  

These features will significantly expand the testing responsibilities.

---

# 3. Test Responsibilities (Current vs Future)

## Current Responsibilities

Testers must validate:

- **Commit behavior**
  - `SaveChangesAsync` is called exactly once
  - Return value matches EF Core’s result

- **Cancellation behavior**
  - Cancellation token is passed through
  - Operation cancels when requested

- **Exception behavior**
  - EF Core exceptions propagate unchanged
  - No exceptions are swallowed

- **DI and lifetime behavior**
  - UoW is scoped
  - DbContext is scoped
  - Repositories and UoW share the same DbContext instance

- **No additional behavior**
  - No domain events are dispatched
  - No retries occur
  - No pre/post commit logic runs

## Future Responsibilities

Once the capability expands, testers will validate:

- Transaction orchestration  
- Domain event dispatching  
- Outbox integration  
- Retry and resilience behavior  
- Observability and logging  
- Multi‑provider consistency  
- Ambient UoW behavior  

---

# 4. Required Test Types (Current State)

## 4.1 Commit Behavior Tests

Validate:

- `CommitAsync` calls `SaveChangesAsync` exactly once  
- Return value is passed through  
- No additional behavior occurs  

### Example

- Arrange: Fake DbContext with a counter  
- Act: Call `CommitAsync`  
- Assert: Counter == 1  

---

## 4.2 Cancellation Token Tests

Validate:

- Cancellation token is passed to `SaveChangesAsync`  
- Operation cancels when token is triggered  

### Example

- Arrange: DbContext configured to delay  
- Act: Cancel token before delay completes  
- Assert: Operation throws `TaskCanceledException`  

---

## 4.3 Exception Propagation Tests

Validate:

- EF Core exceptions propagate unchanged  
- No wrapping or swallowing occurs  

### Example

- Arrange: DbContext throws `DbUpdateException`  
- Act: Call `CommitAsync`  
- Assert: Same exception is thrown  

---

## 4.4 DI Lifetime Tests

Validate:

- UoW is scoped  
- DbContext is scoped  
- Repositories and UoW share the same DbContext instance  

### Example

- Arrange: Create a DI scope  
- Resolve UoW and repository  
- Assert: Both reference the same DbContext instance  

---

# 5. Anti‑Patterns (Tests Must Reject)

- Tests that assume pre‑commit or post‑commit hooks exist  
- Tests that assume domain events are dispatched  
- Tests that assume retries occur  
- Tests that assume multiple providers exist  
- Tests that rely on static/global state  
- Tests that assume distributed transactions  

These features do **not** exist today.

---

# 6. Summary

**Current State:**  
Frank provides a minimal EF Core–based Unit of Work implementation.  
Testers validate:

- Commit behavior  
- Cancellation behavior  
- Exception propagation  
- DI lifetime rules  

**Future State:**  
The capability will expand to include:

- Multiple providers  
- Transaction orchestration  
- Domain event dispatching  
- Outbox integration  
- Observability  
- Resilience  
- Ambient scopes  

This Tester Guide prepares testers for both the current minimal implementation and the richer future capability.

