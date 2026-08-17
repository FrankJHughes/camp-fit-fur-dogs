# Identity Application — Users / GetUserByExternalId

The **GetUserByExternalId** folder contains the application‑layer abstractions
required to resolve an internal user record using an external identity provider’s
unique identifier (e.g., OIDC `sub` claim).

This lookup is a core part of authentication and onboarding flows, allowing the
system to map an external identity to an internal user without exposing any
external‑provider details beyond the lookup boundary.

All files in this folder are **pure application‑layer contracts** or **immutable
response models**.  
Infrastructure concerns (database queries, caching, normalization) are handled in
the infrastructure layer.

---

## Folder Structure

```
GetUserByExternalId/
├── GetUserByExternalIdResponse.cs
└── IGetUserByExternalIdReader.cs
```

---

# GetUserByExternalIdResponse

Represents the result of resolving a user by their external identity provider ID.

### Responsibilities

- Expose the internal `Guid` user identifier  
- Provide a minimal, safe response model for authentication flows  
- Avoid leaking external identity provider details  

### Notes

- The response is intentionally minimal  
- If no user is found, the reader returns `null` instead of this model  
- Used by onboarding, login, and session‑creation pipelines

---

# IGetUserByExternalIdReader

Defines the contract for retrieving a user using an external identity provider ID.

### Responsibilities

- Perform lookup by external ID  
- Return `GetUserByExternalIdResponse?`  
- Support cancellation  
- Abstract away all persistence and normalization concerns  

### Notes

- Reader implementations may enforce:
  - External ID normalization  
  - Uniqueness constraints  
  - Database or cache lookup  
  - Transactional consistency  
- The reader performs no domain logic beyond lookup  
- If no user exists for the external ID, it returns `null`

---

# External‑ID Lookup Pipeline Overview

```
[ External Identity Provider ID ]
                ↓
IGetUserByExternalIdReader
                ↓
[ User Found ] → GetUserByExternalIdResponse
[ Not Found ] → null
```

This pipeline ensures:

- Clean separation of concerns  
- Minimal, safe exposure of internal identifiers  
- Deterministic lookup behavior  
- Infrastructure‑agnostic design  

---

# Summary

The GetUserByExternalId folder defines the complete application‑layer abstraction
for resolving users by external identity provider identifiers:

### **GetUserByExternalIdResponse**
Minimal model exposing only the internal user ID.

### **IGetUserByExternalIdReader**
Lookup contract for resolving users by external identity provider ID.

Together, these abstractions form a clean, testable, and deterministic external‑ID
lookup subsystem within Identity.

---
