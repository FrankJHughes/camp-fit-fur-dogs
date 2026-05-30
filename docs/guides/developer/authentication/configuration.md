# Authentication Configuration

The following configuration keys are required for OIDC authentication.  
These values must be provided in environment variables for all environments (local, preview, production).

## Required Keys

```yaml
Oidc:
  Domain: "<tenant>.auth0.com"
  ClientId: "<client-id>"
  ClientSecret: "<client-secret>"
  Audience: "<optional>"
  CallbackUrl: "https://<host>/api/auth/callback"
```

## Usage

- `/api/auth/login` uses:
  - Domain  
  - ClientId  
  - Audience  
  - CallbackUrl  

- `/api/auth/callback` uses:
  - Domain  
  - ClientId  
  - ClientSecret  
  - CallbackUrl  

## Notes

- No identity provider tokens are persisted  
- Callback URL must match the Auth0 application configuration  
- Missing configuration triggers `BadConfigurationException` → 500  

See also:  
- [Authentication Overview](ca://s?q=Show_authentication_overview)  
- [Login Endpoint](ca://s?q=Show_login_endpoint_doc)  
- [Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)
