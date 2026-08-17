# Identity Application — RevokeSession

The **RevokeSession** folder contains the application‑layer components responsible
for revoking an authenticated session using its token hash.  
This functionality is used during logout, forced session invalidation, and
administrative session revocation.

The folder follows the CQRS command pattern:

- A **command** (`RevokeSessionCommand`)
- A **handler** (`RevokeSessionHandler`)
- A **writer abstraction** (`IRevokeSessionWriter`)
- A **unit of work** (`IFrankIdentityUnitOfWork`)

This separation ensures clarity, testability, and a clean boundary between
application logic and persistence.

---

## Folder Structure

```
RevokeSession/
├── RevokeSessionHandler.cs
```

(Other abstractions such as `RevokeSessionCommand` and `IRevokeSessionWriter`
live in the Abstractions layer.)

---

# RevokeSessionHandler

The handler responsible for revoking a session by its token hash.

### Responsibilities

- Accept a `RevokeSessionCommand`
- Convert the raw token hash into a domain `SessionTokenHash`
- Use `IRevokeSessionWriter` to persist the revocation
- Commit the change using `IFrankIdentityUnitOfWork`

### Notes

- The handler performs external side effects (write + commit)
- It does not return a value — revocation is a fire‑and‑forget operation
- Used during:
  - Logout flows
  - Forced session invalidation
  - Administrative session cleanup

---

# Command Flow

```
[ TokenHash ]
     ↓
RevokeSessionCommand
     ↓
RevokeSessionHandler
     ↓
IRevokeSessionWriter.WriteAsync()
     ↓
UnitOfWork.CommitAsync()
     ↓
[ Session Revoked ]
```

---

# Abstractions (in Identity.Application.Abstractions)

### **RevokeSessionCommand**
Represents a request to revoke a session by token hash.

### **IRevokeSessionWriter**
Abstraction for writing session revocation operations to the persistence layer.

### **SessionTokenHash**
Domain value object representing a hashed session token.

### **IFrankIdentityUnitOfWork**
Coordinates transactional persistence for session operations.

---

# Summary

The RevokeSession folder defines the application‑layer handler for revoking
sessions:

### Core Handler
- `RevokeSessionHandler`

### Abstractions (external to this folder)
- `RevokeSessionCommand`
- `IRevokeSessionWriter`
- `SessionTokenHash`
- `IFrankIdentityUnitOfWork`

Together, these components form a clean, testable, and predictable session
revocation flow within the Identity subsystem.

---
