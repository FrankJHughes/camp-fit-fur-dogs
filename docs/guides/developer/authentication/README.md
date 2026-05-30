# Authentication Guide

This section documents the backend authentication architecture for CampFitFurDogs.  
Authentication is implemented using **OIDC (OpenID Connect)** with an external identity provider (Auth0).  
All authentication logic is backend‑driven, session‑based, and aligned with the system’s purity rules.

---

## Contents

- [Authentication Configuration](authentication-configuration.md)  
  Required configuration keys for local, preview, and production environments.

- [Authentication Overview](authentication-overview.md)  
  High‑level explanation of the OIDC flow and system principles.

- [Login Endpoint](login-endpoint.md)  
  Details for `/api/auth/login`, which initiates the external login flow.

- [Callback Endpoint](callback-endpoint.md)  
  Details for `/api/auth/callback`, which completes the OIDC flow and issues the session cookie.

- [Session Management](session-management.md)  
  How session cookies are created, stored, and validated.

---

## Summary

- Authentication is **external** — no passwords are stored locally.  
- Login initiation is **pure** — no domain logic or persistence.  
- Callback endpoint performs:
  - Authorization code exchange  
  - Userinfo retrieval  
  - Owner creation or lookup  
  - Session cookie issuance  
- Session management persists and validates the authenticated Owner identity.

---

## Related Stories

- US‑110 — Authentication: Owner Login  
- US‑111 — Authentication: Session Management  
- US‑148 — Email Verification (depends on authenticated owner identity)
