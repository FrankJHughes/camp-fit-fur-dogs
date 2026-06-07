# Authentication Overview

The application uses **OIDC‑based authentication** via an external identity provider (Auth0).  
Owners authenticate using Auth0’s hosted login page; **no passwords are stored locally**, **no identity provider tokens are persisted**, and **the backend manages all session state**.

Authentication is implemented as:

- A **pure login initiation endpoint** (no domain logic, no persistence)  
- A **deterministic callback pipeline** that performs configuration validation, token exchange, userinfo retrieval, identity resolution, audit logging, session creation, and redirect construction  

All authentication behavior follows **Architecture Governance**, **Security Governance**, and **API Endpoint Purity**.

---

# Flow Summary

1. Client calls **GET `/api/auth/login`**  
2. API constructs an Auth0 authorization URL  
3. API returns **302 Redirect** to Auth0  
4. User authenticates externally  
5. Auth0 redirects back to **GET `/api/auth/callback`** with an authorization code  
6. Callback pipeline validates configuration  
7. Pipeline exchanges the authorization code for an access token  
8. Pipeline fetches the Auth0 user profile  
9. Pipeline validates required user claims (e.g., `sub`)  
10. Pipeline resolves or creates the internal Owner record  
11. Pipeline logs a successful login audit event  
12. Pipeline generates a session token + cookie  
13. Pipeline creates and persists the session  
14. Pipeline builds the final redirect URL  
15. API issues the secure session cookie and redirects the user to the frontend dashboard  

This flow is deterministic, invariant‑checked, and enforced by the dispatcher pipeline.

---

# Principles

- Authentication is **external** — Auth0 performs identity proof  
- Login initiation endpoint is **pure** — no domain logic, no persistence, no identity resolution  
- Callback endpoint is implemented as a **strict, ordered pipeline**  
- Pipeline steps are **small, deterministic, and invariant‑checked**  
- No identity provider tokens are stored  
- Session token hashes are the only persisted authentication state  
- Session cookie is the only client‑side authentication state  
- Audit logging occurs **before** session creation  
- Redirect construction occurs **after** session creation  
- API endpoints rely on **Frank security headers**, **Frank CORS**, and **Frank error boundary**  
- Identity is resolved exclusively through `IIdentityResolver`  
- No endpoint performs identity parsing or authorization logic  
- No pipeline step accesses hosting providers or environment directly  

These principles align with **[Architecture Governance](ca://s?q=Open_architecture_governance)**, **Security Governance**, and **Session Management Governance**.

---

# See Also

- **[Login Endpoint](ca://s?q=Show_login_endpoint_doc)**  
- **[Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
- **[Identity Mapping Guide](ca://s?q=Show_identity_mapping_guide)**  
- **[Session Management Guide](ca://s?q=Show_session_management_guide)**  
