# Auth Callback Architecture

This guide documents the **current** authentication callback flow after the refactor to:

- A **Frank protocol pipeline**
- An **Application business pipeline**
- A **thin API callback endpoint**

The previous `AuthCallbackExecutor`/`IAuthCallbackStep` engine has been replaced by **immutable context builders** and a **three‑layer architecture**:

1. Frank — OIDC protocol handling  
2. Application — identity + session + redirect  
3. API — cookie issuance + redirect orchestration  

This guide describes the **post‑refactor architecture** and how responsibilities are split.

---

# 1. Purpose

The authentication callback architecture orchestrates the **OIDC authorization‑code callback flow**:

1. Validate configuration  
2. Exchange authorization code  
3. Extract and normalize claims  
4. Resolve identity  
5. Create or load customer  
6. Create session  
7. Compute redirect URL  
8. Compute cookie value  
9. Issue session cookie and redirect  

Each responsibility is implemented in the **correct layer**:

- Frank: protocol  
- Application: business  
- API: boundary orchestration  

The architecture is:

- deterministic  
- pure at each layer  
- invariant‑checked  
- testable  
- cross‑cutting  
- reusable  
- governed  

---

# 2. Architecture Overview

```text
API Callback Endpoint
    ↓
Frank Auth Callback Pipeline (protocol)
    ↓
Application Auth Callback Pipeline (business)
    ↓
API issues cookie + redirect
```

The flow is composed of:

| Component | Responsibility |
|----------|----------------|
| API endpoint | HTTP boundary, `code` extraction, cookie issuance, redirect |
| Frank pipeline | OIDC protocol, token exchange, claims extraction, normalization |
| Application pipeline | Identity resolution, customer/session creation, redirect + cookie value computation |

Each pipeline is implemented as an **immutable context builder**.

---

# 3. Frank Auth Callback Pipeline

Frank owns the **protocol layer**.

```csharp
IImmutableContextBuilder<
    FrankAuthCallbackRequest,
    OidcAuthCallbackContext,
    FrankAuthCallbackResult>
```

### 3.1 Responsibilities

- Validate configuration  
- Exchange authorization code for tokens  
- Fetch/parse userinfo (if required)  
- Normalize provider‑specific claims into a stable shape  
- Produce a protocol‑level result for Application  

### 3.2 Non‑Responsibilities

- No business rules  
- No customer creation  
- No session creation  
- No cookie issuance  
- No redirect computation  

Frank’s pipeline is **pure protocol**.

---

# 4. Application Auth Callback Pipeline

Application owns the **business layer**.

```csharp
IImmutableContextBuilder<
    ApplicationAuthCallbackRequest,
    ApplicationAuthCallbackContext,
    ApplicationAuthCallbackContextBuilderResult>
```

### 4.1 Responsibilities

Given the normalized protocol result, Application must:

- Resolve identity (find or create customer)  
- Create or load customer record  
- Create session  
- Compute redirect URL  
- Compute cookie value (opaque, secure)  
- Produce a complete result for the API boundary  

`ApplicationAuthCallbackContextBuilderResult` must include:

- `CustomerId`  
- `SessionId`  
- `TokenHash` (or equivalent audit field)  
- `CookieValue`  
- `RedirectUrl`  

### 4.2 Non‑Responsibilities

- No OIDC protocol logic  
- No token exchange  
- No direct identity provider calls  
- No cookie issuance  
- No HTTP concerns  

Application’s pipeline is **pure business**.

---

# 5. API Callback Endpoint

The API callback endpoint is a **thin orchestrator**.

### 5.1 Responsibilities

- Extract the `code` query parameter  
- Validate that `code` is present  
- Invoke the Frank pipeline  
- Invoke the Application pipeline  
- Issue the session cookie using `CookieValue`  
- Redirect to `RedirectUrl`  

### 5.2 Non‑Responsibilities

- No protocol logic  
- No business logic  
- No persistence  
- No identity provider calls  
- No redirect computation  
- No cookie value computation  

The endpoint is responsible only for **HTTP boundary behavior**.

---

# 6. Immutable Context Builder Pattern

Both pipelines use the same architectural pattern:

```csharp
public interface IImmutableContextBuilder<TRequest, TContext, TResult>
{
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
```

### 6.1 Rules

- `TRequest`, `TContext`, and `TResult` must be immutable  
- Builders must be deterministic  
- Builders must not mutate external state  
- Builders must not perform HTTP in Application layer  
- Builders must not perform persistence in Frank layer  

This replaces the old `AuthCallbackExecutor`/`IAuthCallbackStep` engine with a **simpler, strongly typed pipeline**.

---

# 7. Error Model

Errors are handled at the correct layer:

### 7.1 API

- Missing `code` → `400 Bad Request` via a shaped exception  
- All other errors → flow into Frank’s global error boundary  

### 7.2 Frank Pipeline

- Protocol failures (token exchange, invalid response, etc.)  
- Must throw domain‑specific protocol exceptions  
- Must never leak provider details directly to the API  

### 7.3 Application Pipeline

- Identity/session failures  
- Must throw application‑level exceptions  
- Must not leak infrastructure details  

All errors are ultimately shaped by:

- Frank’s error boundary  
- ProblemDetails conventions  
- API Governance rules  

---

# 8. Observability

The callback flow must be observable at each layer:

- **API**: callback invocation, cookie issuance, redirect target (non‑PII)  
- **Frank**: protocol events (token exchange, claims extraction)  
- **Application**: identity resolution, session creation, redirect decision  

Logs must never contain:

- Authorization codes  
- Tokens  
- PII  
- Provider error messages  

---

# 9. Testing Strategy

### 9.1 Frank Pipeline Tests

- Protocol success/failure paths  
- Invalid configuration  
- Provider error handling  
- Claims normalization  

### 9.2 Application Pipeline Tests

- New customer flow  
- Existing customer flow  
- Session creation  
- Redirect URL selection  
- Cookie value computation  

### 9.3 API Endpoint Tests

- Missing `code` → `400`  
- Valid `code` → cookie issued + redirect  
- Error propagation into global error boundary  

Tests must use:

- Fake Frank pipeline implementation  
- Fake Application pipeline implementation  
- ApiFactory / ApiContext for endpoint tests  

---

# 10. Migration Notes (From Old Engine)

The previous engine:

- `AuthCallbackExecutor`  
- `IAuthCallbackStep`  
- `AuthCallbackContext`  
- `AuthCallbackDiagnosticEvent`  

has been **replaced** by:

- Frank immutable context builder (protocol)  
- Application immutable context builder (business)  
- Thin API callback endpoint  

Key changes:

- Step‑based engine → builder‑based pipelines  
- Mutable context → immutable request/context/result types  
- Engine‑local diagnostics → layer‑appropriate observability  
- Single engine → three‑layer architecture (Frank, Application, API)  

---

# 11. Summary

The Auth Callback Architecture is now:

- **Three‑layered** — Frank (protocol), Application (business), API (boundary)  
- **Immutable** — request/context/result types  
- **Deterministic** — predictable, testable behavior  
- **Governed** — aligned with Architecture, API, and Security Governance  
- **Extensible** — new providers and business rules can be added without breaking boundaries  

This architecture replaces the old `AuthCallbackExecutor` engine and is the canonical model for all authentication callback behavior going forward.
