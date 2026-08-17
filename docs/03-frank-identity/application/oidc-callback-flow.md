# Frank.Identity.Application — OIDC Callback Flow

The OIDC Callback Flow is the core mechanism by which the Frank Identity subsystem completes authentication. After an owner initiates login and is redirected to the identity provider, the provider returns control to the platform through the callback endpoint. The Identity Application layer processes this callback, validates the provider response, extracts claims, and issues a session token. This flow contains **no HTTP routing**, **no domain logic**, and **no business rules** — only pure identity‑application behavior.

This document describes the OIDC Callback Flow under:

```
docs/03-frank-identity/application
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

The OIDC Callback Flow exists to:

- complete the authentication handshake with the identity provider  
- validate the provider’s response and token signatures  
- extract identity claims and map them to platform identity  
- issue a secure session token for the authenticated owner  
- record audit events for authentication success or failure  
- ensure identity behavior is consistent across environments  

This flow is the heart of OIDC authentication in the Frank platform.

---

## Responsibilities of the Subsystem

### 1. Receiving the Provider Callback  
The callback handler receives the authorization response from the identity provider.  
See: [Authentication Services](ca://s?q=Explain_identity_authentication_services)

Responsibilities:

- read `code`, `state`, and provider metadata  
- validate the `state` parameter to prevent CSRF  
- reject malformed or incomplete responses  

The callback handler does not perform any HTTP routing — that occurs in the API layer.

---

### 2. Exchanging the Authorization Code  
The application layer exchanges the authorization code for tokens.

Responsibilities:

- send a token request to the identity provider  
- receive ID token, access token, and metadata  
- validate token signatures and issuer  
- validate token expiration and nonce  

This step ensures the provider response is authentic and untampered.

---

### 3. Extracting and Mapping Claims  
The application layer extracts identity claims from the ID token.  
See: [Token & Claim Services](ca://s?q=Explain_identity_token_services)

Responsibilities:

- parse standard OIDC claims (subject, email, name, etc.)  
- map provider identity → platform identity (owner ID)  
- validate required claims  
- apply environment‑specific claim rules  

Claim mapping is identity logic, not domain logic.

---

### 4. Evaluating Lockout State  
Before issuing a session, the application layer checks lockout rules (US‑133).  
See: [Lockout Services](ca://s?q=Explain_identity_lockout_services)

Responsibilities:

- determine whether the owner is currently locked out  
- increment failed login counters if provider authentication failed  
- reset lockout state on successful login  

Lockout evaluation is performed before session issuance.

---

### 5. Issuing the Session Token  
Once identity is validated, the application layer issues a session token.  
See: [Session Services](ca://s?q=Describe_identity_session_services)

Responsibilities:

- generate a secure session token  
- apply expiration and sliding‑expiration rules  
- attach identity claims to the session  
- return token metadata to the API layer  

Session issuance is pure identity logic.

---

### 6. Recording Audit Events  
Audit logging captures all callback outcomes.  
See: [Audit Logging](ca://s?q=Explain_identity_authentication_logging)

Events include:

- callback received  
- token exchange success/failure  
- claim extraction success/failure  
- session issued  
- lockout triggered or reset  

Audit logs include correlation IDs and environment metadata.

---

## How the OIDC Callback Flow Connects to the Broader Platform

The callback flow collaborates with:

- **Frank.Identity.Api**  
  - callback endpoint invokes the application handler  
  - session token returned to the client  

- **Frank.Identity.Application**  
  - authentication services  
  - token/claim services  
  - lockout services  
  - session services  

- **Frank.Core.Infrastructure**  
  - environment detection  
  - observations (correlation IDs, causation chains)  
  - exception handling  
  - configuration binding  

- **Frank.Core.Api**  
  - middleware that consumes the session token on future requests  

The callback flow is the bridge between external identity providers and the Frank platform.

---

## Runtime Collaboration Points

The callback flow interacts with the runtime by:

- validating provider responses  
- issuing secure session tokens  
- enforcing lockout rules  
- logging authentication events  
- shaping identity error semantics  
- integrating with environment‑specific behavior  

It ensures authentication is secure, predictable, and fully observable.

---

## Composition Flow (Provider → Callback → Application → Session)

```
Identity Provider
    ↓
OIDC Callback Endpoint (API)
    ↓
Callback Handler (Application)
        - Validate state
        - Exchange authorization code
        - Validate tokens
        - Extract claims
        - Evaluate lockout
        - Issue session token
        - Audit log
    ↓
Session Token Returned to API
    ↓
Client Receives Authenticated Session
```

This flow completes the authentication handshake and establishes the owner’s identity.

---

## What Belongs in This Document

- callback flow responsibilities  
- token exchange and validation behavior  
- claim extraction and mapping rules  
- lockout evaluation during authentication  
- session issuance behavior  
- audit logging for authentication events  
- how the callback flow integrates with identity services  

This document does **not** include:

- HTTP endpoint routing  
- middleware behavior  
- domain authorization rules  
- persistence logic  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity callback implementation.  
Whenever identity‑provider behavior, token validation rules, or session issuance evolves, update this section to reflect the current platform architecture.
