# Frank.Identity — RevokeSession Slice  
## Developer Guide

The **RevokeSession** slice revokes an existing session by marking it as no longer valid.  
It is part of the internal authentication infrastructure and is used by logout flows, admin tools, and session‑management middleware.

It spans:

- **Application** — `RevokeSessionHandler`
- **Domain** — `Session`, `SessionTokenHash`
- **Infrastructure** — EF Core repository (`SessionRepository`)
- **Unit of Work** — `IFrankIdentityUnitOfWork`

---

# 1. End‑to‑End Execution Flow (Swimlane Diagram)

```mermaid
flowchart LR
    %% Lanes
    subgraph APP["Application Layer"]
        A1["1. Command: RevokeSessionCommand(tokenHash)"]
        A2["2. Handler converts tokenHash → SessionTokenHash VO"]
        A3["3. Handler invokes ISessionRepository.RevokeAsync"]
        A4["4. Handler commits unit of work"]
    end

    subgraph INFRA["Infrastructure (EF Core)"]
        I1["Reader queries DB by TokenHash"]
        I2["If found → session.Revoke(now)"]
        I3["EF tracks changes; SaveChanges via UnitOfWork"]
    end

    %% Flow
    A1 --> A2 --> A3 --> I1 --> I2 --> I3 --> A4
```

---

# 2. RevokeSessionHandler (Application Layer)

**Location:**

```
Frank.Identity.Application.Sessions.RevokeSession.RevokeSessionHandler.cs
```

### Responsibilities

- Handle `RevokeSessionCommand`
- Convert raw token hash → `SessionTokenHash` value object
- Delegate revocation to `ISessionRepository`
- Commit changes via `IFrankIdentityUnitOfWork`

### Key Method

```csharp
public async Task HandleAsync(RevokeSessionCommand command, CancellationToken cancellationToken)
{
    var tokenHash = SessionTokenHash.From(command.TokenHash);

    await _repository.RevokeAsync(tokenHash, cancellationToken);
    await _unitOfWork.CommitAsync(cancellationToken);
}
```

### Developer Notes

- Handler does not check whether the session exists.
- If the session does not exist, revocation is a no‑op.
- This design keeps revocation idempotent and safe.

---

# 3. ISessionRepository (Application Abstraction)

**Purpose:**

- Abstracts session persistence
- Allows EF Core or other stores to implement revocation
- Exposes:

```csharp
Task RevokeAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken);
```

---

# 4. SessionRepository (Infrastructure Layer)

**Location:**

```
Frank.Identity.EntityFrameworkCore.Sessions.SessionRepository.cs
```

### Responsibilities

- Query EF Core for a session by `SessionTokenHash`
- Invoke domain behavior (`session.Revoke`)
- Allow EF Core change tracking to persist the update

### Key Method

```csharp
public async Task RevokeAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken)
{
    var session = await _db.Set<Session>()
        .SingleOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);

    if (session is null)
        return;

    // Domain behavior
    session.Revoke(DateTimeOffset.UtcNow);

    // EF will track the change; SaveChanges is handled by the unit of work
}
```

### Developer Notes

- Revocation is **domain‑driven**: the aggregate enforces its own invariants.
- EF Core tracks the updated `RevokedAt` timestamp.
- Unit of work commits the change.

---

# 5. Domain Model (Session Aggregate)

### Relevant Value Objects

- `SessionId`
- `SessionTokenHash`
- `UserId`

### Relevant Properties

- `Id`
- `TokenHash`
- `OwnerId`
- `CreatedAt`
- `RevokedAt`

### Domain Behavior

```csharp
session.Revoke(DateTimeOffset.UtcNow);
```

### Developer Notes

- Revocation sets `RevokedAt` to a timestamp.
- A revoked session is considered invalid for authentication.
- Revocation is idempotent — calling it twice does not change behavior.

---

# 6. Unit of Work

The handler commits changes using:

```csharp
await _unitOfWork.CommitAsync(cancellationToken);
```

### Developer Notes

- Ensures revocation is persisted atomically.
- Allows batching multiple operations if needed.
- Keeps EF Core persistence concerns out of the handler.

---

# 7. Error Handling

### Session Not Found

- Repository returns silently.
- Handler commits no changes.
- No exception is thrown.

### Why no exception?

- Revocation is intentionally idempotent.
- Attempting to revoke a non‑existent session is not considered an error.

---

# 8. Testing Strategy

## Unit Tests

- Handler:
  - Correct conversion of token hash → VO
  - Repository invoked with correct VO
  - Unit of work commit invoked

- Repository:
  - Session found → `RevokedAt` updated
  - Session missing → no exception, no update
  - EF Core tracks changes correctly

## Integration Tests

- Revoking an existing session sets `RevokedAt`
- Revoking a missing session is a no‑op
- Unit of work commits changes
- Revoked sessions are treated as invalid by authentication middleware

---

# 9. Summary

The **RevokeSession** slice:

- Converts raw token hash → `SessionTokenHash`
- Loads session via repository
- Applies domain revocation behavior
- Persists changes via unit of work
- Treats missing sessions as a no‑op

It is a core building block of Frank.Identity’s session lifecycle and is used by logout flows, admin tools, and session‑validation middleware.

