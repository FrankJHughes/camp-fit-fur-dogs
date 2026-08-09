# Identity Application — OIDC Callback Save Phase

The **Save** folder contains all immutable‑context abstractions required to execute
the “save” phase of the OIDC callback pipeline.  
This phase is responsible for transforming normalized upstream identity data into
local application state, including:

- Resolving or creating a local user  
- Creating a session  
- Hashing the session token  
- Generating the cookie value  
- Determining the final redirect URL  

These abstractions define a deterministic, testable, and immutable pipeline for
completing the OIDC callback flow.

---

## Folder Structure

```
Save/
├── CallbackSaveContext.cs
├── CallbackSaveContextBuilderRequest.cs
├── CallbackSaveContextBuilderResult.cs
└── ICallbackSaveContextBuilder.cs
```

---

# CallbackSaveContext

Represents the **final immutable snapshot** of the save phase.

### Contains

- Upstream identity (`External`)
- Timestamp (`Now`) captured via the Clock abstraction
- Optional requested redirect URL
- Resolved user ID
- Created session ID
- Token hash
- Cookie value
- Final redirect URL

### Purpose

This context drives the final steps of the callback pipeline:

- Persisting identity and session data  
- Issuing cookies  
- Redirecting the user  
- Emitting audit events  

It is **pure**, **immutable**, and **serializable**.

---

# CallbackSaveContextBuilderRequest

Represents the **minimal inputs** required to begin the save phase.

### Contains

- Normalized upstream identity (`External`)
- Optional requested redirect URL
- Clock‑captured timestamp (`Now`)

### Purpose

This request is intentionally small and contains only the data needed to begin
application‑level processing.  
All domain logic is performed by the builder.

---

# CallbackSaveContextBuilderResult

Represents the **post‑processing application‑level artifacts** produced during the
save phase.

### Contains

- Resolved user ID  
- Created session ID  
- Token hash  
- Cookie value  

### Purpose

This result is produced *after* user resolution and session creation.  
It contains the final application‑level values needed to construct the immutable
`CallbackSaveContext`.

---

# ICallbackSaveContextBuilder

Defines the contract for constructing a `CallbackSaveContext` from:

1. A `CallbackSaveContextBuilderRequest`  
2. A `CallbackSaveContextBuilderResult`  
3. Application‑level save‑phase logic  

### Responsibilities

Implementations must:

- Resolve or create the local user  
- Create the session  
- Hash the session token  
- Generate the cookie value  
- Determine the final redirect URL  
- Produce the final immutable context  

### Architectural Role

This builder encapsulates the entire save‑phase pipeline and ensures:

- Deterministic behavior  
- Testability  
- Separation of concerns  
- Immutable context construction  

---

# Pipeline Overview

```
[ CallbackSaveContextBuilderRequest ]
            ↓
  (User Resolution + Session Creation)
            ↓
[ CallbackSaveContextBuilderResult ]
            ↓
  (Context Assembly)
            ↓
[ CallbackSaveContext ]
            ↓
  (Cookie Issuance + Redirect)
```

This structure ensures that the save phase is:

- Pure  
- Immutable  
- Replayable  
- Easy to test  
- Cleanly separated from protocol‑level OIDC concerns  

---

# Summary

The Save folder provides a complete immutable‑context model for the save phase of
the OIDC callback pipeline:

### **CallbackSaveContext**
Final immutable snapshot of the save operation.

### **CallbackSaveContextBuilderRequest**
Minimal upstream and pipeline‑captured inputs.

### **CallbackSaveContextBuilderResult**
Application‑level artifacts (user, session, token hash, cookie).

### **ICallbackSaveContextBuilder**
Contract for constructing the full save‑phase context.

Together, these abstractions form a deterministic, testable, and well‑structured
save pipeline within the Identity subsystem.

---
