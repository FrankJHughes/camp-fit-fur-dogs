# Callback Endpoint — `/api/identity/callback`  
**Aligned With Exclusive OIDC Authentication & Auth Callback Refactor**

The callback endpoint completes the **OIDC authorization code flow**.  
It extracts the encoded `state` (including `return_url`), extracts the authorization code, invokes the **Frank Auth Callback Pipeline** (protocol), invokes the **Application Auth Callback Pipeline** (business), issues the `session` cookie, and redirects the user.

The endpoint itself contains:

- **no business logic**  
- **no identity logic**  
- **no Infrastructure calls**  
- **no protocol logic**  
- **no redirect computation**  
- **no cookie value computation**  

It is a **thin orchestrator** that performs only HTTP‑boundary responsibilities.

All authentication behavior is implemented inside the Frank and Application pipelines and governed by:

- Architecture Governance  
- Security Governance  
- Session Management Governance  
- Identity Mapping Governance  
- API Endpoint Purity  

---

# HTTP Request

````http
GET /api/identity/callback?code=XYZ&state=<encoded>
````

The endpoint accepts:

- `code` — required  
- `state` — required (contains `return_url`)  

**Important:**  
The real implementation does **not** accept `returnUrl` as a query parameter.  
It is always encoded inside `state` by the login endpoint.

Identity, protocol, and session logic are handled exclusively inside the pipelines.

---

# Behavior (Post‑Refactor)

The callback endpoint performs **three** responsibilities:

1. **Extract and validate `state` + authorization code**  
2. **Invoke the Frank + Application pipelines**  
3. **Issue the session cookie and redirect**  

Everything else happens inside the pipelines.

---

# 1. Extract and Validate `state` + Authorization Code

- Reads `state` from the query string  
- Decodes `state` using `OidcStateEncoder`  
- Extracts `return_url` from decoded state  
- Reads `code` from the query string  
- If `state` is missing → **400 Bad Request**  
- If `state` is malformed → **400 Bad Request**  
- If `return_url` is missing or malformed → **400 Bad Request**  
- If `code` is missing → redirect to `return_url` **without** issuing a session cookie  

The endpoint performs **shape validation only**.

No protocol logic.  
No identity logic.  
No Infrastructure access.  
No redirect computation.

---

# 2. Invoke the Frank Auth Callback Pipeline (Protocol Layer)

The endpoint calls:

````csharp
frankEngine.BuildAsync(frankAuthCallbackRequest, CancellationToken.None)
````

Frank performs all **OIDC protocol work**:

- Validates OIDC configuration  
- Exchanges the authorization code for tokens  
- Validates issuer, audience, signature, nonce, state  
- Normalizes provider‑specific claims  
- Produces a stable, provider‑agnostic identity payload  

Frank pipeline **does not**:

- Resolve identity  
- Create sessions  
- Compute redirect URLs  
- Interpret or validate `return_url`  
- Compute cookie values  
- Perform any business logic  

Frank pipeline errors are shaped by Frank’s error boundary.

---

# 3. Invoke the Application Auth Callback Pipeline (Business Layer)

The endpoint then calls:

````csharp
appEngine.BuildAsync(appAuthCallbackRequest, CancellationToken.None)
````

Application performs all **business logic**:

- Validates required identity claims (e.g., `sub`)  
- Resolves or creates the internal Owner record  
- Logs the login audit event  
- Creates the session  
- Computes the session token hash  
- Computes the cookie value (opaque token)  

**Important:**  
The Application pipeline does **not** compute the redirect URL.  
Redirect computation is done entirely by the API endpoint using the `return_url` extracted from `state`.

Application pipeline **does not**:

- Perform OIDC protocol logic  
- Perform token exchange  
- Perform userinfo calls  
- Issue cookies  
- Perform HTTP operations  
- Validate or sanitize `return_url`  

The result object includes:

- `CustomerId`  
- `SessionId`  
- `TokenHash`  
- `CookieValue`  

---

# 4. Issue the Session Cookie (API Boundary)

The API endpoint:

- Uses the `CookieValue` from the Application pipeline  
- Issues the secure session cookie  
- Applies Frank security headers  
- Applies Frank CORS  
- Applies Frank error boundary  

Cookie properties (from real code):

````csharp
http.Response.Cookies.Append(
    "session",
    appAuthCallbackResult.CookieValue,
    new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict
    });
````

Cookie characteristics:

- **HttpOnly**  
- **Secure** (preview/prod)  
- **SameSite=Strict**  
- Contains an **opaque, random session token**  
- Backed by a **hashed token** stored in the database  

Local development uses `Secure=false`.

---

# 5. Redirect the User

The endpoint returns:

````http
302 Found
Location: <return_url from decoded state>
Set-Cookie: session=...
````

The redirect URL is computed **only** by the API endpoint using the decoded `return_url`.

The endpoint does **not**:

- Construct redirect URLs  
- Validate `return_url` beyond shape  
- Perform business logic  
- Perform identity logic  

Redirect computation is intentionally minimal and governed.

---

# Error Handling

All errors flow through Frank’s global exception → ProblemDetails mapping.

| Condition | Error Code | HTTP Status |
|----------|------------|-------------|
| Missing `state` | `ValidationError` | 400 |
| Malformed `state` | `ValidationError` | 400 |
| Missing `return_url` | `ValidationError` | 400 |
| Missing `code` | Redirect (no cookie) | 302 |
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

- Extracts and decodes `state`  
- Extracts the authorization code  
- Invokes Frank (protocol)  
- Invokes Application (business)  
- Issues the `session` cookie  
- Redirects the user to `return_url`  

It contains **no business logic**, **no protocol logic**, and **no Infrastructure logic**.

---

# See Also

- **[Login Endpoint](ca://s?q=Show_login_endpoint_doc)**  
- **[Authentication Overview](ca://s?q=Show_authentication_overview)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
- **[Identity Mapping Guide](ca://s?q=Show_identity_mapping_guide)**  
- **[Session Management Guide](ca://s?q=Show_session_management_guide)**
