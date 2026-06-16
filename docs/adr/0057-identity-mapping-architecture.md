# ADR‑0057 — Identity Mapping Architecture

## Status  
Accepted

## Context  
Identity mapping answers a single architectural question:

> **Which internal Owner corresponds to this external OIDC identity?**

Historically, identity mapping was implemented inside the step‑engine authentication callback:

- Steps extracted the `sub` claim  
- Steps validated userinfo  
- Steps queried the database  
- Steps created new Owners  
- Steps mutated a shared context object  

This approach created several architectural problems:

### 1. Cross‑layer leakage  
Protocol logic and business logic were interwoven:

- Frank steps performed business logic  
- Application steps performed protocol logic  
- Identity mapping was not isolated  

### 2. Mutable shared state  
Identity mapping mutated the callback context, making it difficult to reason about:

- When identity was resolved  
- Whether identity could be overwritten  
- Whether identity could be cleared  

### 3. Ordering fragility  
Identity mapping depended on:

- Claims being set by earlier steps  
- Userinfo being normalized  
- Steps running in the correct order  

### 4. Poor testability  
Testing required:

- Full callback flows  
- Step lists  
- Deep mocks  
- Manual context construction  

### 5. Governance violations  
Identity mapping leaked:

- Protocol concerns into Application  
- Business concerns into Frank  
- Infrastructure concerns into steps  

The authentication refactor introduced:

- **ImmutableContextBuilder**  
- **Frank pipeline → Application pipeline → Api endpoint**  
- **Pure identity resolution**  
- **Strict layering boundaries**  

Identity mapping must now be formalized as a **pure Application‑layer builder transformation**.

---

## Decision  
Identity mapping is implemented as a **pure, deterministic transformation** inside the **Application Auth Callback Pipeline**, using **ImmutableContextBuilder**.

Identity mapping is now:

- Pure  
- Deterministic  
- Immutable  
- Strongly typed  
- Fully testable  
- Strictly layered  

---

# 1. Identity Mapping Location

Identity mapping occurs **only** inside:

```
IImmutableContextBuilder<
    ApplicationAuthCallbackRequest,
    ApplicationAuthCallbackContext,
    ApplicationAuthCallbackContextBuilderResult>
```

It is **never** performed in:

- Frank pipeline  
- Api endpoint  
- Infrastructure  
- Domain  
- Middleware  
- Controllers  

Identity mapping is an **Application‑layer responsibility**.

---

# 2. Identity Mapping Inputs

Identity mapping requires:

- `ExternalSubject` (OIDC `sub`)  
- `Email` (optional, not used for identity)  
- `Provider` (Auth0)  
- `NormalizedClaims` (from Frank pipeline)  

### Rules

- `sub` is **required**  
- `email` is **not** an identity key  
- No other claim may be used for identity  
- No protocol logic may run in Application  

---

# 3. Identity Mapping Algorithm

Identity mapping follows a strict, deterministic sequence:

### 1. Extract external identity  
From the Frank pipeline result:

```
ExternalSubject = claims["sub"]
```

### 2. Lookup Owner by external identity  
Using:

```
IOwnerIdentityReader.FindByExternalId(ExternalSubject)
```

### 3. If found → reuse Owner  
Return the existing OwnerId.

### 4. If not found → create Owner  
Using:

```
IOwnerFactory.CreateNewOwner(ExternalSubject)
```

### 5. Persist via Unit of Work  
Commit the new Owner.

### 6. Return OwnerId  
Immutable, strongly typed.

---

# 4. Identity Mapping Output

Identity mapping produces:

- `OwnerId` (Guid)  
- `IsNewOwner` (bool)  

These values are stored in the **ApplicationAuthCallbackContext** and flow into:

- Session creation  
- Redirect computation  
- Welcome email (future)  

---

# 5. Layering Rules

Identity mapping must follow strict layering:

### Frank Layer  
❌ Must not perform identity mapping  
❌ Must not access persistence  
❌ Must not create Owners  
✔ May normalize claims  
✔ May extract `sub`  

### Application Layer  
✔ Performs identity mapping  
✔ Creates Owners  
✔ Uses repositories + Unit of Work  
❌ Must not decode tokens  
❌ Must not call external identity providers  

### Domain Layer  
✔ Contains Owner aggregate  
❌ Contains no identity mapping logic  

### Infrastructure Layer  
✔ Implements repositories  
❌ Contains no identity mapping logic  

### Api Layer  
❌ Must not perform identity mapping  

---

# 6. Purity & Immutability Requirements

Identity mapping must be:

- Pure  
- Deterministic  
- Free of side effects (except persistence)  
- Immutable  
- Strongly typed  

Identity mapping must **never**:

- Mutate context  
- Overwrite fields  
- Clear fields  
- Depend on ordering  
- Depend on HttpContext  
- Depend on environment variables  

---

# 7. Testing Requirements

### Unit Tests  
- Lookup existing Owner  
- Create new Owner  
- Reject missing `sub`  
- Reject invalid claims  

### Pipeline Tests  
- Identity mapping integrated with session creation  
- Identity mapping integrated with redirect logic  

### Guardrail Tests  
- No protocol logic in Application  
- No business logic in Frank  
- No Infrastructure access in Frank  
- No HttpContext access in Application  

---

## Consequences

### Positive  
- Deterministic identity mapping  
- Immutable, pure logic  
- Strong layering boundaries  
- Perfect alignment with Authentication Callback Architecture  
- Perfect alignment with ImmutableContextBuilder  
- Fully testable  
- No ordering bugs  
- No mutable state  
- No cross‑layer leakage  

### Neutral  
- Requires developers to understand the identity mapping lifecycle  

### Negative  
- Existing step‑engine identity mapping required migration (completed)  

---

## Summary  
ADR‑0057 defines the **Identity Mapping Architecture** for CampFitFurDogs:

- Pure, deterministic, builder‑based identity resolution  
- Application‑layer responsibility  
- External identity = OIDC `sub`  
- OwnerId = internal identity  
- No protocol logic in Application  
- No business logic in Frank  
- Immutable, strongly typed, fully testable  

Identity mapping is now a stable, governed architectural primitive within the authentication callback pipeline.
