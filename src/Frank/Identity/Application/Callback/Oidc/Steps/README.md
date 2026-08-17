# Identity Application — OIDC Callback Pipeline Steps

The **Steps** folder contains the immutable‑context build steps that make up the
OIDC authentication callback pipeline.  
Each step performs one deterministic transformation of the
`CallbackOidcContext`, enriching it with validated information from the identity
provider.

All steps follow these principles:

- **Immutable context** — each step returns a new enriched context instance  
- **Deterministic execution** — no hidden side effects  
- **Single responsibility** — each step performs exactly one transformation  
- **Composability** — steps can be chained to form a complete callback pipeline  

---

## Folder Structure

```
Steps/
├── ExchangeCodeStep.cs
├── FetchUserInfoStep.cs
└── ValidateTokensStep.cs
```

---

# ExchangeCodeStep

Exchanges the authorization code for tokens.

### Responsibilities

- Execute only when `ctx.Code` is present  
- Call the token endpoint via `IOidcTokenClient`  
- Populate:
  - `AccessToken`
  - `IdToken`

### Notes

- This step must run before any UserInfo or token‑validation steps  
- The returned context is a new immutable instance  
- Errors should be surfaced using `AuthCallbackException` when appropriate  

---

# FetchUserInfoStep

Retrieves OIDC UserInfo using the access token.

### Responsibilities

- Execute only when `ctx.AccessToken` is present  
- Call the UserInfo endpoint via `IOidcUserInfoClient`  
- Populate:
  - `SubjectId`
  - `Claims`
  - `Email`
  - `GivenName`
  - `FamilyName`
  - `Picture`

### Notes

- This step enriches the context with identity‑provider claims  
- It does not perform validation — only retrieval  
- The returned context is a new immutable instance  

---

# ValidateTokensStep

Validates the ID token and extracts claims.

### Responsibilities

- Execute only when `ctx.IdToken` is present  
- Validate the ID token via `IOidcTokenValidator`  
- Populate:
  - `SubjectId`
  - `Claims`

### Notes

- This step ensures cryptographic and structural correctness of the ID token  
- It may run before or after UserInfo retrieval depending on pipeline design  
- The returned context is a new immutable instance  

---

# Pipeline Overview

```
[ Authorization Code ]
        ↓
ExchangeCodeStep
        ↓
[ AccessToken, IdToken ]
        ↓
ValidateTokensStep
        ↓
[ Validated SubjectId, Claims ]
        ↓
FetchUserInfoStep
        ↓
[ Enriched Claims, Profile Info ]
```

This pipeline ensures:

- Deterministic OIDC callback processing  
- Clear separation of responsibilities  
- Immutable context transformations  
- Infrastructure‑agnostic design  

---

# Summary

The Steps folder defines the complete set of OIDC callback pipeline operations:

### **ExchangeCodeStep**
Exchanges the authorization code for tokens.

### **FetchUserInfoStep**
Retrieves UserInfo claims using the access token.

### **ValidateTokensStep**
Validates the ID token and extracts subject + claims.

Together, these steps form a clean, testable, and predictable OIDC callback
pipeline within the Identity subsystem.

---
