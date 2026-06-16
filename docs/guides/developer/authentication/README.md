# Authentication Guide

This section documents the backend authentication architecture for CampFitFurDogs.  
Authentication is implemented using **OIDC (OpenID Connect)** with an external identity provider (Auth0).  
All authentication logic is backend‑driven, session‑based, and aligned with the system’s purity rules and the **ImmutableContextBuilder** architecture.

Authentication is composed of three layers:

1. **Frank Auth Callback Pipeline** — protocol logic  
2. **Application Auth Callback Pipeline** — business logic  
3. **Api Callback Endpoint** — boundary logic  

This guide explains each part of the system and how they work together.

---

## Contents

- **[Authentication Configuration](authentication-configuration.md)**  
  Required configuration keys for local, preview, and production environments.

- **[Authentication Overview](authentication-overview.md)**  
  High‑level explanation of the OIDC flow, the three‑layer callback architecture, and system principles.

- **[Login Endpoint](login-endpoint.md)**  
  Details for `/api/auth/login`, which initiates the external login flow.  
  Pure redirect logic — no domain or persistence.

- **[Callback Endpoint](callback-endpoint.md)**  
  Details for `/api/auth/callback`, which orchestrates the Frank pipeline, Application pipeline, cookie issuance, and redirect.

- **[Frank Callback Pipeline](frank-callback-pipeline.md)**  
  Protocol‑level pipeline implemented using `ImmutableContextBuilder`.  
  Handles authorization code exchange, userinfo retrieval, and claim normalization.

- **[Application Callback Pipeline](application-callback-pipeline.md)**  
  Business‑level pipeline implemented using `ImmutableContextBuilder`.  
  Handles identity mapping, session creation, token hashing, cookie value computation, and redirect computation.

- **[Identity Mapping](identity-mapping.md)**  
  How external Auth0 identities map to internal Owner identities using a pure Application‑layer transformation.

- **[Session Token Architecture](session-token-architecture.md)**  
  How session tokens are generated, hashed, stored, and validated.

- **[Session Cookie Specification](session-cookie-specification.md)**  
  Technical details of cookie format, flags, security properties, and lifetime.

- **[Session Management](session-management.md)**  
  How session records are created, persisted, validated, and expired.

- **[Authentication Error Handling](authentication-error-handling.md)**  
  How errors are surfaced during login, callback, and session validation using the global exception boundary.

---

## Summary

- Authentication is **external** — no passwords are stored locally.  
- Login initiation is **pure** — no domain logic or persistence.  
- The authentication callback uses a **three‑layer builder‑based architecture**:
  - **Frank pipeline**  
    - Authorization code exchange  
    - Userinfo retrieval  
    - Claim normalization  
  - **Application pipeline**  
    - Identity mapping  
    - Owner creation or lookup  
    - Session creation  
    - Token hashing  
    - Cookie value computation  
    - Redirect computation  
  - **Api endpoint**  
    - Cookie issuance  
    - Redirect to frontend  

- Session management persists and validates the authenticated Owner identity.  
- Identity mapping ensures stable linkage between Auth0 users and internal Owners.  
- Session cookies are opaque, HttpOnly, Secure (preview/prod), SameSite‑Lax, and environment‑safe.  
- Authentication errors follow API error boundary rules and never leak provider details.  
- The previous step‑engine architecture has been **fully replaced** by the ImmutableContextBuilder model (ADR‑0054).

---

## Related Stories

- US‑110 — Authentication: Owner Login  
- US‑111 — Authentication: Session Management  
- US‑148 — Email Verification (depends on authenticated Owner identity)  
- US‑145 — Welcome Email (requires authenticated Owner identity)  
- US‑146 — Password Reset Email (requires verified identity)
