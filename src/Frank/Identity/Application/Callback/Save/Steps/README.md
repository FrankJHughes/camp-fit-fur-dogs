# Identity Application — Save Pipeline Steps (`tdeps`)

The **tdeps** folder contains the immutable‑context build steps that make up the
**Save** portion of the authentication callback pipeline.  
These steps run *after* the OIDC callback pipeline has produced a validated,
enriched external identity context. Their responsibility is to:

1. Resolve the internal user  
2. Generate a session token + cookie  
3. Persist the authenticated session  
4. Emit audit logs for the successful login  

All steps follow the immutable‑context pattern:

- Each step receives a `CallbackSaveContext`
- Produces a new enriched context
- Never mutates existing state
- Executes only when its `CanExecute` condition is satisfied

This ensures deterministic behavior, strong correctness guarantees, and
excellent observability.

---

## Folder Structure

```
tdeps/
├── ResolveUserStep.cs
├── BuildCookieStep.cs
├── CreateSessionStep.cs
└── AuditLoginStep.cs
```

---

# ResolveUserStep

Resolves the internal user ID from the external identity provider information.

### Responsibilities

- Execute only when `UserId` is null  
- Use `IUserResolver` to map external identity → internal user  
- Populate:
  - `UserId`

### Notes

- Always runs first in the Save pipeline  
- Produces a new immutable context containing the resolved user ID  

---

# BuildCookieStep

Generates a new session token and constructs the authentication cookie value.

### Responsibilities

- Execute only when `CookieValue` is null  
- Use `ISessionTokenGenerator` to produce:
  - Plaintext token
  - Hashed token  
- Build cookie value using `SessionCookie.FromPlaintextToken`  
- Populate:
  - `TokenHash`
  - `CookieValue`

### Notes

- Runs exactly once  
- Produces a new immutable context containing the token hash + cookie value  

---

# CreateSessionStep

Creates and persists a new authenticated session.

### Responsibilities

- Execute only when:
  - `UserId` is present  
  - `TokenHash` is present  
  - `SessionId` is null  
- Construct a new `Session` domain object  
- Persist via `ICreateSessionWriter`  
- Commit via `IFrankIdentityUnitOfWork`  
- Populate:
  - `SessionId`

### Notes

- Performs external side effects (database write + commit)  
- Returns a new immutable context containing the session ID  

---

# AuditLoginStep

Emits an audit log for the successful login.

### Responsibilities

- Execute only when `UserId` is present  
- Use `IAuditLogger.LoginSucceeded` to record:
  - Internal user ID  
  - External subject ID  

### Notes

- Does not modify the context  
- Performs an external side effect (audit logging)  
- Always runs after user resolution  

---

# Pipeline Overview

```
[ External Identity ]
        ↓
ResolveUserStep
        ↓
[ UserId ]
        ↓
BuildCookieStep
        ↓
[ TokenHash, CookieValue ]
        ↓
CreateSessionStep
        ↓
[ SessionId ]
        ↓
AuditLoginStep
        ↓
[ Final Save Result ]
```

This pipeline ensures:

- Deterministic Save‑phase processing  
- Immutable context transformations  
- Strong correctness guarantees  
- Clear separation of responsibilities  
- Full observability of each step  

---

# Summary

The **tdeps** folder defines the complete Save‑phase pipeline:

### Steps
- **ResolveUserStep** — map external identity → internal user  
- **BuildCookieStep** — generate session token + cookie  
- **CreateSessionStep** — persist authenticated session  
- **AuditLoginStep** — record successful login  

Together, these steps form a clean, testable, and predictable Save pipeline
within the Identity subsystem.

---
