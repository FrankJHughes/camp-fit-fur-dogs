# Identity Application — Audit Logging

The **AuditLogging** folder contains abstractions for recording significant
security‑sensitive or operationally important events within the Identity
subsystem.  
Audit logs provide durable, structured records that support:

- Compliance requirements  
- Operational monitoring  
- Security investigations  
- Forensic analysis  
- Administrative review  

This folder defines **contracts only** — not implementations.  
Concrete audit loggers live in infrastructure‑level assemblies where persistence,
transport, and durability concerns are handled.

---

## Folder Structure

```
AuditLogging/
└── IAuditLogger.cs
```

---

# IAuditLogger

`IAuditLogger` defines the contract for emitting audit events from the Identity
application layer.

### Purpose

Audit logging is a cross‑cutting concern that must be:

- **Durable** — persisted in a reliable store  
- **Structured** — consistent event shape  
- **Application‑level** — triggered by meaningful domain actions  
- **Infrastructure‑agnostic** — callers should not know where logs are stored  

The abstraction ensures that application code can emit audit events without
depending on specific logging technologies (database, file, queue, SIEM, etc.).

---

## Methods

### `Task LoginSucceeded(Guid userId, string externalId)`

Records an audit event indicating that a user successfully completed a login
operation.

#### Captures

- **Internal user identifier** — the domain‑level identity of the user  
- **External identifier** — an upstream or provider‑issued identifier  
  (e.g., OIDC subject, external system ID)

#### Why both identifiers?

Identity flows often involve multiple identity sources.  
The application must record both:

- The user’s internal representation  
- The external identity context used during authentication  

This supports traceability across systems.

---

## Design Principles

- **Separation of concerns**  
  Application code emits audit events; infrastructure decides how they are stored.

- **Minimal surface area**  
  Only meaningful domain events are exposed.

- **Extensibility**  
  Additional audit events can be added without breaking existing implementations.

- **Security awareness**  
  Audit logs are part of the system’s security posture and must be treated as
  sensitive operational data.

---

## Typical Usage

Application layer code calls the abstraction:

```csharp
await auditLogger.LoginSucceeded(user.Id, externalSubject);
```

Infrastructure implements the abstraction:

- Database audit table  
- Append‑only event log  
- External SIEM  
- Message queue  
- File‑based audit trail  

The application layer never knows which one is used.

---

## Summary

The AuditLogging folder provides:

### **IAuditLogger**
A clean, infrastructure‑agnostic contract for recording audit events such as
successful logins.

This ensures that the Identity subsystem maintains a durable, structured audit
trail without coupling application logic to persistence or transport concerns.

---
