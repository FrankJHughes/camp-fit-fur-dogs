# Authentication Architecture Guide

This guide documents the **authentication architecture** implemented across:

- **US‑110 — Authentication: Owner Login (OIDC)**
- **US‑111 — Authentication: Session Management**

It explains how the system performs OIDC login, validates configuration, fetches userinfo, resolves identity, issues session cookies, creates sessions, and builds the final redirect — all using the dispatcher pipeline.

This guide does **not** define rules, boundaries, or decisions.  
Those live in:

- Governance (process + enforcement)  
- Conventions (how we implement)  
- ADRs (why decisions were made)

This guide focuses solely on **how authentication works today**.

---

# High‑Level Architecture

Authentication consists of three cooperating components:

1. **OIDC Login Initiation**  
2. **Auth Callback Pipeline**  
3. **Session Issuance + Identity Mapping + Audit Logging**

These components work together to produce a secure, server‑managed session cookie.

---

# Component 1 — OIDC Login Initiation

The login initiation endpoint:

- Generates the Auth0 authorization URL  
- Includes the correct redirect URI  
- Includes PKCE parameters  
- Redirects the browser to Auth0  

This endpoint performs no identity logic and no session logic.  
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

- Performs one job  
- Has no side effects outside its boundary  
- Receives a shared immutable context  
- Writes results back into the context via `with`  
- Must obey context invariants enforced by the executor  

This makes the authentication flow deterministic, testable, and composable.

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

---

## 2. ExchangeCodeStep
Exchanges the authorization code for an access token.

Input:  
- `ctx.Code`

Output:  
- `ctx.Token`

If the code is missing → **400 Bad Request** (handled by endpoint).  
If Auth0 returns no access token → next step fails with **502**.

---

## 3. FetchUserStep
Uses the access token to fetch the Auth0 user profile.

Input:  
- `ctx.Token`

Output:  
- `ctx.User`

If userinfo retrieval fails → **502 Bad Gateway**.

---

## 4. ValidateUserStep
Ensures the userinfo contains a valid external identifier:

- `ExternalId` (Auth0 `sub`)

If missing or empty → **502 Bad Gateway**.

---

## 5. ResolveIdentityStep
Maps the external identity to an internal domain identity.

Input:  
- `ctx.User`

Output:  
- `ctx.CustomerId`

Identity resolution:

- Is pure  
- May create a new Owner  
- Never uses email for identity  
- Never exposes internal IDs to Auth0  

---

## 6. AuditLoginStep
Writes an audit log entry for successful authentication.

Input:  
- `ctx.CustomerId`  
- `ctx.User.ExternalId`

Output:  
- No context mutation except returning same instance

Runs **before** session creation so login is recorded even if session creation fails.

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

Runs unconditionally.

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

---

# Identity Mapping in the Architecture

Identity mapping is a core architectural boundary:

- External identity → internal domain identity  
- Implemented via `IIdentityResolver`  
- Pure and deterministic  
- Never uses email  
- Never exposes internal IDs to Auth0  

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

## 2. Application Layer
- Contains pipeline steps  
- Contains identity resolution logic  
- Contains session creation logic  
- Contains audit logging logic  

## 3. Domain Layer
- Owns Owner aggregate  
- Owns Session entity  
- Owns invariants  

## 4. Infrastructure Layer
- Implements Auth0 client  
- Implements session repository  
- Implements Owner repository  
- Implements audit logger  

Purity and dependency rules are defined in Conventions.

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

All errors are surfaced as:

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
