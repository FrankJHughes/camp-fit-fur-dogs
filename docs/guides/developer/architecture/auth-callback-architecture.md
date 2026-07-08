# Authentication Callback Architecture  
Camp Fit Fur Dogs — Product‑Specific Authentication Flow

This document describes the **Camp Fit Fur Dogs authentication callback architecture** — the flow that processes an external OIDC login response and converts it into a CFFD application session.

Frank provides the OIDC plumbing, dispatcher, and environment abstractions.  
Camp Fit Fur Dogs defines the **product‑specific rules**, **session behavior**, and **domain interactions**.

This guide explains how the callback endpoint, Frank’s OIDC capabilities, and the CFFD Application layer work together.

---

# 1. Purpose

The authentication callback flow exists to:

- Receive the OIDC authorization response  
- Validate and decode the OIDC `state`  
- Validate the authorization code  
- Exchange the code for tokens  
- Normalize identity claims  
- Resolve or create the authenticated Owner  
- Establish a CFFD application session  
- Issue the `session` cookie  
- Redirect the user to the correct post‑login location  

This flow is **product‑specific** and sits on top of Frank’s OIDC capabilities.

---

# 2. High‑Level Architecture

The callback flow spans **three layers**:

```
HTTP Callback Endpoint (API)
        ↓
Frank OIDC Callback Pipeline (Framework)
        ↓
CFFD Application Authentication Pipeline (Product)
        ↓
Session Cookie Issuance + Redirect (API)
```

Each layer has a distinct responsibility.

---

# 3. Layer Responsibilities

## 3.1 API Callback Endpoint (CFFD)

The endpoint:

- Accepts the OIDC redirect request  
- Performs **shape validation only**  
- Extracts and decodes `state`  
- Extracts the authorization code  
- Invokes Frank’s OIDC callback pipeline  
- Receives the normalized identity payload  
- Invokes the CFFD Application authentication pipeline  
- Issues the **CFFD session cookie (`session`)**  
- Redirects the user to the `return_url` from decoded state  

The endpoint contains **no business logic**.

---

## 3.2 Frank OIDC Callback Pipeline (Framework)

Frank handles:

- State validation  
- Code exchange  
- Token validation  
- Claims extraction  
- Identity normalization  
- Error shaping  
- Security boundaries  

Frank returns a **normalized identity payload**:

```
NormalizedIdentity {
    ExternalId
    Email
    Name
    Provider
    Claims[]
}
```

Frank does **not**:

- create CFFD users  
- create sessions  
- enforce product rules  
- perform domain logic  

---

## 3.3 CFFD Application Authentication Pipeline (Product)

This layer performs all **product‑specific logic**:

- Resolve Owner by external identity  
- Create Owner if needed  
- Enforce product authentication rules  
- Emit authentication observability events  
- Create a CFFD session  
- Produce a `SessionEstablishedResult` containing the session token value  

This pipeline is implemented using:

- Commands  
- Queries  
- Validators  
- Application services  
- Domain rules  

Frank is not involved in this logic.

---

# 4. Detailed Flow

```
1. User logs in via external provider
2. Provider redirects to /api/identity/callback
3. API endpoint receives request
4. API decodes state → return_url
5. Frank validates state + exchanges code for tokens
6. Frank normalizes identity → NormalizedIdentity
7. API passes identity to Application
8. Application resolves or creates Owner
9. Application creates session → CookieValue
10. API issues the "session" cookie
11. API redirects user to return_url
```

---

# 5. Session Establishment

The Application layer produces:

```
SessionEstablishedResult {
    OwnerId
    SessionId
    CookieValue
}
```

The API endpoint:

- issues the **domain session cookie (`session`)**  
- performs the redirect  

Session cookies:

- are HTTP‑only  
- are Secure  
- use SameSite=Strict  
- contain no PII  
- contain no tokens  
- encode expiration internally  

> **Note:** The legacy ASP.NET cookie (`cffd.session`) has been fully removed.  
> The **only** authentication cookie is the domain cookie: `session`.

---

# 6. Redirect Rules

The callback endpoint determines the redirect target using:

1. `return_url` from the original login request (encoded in `state`)  
2. A product‑specific default (`Frontend:BaseUrl`) if missing  

Rules:

- returnUrl must be validated  
- returnUrl must be local or well‑formed  
- invalid returnUrl → default redirect  

Redirect computation is **API‑level**, not Application‑level.

---

# 7. Error Handling

Frank handles:

- invalid state  
- invalid code  
- token exchange failures  
- token validation failures  

CFFD handles:

- Owner resolution failures  
- Domain rule violations  
- Session creation failures  

The API endpoint converts errors into:

- safe redirects  
- safe error pages  
- structured observability events  

---

# 8. Observability

CFFD emits product‑specific events:

- `cffd.auth.callback.started`  
- `cffd.auth.callback.identity_resolved`  
- `cffd.auth.callback.owner_created`  
- `cffd.auth.callback.session_created`  
- `cffd.auth.callback.completed`  
- `cffd.auth.callback.failed`  

Frank emits framework‑level events (token exchange, validation, etc.).

---

# 9. Security Rules

- No tokens are stored in cookies  
- No tokens are stored in the database  
- No tokens are logged  
- No PII is logged  
- Identity is resolved only through Frank’s normalized payload  
- returnUrl must be validated  
- Session cookies must be HTTP‑only and secure  
- Session cookies must be deleted on logout  

---

# 10. Logout Architecture (US‑222)

Logout is now a **first‑class identity operation**.

The logout endpoint:

- Deletes the **session** cookie  
- Determines the post‑logout redirect target  
- Returns a `LogoutResponse(returnUrl)`  
- Leaves navigation to the frontend  

Logout does **not**:

- call `SignOutAsync`  
- interact with ASP.NET authentication handlers  
- clear any legacy cookies  

This aligns logout with the new session model introduced in the callback flow.

---

# 11. Identity Retrieval

The identity endpoint uses Frank’s `ICurrentUser` abstraction.

It:

- requires authorization  
- returns the resolved identity (currently `Name`)  
- reflects the authenticated session created during callback  

Identity resolution is performed entirely by the authentication pipeline.

---

# 12. Contributor Guidelines

When modifying the callback flow:

1. Do not bypass Frank’s OIDC pipeline  
2. Do not perform token validation manually  
3. Do not create sessions in the API layer  
4. Do not store tokens  
5. Do not embed domain logic in the endpoint  
6. Do not modify Frank internals  
7. Keep all product logic in the Application layer  
8. Emit observability events for each major step  
9. Use the `session` cookie exclusively  
10. Ensure logout deletes the `session` cookie  

---

# 13. Summary

- Frank handles OIDC protocol details  
- CFFD handles product‑specific authentication logic  
- The API endpoint orchestrates the flow  
- The Application layer resolves identity and creates sessions  
- The Domain layer enforces invariants  
- The endpoint issues the `session` cookie and redirects the user  
- Logout deletes the `session` cookie and returns a redirect target  

This architecture keeps authentication:

- deterministic  
- testable  
- secure  
- maintainable  
- product‑specific  

---

# Related Documents

- CFFD Developer Guide  
- API Endpoint Purity Guide  
- Authentication Overview  
- Session Management Guide  
- Frank OIDC Callback Pipeline Guide
