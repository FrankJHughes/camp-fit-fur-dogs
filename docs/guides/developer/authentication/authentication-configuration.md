# Authentication Configuration (Aligned With Auth Callback Refactor)

The following configuration keys are required for **OIDC authentication**.  
These values must be provided as **environment variables / appsettings** in all environments:

- Local development  
- Preview (Render PR Previews)  
- Production  

Missing or malformed configuration results in a **startup failure** or a  
**500 Bad Configuration** error during the **Frank Auth Callback Pipeline**.

---

# Configuration Shape (Canonical)

```json
{
  "Authentication": {
    "Callback": {
      "PostLoginRedirectUrl": "https://yourapp.com/dashboard",
      "Oidc": {
        "Authority": "https://YOUR_DOMAIN",
        "ClientId": "YOUR_CLIENT_ID",
        "ClientSecret": "YOUR_CLIENT_SECRET",
        "CallbackUrl": "https://yourapp.com/auth/callback",
        "Disabled": false
      }
    }
  }
}
```

Environment variable equivalents:

- `Authentication__Callback__PostLoginRedirectUrl`  
- `Authentication__Callback__Oidc__Authority`  
- `Authentication__Callback__Oidc__ClientId`  
- `Authentication__Callback__Oidc__ClientSecret`  
- `Authentication__Callback__Oidc__CallbackUrl`  
- `Authentication__Callback__Oidc__Disabled`  

---

# Field Meanings

| Key                                               | Purpose |
|---------------------------------------------------|---------|
| `Authentication:Callback:Oidc:Authority`          | OIDC authority (issuer, discovery, userinfo) |
| `Authentication:Callback:Oidc:ClientId`           | Public identifier for the OIDC client |
| `Authentication:Callback:Oidc:ClientSecret`       | Secret used during authorization‑code exchange |
| `Authentication:Callback:Oidc:CallbackUrl`        | Redirect target for the OIDC callback |
| `Authentication:Callback:Oidc:Disabled`           | **Disables OIDC entirely** (used for tests, local offline mode) |
| `Authentication:Callback:PostLoginRedirectUrl`    | Final redirect after session creation |

All values must be **non‑empty** except `Disabled`.

---

# How Configuration Is Used in the New Architecture

The authentication callback architecture consists of:

1. **Frank Auth Callback Pipeline (protocol)**  
2. **Application Auth Callback Pipeline (business)**  
3. **API Callback Endpoint (boundary)**  

Configuration is consumed **only** by the Frank pipeline.

---

## `/api/auth/login` uses:

- `Authority`  
- `ClientId`  
- `CallbackUrl`  

Used to construct the authorization URL and initiate the OIDC flow.

The login endpoint contains:

- No protocol logic  
- No business logic  
- No identity logic  
- No session logic  

---

## `/api/auth/callback` uses:

The API callback endpoint:

- Extracts the `code` query parameter  
- Passes a request into the **Frank Auth Callback Pipeline**

The **Frank pipeline** consumes:

- `Authority`  
- `ClientId`  
- `ClientSecret`  
- `CallbackUrl`  
- `Disabled`  

The **Application pipeline** consumes:

- The normalized protocol result from Frank  
- `PostLoginRedirectUrl` (to compute the final redirect)

The **API endpoint**:

- Uses the Application pipeline result (`CookieValue`, `RedirectUrl`)  
- Issues the cookie  
- Returns the redirect response  

---

# The `Oidc:Disabled` Flag

`Authentication:Callback:Oidc:Disabled` is a **first‑class switch** that:

- Completely disables OIDC authentication  
- Causes the Frank pipeline to short‑circuit  
- Prevents any token exchange  
- Prevents any userinfo calls  
- Prevents any identity resolution  
- Prevents any session creation  
- Causes the callback endpoint to return a shaped **501 Not Implemented** or equivalent (depending on implementation)

This flag is used for:

- Local development without Auth0  
- Automated tests  
- CI environments without secrets  
- Offline mode  

When `Disabled=true`, the system must not attempt any OIDC operations.

---

# Configuration Validation (Aligned With Refactor)

Validation happens inside the **Frank Auth Callback Pipeline**:

- All required keys are present  
- No value is empty  
- `CallbackUrl` matches the OIDC client configuration  
- `PostLoginRedirectUrl` is a valid absolute URL  
- Options bind correctly into strongly typed options (e.g. `AuthenticationCallbackOptions`)  
- If `Disabled=true`, validation short‑circuits and no OIDC calls are made  

### Failure Mode

- Missing or invalid configuration → `BadConfigurationException`  
- Mapped to **500 Internal Server Error** by Frank’s error boundary  
- No cookies are issued  
- No session is created  

---

# Environment‑Specific Notes

## Local Development

Example:

```bash
Authentication__Callback__Oidc__Authority=https://dev-tenant.us.auth0.com
Authentication__Callback__Oidc__ClientId=abc123
Authentication__Callback__Oidc__ClientSecret=xyz789
Authentication__Callback__Oidc__CallbackUrl=http://localhost:5000/auth/callback
Authentication__Callback__PostLoginRedirectUrl=http://localhost:3000/
Authentication__Callback__Oidc__Disabled=false
```

- `Secure=false` cookies  
- `SameSite=Lax`  
- Callback URL must match the OIDC dev application  

## Preview / Production

- `Secure=true`  
- `HttpOnly=true`  
- `SameSite=Lax`  
- Callback URL must match the deployed API host  
- Redirect URL must match the deployed frontend host  

In **Render PR Previews**, these values are injected via environment variables  
and validated by the **Frank pipeline** before any business logic runs.

---

# Notes

- No identity provider tokens are persisted  
- `CallbackUrl` must match the OIDC client configuration exactly  
- Missing configuration triggers `BadConfigurationException` → 500  
- All configuration is consumed through strongly typed options under `Authentication:Callback`  
- Configuration is immutable at runtime  
- HostingEngine does **not** modify authentication configuration; it only sets environment context  
- Application pipeline does **not** read OIDC configuration directly  
- `Disabled=true` fully bypasses OIDC  

---

# See Also

- **Authentication Architecture Guide**  
- **Callback Endpoint Guide**  
- **Identity Mapping Guide**  
- **Session Management Guide**
