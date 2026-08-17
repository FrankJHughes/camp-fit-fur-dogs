# Frank.Identity.Application — Overview

The Identity Application layer contains the executable logic that powers authentication, session management, lockout enforcement, rate‑limiting evaluation, token validation, and identity‑provider integration. It sits between the Identity API and the Identity Domain, providing pure identity behaviors without any HTTP routing, domain rules, or business logic. This layer ensures identity flows are secure, predictable, and consistent across all environments.

This document describes the Identity Application subsystem under:

```
docs/03-frank-identity/application
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

The Identity Application layer exists to:

- execute OIDC authentication flows  
- validate identity‑provider responses  
- issue, validate, and revoke session tokens  
- enforce lockout and rate‑limiting protections  
- extract and map identity claims  
- coordinate identity‑provider interactions  
- provide identity‑specific services consumed by the Identity API  

It is the operational core of the identity subsystem.

---

## Responsibilities of the Subsystem

### Authentication Services  
Responsible for executing OIDC login and callback flows.  
See: [Authentication Services](ca://s?q=Explain_identity_authentication_services)

### Session Services  
Manage session issuance, validation, expiration, and revocation.  
See: [Session Services](ca://s?q=Describe_identity_session_services)

### Lockout Services  
Evaluate lockout thresholds and reset lockout state (US‑133).  
See: [Lockout Services](ca://s?q=Explain_identity_lockout_services)

### Rate‑Limiting Services  
Apply identity‑specific rate‑limit rules to sensitive flows (US‑132).  
See: [Rate‑Limiting Services](ca://s?q=Explain_identity_rate_limiting_services)

### Token & Claim Services  
Validate tokens, extract claims, and map provider identity to platform identity.  
See: [Token Services](ca://s?q=Explain_identity_token_services)

### Audit Logging  
Record authentication, session, lockout, and provider‑interaction events.  
See: [Audit Logging](ca://s?q=Explain_identity_authentication_logging)

---

## How the Application Layer Connects to the Broader Platform

Identity Application collaborates with:

- **Frank.Identity.Api**  
  - endpoints call application services  
  - middleware uses session and lockout services  

- **Frank.Identity.Domain**  
  - minimal domain involvement (only where legacy email/password remains)  
  - lockout counters may be stored in domain entities if configured  

- **Frank.Core.Infrastructure**  
  - environment detection  
  - observations (correlation IDs, causation chains)  
  - exception handling  
  - configuration binding  

- **Frank.Core.Api**  
  - authentication/authorization middleware  
  - request pipeline integration  

The Application layer is the execution engine behind identity flows.

---

## Runtime Collaboration Points

Identity Application interacts with the runtime by:

- performing OIDC authentication  
- issuing and validating session tokens  
- evaluating lockout state  
- applying rate‑limit decisions  
- shaping identity error semantics  
- providing identity metadata to middleware  
- emitting audit logs for all identity events  

It ensures identity behavior is secure, predictable, and environment‑aware.

---

## Composition Flow (API → Application → Domain → Infrastructure)

```
Identity Endpoint
    ↓
Identity Application Services
        - Authentication
        - Session issuance/validation
        - Lockout evaluation
        - Rate‑limit evaluation
        - Claim extraction
        - Audit logging
    ↓
Identity Domain (minimal involvement)
    ↓
Infrastructure (environment, logging, configuration)
```

The Application layer is where identity logic actually executes.

---

## What Belongs in This Folder

- authentication service documentation  
- session service documentation  
- lockout service documentation  
- rate‑limiting service documentation  
- token/claim service documentation  
- audit logging documentation  
- identity application flow diagrams  
- identity configuration binding details  

This folder does **not** include:

- HTTP endpoints  
- middleware  
- domain aggregates  
- persistence logic  

Those belong in their respective vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity application implementation.  
Whenever identity flows, OIDC integration, or session behavior evolves, update this section to reflect the current platform architecture.
