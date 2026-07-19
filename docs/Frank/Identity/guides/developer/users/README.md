# Frank.Identity — Sessions  
## Overview

The **Sessions** subsystem defines the domain model, persistence, and vertical slices for managing authentication sessions in Frank.Identity.  
A *Session* represents an authenticated identity and is used by middleware, resolvers, and endpoints to determine who the current user is.

Sessions are intentionally simple:

- Represented as a domain aggregate (`Session`)
- Identified by a **hashed token** (`SessionTokenHash`)
- Linked to a user (`UserId`)
- Have a creation timestamp (`CreatedAt`)
- May be revoked (`RevokedAt`)
- Expire deterministically (`CreatedAt + TTL`)
- Persisted and retrieved through **slice‑aligned writers and readers**
- Stored in **FrankIdentityDbContext**
- Committed atomically via **FrankIdentityUnitOfWork**

This folder contains documentation for each slice of the session lifecycle, plus the domain model and EF Core configuration.

---

## Contents

### [create-session](./create-session/README.md)
Persists a newly created `Session` aggregate.  
The caller constructs the aggregate; the writer adds it to EF Core’s change tracker.  
Persistence is finalized by **FrankIdentityUnitOfWork**.

### [get-session](./get-session/README.md)
Retrieves a session by its hashed token.  
Used by authentication middleware and identity resolution.  
Computes expiration (`ExpiresAt = CreatedAt + TTL`) and returns `null` when missing.

### [revoke-session](./revoke-session/README.md)
Marks a session as revoked.  
Revocation is idempotent — revoking an already‑revoked session is a no‑op.  
Changes are persisted via **FrankIdentityUnitOfWork**.

---

# 1. Domain Layer  
## Session Aggregate

The `Session` aggregate represents an authenticated identity.

### Value Objects

- **SessionId** — unique identifier  
- **SessionTokenHash** — hashed session token  
- **UserId** — owner of the session  

### Properties

| Property     | Type                | Notes |
|--------------|---------------------|-------|
| `Id`         | `SessionId`         | Primary key |
| `TokenHash`  | `SessionTokenHash`  | Unique; used for lookup |
| `OwnerId`    | `UserId`            | Required |
| `CreatedAt`  | `DateTimeOffset`    | Required |
| `RevokedAt`  | `DateTimeOffset?`   | Optional |

### Behavior

```csharp
session.Revoke(DateTimeOffset.UtcNow);
```

- Sets `RevokedAt`
- Idempotent — calling twice does nothing
- Revoked sessions are invalid for authentication

---

# 2. EntityFrameworkCore Layer  
## 2.1 SessionConfiguration

The EF Core configuration ensures correct persistence of the `Session` aggregate and its value objects.

### Table: `sessions`

### Mapping Summary

| Column        | VO / Type             | Notes |
|---------------|------------------------|-------|
| `id`          | `SessionId`            | Stored as primitive |
| `token_hash`  | `SessionTokenHash`     | Required + unique |
| `owner_id`    | `UserId`               | Required |
| `created_at`  | `DateTimeOffset`       | Required |
| `revoked_at`  | `DateTimeOffset?`      | Optional |

### Configuration Highlights

```csharp
builder.Property(s => s.Id)
    .HasConversion(id => id.Value, value => SessionId.From(value))
    .HasColumnName("id");

builder.Property(s => s.TokenHash)
    .HasConversion(v => v.Value, v => SessionTokenHash.From(v))
    .HasColumnName("token_hash")
    .IsRequired();

builder.HasIndex(s => s.TokenHash)
    .IsUnique();

builder.Property(s => s.OwnerId)
    .HasConversion(v => v.Value, v => UserId.From(v))
    .HasColumnName("owner_id")
    .IsRequired();

builder.Property(s => s.CreatedAt)
    .HasColumnName("created_at")
    .IsRequired();

builder.Property(s => s.RevokedAt)
    .HasColumnName("revoked_at")
    .IsRequired(false);
```

