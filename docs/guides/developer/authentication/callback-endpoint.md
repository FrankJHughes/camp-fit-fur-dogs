# Callback Endpoint — `/api/auth/callback`

The callback endpoint completes the **OIDC authorization code flow**.  
It validates configuration, exchanges the authorization code for tokens, fetches and validates the Auth0 user profile, resolves identity, logs the login event, issues a session cookie, creates the session, and redirects the user to the frontend.

The endpoint itself contains **no business logic**, **no identity logic**, and **no Infrastructure calls**.  
It simply orchestrates the dispatcher pipeline.

All authentication behavior is implemented inside the pipeline steps and governed by:

- Architecture Governance  
- Security Governance  
- Session Management Governance  
- Identity Mapping Governance  
- API Endpoint Purity  

---

# HTTP Request

```http
GET /api/auth/callback?code=XYZ
```

The endpoint accepts only the authorization code.  
Identity, authorization, and session logic are handled exclusively inside the pipeline.

---

# Behavior

The callback endpoint executes a strict, ordered **authentication pipeline**:

1. **Validate configuration**  
   - Ensures all required OIDC settings are present  
   - Missing values → 500 Internal Server Error  
   - Enforces Security + Operations Governance  
   - No environment access inside steps (uses abstractions only)

2. **Validate that `code` is present**  
   - Missing or empty → 400 Bad Request  
   - Shape validation only (no business logic)

3. **Exchange authorization code**  
   - Calls Auth0 `/oauth/token` via Infrastructure abstraction  
   - Retrieves an access token  
   - No tokens are persisted  
   - No Infrastructure types leak into Application

4. **Fetch userinfo**  
   - Calls Auth0 `/userinfo`  
   - Retrieves:
     - `sub` (external ID)
     - `email`
     - `given_name`
     - `family_name`  
   - Uses Infrastructure only through abstractions  
   - No identity logic here

5. **Validate userinfo**  
   - Ensures `sub` is present  
   - Missing `sub` → 502 Bad Gateway  
   - Enforces Identity Mapping Governance  
   - Email is never used for identity

6. **Resolve identity**  
   - Maps external identity to internal `CustomerId`  
   - Creates Owner if needed  
   - Uses `IIdentityResolver` (Application abstraction)  
   - Never exposes internal IDs to Auth0  
   - Never uses email for identity  
   - Pure, deterministic, invariant‑checked

7. **Audit login**  
   - Logs successful login with `CustomerId` + external ID  
   - Runs **before** session creation  
   - Uses Infrastructure via abstractions  
   - Does not mutate context

8. **Issue session cookie**  
   - Generates a 256‑bit session token  
   - Hashes it using SHA‑256  
   - Creates a `SessionCookie` value object  
   - Does **not** persist anything  
   - Cookie issuance happens in the API layer  
   - Enforces Session Token Governance

9. **Create session**  
   - Persists the session with the hashed token  
   - Associates it with the Owner  
   - Uses `IUnitOfWork`  
   - Enforces Session Management Governance  
   - No Infrastructure leakage into Domain

10. **Build redirect**  
    - Produces the final redirect URL for the frontend  
    - Uses configured `PostLoginRedirectUrl`  
    - Pure string construction (no environment access)

11. **Return redirect response**  
    - API layer issues the session cookie  
    - API layer returns `302 Found`  
    - API layer relies on:
      - Frank security headers  
      - Frank CORS  
      - Frank error boundary  
    - No business logic in the endpoint

---

# Session Cookie

The session cookie is:

- **HttpOnly**  
- **Secure** (production)  
- **SameSite=Lax**  
- **Max‑Age** configured  
- Contains an **opaque, random session token**  
- Backed by a **hashed** token stored in the database  
- Issued only after session creation succeeds  

Local development uses `Secure=false`.

Cookie issuance follows:

- Security Governance  
- Session Management Governance  
- API Endpoint Purity  

---

# Error Handling

All errors flow through the global exception → ProblemDetails mapping provided by Frank.

| Condition | Error Code | HTTP Status |
|----------|------------|-------------|
| Missing `code` | `ValidationError` | 400 |
| Missing OIDC configuration | `BadConfiguration` | 500 |
| Token exchange failure | `ExternalAuthProviderFailure` | 502 |
| Userinfo failure | `ExternalAuthProviderFailure` | 502 |
| Missing `sub` claim | `ExternalAuthProviderFailure` | 502 |
| Identity resolution failure | `Unexpected` | 500 |
| Session creation failure | `Unexpected` | 500 |
| Any other unhandled error | `Unexpected` | 500 |

Additional guarantees:

- No cookies are issued on failure  
- No partial sessions are created  
- All failures are logged  
- All responses are shaped by Frank’s error boundary  
- No Infrastructure types leak into API or Application  

---

# See Also

- **[Login Endpoint](ca://s?q=Show_login_endpoint_doc)**  
- **[Authentication Overview](ca://s?q=Show_authentication_overview)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
- **[Identity Mapping Guide](ca://s?q=Show_identity_mapping_guide)**  
- **[Session Management Guide](ca://s?q=Show_session_management_guide)**  
