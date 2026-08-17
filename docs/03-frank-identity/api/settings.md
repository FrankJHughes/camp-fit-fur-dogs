# Frank.Identity.Api — Settings

The Identity API exposes a small set of configuration settings that control authentication behavior, session lifecycle, lockout rules, rate‑limiting thresholds, and environment‑specific identity behavior. These settings are consumed by identity middleware and application‑level identity services, ensuring that identity flows remain predictable, secure, and consistent across environments.

This document describes the Identity API settings under:

```
docs/03-frank-identity/api
```

and maps them back to their implementation in:

```
src/Frank/Identity
```

---

## Purpose

Identity settings exist to:

- configure OIDC authentication parameters  
- define session token behavior and expiration  
- control lockout thresholds and reset rules  
- tune rate‑limiting for identity endpoints  
- support environment‑specific identity behavior  
- centralize identity configuration outside domain logic  

Settings ensure identity behavior is consistent across all deployments.

---

## Categories of Identity Settings

### [OIDC Settings](ca://s?q=Explain_OIDC_settings_in_Frank_identity)
OIDC configuration defines how the platform communicates with the identity provider:

- Authority / issuer URL  
- Client ID  
- Client secret (if applicable)  
- Redirect URIs  
- Scopes  
- Token validation parameters  

These settings are consumed by OIDC handlers in `Frank.Identity.Application`.

---

### [Session Settings](ca://s?q=Describe_identity_session_settings)
Session configuration controls how authenticated sessions behave:

- session token lifetime  
- sliding expiration rules  
- session revocation behavior  
- cookie vs. header‑based token transport  
- secure cookie flags (SameSite, HttpOnly, Secure)  

Session settings are used by session middleware and token services (US‑111).

---

### [Lockout Settings](ca://s?q=Explain_identity_lockout_settings)
Lockout configuration defines how the system responds to repeated failed login attempts:

- maximum failed attempts  
- lockout duration  
- lockout reset conditions  
- environment‑specific lockout tuning  

Lockout settings support account protection (US‑133).

---

### [Rate‑Limiting Settings](ca://s?q=Explain_identity_rate_limiting_settings)
Identity endpoints use stricter rate‑limiting thresholds:

- login attempt limits  
- callback request limits  
- session validation frequency  
- per‑IP and per‑account throttling rules  

Rate‑limiting settings support identity protection (US‑132).

---

### [Environment Settings](ca://s?q=Describe_identity_environment_settings)
Identity behavior varies by environment:

- debug endpoints enabled only in Development  
- verbose identity logging in non‑production  
- stricter error semantics in Production  
- optional diagnostic claims inspection in Development  

Environment detection is provided by `IEnvironment` from Frank.Core.Infrastructure.

---

## How Settings Connect to the Broader Platform

Identity settings collaborate with:

- **Frank.Identity.Application**  
  - OIDC handlers  
  - session token services  
  - lockout services  
  - rate‑limiting evaluators  

- **Frank.Core.Infrastructure**  
  - environment detection  
  - observations  
  - exception handling  
  - configuration binding  

- **Frank.Core.Api**  
  - middleware pipeline  
  - endpoint routing  

Settings ensure identity flows behave consistently across all layers.

---

## Runtime Collaboration Points

Identity settings interact with the runtime by:

- shaping authentication flows  
- determining session validity rules  
- enforcing lockout and rate‑limit thresholds  
- controlling identity error semantics  
- enabling or disabling debug identity endpoints  
- ensuring identity purity rules remain intact  

Settings are read at startup and influence all identity middleware and services.

---

## Composition Flow (Configuration → Identity Services → Middleware → API)

```
Configuration Binding
    ↓
Identity Settings (OIDC, Session, Lockout, Rate Limits)
    ↓
Identity Application Services
    ↓
Identity Middleware
    ↓
Identity Endpoints
```

Settings ensure identity behavior is predictable and environment‑aware.

---

## What Belongs in This Document

- identity configuration categories  
- how settings influence identity flows  
- how settings integrate with identity services and middleware  
- how settings fit into the vertical slice lifecycle  
- environment‑specific identity configuration rules  

This document does **not** include:

- domain configuration  
- persistence configuration  
- customer onboarding settings  
- business‑rule authorization settings  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity configuration implementation.  
Whenever identity flows, OIDC integration, or session behavior evolves, update this section to reflect the current platform architecture.
