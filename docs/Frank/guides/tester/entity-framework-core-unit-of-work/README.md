# Frank — Tester Guide — Entity Framework Core Unit of Work  
*How to test the current and future Unit of Work capability.*

This guide documents the **current state** of the Unit of Work (UoW) capability in Frank and the **intended future state** as the capability evolves.

The Unit of Work pattern provides a **transactional boundary** for persistence operations.  
Today, Frank includes only a **single EF Core implementation**, which means the testing surface is small but well‑defined.  
Future enhancements will expand the behavior and therefore the testing responsibilities.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides:

````csharp
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
````

And a single implementation:

````csharp
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
````

---

## What exists today

- a contract (`IUnitOfWork`)  
- a single EF Core implementation (`EfUnitOfWork<TContext>`)  
- scoped DI registration  
- a single behavior: **CommitAsync → SaveChangesAsync**  

---

## What does *not* exist today

- no pre‑commit or post‑commit hooks  
- no domain event dispatching  
- no outbox integration  
- no distributed transactions  
- no retry or resilience layer  
- no alternative providers  
- no orchestration across multiple DbContexts  

---

## What testers can validate today

Testers validate:

- `CommitAsync` calls `SaveChangesAsync` exactly once  
- cancellation tokens are honored  
- exceptions propagate unchanged  
- DbContext lifetime rules are respected  
- UoW is scoped correctly  
- repositories and UoW share the same DbContext instance  

This is a **minimal testing surface**, but it must be validated thoroughly.

---

# 2. Intended Future State (After Capability Expansion)

As the Unit of Work capability evolves, testers will eventually validate:

---

## 2.1 Transaction orchestration

- pre‑commit hooks  
- post‑commit hooks  
- domain event dispatching  
- outbox message persistence  
- cross‑aggregate consistency  

---

## 2.2 Multiple persistence providers

- Dapper  
- MongoDB  
- Cosmos DB  
- Redis  
- EventStore  
- file‑based stores  

Each provider will require its own test suite.

---

## 2.3 Observability

- logging commit attempts  
- metrics for commit duration  
- failure tracking  
- tracing integration  

---

## 2.4 Resilience

- retry policies  
- circuit breakers  
- provider‑specific exception translation  

---

## 2.5 Ambient UoW scopes

- nested scopes  
- automatic enlistment of repositories  
- scope propagation  

These features will significantly expand the testing responsibilities.

---

# 3. Test Responsibilities (Current vs Future)

## Current Responsibilities

Testers must validate:

### **Commit behavior**
- `SaveChangesAsync` is called exactly once  
- return value matches EF Core’s result  

### **Cancellation behavior**
- cancellation token is passed through  
- operation cancels when requested  

### **Exception behavior**
- EF Core exceptions propagate unchanged  
- no exceptions are swallowed  

### **DI and lifetime behavior**
- UoW is scoped  
- DbContext is scoped  
- repositories and UoW share the same DbContext instance  

### **No additional behavior**
- no domain events  
- no retries  
- no pre/post commit logic  

---

## Future Responsibilities

Testers will validate:

- transaction orchestration  
- domain event dispatching  
- outbox integration  
- retry and resilience behavior  
- observability and logging  
- multi‑provider consistency  
- ambient UoW behavior  

---

# 4. Required Test Types (Current State)

## 4.1 Commit Behavior Tests

Validate:

- `CommitAsync` calls `SaveChangesAsync` exactly once  
- return value is passed through  
- no additional behavior occurs  

### Example

````csharp
// Arrange: Fake DbContext with a counter
// Act: Call CommitAsync
// Assert: Counter == 1
````

---

## 4.2 Cancellation Token Tests

Validate:

- cancellation token is passed to `SaveChangesAsync`  
- operation cancels when token is triggered  

### Example

````csharp
// Arrange: DbContext configured to delay
// Act: Cancel token before delay completes
// Assert: TaskCanceledException is thrown
````

---

## 4.3 Exception Propagation Tests

Validate:

- EF Core exceptions propagate unchanged  
- no wrapping or swallowing occurs  

### Example

````csharp
// Arrange: DbContext throws DbUpdateException
// Act: Call CommitAsync
// Assert: Same exception is thrown
````

---

## 4.4 DI Lifetime Tests

Validate:

- UoW is scoped  
- DbContext is scoped  
- repositories and UoW share the same DbContext instance  

### Example

````csharp
// Arrange: Create DI scope
// Resolve UoW and repository
// Assert: Both reference the same DbContext instance
````

---

# 5. Anti‑Patterns (Tests Must Reject)

Tests must reject assumptions about features that **do not exist today**:

- pre‑commit or post‑commit hooks  
- domain event dispatching  
- retries  
- multiple providers  
- static/global state  
- distributed transactions  

Tests must reflect the **current minimal implementation**, not the future one.

---

# 6. Summary

**Current State:**  
Frank provides a minimal EF Core–based Unit of Work implementation.  
Testers validate:

- commit behavior  
- cancellation behavior  
- exception propagation  
- DI lifetime rules  

**Future State:**  
The capability will expand to include:

- multiple providers  
- transaction orchestration  
- domain event dispatching  
- outbox integration  
- observability  
- resilience  
- ambient scopes  

This Tester Guide prepares testers for both the current minimal implementation and the richer future capability.
