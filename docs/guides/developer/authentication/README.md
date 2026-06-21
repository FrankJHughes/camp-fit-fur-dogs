# Authentication Guide  
**Aligned With Exclusive OIDC Authentication, De‑featured Local Identity, and ImmutableContextBuilder Architecture**

Authentication in CampFitFurDogs is implemented using **OIDC (OpenID Connect)** with an external identity provider (Auth0).  
All authentication logic is backend‑driven, session‑based, and implemented as a **horizontal cross‑cutting concern**, not a vertical slice.

Authentication is composed of three layers:

1. **Frank Auth Callback Pipeline** — protocol logic  
2. **Application Auth Callback Pipeline** — business logic  
3. **API Callback Endpoint** — boundary logic  

This guide explains each part of the system and how they work together.

---

## Contents

- **[Authentication Configuration](authentication-configuration.md)**  
  Required configuration keys for local, preview, and production environments.

- **[Authentication Overview](authentication-overview.md)**  
  High‑level explanation of the OIDC flow and the three‑layer callback architecture.

- **[Login Endpoint](login-endpoint.md)**  
  Pure redirect logic — no domain or persistence.

- **[Callback Endpoint](callback-endpoint.md)**  
  Orchestrates the Frank pipeline, Application pipeline, cookie issuance, and redirect.

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
- Authentication is a **horizontal concern**, not a vertical slice.  
- Login initiation is **pure** — no domain logic or persistence.  
- The authentication callback uses a **three‑layer builder‑based architecture**:

### **Frank Pipeline (Protocol Layer)**
- Authorization code exchange  
- Userinfo retrieval  
- Claim normalization  
- No domain logic  
- No session logic  

### **Application Pipeline (Business Layer)**
- Identity mapping  
- Owner creation or lookup  
- Session creation  
- Token hashing  
- Cookie value computation  
- Redirect computation  
- No protocol logic  
- No HTTP logic  

### **API Callback Endpoint (Boundary Layer)**
- Cookie issuance  
- Redirect to frontend  
- No business logic  
- No protocol logic  

- Session management persists and validates the authenticated Owner identity.  
- Identity mapping ensures stable linkage between Auth0 users and internal Owners.  
- Session cookies are opaque, HttpOnly, Secure (preview/prod), SameSite‑Lax, and environment‑safe.  
- Authentication errors follow API error boundary rules and never leak provider details.  
- The previous step‑engine architecture has been **fully replaced** by the ImmutableContextBuilder model (ADR‑0054).  
- Authentication is **not** a vertical slice and does **not** appear in the Vertical Slice Index.

---

## Related Stories

- US‑110 — Authentication: Owner Login  
- US‑111 — Authentication: Session Management  
- US‑148 — Email Verification (depends on authenticated Owner identity)  
- US‑145 — Welcome Email (requires authenticated Owner identity)  
- US‑146 — Password Reset Email (requires verified identity)  
