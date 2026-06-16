# ADR‑0055 — Authentication Callback Architecture (Builder‑Based)

## Status  
Accepted

## Context  
The original authentication callback (ADR‑0041) used a **step‑engine architecture**:

- Ordered step lists  
- Mutable shared context  
- Dispatcher‑driven execution  
- Steps mixing protocol and business logic  
- Steps performing persistence and side effects  
- Steps relying on ordering guarantees  

This approach became increasingly fragile as authentication requirements expanded:

- Identity mapping  
- Session creation  
- Session token hashing  
- Cookie value computation  
- Redirect computation  
- Error handling  
- Observability  
- Hosting‑aware configuration  

The step engine introduced architectural problems:

- Mutable state made flows difficult to reason about  
- Ordering bugs were common and hard to detect  
- Steps leaked cross‑layer concerns  
- Testing required complex harnesses  
- Guardrails could not enforce purity or invariants  
- Authentication callback logic became brittle  

ADR‑0053 introduced **ImmutableContextBuilder** as a deterministic, pure, strongly‑typed replacement for step‑based pipelines.  
ADR‑0054 formally deprecated the step engine.

The authentication callback architecture must now be rewritten around the builder model.

## Decision  
The authentication callback flow is now implemented using a **three‑layer, builder‑based architecture**:

1. **Frank Auth Callback Pipeline** — protocol layer  
2. **Application Auth Callback Pipeline** — business layer  
3. **Api Callback Endpoint** — infrastructure boundary  

This replaces the step engine entirely.

---

# 1. Frank Auth Callback Pipeline (Protocol Layer)

Implemented as:

```
IImmutableContextBuilder<
    FrankAuthCallbackRequest,
    OidcAuthCallbackContext,
    FrankAuthCallbackResult>
```

### Responsibilities

- Validate OIDC configuration  
- Exchange authorization code  
- Retrieve userinfo  
- Normalize provider‑specific claims  
- Produce a stable, provider‑agnostic identity payload  

### Rules

- **No business logic**  
- **No persistence**  
- **No session creation**  
- **No cookie logic**  
- **No redirect logic**  
- **No domain interaction**  

Frank pipeline produces a **pure protocol result** consumed by the Application pipeline.

---

# 2. Application Auth Callback Pipeline (Business Layer)

Implemented as:

```
IImmutableContextBuilder<
    ApplicationAuthCallbackRequest,
    ApplicationAuthCallbackContext,
    ApplicationAuthCallbackContextBuilderResult>
```

### Responsibilities

- Validate identity claims  
- Resolve or create Owner  
- Create session  
- Generate session token + hash  
- Compute cookie value  
- Compute redirect URL  

### Rules

- **No protocol logic**  
- **No external HTTP calls**  
- **No token exchange**  
- **No userinfo retrieval**  
- **No cookie issuance** (only compute value)  
- **No HttpContext access**  

Application pipeline produces a **complete authentication result** for the Api endpoint.

---

# 3. Api Callback Endpoint (Infrastructure Boundary)

The endpoint is a **thin orchestrator**:

- Extracts the `code` query parameter  
- Invokes Frank pipeline  
- Invokes Application pipeline  
- Issues the session cookie  
- Redirects the user  

### Rules

- **No protocol logic**  
- **No business logic**  
- **No persistence**  
- **No identity mapping**  
- **No session creation**  
- **No redirect computation**  

The endpoint must remain pure and boundary‑focused.

---

# 4. Immutable Context Builder Requirements

Both pipelines must:

- Use immutable request, context, and result types  
- Never mutate state  
- Never overwrite or clear fields  
- Enforce invariants at each stage  
- Produce deterministic output  
- Be fully testable at:
  - Unit level  
  - Pipeline level  
  - Guardrail level  

Builders replace:

- Step engines  
- Mutable contexts  
- Ordered step lists  
- Dispatcher‑driven pipelines  

---

# 5. Error Handling Model

- Frank pipeline errors → protocol failures (502)  
- Application pipeline errors → business failures (500)  
- Missing `code` → 400  
- Missing `sub` → 502  
- Identity mapping failures → 500  
- Session creation failures → 500  

All errors are surfaced through global exception middleware.

---

# 6. Testing Architecture

### Frank Pipeline Tests
- Token exchange  
- Userinfo retrieval  
- Claims normalization  
- Protocol error handling  

### Application Pipeline Tests
- Identity resolution  
- Owner creation  
- Session creation  
- Cookie value computation  
- Redirect computation  

### Integration Tests
- Full callback flow  
- Cookie issuance  
- Redirect behavior  

### Guardrail Tests
- Cookie opacity  
- Cookie flags  
- No protocol logic in Application  
- No business logic in Frank  
- No Infrastructure access in Application  
- No HttpContext access in builders  

---

# Consequences

## Positive  
- Deterministic, pure, immutable authentication flow  
- Strong separation of protocol vs business logic  
- Perfect alignment with Architecture Governance  
- Perfect alignment with Security Governance  
- Perfect alignment with Session Management Governance  
- Builders are trivial to test  
- No mutable state  
- No ordering bugs  
- No step engine complexity  
- Fully composable and maintainable  

## Neutral  
- Requires developers to understand the builder pattern  
- Requires updates to documentation and conventions  

## Negative  
- Existing step‑engine code must be migrated (completed for authentication)  

---

## Summary  
ADR‑0055 formalizes the **builder‑based authentication callback architecture**, replacing the deprecated step engine with a deterministic, immutable, strongly‑typed, three‑layer pipeline:

1. **Frank pipeline** — protocol  
2. **Application pipeline** — business  
3. **Api endpoint** — boundary  

This architecture is now the canonical model for all authentication flows in CampFitFurDogs.
