# Authentication Configuration

The following configuration keys are required for **OIDC authentication**.  
These values must be provided as **environment variables** in all environments:

- Local development  
- Preview (Render PR Previews)  
- Production  

Missing or malformed configuration results in a **startup failure** or a  
**500 Bad Configuration** error during the callback pipeline.

---

# Required Keys

```yaml
Oidc:
  Domain: "<tenant>.auth0.com"
  ClientId: "<client-id>"
  ClientSecret: "<client-secret>"
  Audience: "<optional>"
  CallbackUrl: "https://<api-host>/api/auth/callback"
  PostLoginRedirectUrl: "https://<frontend-host>/"
```

### Field meanings

| Key | Purpose |
|-----|---------|
| `Domain` | Auth0 tenant domain (issuer + userinfo base) |
| `ClientId` | Public identifier for the Auth0 application |
| `ClientSecret` | Secret used during code exchange |
| `Audience` | Optional API audience for Auth0 access tokens |
| `CallbackUrl` | Redirect target for Auth0 after login |
| `PostLoginRedirectUrl` | Final redirect after session creation |

All values must be **non‑empty** except `Audience`.

---

# Usage

## `/api/auth/login` uses:

- Domain  
- ClientId  
- Audience  
- CallbackUrl  

Used to construct the authorization URL and initiate the OIDC flow.

## `/api/auth/callback` uses:

- Domain  
- ClientId  
- ClientSecret  
- CallbackUrl  
- PostLoginRedirectUrl  

Used by the authentication pipeline to:

- Exchange the authorization code  
- Fetch userinfo  
- Resolve identity  
- Issue session cookie  
- Redirect the browser  

---

# Validation Behavior

Configuration is validated in the **first step** of the callback pipeline  
via the **ValidateConfigurationStep**.

### Validation ensures:

- All required keys are present  
- No value is empty  
- Callback URL matches the Auth0 application configuration  
- Redirect URL is valid  
- Options bind correctly into strongly typed `OidcOptions`  

### Failure Mode

- Missing or invalid configuration → `BadConfigurationException`  
- Mapped to **500 Internal Server Error**  
- No cookies are issued  
- No session is created  

This aligns with **[Security Governance](ca://s?q=Show_security_governance)**  
and **[Operations Governance](ca://s?q=Show_operations_governance)**.

---

# Environment‑Specific Notes

## Local Development

Example:

```
Oidc__Domain=dev-tenant.us.auth0.com
Oidc__ClientId=abc123
Oidc__ClientSecret=xyz789
Oidc__CallbackUrl=http://localhost:5000/api/auth/callback
Oidc__PostLoginRedirectUrl=http://localhost:3000/
```

- `Secure=false` cookies  
- `SameSite=Lax`  
- Callback URL must match the Auth0 dev application  

## Preview / Production

- `Secure=true`  
- `HttpOnly=true`  
- `SameSite=Lax`  
- Callback URL must match the deployed API host  
- Redirect URL must match the deployed frontend host  

In **Render PR Previews**, these values are injected via environment variables  
and validated before any session logic runs.

---

# Notes

- No identity provider tokens are persisted  
- Callback URL must match the Auth0 application configuration exactly  
- Missing configuration triggers `BadConfigurationException` → 500  
- All configuration is consumed through strongly typed `OidcOptions`  
- Configuration is immutable at runtime  
- HostingEngine does **not** modify OIDC configuration; it only sets environment context  

---

# See Also

- **[Authentication Overview](ca://s?q=Show_authentication_overview)**  
- **[Login Endpoint](ca://s?q=Show_login_endpoint_doc)**  
- **[Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Generate_Authentication_Architecture_Guide)**  
- **[Session Management Guide](ca://s?q=Generate_Session_Management_Guide)**  
