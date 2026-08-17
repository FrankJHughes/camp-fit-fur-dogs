# Identity Application — OIDC Callback Pipeline

The **Oidc** folder contains the full application‑layer implementation of the
OIDC authentication callback pipeline.  
This pipeline transforms an incoming authorization code into a fully enriched
identity context containing validated tokens, claims, profile information, and
the resolved subject identifier.

The pipeline is built on the **immutable context** pattern:

- Each step receives a context
- Produces a new enriched context
- Never mutates existing state
- Executes only when its `CanExecute` condition is satisfied

This ensures deterministic behavior, strong correctness guarantees, and
excellent observability.

---

## Folder Structure

```
Oidc/
├── CallbackOidcContextBuilder.cs
├── OidcProtocolException.cs
├── ServiceCollectionExtensions.cs
└── Steps/
    ├── ExchangeCodeStep.cs
    ├── FetchUserInfoStep.cs
    └── ValidateTokensStep.cs
```

---

# CallbackOidcContextBuilder

The central orchestrator of the OIDC callback pipeline.

### Responsibilities

- Initialize a minimal `CallbackOidcContext` from the request
- Execute all registered `IImmutableContextBuildStep<CallbackOidcContext>` steps
- Enforce immutability guarantees between steps
- Emit structured observability events for each step
- Produce a final `CallbackOidcContextBuilderResult`
- Ensure the pipeline yields a valid `SubjectId`

### Guarantees

- Steps cannot modify immutable fields (`Code`, `Timestamp`)
- Steps cannot return `null`
- Pipeline must produce a subject identifier or fail fast

---

# OidcProtocolException

Thrown when the pipeline encounters an unexpected or unrecoverable OIDC protocol
violation.

Examples:

- Missing required fields after pipeline execution
- Invalid or inconsistent identity provider responses
- Structural issues not covered by `AuthCallbackException`

This exception signals that the identity provider behaved outside expected norms.

---

# ServiceCollectionExtensions

Registers the entire OIDC callback pipeline into DI.

### Adds:

- `ExchangeCodeStep`
- `FetchUserInfoStep`
- `ValidateTokensStep`
- `CallbackOidcContextBuilder`

All steps are registered as:

```
AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, TStep>()
```

This ensures each pipeline execution receives fresh step instances.

---

# Pipeline Steps

The **Steps** folder contains the three immutable‑context build steps that form
the OIDC callback pipeline.

## 1. ExchangeCodeStep

Exchanges the authorization code for tokens.

### Inputs
- `Code`

### Outputs
- `AccessToken`
- `IdToken`

### Notes
Must run before any validation or UserInfo retrieval.

---

## 2. ValidateTokensStep

Validates the ID token and extracts claims.

### Inputs
- `IdToken`

### Outputs
- `SubjectId`
- `Claims`

### Notes
Ensures cryptographic and structural correctness of the ID token.

---

## 3. FetchUserInfoStep

Retrieves OIDC UserInfo using the access token.

### Inputs
- `AccessToken`

### Outputs
- `SubjectId`
- `Claims`
- `Email`
- `GivenName`
- `FamilyName`
- `Picture`

### Notes
Enriches the context with identity‑provider profile information.

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
        ↓
CallbackOidcContextBuilderResult
```

This pipeline ensures:

- Deterministic OIDC callback processing  
- Immutable context transformations  
- Strong validation guarantees  
- Infrastructure‑agnostic design  
- Full observability of each step  

---

# Summary

The OIDC folder defines the complete application‑layer callback pipeline:

### Core Builder
- `CallbackOidcContextBuilder`

### Exceptions
- `OidcProtocolException`

### DI Registration
- `ServiceCollectionExtensions`

### Pipeline Steps
- `ExchangeCodeStep`
- `ValidateTokensStep`
- `FetchUserInfoStep`

Together, these components form a clean, testable, and predictable OIDC callback
pipeline within the Identity subsystem.

---
