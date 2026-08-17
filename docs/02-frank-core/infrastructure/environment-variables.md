# Frank.Core.Infrastructure — Environment Variables

The environment‑variable subsystem in `Frank.Core.Infrastructure` provides a consistent, centralized way to read configuration values from the host environment. It ensures that application and domain layers never directly access `Environment.GetEnvironmentVariable`, keeping configuration concerns isolated within infrastructure.

This document maps the environment‑variable subsystem under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure/Configuration
```

---

## Purpose

Environment variables exist to:

- supply configuration values to the application at startup  
- allow different settings per environment (dev, staging, prod)  
- keep secrets and operational parameters out of source code  
- provide a stable abstraction for retrieving configuration  
- ensure domain and application layers remain environment‑agnostic  

They are the foundation of runtime configuration across the Frank platform.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Infrastructure/Configuration`

- **Documentation folder:**  
  `docs/02-frank-core/infrastructure`

This documentation must remain aligned with the actual configuration helpers and environment‑variable access patterns.

---

## Responsibilities of the Environment‑Variable Subsystem

### [Centralized Access](ca://s?q=Frank_Core_Infrastructure_Environment_Access)
Environment variables are accessed through infrastructure services rather than directly:

- prevents scattering `Environment.GetEnvironmentVariable` calls  
- ensures consistent naming conventions  
- allows validation and fallback logic  
- supports typed configuration objects  

### [Configuration Binding](ca://s?q=Frank_Core_Infrastructure_Configuration_Binding)
Environment variables flow into:

- strongly typed configuration records  
- options classes (`IOptions<T>`)  
- infrastructure services  
- application‑level settings  

This keeps configuration structured and predictable.

### [Environment Awareness](ca://s?q=Frank_Core_Infrastructure_Environment_Detection)
Infrastructure detects the current environment:

- Development  
- Staging  
- Production  

This enables environment‑specific behavior such as:

- verbose logging in development  
- strict validation in production  
- different connection strings per environment  

### [Secret Management](ca://s?q=Frank_Core_Infrastructure_Secret_Management)
Environment variables often store:

- API keys  
- connection strings  
- private endpoints  
- authentication secrets  

Infrastructure ensures these values never leak into domain logic.

---

## Runtime Collaboration Points

Environment variables interact with the runtime by:

- providing configuration values at startup  
- enabling feature toggles  
- controlling logging verbosity  
- configuring persistence and external integrations  
- supporting environment‑specific behavior  

They are read once at startup and then injected throughout the platform.

---

## Composition Flow (API → Application → Domain → Persistence)

```
Environment Variables
    ↓
Infrastructure Configuration Binding
    ↓
Application Services (IOptions<T>)
    ↓
Domain Logic (receives typed settings)
    ↓
Persistence / External Integrations
```

Environment variables shape the runtime without polluting domain or application code.

---

## What Belongs in This Document

This page should describe:

- how environment variables are accessed  
- how configuration is bound and validated  
- how environment detection works  
- how secrets and operational parameters flow through the platform  
- how environment variables fit into the vertical slice lifecycle  

It should **not** include:

- product‑specific environment variables  
- deployment‑specific secrets  
- infrastructure‑specific hosting details  

Those belong in product or deployment documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure configuration implementation.  
Whenever environment‑variable access patterns, configuration binding, or environment detection evolve, update this section to reflect the current platform architecture.
