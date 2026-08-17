# Frank.Identity.Application — Users

The **Users subsystem** defines how identity‑level user information is represented, retrieved, and used within the Identity Application layer. Unlike domain‑level “owners,” identity‑level users are strictly authentication subjects: they originate from the identity provider, are mapped into platform identity, and participate in session, lockout, and authorization flows. This subsystem contains **no domain logic**, **no business rules**, and **no persistence of domain aggregates** — only identity‑specific user handling.

This document describes the Users subsystem under:

```
docs/03-frank-identity/application
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

The Users subsystem exists to:

- represent authenticated identity subjects within the platform  
- map identity‑provider claims into platform user identifiers  
- provide identity metadata to session, lockout, and authorization services  
- support identity flows without leaking domain concepts  
- ensure identity behavior remains consistent across environments  

Identity “users” are authentication subjects — not domain owners, staff, or admins.

---

## Responsibilities of the Subsystem

### Identity User Representation  
Identity users are represented as lightweight identity objects.

Responsibilities:

- store provider subject identifiers  
- store mapped platform identity identifiers  
- expose identity claims relevant to authentication and authorization  
- provide metadata for session issuance and validation  

Identity users do **not** contain domain fields such as dog ownership, bookings, or customer profile data.

---

### Claim Mapping  
Identity users are created from OIDC claims.  
See: [Token & Claim Services](ca://s?q=Explain_identity_token_services)

Responsibilities:

- extract required claims from ID tokens  
- validate presence of mandatory identity attributes  
- map provider identity → platform identity (owner ID)  
- normalize claim formats across providers  

Claim mapping is identity logic, not domain logic.

---

### Identity Provider Integration  
Identity users reflect the identity provider’s representation of the authenticated subject.

Responsibilities:

- store provider subject (`sub`)  
- store provider issuer (`iss`)  
- store provider metadata (optional, environment‑specific)  
- support multi‑provider scenarios if configured  

Identity users remain provider‑agnostic at the application layer.

---

### Session Integration  
Identity users participate in session issuance and validation.  
See: [Sessions](ca://s?q=Describe_identity_session_services)

Responsibilities:

- embed identity user metadata into session tokens  
- provide identity attributes for session validation  
- support sliding expiration and revocation flows  
- expose identity information to middleware  

Identity users are the core identity payload carried across requests.

---

### Lockout Integration  
Identity users participate in lockout evaluation (US‑133).  
See: [Lockout Services](ca://s?q=Explain_identity_lockout_services)

Responsibilities:

- identify which user is being locked out  
- track failed login attempts (identity‑level, not domain‑level)  
- reset lockout state after successful authentication  

Lockout state is identity state, not domain state.

---

### Authorization Integration  
Identity users provide identity metadata for authorization decisions.  
See: [Authorization](ca://s?q=Explain_identity_authorization)

Responsibilities:

- expose identity claims for authorization middleware  
- provide platform identity identifiers for downstream checks  
- support environment‑specific authorization behavior  

Authorization uses identity users as the source of identity truth.

---

## How Users Connect to the Broader Platform

Identity users collaborate with:

- **Frank.Identity.Application**  
  - authentication services (user creation)  
  - session services (user embedding)  
  - lockout services (user evaluation)  
  - rate‑limiting services (user‑aware throttling)  

- **Frank.Identity.Domain**  
  - minimal involvement (only where legacy identity remains)  

- **Frank.Core.Infrastructure**  
  - environment detection  
  - observations (correlation IDs, causation chains)  
  - exception handling  

- **Frank.Core.Api**  
  - authorization middleware  
  - identity‑aware endpoint filters  

Identity users are the bridge between identity providers and the platform.

---

## Runtime Collaboration Points

Identity users interact with the runtime by:

- carrying identity metadata across requests  
- shaping session issuance and validation  
- participating in lockout and rate‑limit decisions  
- providing identity attributes for authorization  
- emitting audit logs for identity events  
- supporting environment‑specific identity behavior  

Identity users ensure authentication remains secure, predictable, and observable.

---

## Composition Flow (Provider → Claims → Identity User → Session → Application)

```
Identity Provider
    ↓
Claims Extracted (Application)
    ↓
Identity User Created
    ↓
Session Issued
    ↓
Identity Middleware
        - Validate session
        - Attach identity user
    ↓
Authorization Middleware
    ↓
Application Handler (if authorized)
```

Identity users are the identity payload that flows through the platform.

---

## What Belongs in This Document

- identity user representation  
- claim mapping rules  
- provider identity integration  
- session and lockout integration  
- authorization integration  
- identity metadata structure  

This document does **not** include:

- domain user/owner models  
- customer profile data  
- persistence logic  
- business‑rule authorization  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity user‑handling implementation.  
Whenever identity‑provider behavior, claim mapping rules, or session metadata evolves, update this section to reflect the current platform architecture.
