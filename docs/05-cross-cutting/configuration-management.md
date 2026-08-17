# Frank.CrossCutting — Configuration Management

Configuration management provides strongly typed, environment‑aware settings across the entire platform. It ensures safe, predictable, compile‑time validated access to configuration values and prevents accidental misuse of secrets or environment‑specific behavior.

This document describes the configuration subsystem under:

```
/docs/05-cross-cutting
```

and maps it back to its implementation under:

```
/src/Frank/Core.Infrastructure
/src/Frank/Core.Api
/src/Frank/Identity/Infrastructure
```

Configuration is a **cross‑cutting concern**: every vertical slice depends on it.

---

## Configuration Files

Settings are layered to support environment‑specific behavior:

- **`appsettings.json`** — base configuration  
- **`appsettings.Development.json`** — development overrides  
- **`appsettings.Testing.json`** — test environment overrides  
- **`appsettings.Production.json`** — production overrides  
- **Environment variables** — runtime overrides  

This layering follows ASP.NET Core’s standard configuration precedence.

See also:  
- [Environment Detection](ca://s?q=Explain_identity_environment_detection)  
- [Configuration Binding](ca://s?q=Frank_Core_Infrastructure_Configuration_Binding)

---

## Environment Variables

Environment variables override file‑based configuration using **double underscores** (`__`) to represent nested JSON paths.

```bash
ConnectionStrings__DefaultConnection=Server=prod.db.example.com;...
Identity__Oidc__Authority=https://auth.example.com
ASPNETCORE_ENVIRONMENT=Production
```

Environment variables are the **highest‑precedence** configuration source and are required for:

- secrets  
- deployment‑specific values  
- containerized environments  
- CI/CD pipelines  

See: [Secret Management](ca://s?q=Explain_crosscutting_secret_management)

---

## Strongly Typed Settings

Configuration is bound to C# objects during startup:

- provides compile‑time type safety  
- enables IntelliSense  
- ensures required fields are validated  
- prevents runtime errors due to missing or malformed settings  

Example:

```csharp
services.Configure<IdentityOptions>(configuration.GetSection("Identity"));
```

Strong typing ensures configuration errors fail fast during startup rather than at runtime.

---

## Environment‑Aware Behavior

Hosting modules adapt configuration based on the deployment environment:

- **Development**  
  - verbose logging  
  - relaxed security policies  
  - local secrets via `appsettings.Development.json`  

- **Testing**  
  - deterministic configuration  
  - fake providers  
  - in‑memory databases  

- **Production**  
  - strict validation  
  - hardened security headers  
  - real OIDC provider integration  
  - secrets injected via environment variables  

Environment detection is handled by ASP.NET Core and consumed by cross‑cutting modules.

See: [Environment Detection](ca://s?q=Explain_identity_environment_detection)

---

## Secrets Management

Sensitive values must:

- **never be committed** to version control  
- be stored in secure vaults or managed services  
- be injected via environment variables at runtime  
- be marked as **`[Required]`** in configuration objects  
- be validated during startup  

Examples of secrets:

- OIDC client secrets  
- database passwords  
- API keys  
- encryption keys  

Secrets belong in:

- environment variables  
- secret managers (Azure Key Vault, AWS Secrets Manager, etc.)  
- deployment‑specific configuration stores  

They **never** belong in:

- `appsettings.json`  
- source code  
- Docker images  
- CI/CD logs  

See: [Secret Management](ca://s?q=Explain_crosscutting_secret_management)

---

## Runtime Collaboration Points

Configuration interacts with:

- **Identity Infrastructure** — OIDC settings, session TTL  
- **Core Infrastructure** — logging, environment detection  
- **EF Core** — connection strings, provider settings  
- **API Layer** — CORS, security headers, rate limiting  
- **Testing** — mutated contexts, test configuration overrides  

Configuration is the backbone of runtime behavior across the entire platform.

---

## Notes

Keep this document grounded in the actual configuration implementation.  
Whenever new settings, environment behaviors, or secret‑handling rules are added, update this section to reflect the current architecture.
