# Login Endpoint — `/api/identity/login`  
**Aligned With Exclusive OIDC Authentication & Auth Callback Refactor**

The login endpoint initiates the **OIDC authorization code flow** by constructing the authorization URL and returning it to the frontend.  
The frontend performs the actual redirect to Auth0.

This endpoint is **pure** — it performs no domain logic, no identity logic, and no persistence.  
It only validates configuration, constructs the authorization URL, and returns it.

The endpoint relies on:

- Frank security headers  
- Frank CORS  
- Frank error boundary  

It does **not** access Infrastructure, Domain, or Application handlers directly.

---

# HTTP Request

````http
GET /api/identity/login?return_url=/dashboard
````

The request accepts an optional `return_url` parameter.

The login endpoint:

- **validates** `return_url` shape (must be a valid URI)  
- **does not** interpret or sanitize `return_url`  
- **does not** compute redirect URLs  
- **does not** perform identity or session logic  

The `return_url` is forwarded to Auth0 via the OIDC `state` parameter.

---

# Behavior (Post‑Refactor)

The login endpoint performs exactly **three** responsibilities:

1. **Validate OIDC + frontend configuration**  
2. **Construct the OIDC authorization URL**  
3. **Return `200 OK` with `LoginResponse(nextUrl)`**  

The frontend performs the redirect.

### Configuration Used

- `Authentication:Callback:Oidc:Authority`  
- `Authentication:Callback:Oidc:ClientId`  
- `Authentication:Callback:Oidc:CallbackUrl`  
- `Frontend:BaseUrl`  

### The endpoint:

- Validates `Authority`, `ClientId`, and `Frontend:BaseUrl`  
- Constructs callback URL  
  - Uses configured value if present  
  - Otherwise builds from request scheme + host  
- Validates optional `return_url` shape  
- Encodes `return_url` into OIDC `state` using `OidcStateEncoder`  
- Builds the authorization URL:

````csharp
var nextUrl =
    $"{authority.TrimEnd('/')}/authorize" +
    $"?response_type=code" +
    $"&client_id={Uri.EscapeDataString(clientId)}" +
    $"&redirect_uri={Uri.EscapeDataString(callback)}" +
    $"&scope=openid profile email" +
    $"&state={Uri.EscapeDataString(encodedState)}";
````

- Returns:

````csharp
return Results.Ok(new LoginResponse(nextUrl));
````

### Important Corrections

- **No 302 redirect** is issued by the backend  
- **PKCE is not implemented**  
- **Disabled mode does not exist**  
- **return_url is validated for shape** (not deferred to Application)  
- **Frontend performs the redirect**  

This endpoint is intentionally thin and deterministic, following **API Endpoint Purity**.

---

# Error Handling

All errors flow through Frank’s global exception → ProblemDetails mapping.

| Condition | Error Code | HTTP Status |
|----------|------------|-------------|
| Missing Authority / ClientId | `BadConfiguration` | 500 |
| Missing Frontend BaseUrl | `BadConfiguration` | 500 |
| Malformed `return_url` | `BadRequest` | 400 |
| Unexpected failure | `Unexpected` | 500 |

Additional guarantees:

- No cookies are issued  
- No partial state is created  
- No Infrastructure types leak into API  

---

# Tests Required

A complete test suite must verify:

### Authorization URL Construction
- `client_id` is present  
- `redirect_uri` is correct  
- `response_type=code`  
- `scope=openid profile email`  
- `state` includes encoded `return_url` when provided  
- Callback URL is dynamically constructed when missing  

### Error Conditions
- Missing configuration → 500  
- Malformed `return_url` → 400  
- Valid configuration → 200 OK with `nextUrl`  

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
