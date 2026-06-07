# Authentication Architecture Guide

This guide documents the **authentication architecture** implemented across:

- **US‑110 — Authentication: Owner Login (OIDC)**
- **US‑111 — Authentication: Session Management**

It explains how the system performs OIDC login, validates configuration, fetches userinfo, resolves identity, issues session cookies, creates sessions, and builds the final redirect — all using the dispatcher pipeline.

This guide does **not** define rules, boundaries, or decisions.  
Those live in:

- Architecture Governance  
- Security Governance  
- Operations Governance  
- API Governance  
- Code Conventions  
- ADRs  

This guide focuses solely on **how authentication works today**.

---

# High‑Level Architecture

Authentication consists of three cooperating components:

1. **OIDC Login Initiation**  
2. **Auth Callback Pipeline**  
3. **Session Issuance + Identity Mapping + Audit Logging**

These components work together to produce a secure, server‑managed session cookie.

Authentication spans:

- API (endpoint orchestration + cookie issuance)  
- Application (pipeline steps + identity resolution + session creation)  
- Domain (Owner + Session invariants)  
- Infrastructure (Auth0 client + repositories + audit logger)  

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

# Component 2 — Auth Callback Pipeline

The callback endpoint uses the **dispatcher pipeline**, which breaks the authentication flow into small, composable steps.

The **actual pipeline**, in execution order, is:

1. **ValidateConfigurationStep**  
2. **ExchangeCodeStep**  
3. **FetchUserStep**  
4. **ValidateUserStep**  
5. **ResolveIdentityStep**  
6. **AuditLoginStep**  
7. **IssueCookieStep**  
8. **CreateSessionStep**  
9. **BuildRedirectStep**

Each step:

- Performs exactly one responsibility  
- Has no side effects outside its boundary  
- Receives a shared immutable context  
- Returns a new context via `with`  
- Must obey context invariants enforced by the executor  
- Uses Infrastructure only through abstractions  
- Never touches hosting providers, environment variables, or configuration directly  

This makes the authentication flow deterministic, testable, and composposable.

---

# Pipeline Step Responsibilities

## 1. ValidateConfigurationStep
Ensures all required OIDC configuration values are present:

- Authority  
- ClientId  
- ClientSecret  
- CallbackUrl  
- PostLoginRedirectUrl  

If any are missing → **500 Internal Server Error**.

Runs unconditionally.

This step enforces **startup configuration safety** required by Security + Operations Governance.

---

## 2. ExchangeCodeStep
Exchanges the authorization code for an access token.

Input:  
- `ctx.Code`

Output:  
- `ctx.Token`

If the code is missing → **400 Bad Request** (endpoint-level).  
If Auth0 returns no access token → next step fails with **502 Bad Gateway**.

Uses Infrastructure via abstractions (never directly).

---

## 3. FetchUserStep
Uses the access token to fetch the Auth0 user profile.

Input:  
- `ctx.Token`

Output:  
- `ctx.User`

If userinfo retrieval fails → **502 Bad Gateway**.

This step performs no identity logic.

---

## 4. ValidateUserStep
Ensures the userinfo contains a valid external identifier:

- `ExternalId` (Auth0 `sub`)

If missing or empty → **502 Bad Gateway**.

This step enforces **external identity integrity**.

---

## 5. ResolveIdentityStep
Maps the external identity to an internal domain identity.

Input:  
- `ctx.User`

Output:  
- `ctx.CustomerId`

Identity resolution:

- Is pure  
- Uses `IIdentityResolver`  
- May create a new Owner  
- Never uses email  
- Never exposes internal IDs to Auth0  
- Never touches Infrastructure directly  

This step enforces **Identity Mapping Governance**.

---

## 6. AuditLoginStep
Writes an audit log entry for successful authentication.

Input:  
- `ctx.CustomerId`  
- `ctx.User.ExternalId`

Output:  
- Same context instance (no mutation)

Runs **before** session creation so login is recorded even if session creation fails.

This step enforces **Audit Logging Governance**.

---

## 7. IssueCookieStep
Generates a new session token and cookie.

Output:  
- `ctx.TokenHash`  
- `ctx.SessionCookie`

This step:

- Generates a 256‑bit random token  
- Hashes it using SHA‑256  
- Creates a `SessionCookie` value object  
- Does **not** persist anything  
- Does **not** issue the cookie (API layer does that)  

