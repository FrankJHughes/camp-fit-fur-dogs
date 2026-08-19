# Helpers

The **Helpers** folder contains small, focused utility classes that support API
startup, hosting, and environment‑specific configuration.  
These helpers are not part of the domain or application layers — instead, they
provide glue logic that adapts the API to its runtime environment.

Currently, this folder contains hosting‑module orchestration used during API
startup.

---

## Files

### Hosting.cs

Provides hosting‑environment adaptation for the Camp Fit Fur Dogs API using
Frank.Core’s hosting‑module engine.

#### Responsibilities

- Constructs the list of hosting modules used by the API  
- Initializes a `HostingEngine` with those modules  
- Applies environment‑specific configuration to the `WebApplicationBuilder`  
- Ensures consistent hosting behavior across local development, Render PR
  preview environments, and future deployment targets

#### Hosting Modules

The following hosting module is currently included:

- **RenderPrPreviewHostingModule**  
  Applies configuration required when running inside Render PR preview
  environments (e.g., environment variables, logging adjustments, preview‑safe
  defaults).

#### Usage

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

This call:

1. Constructs the hosting modules  
2. Creates a `HostingEngine`  
3. Applies environment‑specific configuration asynchronously  

---

## Design Principles

Helpers in this folder follow these principles:

- **Minimalism** — small, focused utilities with no domain logic  
- **Environment awareness** — adapt behavior based on hosting context  
- **Separation of concerns** — hosting logic is isolated from endpoint and
  application logic  
- **Extensibility** — new hosting modules can be added without modifying startup
  code  

---

## Summary

The Helpers folder provides environment‑adaptation utilities for the Camp Fit Fur
Dogs API:

- Centralized hosting‑module construction  
- Unified hosting‑environment configuration  
- Clean separation from domain and application layers  

This structure ensures the API behaves consistently across all deployment
environments.

