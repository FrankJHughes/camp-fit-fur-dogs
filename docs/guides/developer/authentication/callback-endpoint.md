# Callback Endpoint — `/api/auth/callback`

The callback endpoint completes the **OIDC authorization code flow**.  
It exchanges the authorization code for tokens, loads or creates the Owner, and issues the session cookie.

## HTTP Request

```http
GET /api/auth/callback?code=XYZ
```

## Behavior

1. Validates that `code` is present
2. Exchanges the code for tokens using Auth0
3. Calls `/userinfo` to retrieve:
   - `sub`
   - `email`
   - `given_name`
   - `family_name`
4. Loads or creates the Owner record
5. Issues a secure session cookie
6. Redirects to the frontend dashboard

## Session Cookie

- HttpOnly  
- Secure  
- SameSite=Lax  
- Max‑Age configured  
- Contains opaque session token  

## Error Handling

| Condition | Error Code | HTTP Status |
|----------|------------|-------------|
| Missing `code` | `ValidationError` | 400 |
| Missing `sub` | `ExternalAuthProviderFailure` | 502 |
| Token exchange failure | `ExternalAuthProviderFailure` | 502 |
| Missing configuration | `BadConfiguration` | 500 |
| Unexpected failure | `Unexpected` | 500 |

See also:  
- [Login Endpoint](ca://s?q=Show_login_endpoint_doc)  
- [Authentication Overview](ca://s?q=Show_authentication_overview)  
- [Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)

