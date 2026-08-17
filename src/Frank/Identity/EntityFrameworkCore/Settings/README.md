# Identity EntityFrameworkCore — Settings

The **Settings** folder contains configuration objects used by the Identity
EntityFrameworkCore infrastructure. These settings are bound from application
configuration and validated at startup to ensure the Identity subsystem behaves
predictably across all environments.

This folder currently contains configuration for session management.

---

## Purpose

The Settings folder provides strongly‑typed configuration objects that:

- Bind to application configuration (e.g., `appsettings.json`)
- Support validation via data annotations
- Are consumed by EF Core–backed readers and writers
- Allow the Identity subsystem to remain environment‑aware without leaking
  configuration concerns into the Domain layer

---

## Files

### **SessionSettings**

Represents configuration for authentication session behavior.

Responsibilities:

- Defines the session time‑to‑live (TTL)
- Provides a single source of truth for session expiration policy
- Is bound from configuration under the key:  
  **`Identity:Session`**
- Is validated at startup using:
  - `ValidateDataAnnotations()`
  - `ValidateOnStart()`

Used by:

- `GetSessionReader` — to compute `ExpiresAt = CreatedAt + Ttl`
- Authentication middleware — to determine whether a session is still valid
- Security policy enforcement — to ensure sessions expire consistently

---

## Design Principles

The Settings subsystem follows these architectural principles:

- **Strong typing**  
  Configuration values are represented as immutable objects rather than raw
  strings or primitives.

- **Separation of concerns**  
  Configuration lives outside the Domain layer and is consumed only by
  infrastructure components.

- **Fail‑fast validation**  
  Misconfiguration is detected at startup, not at runtime.

- **Environment awareness**  
  Settings can vary between development, staging, and production without
  changing code.

---

## How Settings Are Used

1. **Configuration binding**  
   `SessionSettings` is bound from `Identity:Session` via `AddOptions`.

2. **Validation**  
   Data annotations and `ValidateOnStart` ensure correctness.

3. **Consumption**  
   - `GetSessionReader` uses TTL to evaluate expiration  
   - Session lifecycle logic uses TTL to enforce security policies

4. **Testing**  
   Settings can be overridden in test environments for deterministic behavior.

---

## Summary

The **Settings** folder provides the configuration backbone for the Identity
EntityFrameworkCore subsystem.  
It ensures that session behavior—especially expiration—is driven by validated,
environment‑specific configuration rather than hard‑coded values.

This keeps the Identity subsystem flexible, predictable, and aligned with
domain‑driven design principles.

