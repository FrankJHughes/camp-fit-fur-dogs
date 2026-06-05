# Login Endpoint — `/api/auth/login`

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
GET /api/auth/login
```

The request contains no parameters.  
Identity, authorization, and session logic are handled exclusively in the callback pipeline.

---

# Behavior

The login endpoint:

- Constructs an Auth0 authorization URL using validated configuration  
- Includes:
  - `client_id`
  - `redirect_uri`
  - `response_type=code`
  - `scope`
  - `audience` (if configured)
  - PKCE parameters (if enabled)  
- Returns **302 Redirect** to the identity provider  
- Performs **no** domain logic  
- Performs **no** persistence  
- Performs **no** identity resolution  
- Performs **no** session logic  
- Uses the global error pipeline for all failures  

This endpoint is intentionally thin and deterministic, following **API Endpoint Purity**.

---

# Error Handling

All errors flow through Frank’s global exception → ProblemDetails mapping.

| Condition | Error Code | HTTP Status |
|----------|------------|-------------|
| Missing configuration | `BadConfiguration` | 500 |
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
- `audience` is included when configured  
- PKCE parameters are included when enabled  

### Error Conditions
- Missing configuration → 500  
- Valid configuration → 302 redirect  

### Purity
- No domain calls  
- No repository calls  
- No handler calls  
- No session logic  
- No identity logic  

---

# See Also

- **[Authentication Overview](ca://s?q=Show_authentication_overview)**  
- **[Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
