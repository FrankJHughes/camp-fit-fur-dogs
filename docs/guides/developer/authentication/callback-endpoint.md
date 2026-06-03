# Callback Endpoint — `/api/auth/callback`

The callback endpoint completes the **OIDC authorization code flow**.  
It validates configuration, exchanges the authorization code for tokens, fetches and validates the Auth0 user profile, resolves identity, logs the login event, issues a session cookie, creates the session, and redirects the user to the frontend.

---

# HTTP Request

```http
GET /api/auth/callback?code=XYZ
```

---

# Behavior

The callback endpoint executes a strict, ordered **authentication pipeline**:

1. **Validate configuration**  
   - Ensures all required OIDC settings are present  
   - Missing values → 500 Internal Server Error  

2. **Validate that `code` is present**  
   - Missing or empty → 400 Bad Request  

3. **Exchange authorization code**  
   - Calls Auth0 `/oauth/token`  
   - Retrieves an access token  

4. **Fetch userinfo**  
   - Calls Auth0 `/userinfo`  
   - Retrieves:
     - `sub` (external ID)
     - `email`
     - `given_name`
     - `family_name`

5. **Validate userinfo**  
   - Ensures `sub` is present  
   - Missing `sub` → 502 Bad Gateway  

6. **Resolve identity**  
   - Maps external identity to internal `CustomerId`  
   - Creates Owner if needed  

7. **Audit login**  
   - Logs successful login with `CustomerId` + external ID  

8. **Issue session cookie**  
   - Generates a 256‑bit session token  
   - Hashes it  
   - Creates a `SessionCookie` value object  

9. **Create session**  
   - Persists the session with the hashed token  
   - Associates it with the Owner  

10. **Build redirect**  
    - Produces the final redirect URL for the frontend  

11. **Return redirect response**  
    - Issues the session cookie  
    - Returns `302 Found` to the frontend  

---

# Session Cookie

The session cookie is:

- **HttpOnly**  
- **Secure** (production)  
- **SameSite=Lax**  
- **Max‑Age** configured  
- Contains an **opaque, random session token**  
- Backed by a **hashed** token stored in the database  

The cookie is written after session creation and before redirect.

---

# Error Handling

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

All errors are surfaced through the global exception → ProblemDetails mapping.

---

# See Also

- **[Login Endpoint](ca://s?q=Show_login_endpoint_doc)**  
- **[Authentication Overview](ca://s?q=Show_authentication_overview)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
