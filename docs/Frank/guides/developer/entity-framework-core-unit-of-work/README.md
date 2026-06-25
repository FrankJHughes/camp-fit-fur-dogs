# Frank — Developer Guide — Entity Framework Core Unit of Work  
*Current state and future direction of Frank’s Unit of Work capability.*

This guide documents the **current state** of the Unit of Work (UoW) capability in Frank and the **intended future state** as the platform evolves.

The Unit of Work pattern coordinates **transactional consistency** across a persistence boundary.  
In Frank, the only implementation today is based on **Entity Framework Core**, but the contract is intentionally **persistence‑agnostic**.

---

# 1. Current State (Before Future Enhancements)

Frank currently defines:

````csharp
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
````

And provides a single implementation:

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

- a **contract** (`IUnitOfWork`)  
- a **single EF Core implementation** (`EfUnitOfWork<TContext>`)  
- auto‑registration for the contract and implementation  
- scoped lifetime semantics  
- a simple transactional boundary:  
  **CommitAsync → DbContext.SaveChangesAsync**  

---

## What does *not* exist today

- no cross‑aggregate transaction orchestration  
- no distributed transaction support  
- no outbox pattern integration  
- no ambient UoW scopes  
- no retry policies or resilience layer  
- no non‑EF Core providers  
- no pipeline behaviors or pre/post commit hooks  

---

## Developer implications today

- UoW is a **thin wrapper** around EF Core’s `SaveChangesAsync`  
- transaction boundaries are defined by DI scope  
- all transactional consistency is delegated to EF Core  
- no additional behavior is injected before or after commit  

The current capability is intentionally minimal.

---

# 2. Intended Future State (After Capability Expansion)

The long‑term vision for the Unit of Work capability includes:

---

## 2.1 Multiple persistence providers

Future implementations may include:

- Dapper  
- MongoDB  
- Cosmos DB  
- Redis  
- EventStore  
- file‑based stores  
- outbox‑enabled stores  

Each provider would implement `IUnitOfWork` with provider‑specific semantics.

---

## 2.2 Transaction orchestration

Future UoW may support:

- pre‑commit hooks  
- post‑commit hooks  
- domain event dispatching after commit  
- outbox message persistence  
- retry policies  
- transaction scopes across multiple repositories  

---

## 2.3 Ambient Unit of Work

Potential future support for:

- nested scopes  
- ambient context propagation  
- automatic enlistment of repositories  

---

## 2.4 Observability

Future enhancements may include:

- logging commit attempts  
- metrics for commit duration  
- failure tracking  
- tracing integration  

---

## 2.5 Error handling and resilience

Potential future features:

- retry policies  
- circuit breakers  
- provider‑specific exception translation  

---

# 3. Developer Responsibilities (Current vs Future)

## Current Responsibilities

Developers must:

- inject `IUnitOfWork` where transactional boundaries are needed  
- ensure DbContext is scoped correctly  
- ensure repositories operate within the same DI scope  
- call `CommitAsync` explicitly  
- handle exceptions thrown by EF Core  
- understand that UoW is a thin wrapper today  

---

## Future Responsibilities

Once the capability expands, developers will:

- use UoW as the central transactional boundary  
- rely on UoW for domain event dispatching  
- use UoW for outbox message persistence  
- use UoW for cross‑aggregate consistency  
- gain observability and resilience features automatically  

---

# 4. Architecture and Invariants

## 4.1 Contract Invariants

`IUnitOfWork` guarantees:

- a single method: `CommitAsync`  
- commit is explicit, not implicit  
- commit returns the number of affected rows (EF Core behavior)  
- commit is atomic within the DbContext transaction boundary  

---

## 4.2 Implementation Invariants (EF Core)

`EfUnitOfWork<TContext>` guarantees:

- one DbContext instance per scope  
- commit delegates to `SaveChangesAsync`  
- no additional behavior is injected  
- no domain events are dispatched  
- no retries or resilience policies are applied  

---

## 4.3 DI Invariants

- UoW is **scoped**  
- DbContext is **scoped**  
- repositories must be scoped  
- all must share the same DI scope to participate in the same transaction  

---

# 5. Example Usage (Developer Perspective)

## Injecting UoW

````csharp
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
````

## What happens under the hood

- repository writes to DbContext  
- UoW calls `SaveChangesAsync`  
- EF Core commits the transaction  

---

# 6. Summary

**Current State:**  
Frank provides a simple, EF Core–based Unit of Work implementation.  
It wraps `DbContext.SaveChangesAsync` and defines a transactional boundary per DI scope.

**Future State:**  
The Unit of Work capability will expand to support:

- multiple persistence providers  
- transaction orchestration  
- domain event dispatching  
- outbox integration  
- observability and resilience  
- ambient scopes  

As a developer:

- today, you use UoW as a thin EF Core wrapper  
- in the future, UoW will become a full transactional orchestration capability  
