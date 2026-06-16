# Authentication Architecture Guide

This guide documents the **current authentication architecture** implemented across:

- **US‑110 — Authentication: Owner Login (OIDC)**
- **US‑111 — Authentication: Session Management**

It describes how the system performs OIDC login, executes the authentication callback, resolves identity, creates sessions, and issues cookies — using the **new three‑layer authentication callback architecture**:

1. **OIDC Login Initiation**  
2. **Frank Auth Callback Pipeline (protocol)**  
3. **Application Auth Callback Pipeline (business)**  
4. **API Callback Endpoint (boundary orchestration)**  

This guide explains **how authentication works today**, not governance.  
Rules and boundaries live in:

- Architecture Governance  
- API Governance  
- Security Governance  
- Code Conventions  
- ADRs  

---

# High‑Level Architecture

Authentication consists of three cooperating components:

1. **OIDC Login Initiation**  
2. **Auth Callback Architecture (Frank + Application + API)**  
3. **Session Issuance + Identity Mapping + Audit Logging**

Authentication spans:

- **API** — endpoint orchestration, cookie issuance, redirect  
- **Frank** — OIDC protocol pipeline  
- **Application** — identity resolution, session creation, redirect + cookie value  
- **Domain** — Owner + Session invariants  
- **Infrastructure** — Auth0 client, repositories, audit logger  

---

# Component 1 — OIDC Login Initiation

The login initiation endpoint:

- Generates the Auth0 authorization URL  
- Includes the correct redirect URI  
- Includes PKCE parameters  
- Redirects the browser to Auth0  

This endpoint:

- Contains **no identity logic**  
- Contains **no session logic**  
- Contains **no business logic**  
- Uses only configuration + URL generation  
- Relies on Frank middleware for security headers and CORS  

It simply begins the OIDC flow.

---

# Component 2 — Auth Callback Architecture (Post‑Refactor)

The old dispatcher‑based callback engine has been replaced by a **three‑layer architecture**:

```
API Callback Endpoint
    ↓
Frank Auth Callback Pipeline (protocol)
    ↓
Application Auth Callback Pipeline (business)
    ↓
API issues cookie + redirect
```

This architecture is:

- deterministic  
- immutable  
- testable  
- governed  
- cross‑cutting  
- aligned with Frank’s engine model  

---

# Layer 1 — Frank Auth Callback Pipeline (Protocol Layer)

Frank owns the **OIDC protocol**.

```csharp
IImmutableContextBuilder<
    FrankAuthCallbackRequest,
    OidcAuthCallbackContext,
    FrankAuthCallbackResult>
```

## Responsibilities

- Validate OIDC configuration  
- Exchange authorization code for tokens  
- Fetch userinfo (if required)  
- Normalize provider‑specific claims  
- Produce a stable, provider‑agnostic result  

## Non‑Responsibilities

- No business logic  
- No identity resolution  
- No session creation  
- No redirect computation  
- No cookie value computation  

Frank’s pipeline is **pure protocol**.

---

# Layer 2 — Application Auth Callback Pipeline (Business Layer)

Application owns the **identity + session + redirect** logic.

```csharp
IImmutableContextBuilder<
    ApplicationAuthCallbackRequest,
    ApplicationAuthCallbackContext,
    ApplicationAuthCallbackContextBuilderResult>
```

## Responsibilities

Given the normalized protocol result, Application must:

- Resolve identity (find or create customer)  
- Create or load Owner  
- Create session  
- Compute redirect URL  
- Compute cookie value (opaque, secure)  

The result object must include:

- `CustomerId`  
- `SessionId`  
- `TokenHash`  
- `CookieValue`  
- `RedirectUrl`  

## Non‑Responsibilities

- No OIDC protocol logic  
- No token exchange  
- No direct identity provider calls  
- No cookie issuance  
- No HTTP concerns  

Application’s pipeline is **pure business**.

---

# Layer 3 — API Callback Endpoint (Boundary Layer)

The API callback endpoint is a **thin orchestrator**.

## Responsibilities

- Extract the `code` query parameter  
- Validate that `code` is present  
- Invoke the Frank pipeline  
- Invoke the Application pipeline  
- Issue the session cookie using `CookieValue`  
- Redirect to `RedirectUrl`  

## Non‑Responsibilities

- No protocol logic  
- No business logic  
- No persistence  
- No identity provider calls  
- No redirect computation  
- No cookie value computation  

