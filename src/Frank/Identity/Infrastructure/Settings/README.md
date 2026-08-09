# Identity Infrastructure — Settings

The **Settings** folder contains configuration objects used by the Identity
infrastructure layer. These settings are bound from application configuration
(e.g., `appsettings.json`, environment variables, or secret stores) and validated
on startup to ensure safe, predictable behavior across all OIDC flows.

This folder currently contains the configuration model used for Auth0 and other
OIDC providers.

---

## Purpose

The Settings subsystem provides:

- Strongly typed configuration models  
- Early validation of required OIDC values  
- A single source of truth for Identity OIDC configuration  
- Integration with `.AddOptions()` and `.BindConfiguration()`  

These settings are consumed by:

- Token exchange client  
- Token validator  
- UserInfo client  
- Audit logging (indirectly, via OIDC flows)  
- DI registration in the Auth0 infrastructure layer  

---

## Files

### **OidcSettings**

Represents the configuration required to integrate with an OpenID Connect
provider such as Auth0.

Includes:

- **Authority** — The base issuer URL  
- **ClientId** — The registered OIDC client identifier  
- **ClientSecret** — The confidential client secret  
- **CallbackUrl** — The redirect URI used during the authorization‑code flow  

All properties are marked `required` to ensure the application fails fast if
configuration is missing or incomplete.

Example configuration:

```json
{
  "Identity": {
    "Oidc": {
      "Authority": "https://your-tenant.auth0.com",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "CallbackUrl": "https://your-app/callback"
    }
  }
}
```

---

## Design Principles

The Settings subsystem follows these principles:

- **Fail‑fast configuration**  
  Required properties ensure misconfiguration is detected immediately.

- **Separation of concerns**  
  Settings are isolated from infrastructure logic and consumed via DI.

- **Environment flexibility**  
  Supports multiple environments (local, staging, production) via standard
  configuration providers.

- **Reloadable configuration**  
  Works with `IOptionsMonitor` to support dynamic updates when needed.

---

## Summary

The **Settings** folder provides the foundational configuration model for OIDC
integration within the Identity subsystem:

- Strongly typed settings  
- Early validation  
- Clean DI integration  
- Support for all Auth0 OIDC clients  

It ensures that authentication flows operate with correct, validated, and secure
configuration values.

