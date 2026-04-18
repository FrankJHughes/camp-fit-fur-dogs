# ADR-0017: Unit of Work Pattern

## Status

Accepted

## Context

Prior to this change, each repository called `SaveChangesAsync` after its own `AddAsync`. This meant:

- No transactional boundary — if a handler called two repositories, each persisted independently.
- Repositories had persistence side-effects, violating the collection-abstraction intent of DDD repositories.
- Testing persistence behavior required mocking `SaveChangesAsync` inside each fake repository.

## Decision

Introduce an `IUnitOfWork` interface (`CommitAsync`) in the Application.Abstractions layer. Handlers call it once after all repository operations. Repositories become pure in-memory collection facades — they stage changes on the `DbContext` change tracker but never flush to the database.

`EfUnitOfWork` in Infrastructure delegates to `AppDbContext.SaveChangesAsync`, giving the handler explicit control over the transactional boundary.

## Consequences

- **Handlers own persistence.** Every command handler ends with `await _unitOfWork.CommitAsync(ct)`.
- **Repos are side-effect-free.** Easier to reason about, easier to fake.
- **Infrastructure tests must call `SaveChangesAsync` directly** when testing repo behavior outside of a handler.
- **DI registration is explicit.** `EfUnitOfWork` doesn't match Scrutor's suffix scan (`Repository`, `Service`, `Provider`), so it's registered manually with `AddScoped`.
- **Future benefit:** cross-aggregate operations in a single handler automatically share a transaction.
