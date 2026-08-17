# Identity Application — Sessions Subsystem

The **Sessions** folder contains abstractions related to creating, persisting, and
managing authenticated user sessions within the Identity subsystem.

Sessions represent authenticated, durable relationships between a user and the
application. They are created during the OIDC callback save phase and consumed by
downstream authentication and authorization components.

This folder contains **application‑layer contracts only**.  
All persistence, storage, and infrastructure behavior is implemented in
infrastructure‑level projects.

---

## Folder Structure

```
Sessions/
└── CreateSession/
    └── ICreateSessionWriter.cs
```

---

# ICreateSessionWriter

Defines the contract for persisting a newly created `Session` domain object.

### Responsibilities

- Write the fully constructed `Session` to persistent storage  
- Ensure durability and consistency  
- Support cancellation  
- Abstract away infrastructure concerns (SQL, NoSQL, Redis, etc.)

### Input

A domain `Session` containing:

- User ID  
- Session ID  
- Token hash  
- Creation timestamp  
- Expiration metadata  
- Any additional domain‑level session attributes  

### Purpose

This writer is invoked by the session‑creation pipeline after:

1. The OIDC callback save phase resolves the user  
2. A session is created in the domain layer  
3. A token hash and cookie value are generated  

The writer ensures the session is durably stored so that:

- Authentication middleware can validate session tokens  
- Authorization components can resolve user identity  
- Audit logs can correlate session activity  
- Session expiration and revocation rules can be enforced  

---

# Architectural Role

The Sessions subsystem provides a clean separation between:

- **Domain logic** (session creation, token hashing, expiration rules)  
- **Application logic** (invoking writers, orchestrating pipelines)  
- **Infrastructure logic** (actual persistence implementation)

This separation ensures:

- Deterministic session‑creation pipelines  
- Testability (writers can be mocked)  
- Multiple storage backends  
- Clear boundaries between layers  

---

# Summary

The Sessions folder defines the application‑layer contract for persisting
sessions:

### **ICreateSessionWriter**
Responsible for writing domain `Session` objects to persistent storage.

Together with the domain model and the OIDC callback save pipeline, this
abstraction forms the foundation of the Identity subsystem’s session‑management
architecture.

---
