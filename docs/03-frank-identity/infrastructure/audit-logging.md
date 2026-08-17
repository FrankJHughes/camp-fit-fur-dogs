# Frank.Identity.Infrastructure — Audit Logging

The **Audit Logging** subsystem provides structured, environment‑aware logging for identity operations. While the Identity Application layer performs authentication, session issuance, lockout evaluation, and provider integration, the Infrastructure layer ensures these actions are **observable**, **traceable**, and **diagnosable** across environments. Audit Logging contains **no identity logic**, **no domain invariants**, and **no persistence rules** — it is purely operational support for identity flows.

This document describes the Audit Logging subsystem under:

```
docs/03-frank-identity/infrastructure
```

and maps it back to its implementation in:

```
src/Frank/Identity/Infrastructure
```

---

## Purpose

Audit Logging exists to:

- record identity‑related events in a structured, consistent format  
- support debugging, monitoring, and compliance requirements  
- attach correlation and causation metadata to identity flows  
- integrate identity logging with platform‑wide observability (US‑183)  
- ensure identity operations are diagnosable across environments  

Audit Logging is the visibility layer of the Identity subsystem.

---

## Responsibilities of the Subsystem

### [Structured Event Logging](ca://s?q=Explain_identity_structured_logging)
Audit Logging emits structured logs for identity events.

Responsibilities:

- log authentication attempts (success/failure)  
- log session issuance, validation, expiration, and revocation  
- log lockout evaluations and state changes  
- log provider integration events (token validation, claim mapping)  
- log persistence operations (user creation, session updates)  

All logs follow a consistent schema for downstream analysis.

---

### [Correlation & Causation Metadata](ca://s?q=Explain_identity_correlation_metadata)
Audit Logging attaches metadata to identity flows.

Responsibilities:

- generate or propagate correlation IDs  
- attach causation chains across identity operations  
- ensure logs can be traced across API → Application → Infrastructure → EF Core  
- support distributed tracing systems  

Metadata ensures identity flows can be reconstructed end‑to‑end.

---

### [Environment‑Aware Logging](ca://s?q=Explain_identity_environment_detection)
Audit Logging adapts to the hosting environment.

Responsibilities:

- verbose logging in development  
- reduced detail in production  
- toggle diagnostic metadata based on environment  
- integrate with platform‑wide environment detection  

Environment awareness ensures logs are useful but safe.

---

### [Integration with Observability Stack](ca://s?q=Explain_identity_observability)
Audit Logging integrates with the platform’s observability tooling.

Responsibilities:

- emit logs compatible with structured log collectors  
- support metrics and event pipelines  
- support dashboards and alerting (US‑183)  
- ensure identity logs appear in unified observability streams  

Audit Logging is a first‑class participant in platform observability.

---

### [Error & Exception Logging](ca://s?q=Explain_identity_error_logging)
Audit Logging records identity exceptions.

Responsibilities:

- log domain exceptions (validation, token, session, lockout)  
- log application exceptions (provider failures, configuration errors)  
- log EF Core exceptions (persistence failures)  
- ensure logs contain structured error metadata  

Exception logging ensures identity failures are diagnosable.

---

## How Audit Logging Connects to the Broader Platform

Audit Logging collaborates with:

- **Frank.Identity.Application**  
  - logs authentication, session, and lockout events  
  - logs provider integration and claim mapping  
  - logs persistence operations via UoW  

- **Frank.Identity.Domain**  
  - domain exceptions produce structured audit entries  
  - domain invariants influence error semantics  

- **Frank.Identity.EntityFrameworkCore**  
  - logs EF Core persistence operations  
  - logs migrations and provider‑specific behaviors  

- **Frank.Core.Infrastructure**  
  - provides logging engine, configuration, and environment detection  
  - integrates identity logs into unified observability  

- **Frank.Core.Api**  
  - middleware emits identity‑related audit events  
  - logs session validation and authorization decisions  

Audit Logging is the runtime visibility layer for identity flows.

---

## Runtime Collaboration Points

Audit Logging interacts with the runtime by:

- emitting logs for authentication attempts  
- recording session issuance, validation, and revocation  
- logging lockout evaluations and state changes  
- logging provider integration events  
- logging persistence operations and UoW commits  
- integrating with distributed tracing systems  
- supporting environment‑specific logging behavior  

Audit Logging ensures identity operations are transparent and diagnosable.

---

## Composition Flow (Application → Infrastructure → Observability)

```
Identity Application Flow
    ↓
Audit Logging Emits Structured Events
        - Authentication
        - Session Issuance
        - Session Validation
        - Lockout Evaluation
        - Provider Integration
        - Persistence Operations
    ↓
Platform Observability Stack
        - Log Collectors
        - Metrics Pipelines
        - Dashboards
        - Alerts
```

Audit Logging provides the visibility foundation for all identity flows.

---

## What Belongs in This Document

- audit logging responsibilities  
- structured logging schema  
- correlation and causation metadata  
- environment‑aware logging behavior  
- integration with observability stack  
- error and exception logging  

This document does **not** include:

- identity logic  
- authentication flows  
- session issuance rules  
- lockout evaluation  
- HTTP endpoints  
- domain invariants  
- persistence logic  

Those belong in the application, domain, or EF Core layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity Infrastructure implementation.  
Whenever identity flows, observability requirements, or environment behavior evolve, update this section to reflect the current platform architecture.
