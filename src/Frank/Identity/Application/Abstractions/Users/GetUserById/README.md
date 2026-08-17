# Identity Application — Users / GetUserById

The **GetUserById** folder contains the application‑layer abstractions required to
resolve a user using their internal unique identifier (`Guid`).  
This lookup is foundational for authorization checks, profile retrieval, and
owner‑centric workflows throughout the Identity subsystem.

All files in this folder are **pure application‑layer contracts** or **immutable
response models**.  
Infrastructure concerns (database queries, caching, normalization) are handled in
the infrastructure layer.

---

## Folder Structure

```
GetUserById/
├── GetUserByIdResponse.cs
└── IGetUserByIdReader.cs
```

---

# GetUserByIdResponse

Represents the minimal set of user information returned when resolving a user by
their internal ID.

### Responsibilities

- Expose the internal `Guid` user identifier  
- Provide basic profile information:
  - First name  
  - Last name  
- Serve upstream flows such as:
  - Authentication  
  - Authorization  
  - Owner‑centric UI rendering  

### Notes

- The response intentionally avoids exposing sensitive or optional fields  
- Additional user details should be retrieved through dedicated queries  
- If no user is found, the reader returns `null` instead of this model  

---

# IGetUserByIdReader

Defines the contract for retrieving a user using their internal unique identifier.

### Responsibilities

- Perform lookup by internal `Guid` user ID  
- Return `GetUserByIdResponse?`  
- Support cancellation  
- Abstract away all persistence and normalization concerns  

### Notes

- Reader implementations may enforce:
  - Database or cache lookup  
  - Identifier normalization  
  - Transactional consistency  
- The reader performs no domain logic beyond lookup  
- If no user exists for the ID, it returns `null`

---

# Internal‑ID Lookup Pipeline Overview

```
[ Internal UserId (Guid) ]
                ↓
IGetUserByIdReader
                ↓
[ User Found ] → GetUserByIdResponse
[ Not Found ] → null
```

This pipeline ensures:

- Clean separation of concerns  
- Minimal, safe exposure of user information  
- Deterministic lookup behavior  
- Infrastructure‑agnostic design  

---

# Summary

The GetUserById folder defines the complete application‑layer abstraction for
resolving users by internal unique identifier:

### **GetUserByIdResponse**
Minimal model exposing basic profile information.

### **IGetUserByIdReader**
Lookup contract for resolving users by internal `Guid` identifier.

Together, these abstractions form a clean, testable, and deterministic internal‑ID
lookup subsystem within Identity.

---
