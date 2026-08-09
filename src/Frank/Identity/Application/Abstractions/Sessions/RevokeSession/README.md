# Identity Application — Sessions / RevokeSession

The **RevokeSession** folder contains the application‑layer abstractions required
to revoke an authenticated session based on its token hash.

Session revocation is part of the Identity subsystem’s security pipeline.  
It ensures that once a session is invalidated, any future authentication attempts
using the associated token are rejected.

All files in this folder are **application‑layer contracts and immutable models**.
Infrastructure concerns (database updates, cache invalidation, distributed
revocation) are implemented in the infrastructure layer.

---

## Folder Structure

```
RevokeSession/
├── RevokeSessionCommand.cs
└── IRevokeSessionWriter.cs
```

---

# RevokeSessionCommand

Represents the CQRS command used to revoke a session.

### Responsibilities

- Carry the secure, non‑reversible token hash  
- Trigger the session‑revocation pipeline  
- Ensure deterministic, auditable revocation behavior  

### Notes

- The raw session token is never persisted or exposed  
- The command handler injects `IClock` to capture the revocation timestamp  
- The handler delegates persistence to `IRevokeSessionWriter`

---

# IRevokeSessionWriter

Defines the contract for marking a session as revoked in persistent storage.

### Responsibilities

- Locate the session using the token hash  
- Mark the session as revoked  
- Persist the revocation timestamp  
- Support cancellation  
- Ensure durable, atomic write semantics  

### Notes

- Infrastructure implementations may use SQL, NoSQL, Redis, or distributed caches  
- The writer does **not** evaluate expiration or active status  
- Those rules are handled by the session‑retrieval pipeline (`GetSessionResponse`)

---

# Session Revocation Pipeline Overview

```
[ Token Hash ]
       ↓
RevokeSessionCommand
       ↓
Command Handler (injects IClock)
       ↓
IRevokeSessionWriter
       ↓
[ Session marked revoked ]
       ↓
Authentication pipeline rejects future requests
```

This pipeline ensures:

- Deterministic revocation timestamps  
- Immutable auditability  
- Clean separation of application vs infrastructure concerns  
- Consistent behavior across all session‑related subsystems  

---

# Summary

The RevokeSession folder defines the complete application‑layer abstraction for
session revocation:

### **RevokeSessionCommand**
Carries the token hash into the revocation pipeline.

### **IRevokeSessionWriter**
Persists the revocation in durable storage.

Together, these abstractions form a clean, testable, and deterministic
session‑revocation subsystem within Identity.

---
