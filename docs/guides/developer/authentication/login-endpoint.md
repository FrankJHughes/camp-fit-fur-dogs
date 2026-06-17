# Login Endpoint — `/api/auth/login`  
**Aligned With Exclusive OIDC Authentication & Auth Callback Refactor**

The login endpoint initiates the **OIDC authorization code flow** by redirecting the client to the external identity provider (Auth0).  
This endpoint is **pure** — it performs no domain logic, no identity logic, and no persistence.  
It only constructs the authorization URL and issues a redirect.

The endpoint relies on:

- Frank endpoint discovery  
- Frank security headers  
- Frank CORS  
- Frank error boundary  

It does **not** access Infrastructure, Domain, or Application handlers directly.

---

# HTTP Request

```http
GET /api/auth/login?returnUrl=/dashboard
```

The request accepts an optional `returnUrl` parameter.  
Identity, authorization, and session logic are handled exclusively in the **callback architecture** (Frank pipeline → Application pipeline → API boundary).

The login endpoint **does not validate or interpret `returnUrl`** — it simply forwards it to Auth0 via the OIDC `state` parameter.  
Validation occurs **only** in the Application pipeline.

---

# Behavior (Post‑Refactor)

The login endpoint performs exactly **one** responsibility:

### Construct the OIDC authorization URL and redirect the browser.

It uses the following configuration values:

- `Authentication:Callback:Oidc:Authority`  
- `Authentication:Callback:Oidc:ClientId`  
- `Authentication:Callback:Oidc:CallbackUrl`  
- `Authentication:Callback:Oidc:Disabled` (short‑circuit mode)  

### The endpoint:

- Builds the authorization URL  
- Includes:
  - `client_id`
  - `redirect_uri`
  - `response_type=code`
  - `scope=openid profile email`
  - PKCE parameters (if enabled)
  - Encoded `state` containing the optional `returnUrl`
- Returns **302 Redirect** to the identity provider  
- Performs **no** domain logic  
- Performs **no** persistence  
- Performs **no** identity resolution  
- Performs **no** session logic  
- Performs **no** protocol logic  
- Uses the global error pipeline for all failures  

This endpoint is intentionally thin and deterministic, following **API Endpoint Purity**.

---

# Disabled Mode

If:

```
Authentication:Callback:Oidc:Disabled = true
```

Then:

- The login endpoint must **not** construct an OIDC URL  
- The endpoint returns a shaped **501 Not Implemented**  
- No redirect occurs  
- No OIDC flow is initiated  

This is used for:

- Local offline development  
- CI environments without secrets  
- Automated tests  

---

# Error Handling

All errors flow through Frank’s global exception → ProblemDetails mapping.

| Condition | Error Code | HTTP Status |
|----------|------------|-------------|
| Missing configuration | `BadConfiguration` | 500 |
| OIDC disabled | `NotImplemented` | 501 |
| Unexpected failure | `Unexpected` | 500 |

Additional guarantees:

- No cookies are issued  
- No partial state is created  
- No Infrastructure types leak into API  

---

# Tests Required

A complete test suite must verify:

### Redirect URL Construction
- `client_id` is present  
- `redirect_uri` is correct  
- `response_type=code`  
- `scope` is included  
- PKCE parameters are included when enabled  
- `state` includes the encoded `returnUrl` when provided  

### Disabled Mode
- `Oidc:Disabled=true` → 501 Not Implemented  
- No redirect is generated  

### Error Conditions
- Missing configuration → 500  
- Valid configuration → 302 redirect  

### Purity
- No domain calls  
- No repository calls  
- No handler calls  
- No session logic  
- No identity logic  
- No protocol logic  

---

# See Also

- **[Authentication Overview](ca://s?q=Show_authentication_overview)**  
- **[Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**
