# Frank.Core API — Middleware Composition

The **Middleware** root folder provides a unified registration point for all
cross‑cutting middleware subsystems used by the Frank.Core API.  
Rather than requiring each vertical slice or hosting module to register its own
middleware, this layer centralizes the composition of shared infrastructure
concerns such as:

- Observations (correlation, tracing, request context)
- Security Headers (OWASP‑aligned hardening)

This ensures consistent behavior across the entire API surface.

---

## Files

```
Middleware/
├── ServiceCollectionExtensions.cs
```

(Other subsystem folders such as `Observations/` and `SecurityHeaders/` live
alongside this file.)

---

## ServiceCollectionExtensions

`ServiceCollectionExtensions` provides a single entry point for registering all
Frank.Core API middleware subsystems.

### Responsibilities

- Aggregates middleware subsystem registrations:
  - `[Observations]` InboundObservationContextMiddleware  
  - `[Observations]` OutboundObservationContextHandler  
  - `[SecurityHeaders]` SecurityHeadersMiddleware  
- Ensures consistent DI configuration across the entire API.
- Provides a clean, discoverable API for enabling all cross‑cutting middleware.

### Usage

```csharp
services.AddFrankCoreApiMiddleware();
```

This call internally expands to:

```csharp
services
    .AddFrankCoreApiObservations()
    .AddFrankCoreApiSecurityHeaders();
```

Each subsystem registers its own middleware and supporting services.

---

## How Middleware Composition Fits Into the Architecture

This root middleware layer is part of the API’s infrastructure foundation.  
It ensures that all cross‑cutting concerns are:

- centralized  
- consistently configured  
- easy to enable  
- easy to maintain  
- environment‑agnostic  

By consolidating subsystem registration, the API avoids:

- scattered middleware configuration  
- inconsistent DI setup  
- duplicated registration logic  
- slice‑level boilerplate  

This design keeps vertical slices focused on business logic while the middleware
layer handles infrastructure concerns.

---

## Typical Flow

1. **Startup** calls `AddFrankCoreApiMiddleware()`.  
2. Observations subsystem registers inbound + outbound tracing.  
3. SecurityHeaders subsystem registers hardened response headers.  
4. Pipeline is fully prepared for cross‑cutting concerns.  
5. Vertical slices execute with consistent observability and security.

---

## Design Principles

- **Centralized configuration**  
  One place to register all middleware subsystems.

- **Composable subsystems**  
  Each subsystem exposes its own `AddFrankCoreApiXyz()` extension.

- **Cross‑cutting consistency**  
  Observability and security apply uniformly across all slices.

- **Minimal surface area**  
  Consumers only call one method: `AddFrankCoreApiMiddleware()`.

---

## Notes

- This layer does not register pipeline order — that is handled by
  `ApplicationBuilderExtensions` inside each subsystem.
- Subsystems remain independent and testable.
- Safe for all environments, including production.

---
