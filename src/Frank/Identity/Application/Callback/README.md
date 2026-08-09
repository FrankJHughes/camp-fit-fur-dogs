# Identity Application — Authentication Callback Pipelines

The **Callback** folder contains the complete application‑layer implementation of
the authentication callback subsystem.  
This subsystem transforms an incoming OIDC authorization code into a fully
validated external identity, resolves the internal user, creates an authenticated
session, and emits audit logs — all through deterministic, immutable pipelines.

The subsystem is composed of two pipelines:

1. **OIDC Pipeline** — external identity acquisition  
2. **Save Pipeline** — internal identity + session creation

Both pipelines use the **immutable context** pattern:

- Each step receives a context
- Produces a new enriched context
- Never mutates existing state
- Executes only when its `CanExecute` condition is satisfied

This ensures correctness, predictability, and excellent observability.

---

## Folder Structure

```
Callback/
├── Oidc/
│   ├── CallbackOidcContextBuilder.cs
│   ├── OidcProtocolException.cs
│   ├── ServiceCollectionExtensions.cs
│   └── Steps/
│       ├── ExchangeCodeStep.cs
│       ├── ValidateTokensStep.cs
│       └── FetchUserInfoStep.cs
│
├── Save/
│   ├── CallbackSaveContextBuilder.cs
│   ├── ServiceCollectionExtensions.cs
│   └── Steps/
│       ├── ResolveUserStep.cs
│       ├── BuildCookieStep.cs
│       ├── CreateSessionStep.cs
│       └── AuditLoginStep.cs
│
└── ServiceCollectionExtensions.cs
```

---

# OIDC Pipeline

The OIDC pipeline converts an authorization code into a validated external
identity.

### Steps

#### **ExchangeCodeStep**
Exchanges the authorization code for tokens.

#### **ValidateTokensStep**
Validates the ID token and extracts claims.

#### **FetchUserInfoStep**
Retrieves profile information from the UserInfo endpoint.

### Builder

#### **CallbackOidcContextBuilder**
Orchestrates all OIDC steps, enforces immutability, and produces:

- `SubjectId`
- `Claims`
- `Email`
- `GivenName`
- `FamilyName`
- `Picture`
- `Provider`

### Exceptions

#### **OidcProtocolException**
Thrown when the identity provider behaves outside expected protocol norms.

---

# Save Pipeline

The Save pipeline converts the validated external identity into a fully
authenticated internal session.

### Steps

#### **ResolveUserStep**
Maps external identity → internal user.

#### **BuildCookieStep**
Generates session token + cookie.

#### **CreateSessionStep**
Persists the authenticated session.

#### **AuditLoginStep**
Emits a login‑succeeded audit event.

### Builder

#### **CallbackSaveContextBuilder**
Orchestrates all Save steps, enforces immutability, and produces:

- `UserId`
- `SessionId`
- `TokenHash`
- `CookieValue`

---

# Combined Registration

The root extension method wires up both pipelines:

```
AddFrankIdentityApplicationCallbackOidc()
AddFrankIdentityApplicationCallbackSave()
```

This enables the application to resolve:

- `ICallbackOidcContextBuilder`
- `ICallbackSaveContextBuilder`

and execute the full authentication callback flow through DI.

---

# Full Pipeline Overview

```
[ Authorization Code ]
        ↓
OIDC Pipeline
    ExchangeCodeStep
    ValidateTokensStep
    FetchUserInfoStep
        ↓
[ External Identity ]
        ↓
Save Pipeline
    ResolveUserStep
    BuildCookieStep
    CreateSessionStep
    AuditLoginStep
        ↓
[ Authenticated Session ]
```

This unified pipeline ensures:

- Deterministic authentication processing  
- Immutable context transformations  
- Strong correctness guarantees  
- Clear separation of responsibilities  
- Full observability of each step  
- Clean, testable, predictable behavior  

---

# Summary

The Callback folder defines the complete authentication callback subsystem:

### OIDC Pipeline
- Exchange authorization code → tokens  
- Validate ID token  
- Fetch UserInfo  
- Produce external identity  

### Save Pipeline
- Resolve internal user  
- Generate session token + cookie  
- Persist session  
- Audit login  

Together, these pipelines form a robust, production‑grade authentication callback
flow within the Identity subsystem.

---
