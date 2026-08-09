# Identity Application — OIDC Callback Pipeline

The **Callback** folder contains all immutable‑context abstractions and builders
required to execute the full OIDC callback pipeline within the Identity
subsystem.

The pipeline is split into two phases:

1. **OIDC Phase** — protocol‑level processing  
   - Exchange authorization code for tokens  
   - Extract identity claims  
   - Call UserInfo endpoint  
   - Normalize provider metadata  
   - Produce the upstream identity context  

2. **Save Phase** — application‑level processing  
   - Resolve or create the local user  
   - Create a session  
   - Hash the session token  
   - Generate the cookie value  
   - Determine the final redirect URL  
   - Produce the final callback context  

Both phases follow the **immutable context builder pattern**, ensuring
deterministic, testable, and replayable behavior.

---

## Folder Structure

```
Callback/
├── Oidc/
│   ├── CallbackOidcContext.cs
│   ├── CallbackOidcContextBuilderRequest.cs
│   ├── CallbackOidcContextBuilderResult.cs
│   └── ICallbackOidcContextBuilder.cs
│
└── Save/
    ├── CallbackSaveContext.cs
    ├── CallbackSaveContextBuilderRequest.cs
    ├── CallbackSaveContextBuilderResult.cs
    └── ICallbackSaveContextBuilder.cs
```

---

# Phase 1 — OIDC Callback (Protocol Layer)

The **Oidc** folder defines the abstractions required to process the upstream OIDC
provider’s callback.

### **CallbackOidcContextBuilderRequest**
Minimal upstream inputs:
- Authorization code  
- Timestamp (captured via the Clock abstraction)

### **CallbackOidcContextBuilderResult**
Normalized identity data:
- Subject identifier  
- Claims  
- UserInfo fields  
- Provider metadata  

### **CallbackOidcContext**
Final immutable snapshot of the OIDC phase:
- Tokens  
- Claims  
- UserInfo  
- Provider  
- Timestamp  

### **ICallbackOidcContextBuilder**
Contract for the entire OIDC callback pipeline:
- Token exchange  
- Claim extraction  
- UserInfo retrieval  
- Provider normalization  
- Context assembly  

The OIDC phase is **pure**, **deterministic**, and **provider‑agnostic**.

---

# Phase 2 — Save Phase (Application Layer)

The **Save** folder defines the abstractions required to convert upstream identity
into local application state.

### **CallbackSaveContextBuilderRequest**
Minimal inputs for the save phase:
- Normalized upstream identity  
- Optional requested redirect URL  
- Timestamp (Clock abstraction)

### **CallbackSaveContextBuilderResult**
Application‑level artifacts:
- Resolved user ID  
- Created session ID  
- Token hash  
- Cookie value  

### **CallbackSaveContext**
Final immutable snapshot of the save phase:
- User resolution  
- Session creation  
- Cookie generation  
- Final redirect URL  

### **ICallbackSaveContextBuilder**
Contract for the entire save pipeline:
- User resolution  
- Session creation  
- Token hashing  
- Cookie generation  
- Redirect determination  
- Context assembly  

The save phase is **pure**, **immutable**, and **serializable**.

---

# Full Pipeline Overview

```
[ OIDC Phase ]
    CallbackOidcContextBuilderRequest
            ↓
    ICallbackOidcContextBuilder
            ↓
    CallbackOidcContextBuilderResult
            ↓
    CallbackOidcContext

[ Save Phase ]
    CallbackSaveContextBuilderRequest
            ↓
    ICallbackSaveContextBuilder
            ↓
    CallbackSaveContextBuilderResult
            ↓
    CallbackSaveContext
```

This structure ensures:

- Deterministic behavior  
- Full testability  
- Clear separation of concerns  
- Replayable flows  
- Immutable context boundaries  
- Provider‑agnostic identity handling  

---

# Architectural Principles

### **Immutable Contexts**
All contexts are immutable once constructed, ensuring:
- No hidden state  
- No mutation  
- Deterministic execution  

### **Builder Pattern**
Each phase uses:
- A *builder request*  
- A *builder result*  
- A *final immutable context*  
- A *builder interface*  

### **Clock Abstraction**
All timestamps are captured externally using the Clock abstraction.

### **Separation of Protocol vs Application**
- OIDC phase handles upstream identity  
- Save phase handles local identity and session creation  

---

# Summary

The Callback folder provides a complete, structured, and immutable pipeline for
processing OIDC callbacks:

### **Oidc Phase**
Upstream identity normalization.

### **Save Phase**
Local identity resolution and session creation.

Together, these abstractions form a clean, deterministic, and production‑ready
callback pipeline for the Identity subsystem.

---
