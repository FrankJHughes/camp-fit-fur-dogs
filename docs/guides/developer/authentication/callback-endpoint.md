# Callback Endpoint — `/api/auth/callback` (Aligned With Auth Callback Refactor)

The callback endpoint completes the **OIDC authorization code flow**.  
It extracts the authorization code, invokes the **Frank Auth Callback Pipeline** (protocol), invokes the **Application Auth Callback Pipeline** (business), issues the session cookie, and redirects the user to the frontend.

The endpoint itself contains:

- **no business logic**  
- **no identity logic**  
- **no Infrastructure calls**  
- **no protocol logic**  

It is a **thin orchestrator** that coordinates the two pipelines and performs HTTP‑boundary responsibilities only.

All authentication behavior is implemented inside the Frank and Application pipelines and governed by:

- [Architecture Governance](ca://s?q=Open_architecture_governance)  
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
Identity, protocol, and session logic are handled exclusively inside the pipelines.

---

# Behavior (Post‑Refactor)

The callback endpoint performs **three** responsibilities:

1. **Extract and validate the authorization code**  
2. **Invoke the Frank + Application pipelines**  
3. **Issue the session cookie and redirect**  

Everything else happens inside the pipelines.

---

# 1. Extract and Validate the Authorization Code

- Reads `code` from the query string  
- If missing or empty → **400 Bad Request**  
- No protocol logic  
- No identity logic  
- No Infrastructure access  

This is the only validation the endpoint performs.

---

# 2. Invoke the Frank Auth Callback Pipeline (Protocol Layer)

The endpoint calls:

```
FrankAuthCallbackPipeline.BuildAsync(...)
```

Frank performs all **OIDC protocol work**:

- Validates OIDC configuration  
- Exchanges the authorization code for tokens  
- Fetches userinfo (if required)  
- Normalizes provider‑specific claims  
- Produces a stable, provider‑agnostic identity payload  

Frank pipeline **does not**:

- Resolve identity  
- Create sessions  
- Compute redirect URLs  
- Compute cookie values  
- Perform any business logic  

Frank pipeline errors are shaped by Frank’s error boundary.

---

# 3. Invoke the Application Auth Callback Pipeline (Business Layer)

The endpoint then calls:

```
ApplicationAuthCallbackPipeline.BuildAsync(...)
```

Application performs all **business logic**:

- Validates required identity claims (e.g., `sub`)  
- Resolves or creates the internal Owner record  
- Logs the login audit event  
- Creates the session  
- Computes the session token hash  
- Computes the cookie value (opaque token)  
- Computes the final redirect URL  

Application pipeline **does not**:

- Perform OIDC protocol logic  
- Perform token exchange  
- Perform userinfo calls  
- Issue cookies  
- Perform HTTP operations  

The result object includes:

- `CustomerId`  
- `SessionId`  
- `TokenHash`  
- `CookieValue`  
- `RedirectUrl`  

---

# 4. Issue the Session Cookie (API Boundary)

The API endpoint:

- Uses the `CookieValue` from the Application pipeline  
- Issues the secure session cookie  
- Applies Frank security headers  
- Applies Frank CORS  
- Applies Frank error boundary  

Cookie properties:

- **HttpOnly**  
- **Secure** (production)  
- **SameSite=Lax**  
- Contains an **opaque, random session token**  
- Backed by a **hashed token** stored in the database  

Local development uses `Secure=false`.

---

# 5. Redirect the User

The endpoint returns:

```
302 Found
Location: <RedirectUrl from Application pipeline>
Set-Cookie: cfd.session=...
```

The redirect URL is computed **only** by the Application pipeline.

The endpoint does not:

- Construct redirect URLs  
- Perform business logic  
- Perform identity logic  

---

# Error Handling

All errors flow through Frank’s global exception → ProblemDetails mapping.

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
- No domain logic runs on failure paths  

---

# Summary

The callback endpoint:

- Extracts the authorization code  
- Invokes Frank (protocol)  
- Invokes Application (business)  
- Issues the cookie  
- Redirects the user  

It contains **no business logic**, **no protocol logic**, and **no Infrastructure logic**.

---

# See Also

- **[Login Endpoint](ca://s?q=Show_login_endpoint_doc)**  
- **[Authentication Overview](ca://s?q=Show_authentication_overview)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
- **[Identity Mapping Guide](ca://s?q=Show_identity_mapping_guide)**  
- **[Session Management Guide](ca://s?q=Show_session_management_guide)**