The endpoint is responsible only for **HTTP boundary behavior**.

---

# Immutable Context Builder Pattern

Both pipelines use the same architectural pattern:

```csharp
public interface IImmutableContextBuilder<TRequest, TContext, TResult>
{
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
```

## Rules

- All types (`TRequest`, `TContext`, `TResult`) must be immutable  
- Builders must be deterministic  
- Builders must not mutate external state  
- Builders must not perform HTTP in Application layer  
- Builders must not perform persistence in Frank layer  

This replaces the old step engine with a **simpler, strongly typed pipeline**.

---

# Identity Mapping in the Architecture

Identity mapping is performed in the **Application pipeline**:

- External identity → internal domain identity  
- Implemented via `IIdentityResolver`  
- Pure and deterministic  
- Never uses email  
- Never exposes internal IDs to Auth0  
- Never touches Infrastructure directly  

See the **Identity Mapping Guide** for details.

---

# Session Issuance in the Architecture

Session issuance consists of:

1. **Token generation** (Application pipeline)  
2. **Token hashing** (Application pipeline)  
3. **Session creation** (Application pipeline)  
4. **Cookie issuance** (API layer)  
5. **Redirect** (API layer using Application result)  

Session tokens:

- Are 256‑bit random values  
- Are hashed using SHA‑256  
- Are stored only as hashes  
- Are issued as secure cookies  

Production cookie flags:

- `Secure=true`  
- `HttpOnly=true`  
- `SameSite=Lax`  

Local development uses `Secure=false`.

---

# Audit Logging in the Architecture

Audit logging occurs in the **Application pipeline**:

- Logs `CustomerId` + external identity  
- Does not mutate state  
- Runs before session creation  
- Ensures login events are recorded even if session creation fails  

---

# Architectural Boundaries

Authentication spans four layers:

## 1. API Layer
- Defines endpoints  
- Orchestrates Frank + Application pipelines  
- Issues cookies  
- Returns redirect responses  
- Uses Frank error boundary, security headers, and CORS  

## 2. Frank Layer
- Performs OIDC protocol logic  
- Normalizes claims  
- Produces protocol‑level result  
- Contains no business logic  

## 3. Application Layer
- Performs identity resolution  
- Creates sessions  
- Computes redirect URL  
- Computes cookie value  
- Contains no protocol logic  

## 4. Domain Layer
- Owns Owner aggregate  
- Owns Session entity  
- Owns invariants  
- Contains no Infrastructure or API references  

---

# Data Flow Diagram

```
Browser → /api/auth/login
    → Redirect to Auth0

Auth0 → /api/auth/callback?code=XYZ
    → Frank pipeline (protocol)
    → Application pipeline (business)
    → API issues cookie + redirect
```

---

# Error Handling Architecture

Authentication errors fall into these categories:

## 1. API Errors — 400
- Missing authorization code  

## 2. Protocol Errors — 502
- Token exchange failure  
- Missing access token  
- Userinfo failure  
- Missing external ID  

## 3. Business Errors — 500
- Identity resolver failure  
- Owner creation failure  
- Session creation failure  

All errors are surfaced via:

- Frank error boundary  
- ProblemDetails JSON  
- Logged exceptions  
- No cookies issued  

---

# Testing Architecture

Authentication is tested at three levels:

## 1. Frank Pipeline Tests
- Protocol success/failure  
- Claims normalization  
- Provider error handling  

## 2. Application Pipeline Tests
- Identity resolution  
- Session creation  
- Redirect computation  
- Cookie value computation  

## 3. API Endpoint Tests
- Missing `code` → `400`  
- Valid `code` → cookie issued + redirect  
- Error propagation into global error boundary  

Tests use:

- Fake Frank pipeline  
- Fake Application pipeline  
- ApiFactory / ApiContext  

---

# Local Development Notes

## Auth0
Local dev uses:

```
http://localhost:5000/api/auth/callback
```

## Cookies
Local dev:

- `Secure=false`  
- `SameSite=Lax`  

Preview/prod:

- `Secure=true`  
- `HttpOnly=true`  
- `SameSite=Lax`  

---

# Related Documents

- **Session Management Guide**  
- **Identity Mapping Guide**  
- **Authentication Testing Guide**  
- **Authentication Operations Guide**  
- **Architecture Governance**  
- **API Governance**  
- **Security Governance**
