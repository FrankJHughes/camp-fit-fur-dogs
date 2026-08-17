# Identity Infrastructure — Audit Logging

The **AuditLogging** folder contains the infrastructure components responsible for
emitting structured audit events within the Identity subsystem.  
These events support authentication observability, security monitoring, and
compliance requirements by producing machine‑readable logs that can be consumed
by your logging pipeline, SIEM tooling, or monitoring dashboards.

Audit logging is intentionally lightweight and stateless, relying on the hosting
environment’s logging infrastructure for storage, correlation, and transport.

---

## Purpose

The AuditLogging subsystem provides:

- A structured audit logger implementation
- DI registration for audit logging
- A simple, extensible foundation for future audit events

It ensures that authentication‑related events (e.g., successful logins) are
captured consistently and reliably.

---

## Files

### **AuditLogger**

Implements `IAuditLogger` using structured logging.

Responsibilities:

- Emits structured audit events using `ILogger<AuditLogger>`
- Logs successful owner logins with internal and external identifiers
- Produces machine‑readable log entries suitable for:
  - Authentication audit trails  
  - Security monitoring  
  - Compliance reporting  
  - Operational observability

Characteristics:

- Stateless  
- Thread‑safe (delegates concurrency to the logging pipeline)  
- Does not persist or correlate events — logging sinks handle that  

---

### **ServiceCollectionExtensions**

Registers the audit logger with the DI container.

Responsibilities:

- Adds `IAuditLogger` as a `Singleton`
- Ensures consistent audit logging across the application
- Provides a single registration point for the audit‑logging subsystem

Why Singleton?

- Audit logging does not depend on scoped resources (e.g., DbContexts)
- Logging pipelines are designed for concurrent, multi‑threaded use
- A single instance ensures consistent formatting and behavior

---

## Design Principles

The AuditLogging subsystem follows these architectural principles:

- **Structured logging**  
  All audit events use named fields (`UserId`, `ExternalId`) for machine parsing.

- **Separation of concerns**  
  Audit logging is isolated from authentication logic and persistence.

- **Statelessness**  
  The logger does not store or buffer events; it delegates to the logging pipeline.

- **Extensibility**  
  Additional audit events (e.g., password reset, session revocation) can be added
  without modifying existing infrastructure.

---

## Example Audit Event

A successful login produces a structured log entry similar to:

```text
Audit: Owner login succeeded. UserId=3f2c..., ExternalId=abc123
```

This can be consumed by:

- Application logs  
- Centralized logging systems  
- SIEM platforms  
- Monitoring dashboards  

---

## Summary

The **AuditLogging** folder provides the foundational infrastructure for emitting
structured audit events within the Identity subsystem:

- A lightweight, structured audit logger  
- DI registration  
- Support for authentication observability and compliance  

It ensures that critical authentication events are captured consistently and
integrated cleanly into your broader observability pipeline.