This step enforces **Session Token Governance**.

---

## 8. CreateSessionStep
Creates and persists the session.

Input:  
- `ctx.TokenHash`  
- `ctx.CustomerId`  
- `ctx.Now`

Output:  
- `ctx.Session`

This step:

- Creates a domain `Session`  
- Stores the hash in the database  
- Associates the session with the Customer  
- Commits via `IUnitOfWork`  

This step enforces **Session Persistence Governance**.

---

## 9. BuildRedirectStep
Builds the final redirect URL.

Input:  
- `ctx.Session`  
- `ctx.SessionCookie`

Output:  
- `ctx.RedirectUrl`

The API layer uses this to issue:

- `302 Found`  
- `Location: <redirect>`  
- `Set-Cookie: cfd.session=...`  

This step enforces **Post‑Login Redirect Governance**.

---

# Identity Mapping in the Architecture

Identity mapping is a core architectural boundary:

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

1. **Token generation** (IssueCookieStep)  
2. **Token hashing** (IssueCookieStep)  
3. **Session creation** (CreateSessionStep)  
4. **Cookie issuance** (API layer)  
5. **Redirect** (BuildRedirectStep)

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

This enforces **Session Security Governance**.

---

# Audit Logging in the Architecture

Audit logging is performed by **AuditLoginStep**:

- Runs after identity resolution  
- Logs `CustomerId` + `ExternalId`  
- Does not mutate context  
- Does not affect session creation  

This ensures login events are recorded even if session creation fails.

---

# Architectural Boundaries

Authentication spans four layers:

## 1. API Layer
- Defines endpoints  
- Orchestrates the pipeline  
- Issues cookies  
- Returns redirect responses  
- Uses Frank error boundary, security headers, and CORS  

## 2. Application Layer
- Contains pipeline steps  
- Contains identity resolution logic  
- Contains session creation logic  
- Contains audit logging logic  
- Contains no Infrastructure references  

## 3. Domain Layer
- Owns Owner aggregate  
- Owns Session entity  
- Owns invariants  
- Contains no Infrastructure or API references  

## 4. Infrastructure Layer
- Implements Auth0 client  
- Implements session repository  
- Implements Owner repository  
- Implements audit logger  
- Implements hosting provider abstractions  

Purity and dependency rules are defined in **Architecture Governance**.

---

# Data Flow Diagram (Updated)

```
Browser → /api/auth/login
    → Redirect to Auth0

Auth0 → /api/auth/callback?code=XYZ
    → Validate configuration
    → Exchange code
    → Fetch userinfo
    → Validate userinfo
    → Resolve identity
    → Audit login
    → Issue cookie
    → Create session
    → Build redirect
    → Return redirect response
```

---

# Error Handling Architecture

Authentication errors fall into these categories:

## 1. Configuration Errors — 500
- Missing Authority  
- Missing ClientId  
- Missing ClientSecret  
- Missing CallbackUrl  
- Missing PostLoginRedirectUrl  

## 2. Client Errors — 400
- Missing authorization code  

## 3. External Errors — 502
- Missing access token  
- Userinfo failure  
- Missing external ID (`sub`)  

## 4. Identity Errors — 500
- Identity resolver failure  
- Owner creation failure  

## 5. Session Errors — 500
- Database failure  
- Cookie issuance failure  

All errors are surfaced via:

- Frank error boundary  
- ProblemDetails JSON  
- Logged exceptions  
- No cookies issued  

---

# Testing Architecture

Authentication is tested at three levels:

## 1. Unit Tests
- ValidateConfigurationStep  
- ExchangeCodeStep  
- FetchUserStep  
- ValidateUserStep  
- ResolveIdentityStep  
- AuditLoginStep  
- IssueCookieStep  
- CreateSessionStep  
- BuildRedirectStep  

## 2. Executor Tests
- Step ordering  
- Conditional execution  
- Invariant enforcement  
- Diagnostics lifecycle  
- Error propagation  

## 3. Integration Tests
- Full callback flow  
- Cookie issuance  
- Audit logging  
- Error paths  
- Production cookie security  
- Frank middleware integration  

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
- **Create Account Form Guide**  
- **Create Account Feature Slice Guide**  
- **Architecture Governance**  
- **Security Governance**  
- **Operations Governance**
