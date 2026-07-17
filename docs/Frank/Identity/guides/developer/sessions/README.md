# Frank.Identity — Sessions  
## Overview

The **Sessions** subsystem provides the persistence and lifecycle management for authentication sessions in Frank.Identity.  
A session represents an authenticated identity and is used by middleware, resolvers, and endpoints to determine who the current user is.

Sessions are intentionally simple:

- Represented as a domain aggregate (`Session`)
- Identified by a **hashed token** (`SessionTokenHash`)
- Have a **TTL** (`SessionSettings.Ttl`)
- May be **revoked** (`RevokedAt`)
- Are used to hydrate `ICurrentUser` during request processing

This folder contains documentation for each slice of the session lifecycle, plus the domain model and EF Core configuration.

---

## Contents

### [create-session](./create-session/README.md)
Persists a newly created `Session` aggregate.  
The caller constructs the aggregate; the repository simply adds it to EF Core’s change tracker.  
Persistence is finalized by the unit of work.

### [get-session](./get-session/README.md)
Retrieves a session by its hashed token.  
Used by authentication middleware and identity resolution.  
Computes expiration (`ExpiresAt = CreatedAt + TTL`) and throws `SessionNotFoundException` when missing.

### [revoke-session](./revoke-session/README.md)
Marks a session as revoked.  
Revocation is idempotent — revoking a non‑existent session is a no‑op.  
Changes are persisted via the unit of work.

---

# Domain Aggregate: `Session`

The `Session` aggregate represents an authenticated identity.

### Value Objects

- **SessionId** — unique identifier for the session  
- **SessionTokenHash** — hashed session token  
- **UserId** — owner of the session  

### Properties

| Property     | Type                | Description |
|--------------|---------------------|-------------|
| `Id`         | `SessionId`         | Unique session identifier |
| `TokenHash`  | `SessionTokenHash`  | Hashed token used for lookup |
| `OwnerId`    | `UserId`            | User who owns the session |
| `CreatedAt`  | `DateTimeOffset`    | When the session was created |
| `RevokedAt`  | `DateTimeOffset?`   | When the session was revoked (null if active) |

### Behavior

```csharp
session.Revoke(DateTimeOffset.UtcNow);
```

- Sets `RevokedAt`
- Idempotent — calling twice does nothing
- Revoked sessions are invalid for authentication

---

# EF Core Configuration

The EF Core configuration ensures correct persistence of the `Session` aggregate and its value objects.

### Table: `sessions`

### Mapping Summary

| Column        | VO / Type             | Notes |
|---------------|------------------------|-------|
| `id`          | `SessionId`            | Stored as primitive, reconstructed as VO |
| `token_hash`  | `SessionTokenHash`     | Unique index; required |
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

- All value objects are stored as primitive values.
- `token_hash` is unique — only one active session per token.
- EF Core tracks changes; unit of work commits them.

---

# Session Lifecycle Summary

```mermaid
flowchart LR
    A["Session Created"] --> B["Session Stored (create-session)"]
    B --> C["Session Retrieved (get-session)"]
    C --> D["Session Used for Authentication"]
    D --> E["Session Revoked (revoke-session)"]
    E --> F["Session Invalid"]
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

