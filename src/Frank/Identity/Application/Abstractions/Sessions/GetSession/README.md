# Identity Application — Sessions / GetSession

The **GetSession** folder contains all application‑layer abstractions required to
retrieve and evaluate an authenticated session based on its token hash.

This subsystem is part of the Identity authentication pipeline and is responsible
for resolving persisted sessions, evaluating their status deterministically, and
exposing a safe, immutable representation to downstream authentication and
authorization components.

All files in this folder are **application‑layer contracts and immutable models**.
Infrastructure concerns (database, cache, distributed session store) are handled
in the infrastructure layer.

---

## Folder Structure

```
GetSession/
├── GetSessionQuery.cs
├── GetSessionResponse.cs
└── IGetSessionReader.cs
```

---

# GetSessionQuery

Represents the CQRS query used to retrieve a session by its token hash.

### Responsibilities

- Carry the secure, non‑reversible token hash  
- Trigger the session‑retrieval pipeline  
- Return `GetSessionResponse?` (nullable)  

### Notes

- Only the **hash** is stored; the raw token is never persisted  
- The query handler injects `IClock` and captures `EvaluatedAt`  

---

# GetSessionResponse

Immutable application‑level representation of a resolved session.

### Contains

- `Id` — session identifier  
- `OwnerId` — user identifier  
- `CreatedAt` — creation timestamp  
- `RevokedAt` — revocation timestamp (nullable)  
- `ExpiresAt` — expiration timestamp  
- `EvaluatedAt` — timestamp captured by the query handler using `IClock`  

### Deterministic Evaluation

All status checks use **EvaluatedAt**, not ambient time:

- `IsExpired` — `ExpiresAt <= EvaluatedAt`  
- `IsRevoked` — `RevokedAt != null`  
- `IsActive` — not expired and not revoked  

### Why this design?

- Fully immutable  
- Replayable  
- Deterministic  
- Consistent with the Identity subsystem’s immutable‑context architecture  
- No ambient `DateTimeOffset.UtcNow`  

---

# IGetSessionReader

Defines the contract for retrieving a session from persistent storage.

### Responsibilities

- Lookup session by token hash  
- Return `GetSessionResponse?`  
- Support cancellation  
- Apply storage‑level concerns (DB, cache, distributed store)  

### Notes

- Does **not** evaluate expiration or revocation  
- That logic is handled by `GetSessionResponse` using `EvaluatedAt`  
- Reader implementations must ensure secure lookup and null‑return semantics  

---

# Session Retrieval Pipeline Overview

```
[ Token Hash ]
       ↓
GetSessionQuery
       ↓
IGetSessionReader
       ↓
[ Session Data ]
       ↓
Query Handler (injects IClock)
       ↓
GetSessionResponse (EvaluatedAt captured)
       ↓
[ IsExpired / IsRevoked / IsActive ]
```

This pipeline ensures:

- Deterministic evaluation  
- Immutable session state  
- Replayable authentication flows  
- Clean separation of application vs infrastructure concerns  

---

# Summary

The GetSession folder defines the complete application‑layer abstraction for
session retrieval:

### **GetSessionQuery**
Carries the token hash into the pipeline.

### **IGetSessionReader**
Resolves the session from persistent storage.

### **GetSessionResponse**
Provides deterministic, immutable session evaluation using `EvaluatedAt`.

Together, these abstractions form a clean, testable, and replayable session‑retrieval subsystem within Identity.

---
