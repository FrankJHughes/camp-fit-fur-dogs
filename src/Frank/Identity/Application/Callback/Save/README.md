# Identity Application — Save Pipeline

The **Save** folder contains the application‑layer implementation of the
post‑authentication Save pipeline.  
This pipeline runs *after* the OIDC callback pipeline has produced a validated
external identity context. Its responsibility is to convert that external
identity into a fully persisted, auditable, authenticated session within the
system.

The Save pipeline is built on the **immutable context** pattern:

- Each step receives a `CallbackSaveContext`
- Produces a new enriched context
- Never mutates existing state
- Executes only when its `CanExecute` condition is satisfied

This ensures deterministic behavior, strong correctness guarantees, and
excellent observability.

---

## Folder Structure

```
Save/
├── CallbackSaveContextBuilder.cs
├── ServiceCollectionExtensions.cs
└── Steps/
    ├── ResolveUserStep.cs
    ├── BuildCookieStep.cs
    ├── CreateSessionStep.cs
    └── AuditLoginStep.cs
```

---

# CallbackSaveContextBuilder

The orchestrator of the Save pipeline.

### Responsibilities

- Initialize a minimal `CallbackSaveContext` from the request  
- Execute all registered `IImmutableContextBuildStep<CallbackSaveContext>` steps  
- Enforce immutability guarantees:
  - `External` must never change  
  - `Now` must never change  
- Emit structured observability events for each step  
- Produce a final `CallbackSaveContextBuilderResult` containing:
  - `UserId`
  - `SessionId`
  - `TokenHash`
  - `CookieValue`

### Notes

- The builder ensures all required fields are present before returning  
- Steps cannot return `null`  
- Steps cannot modify immutable fields  

---

# ServiceCollectionExtensions

Registers the entire Save pipeline into DI.

### Adds:

- `ResolveUserStep`
- `BuildCookieStep`
- `CreateSessionStep`
- `AuditLoginStep`
- `CallbackSaveContextBuilder`

All steps are registered as:

```
AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, TStep>()
```

This ensures each pipeline execution receives fresh step instances.

---

# Pipeline Steps

The **Steps** folder contains the four immutable‑context build steps that form
the Save pipeline.

## 1. ResolveUserStep

Maps external identity → internal user.

### Responsibilities

- Execute only when `UserId` is null  
- Use `IUserResolver` to resolve the internal user  
- Populate:
  - `UserId`

---

## 2. BuildCookieStep

Generates a session token and constructs the authentication cookie value.

### Responsibilities

- Execute only when `CookieValue` is null  
- Use `ISessionTokenGenerator` to produce:
  - Plaintext token
  - Hashed token  
- Build cookie using `SessionCookie.FromPlaintextToken`  
- Populate:
  - `TokenHash`
  - `CookieValue`

---

## 3. CreateSessionStep

Creates and persists the authenticated session.

### Responsibilities

- Execute only when:
  - `UserId` is present  
  - `TokenHash` is present  
  - `SessionId` is null  
- Construct a new `Session` domain object  
- Persist via `ICreateSessionWriter`  
- Commit via `IFrankIdentityUnitOfWork`  
- Populate:
  - `SessionId`

---

## 4. AuditLoginStep

Emits an audit log for the successful login.

### Responsibilities

- Execute only when `UserId` is present  
- Use `IAuditLogger.LoginSucceeded` to record:
  - Internal user ID  
  - External subject ID  

### Notes

- Does not modify the context  
- Performs an external side effect (audit logging)  

---

# Pipeline Overview

```
[ External Identity ]
        ↓
ResolveUserStep
        ↓
[ UserId ]
        ↓
BuildCookieStep
        ↓
[ TokenHash, CookieValue ]
        ↓
CreateSessionStep
        ↓
[ SessionId ]
        ↓
AuditLoginStep
        ↓
CallbackSaveContextBuilderResult
```

This pipeline ensures:

- Deterministic Save‑phase processing  
- Immutable context transformations  
- Strong correctness guarantees  
- Clear separation of responsibilities  
- Full observability of each step  

---

# Summary

The Save folder defines the complete Save‑phase pipeline:

### Core Builder
- `CallbackSaveContextBuilder`

### DI Registration
- `ServiceCollectionExtensions`

### Pipeline Steps
- `ResolveUserStep` — resolve internal user  
- `BuildCookieStep` — generate session token + cookie  
- `CreateSessionStep` — persist authenticated session  
- `AuditLoginStep` — record successful login  

Together, these components form a clean, testable, and predictable Save pipeline
within the Identity subsystem.

---
