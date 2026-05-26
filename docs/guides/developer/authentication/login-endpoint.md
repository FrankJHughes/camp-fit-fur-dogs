# Login Endpoint — `/api/auth/login`

The login endpoint initiates the **OIDC authorization code flow** by redirecting the client to the external identity provider (Auth0).  
This endpoint is **pure** — it performs no domain logic and persists no data.

## HTTP Request

```http
GET /api/auth/login
```

## Behavior

- Constructs an Auth0 authorization URL using configured values
- Returns **302 Redirect** to the identity provider
- Does **not** create or modify any domain entities
- Does **not** persist tokens
- Uses the global error pipeline for all failures

## Error Handling

| Condition | Error Code | HTTP Status |
|----------|------------|-------------|
| Missing configuration | `BadConfiguration` | 500 |
| Unexpected failure | `Unexpected` | 500 |

## Tests Required

- Redirect URL contains:
  - `client_id`
  - `redirect_uri`
  - `response_type=code`
  - `scope`
  - `audience` (if configured)
- Missing config → 500
- Valid config → 302 redirect

See also:  
- [Authentication Overview](ca://s?q=Show_authentication_overview)  
- [Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)  
- [Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)
