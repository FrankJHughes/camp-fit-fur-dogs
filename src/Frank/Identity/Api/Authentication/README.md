# Identity API — Authentication

The **Authentication** folder contains the configuration and service‑registration
logic required to authenticate requests within the Identity API surface.  
This subsystem establishes the default **session‑based authentication scheme** and
optionally configures **OpenID Connect (OIDC)** when enabled via application
settings.

Authentication in the Identity subsystem follows the purity rules defined in
stories such as US‑110, US‑111, and US‑133:

- No identity provider tokens are exposed through the API  
- No domain logic is embedded in authentication configuration  
- Session authentication is the default mechanism  
- OIDC is optional and must be explicitly enabled  
- Forwarded headers are sanitized to prevent spoofing  

The Authentication subsystem ensures that all identity endpoints operate within a
secure, predictable, and provider‑agnostic authentication environment.

---

## Files

```
Authentication/
└── AuthenticationServiceCollectionExtensions.cs
```

---

## AuthenticationServiceCollectionExtensions

This class configures all authentication services used by the Identity API.

### Responsibilities

- **Forwarded Headers**  
  Configures `X-Forwarded-For` and `X-Forwarded-Proto` while clearing trusted
  networks and proxies to prevent spoofing.

- **Session Authentication**  
  Registers the `"Session"` authentication scheme using
  `SessionAuthenticationHandler`.  
  This is the default authentication mechanism for the Identity API.

- **Optional OIDC Integration**  
  When `Identity:Oidc:Disabled` is `false`, the subsystem validates required OIDC
  settings:
  - `Identity:Oidc:Authority`  
  - `Identity:Oidc:ClientId`  
  - `Identity:Oidc:ClientSecret`  
  - `Identity:Oidc:CallbackUrl` (derived automatically if missing)

  OIDC integration will be added later via `AddOpenIdConnect()`.

### Contract

```csharp
public static IServiceCollection AddFrankIdentityApiAuthentication(
    this IServiceCollection services,
    IConfiguration config)
```

### Notes

- Session authentication is always enabled.  
- OIDC is opt‑in and must be explicitly configured.  
- Callback URL derivation ensures predictable behavior across environments.  
- No tokens, secrets, or provider metadata are ever exposed through the API.

---

## Design Principles

- **Purity**  
  Authentication configuration contains no domain logic and exposes no sensitive
  identity provider details.

- **Safety**  
  Forwarded headers are sanitized; OIDC settings are validated.

- **Minimalism**  
  Only the required authentication mechanisms are registered.

- **Environment Awareness**  
  Callback URLs adapt automatically to hosting configuration.

- **Separation of Concerns**  
  Authentication configuration is isolated from endpoint logic and middleware.

---

## How Authentication Fits Into the Identity Architecture

Authentication sits between:

```
[ API Surface ]
      ↓
[ Authentication ]
      ↓
[ Session Middleware ]
      ↓
[ Identity Application Logic ]
```

It ensures:

- Every identity endpoint runs inside a secure authentication boundary  
- Session state is consistently validated  
- OIDC flows (when enabled) are predictable and safe  
- Reverse proxy environments behave correctly  

---
