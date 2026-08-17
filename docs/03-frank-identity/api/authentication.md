# Frank.Identity.Api — Overview

The Identity API provides authentication, session management, and identity‑related protections for the Frank platform. It exposes pure, infrastructure‑level endpoints for login, logout, session validation, lockout, and rate‑limiting. These endpoints contain **no domain logic** and serve as the entry point for all authenticated owner interactions.

This document describes the Identity API under:

```
docs/03-frank-identity/api
```

and maps it back to its implementation in:

```
src/Frank/Identity/Api
```

---

## Purpose

The Identity API exists to:

- authenticate owners using OIDC  
- issue, validate, and revoke session tokens  
- enforce account lockout and rate‑limiting protections  
- provide secure, pure identity endpoints  
- integrate with the Frank identity subsystem (US‑110, US‑111, US‑133, US‑184)  
- support downstream vertical slices requiring authenticated owners  

Identity endpoints do not mutate domain aggregates and do not contain business rules.

---

## Responsibilities of the Identity API

### [Authentication (US‑110)](ca://s?q=Explain_OIDC_authentication_flow)
- Initiates OIDC login flows  
- Handles identity‑provider callbacks  
- Issues authenticated session tokens  
- Returns correct HTTP semantics for failed login attempts  
- Logs authentication events for observability  

### [Session Management (US‑111)](ca://s?q=Describe_session_management_in_Frank_identity)
- Validates session tokens on every request  
- Rejects expired or invalid tokens with `401 Unauthorized`  
- Provides logout and session revocation endpoints  
- Ensures session middleware remains domain‑free  

### [Account Lockout (US‑133)](ca://s?q=Explain_account_lockout_behavior)
- Locks accounts after repeated failed login attempts  
- Resets lockout state on successful login  
- Prevents account enumeration  
- Logs lockout events for security monitoring  

### [Rate Limiting (US‑132)](ca://s?q=Explain_rate_limiting_for_identity_endpoints)
- Applies stricter rate limits to authentication endpoints  
- Returns `429 Too Many Requests` with `Retry-After`  
- Protects against brute‑force and credential‑stuffing attacks  

### [De‑feature Local Identity (US‑184)](ca://s?q=Describe_de_feature_local_identity)
- Removes password‑based login where OIDC is the only identity model  
- Ensures all identity flows use the Authenticated User Service  
- Eliminates unused local identity code paths  

---

## API Endpoint Purity

Identity endpoints follow strict purity rules:

- **No domain logic**  
- **No aggregate mutation**  
- **No persistence beyond session state**  
- **No business rules**  
- **No cross‑slice dependencies**

Identity endpoints only authenticate, validate, issue tokens, revoke tokens, and return identity‑related responses.

---

## Runtime Collaboration Points

Identity API collaborates with:

- **Frank.Core.Infrastructure**  
  - Observations (correlation IDs, tracing)  
  - Exception handling  
  - Environment detection  
  - Rate‑limiting middleware  

- **Frank.Identity.Application**  
  - OIDC handlers  
  - Session token services  
  - Lockout services  
  - Identity provider integrations  

- **Frank.Identity.Domain**  
  - Minimal domain involvement (only where legacy email/password remains)  

- **Frank.Core.Api**  
  - Authentication/authorization middleware  
  - Request pipeline integration  

Identity API is the authentication gateway for all owner‑facing vertical slices.

---

## Composition Flow (Login → Session → Authenticated Request)

```
Owner → Login Endpoint (OIDC)
    ↓
Identity Provider Callback
    ↓
Session Token Issued
    ↓
Authenticated Request
    ↓
Session Validation Middleware
    ↓
Application Handler
```

Identity API ensures authentication is secure, predictable, and isolated.

---

## What Belongs in This Folder

- Identity API endpoint documentation  
- OIDC login flow documentation  
- Session token issuance & validation  
- Lockout & rate‑limiting behavior  
- Identity purity rules  
- Request/response contracts for identity endpoints  

This folder does **not** contain:

- domain models  
- persistence logic  
- onboarding flows  
- customer registration  
- authorization rules  

Those belong in their respective vertical slices.

---

## Notes

Keep this document grounded in the actual Identity API implementation.  
Whenever identity flows, OIDC integration, or session behavior evolves, update this section to reflect the current platform architecture.
