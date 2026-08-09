# Identity Application — OIDC Callback Abstractions

The **Oidc** folder contains all immutable‑context abstractions required to process
an OpenID Connect (OIDC) callback within the Identity subsystem.

These abstractions define a clean, deterministic, testable pipeline for handling
OIDC callback flows, including:

- Receiving the authorization code  
- Exchanging the code for tokens  
- Extracting identity claims  
- Calling the UserInfo endpoint  
- Normalizing provider‑specific metadata  
- Producing a final immutable callback context  

This folder contains **contracts and immutable data models only**.  
All protocol‑level behavior (token exchange, claim extraction, provider logic)
is implemented in higher layers.

---

## Folder Structure

```
Oidc/
├── CallbackOidcContext.cs
├── CallbackOidcContextBuilderRequest.cs
├── CallbackOidcContextBuilderResult.cs
└── ICallbackOidcContextBuilder.cs
```

---

# CallbackOidcContext

Represents the **final immutable snapshot** of an OIDC callback.

### Contains

- Authorization code  
- Timestamp (captured via the Clock abstraction)  
- Access token / ID token  
- Subject identifier  
- Claims  
- UserInfo fields (email, names, picture)  
- Provider metadata  

### Purpose

This context is passed through the Identity application pipeline and used to:

- Create or update local user records  
- Establish sessions  
- Emit audit events  
- Apply provider‑specific mapping rules  

It is **pure**, **immutable**, and **serializable**.

---

# CallbackOidcContextBuilderRequest

Represents the **minimal upstream inputs** required to begin building an OIDC
callback context.

### Contains

- Authorization code

### Purpose

This request is intentionally small.  
It captures only what the upstream provider sends to the callback endpoint.

The builder uses this request to initiate token exchange and identity extraction.

---

# CallbackOidcContextBuilderResult

Represents the **post‑processing identity data** extracted from the OIDC provider.

### Contains

- Subject identifier  
- Normalized claims  
- UserInfo fields  
- Provider metadata  

### Purpose

This result is produced *after* token exchange and UserInfo retrieval.  
It contains normalized identity information ready to be injected into the final
immutable `CallbackOidcContext`.

---

# ICallbackOidcContextBuilder

Defines the contract for constructing a `CallbackOidcContext` from:

1. A `CallbackOidcContextBuilderRequest`  
2. A `CallbackOidcContextBuilderResult`  
3. Provider‑normalized identity data  

### Responsibilities

Implementations must:

- Exchange the authorization code for tokens  
- Validate and parse the ID token  
- Extract claims  
- Call the UserInfo endpoint  
- Normalize provider‑specific fields  
- Produce the final immutable context  

### Architectural Role

This builder encapsulates the entire OIDC callback pipeline and ensures:

- Deterministic behavior  
- Testability  
- Separation of concerns  
- Provider‑agnostic abstractions  
- Immutable context construction  

---

# Pipeline Overview

```
[ CallbackOidcContextBuilderRequest ]
            ↓
  (Token Exchange + Claim Extraction)
            ↓
[ CallbackOidcContextBuilderResult ]
            ↓
  (Context Assembly)
            ↓
[ CallbackOidcContext ]
            ↓
  (Identity Application Pipeline)
```

This structure ensures that OIDC callback processing is:

- Pure  
- Immutable  
- Replayable  
- Provider‑agnostic  
- Easy to test  

---

# Summary

The OIDC folder provides a complete immutable‑context model for handling OIDC
callback flows:

### **CallbackOidcContext**
Final immutable snapshot of the callback.

### **CallbackOidcContextBuilderRequest**
Minimal upstream inputs.

### **CallbackOidcContextBuilderResult**
Normalized identity data extracted from tokens and UserInfo.

### **ICallbackOidcContextBuilder**
Contract for constructing the full callback context.

Together, these abstractions form a clean, deterministic, and testable OIDC
callback pipeline within the Identity subsystem.

---