### Notes

- All value objects are stored as primitives.
- `token_hash` is unique — only one active session per token.
- EF Core tracks changes; **FrankIdentityUnitOfWork** commits them.

---

## 2.2 FrankIdentityDbContext

The EF Core DbContext for all Identity persistence.

### Responsibilities

- Expose `DbSet<Session>`
- Apply `SessionConfiguration`
- Provide change tracking for writers
- Serve as the persistence boundary for all session slices

### Notes

- Writers add aggregates to the DbContext  
- Readers query aggregates from the DbContext  
- DbContext does **not** commit — the UnitOfWork does

---

## 2.3 FrankIdentityUnitOfWork

The unit of work ensures atomic persistence across slices.

### Responsibilities

- Commit all EF Core changes
- Provide transactional boundaries
- Used by all writers (Create, Revoke)

### API

```csharp
Task CommitAsync(CancellationToken ct);
```

### Notes

- Writers never call `SaveChangesAsync`  
- UnitOfWork is the **only** component allowed to commit  
- Supports batching (e.g., user creation + session creation)

---

# 3. Slice Layer  
The Sessions subsystem consists of three slices:

- **CreateSession** — persist a new session  
- **GetSession** — retrieve a session by token hash  
- **RevokeSession** — mark a session as revoked  

Each slice uses:

- **Slice‑aligned writers** (Create, Revoke)  
- **Slice‑aligned readers** (Get)  
- **FrankIdentityDbContext**  
- **FrankIdentityUnitOfWork**

---

# 4. Unified Session Lifecycle (Sequence Diagram)

```mermaid
sequenceDiagram
    autonumber

    participant CALLER as Application Caller
    participant WRITER as Slice-Aligned Writers (Create/Revoke)
    participant READER as Slice-Aligned Reader (Get)
    participant DB as FrankIdentityDbContext
    participant UOW as FrankIdentityUnitOfWork

    %% Creation
    CALLER->>CALLER: 1. Construct Session aggregate
    CALLER->>WRITER: 2. WriteAsync(session)
    WRITER->>DB: 3. Add Session to DbContext
    DB-->>WRITER: 4. Track new entity
    CALLER->>UOW: 5. CommitAsync()
    UOW->>DB: 6. SaveChanges persists Session

    %% Retrieval
    CALLER->>READER: 7. GetSessionAsync(tokenHash)
    READER->>DB: 8. Query Session by TokenHash
    DB-->>READER: 9. Return Session or null
    READER->>READER: 10. Compute ExpiresAt = CreatedAt + TTL
    READER-->>CALLER: 11. Return GetSessionResponse

    %% Revocation
    CALLER->>WRITER: 12. RevokeAsync(tokenHash)
    WRITER->>DB: 13. Query Session by TokenHash
    DB-->>WRITER: 14. Return Session or null
    WRITER->>WRITER: 15. If found → session.Revoke(now)
    CALLER->>UOW: 16. CommitAsync()
    UOW->>DB: 17. SaveChanges persists revocation
```

---

# Related Settings

### `SessionSettings`

| Setting | Description |
|---------|-------------|
| `Ttl`   | Time‑to‑live for sessions; used to compute `ExpiresAt` |

---

# Usage in Identity

Sessions are used by:

- **Login callback pipeline** — creates new sessions  
- **Identity resolver** — loads session to hydrate `ICurrentUser`  
- **Logout** — deletes session cookie  
- **Session revocation** — invalidates sessions  
- **Session refresh** (future slice) — extends TTL  

---

# Philosophy

Frank.Identity sessions are:

- **Minimal** — no complex token formats  
- **Domain‑driven** — aggregates + value objects  
- **Explicit** — revocation is a domain action  
- **Composable** — used by multiple slices  
- **Predictable** — expiration is deterministic (`CreatedAt + TTL`)  
- **Atomic** — persistence always flows through `FrankIdentityUnitOfWork`  
- **Consistent** — all persistence flows through `FrankIdentityDbContext`
