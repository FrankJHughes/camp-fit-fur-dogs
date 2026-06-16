# Authentication Overview 

The application uses **OIDC‑based authentication** via an external identity provider (Auth0).  
Owners authenticate using Auth0’s hosted login page; **no passwords are stored locally**, **no identity provider tokens are persisted**, and **the backend manages all session state**.

Authentication is implemented as:

- A **pure login initiation endpoint** (no domain logic, no persistence)  
- A **three‑layer authentication callback architecture** consisting of:  
  1. **Frank Auth Callback Pipeline** — OIDC protocol logic  
  2. **Application Auth Callback Pipeline** — identity + session + redirect  
  3. **API Callback Endpoint** — cookie issuance + redirect orchestration  

All authentication behavior follows **[Architecture Governance](ca://s?q=Open_architecture_governance)**, Security Governance, and API Endpoint Purity.

---

# Flow Summary

1. Client calls **GET `/api/auth/login`**  
2. API constructs an Auth0 authorization URL  
3. API returns **302 Redirect** to Auth0  
4. User authenticates externally  
5. Auth0 redirects back to **GET `/api/auth/callback`** with an authorization code  

### Frank Pipeline (Protocol Layer)
6. Validates OIDC configuration  
7. Exchanges the authorization code for tokens  
8. Fetches and normalizes the Auth0 user profile  
9. Produces a stable, provider‑agnostic identity payload  

### Application Pipeline (Business Layer)
10. Validates required identity claims (e.g., `sub`)  
11. Resolves or creates the internal Owner record  
12. Logs a successful login audit event  
13. Creates a session and computes the session token hash  
14. Computes the final redirect URL  
15. Computes the cookie value (opaque session token)  

### API Endpoint (Boundary Layer)
16. Issues the secure session cookie  
17. Redirects the user to the frontend dashboard  

This flow is deterministic, layered, and enforced by the **Frank → Application → API** callback architecture.

---

# Principles (Aligned With Refactor)

- Authentication is **external** — Auth0 performs identity proof  
- Login initiation endpoint is **pure** — no domain logic, no persistence, no identity resolution  
- Callback endpoint is a **thin orchestrator**, not a pipeline  
- Protocol logic lives **only** in the Frank pipeline  
- Business logic lives **only** in the Application pipeline  
- Cookie issuance and redirect live **only** in the API endpoint  
- No identity provider tokens are stored  
- Session token hashes are the only persisted authentication state  
- Session cookie is the only client‑side authentication state  
- Audit logging occurs **before** session creation  
- Redirect computation occurs **after** session creation  
- API endpoints rely on **Frank security headers**, **Frank CORS**, and **Frank error boundary**  
- Identity is resolved exclusively through `IIdentityResolver`  
- No endpoint performs identity parsing or authorization logic  
- No pipeline layer accesses hosting providers or environment directly  
- HostingEngine and StartupEngine do **not** participate in authentication logic  

These principles align with **[Architecture Governance](ca://s?q=Open_architecture_governance)**, Security Governance, and Session Management Governance.

---

# See Also

- **[Login Endpoint](ca://s?q=Show_login_endpoint_doc)**  
- **[Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
- **[Identity Mapping Guide](ca://s?q=Show_identity_mapping_guide)**  
- **[Session Management Guide](ca://s?q=Show_session_management_guide)**
