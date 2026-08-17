# Frank.Identity.Api — Authorization

The Authorization subsystem defines how authenticated owners gain access to protected API resources across the Frank platform. It ensures that identity‑derived claims, roles, and session state are evaluated consistently and securely, without leaking domain logic into the identity layer.

This document describes the Authorization subsystem under:

```
docs/03-frank-identity/api
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

Authorization exists to:

- enforce access control for authenticated owners  
- validate identity claims and session state before allowing resource access  
- ensure consistent authorization behavior across all vertical slices  
- keep authorization logic pure and free from domain‑specific rules  
- integrate with OIDC authentication and session management  

Authorization is a **cross‑cutting identity concern**, not a domain concern.

---

## Responsibilities of the Subsystem

### Identity‑Based Access Control

Authorization evaluates:

- session validity  
- identity provider claims  
- owner identifiers  
- environment‑specific rules (e.g., debug endpoints only in development)

It does **not** evaluate domain rules such as ownership of dogs, booking permissions, or business constraints. Those belong to application/domain layers.

### Claim and Role Evaluation

Authorization uses identity‑provider claims to determine:

- whether the caller is authenticated  
- whether the caller is an owner  
- whether the caller is an admin (if applicable)  
- whether the caller may access protected endpoints  

Claims are mapped from OIDC tokens and session state.

### Session Enforcement

Authorization integrates with session management (US‑111):

- expired sessions → `401 Unauthorized`  
- revoked sessions → `401 Unauthorized`  
- malformed tokens → `401 Unauthorized`  
- valid sessions → request proceeds to application layer  

Authorization does not mutate session state; it only evaluates it.

### Endpoint Protection

Authorization is applied via:

- middleware  
- endpoint filters  
- attribute‑based protection (e.g., `[Authorize]`)  
- custom identity guards for identity‑specific flows  

Protected endpoints require a valid session and appropriate identity claims.

---

## How Authorization Connects to the Broader Platform

Authorization collaborates with:

- **Frank.Identity.Api**  
  - session validation  
  - OIDC token parsing  
  - identity claim extraction  

- **Frank.Core.Infrastructure**  
  - exception handling for unauthorized/forbidden responses  
  - observation context for tracing authorization failures  
  - environment detection for debug‑only endpoints  

- **Frank.Core.Api**  
  - middleware pipeline  
  - endpoint routing and filters  

- **Frank.Identity.Application**  
  - session services  
  - identity provider integrations  

Authorization is the gatekeeper between identity and application layers.

---

## Runtime Collaboration Points

Authorization interacts with the runtime by:

- validating identity tokens on every request  
- enforcing session expiration and revocation  
- rejecting unauthorized access with structured responses  
- logging authorization failures with correlation metadata  
- supporting environment‑specific behavior (e.g., debug endpoints)  

Authorization ensures that only authenticated and properly scoped callers reach application handlers.

---

## Composition Flow (Authentication → Authorization → Application)

```
Owner Login (OIDC)
    ↓
Session Token Issued
    ↓
Request to Protected Endpoint
    ↓
Authorization Middleware
        - Validate session
        - Validate claims
        - Validate environment rules
    ↓
Application Handler (if authorized)
```

Authorization is a pure identity concern that precedes all application logic.

---

## What Belongs in This Document

- authorization responsibilities  
- claim and role evaluation  
- session enforcement rules  
- identity purity boundaries  
- how authorization integrates with identity and infrastructure layers  
- how authorization fits into the vertical slice lifecycle  

This document does **not** include:

- domain‑specific authorization rules  
- business logic permissions  
- resource ownership checks  
- application‑level authorization policies  

Those belong in the application or domain layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity authorization implementation.  
Whenever identity claims, session validation rules, or authorization middleware evolve, update this section to reflect the current platform architecture.
