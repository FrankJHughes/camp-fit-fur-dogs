# Identity Application — UnitOfWork

The **UnitOfWork** folder contains the Identity subsystem’s specialized
application‑layer abstraction for transactional boundaries.

Identity operations—such as session creation, session revocation, and owner‑state
changes—may require atomic, consistent, and durable write guarantees depending on
the underlying persistence mechanism.  
This folder provides a stable application‑layer contract that allows the Identity
subsystem to participate in transactional workflows without referencing any
infrastructure‑specific details.

---

## Folder Structure

```
UnitOfWork/
└── IFrankIdentityUnitOfWork.cs
```

---

# IFrankIdentityUnitOfWork

`IFrankIdentityUnitOfWork` is the Identity subsystem’s dedicated unit‑of‑work
abstraction.  
It extends the core `IUnitOfWork` interface and provides a clear boundary for
transactional operations within Identity.

### Responsibilities

- Represent the transactional scope for Identity operations  
- Allow command handlers to commit or roll back changes atomically  
- Abstract away infrastructure concerns (SQL transactions, NoSQL batches,
  distributed transactions, etc.)  
- Provide a stable contract for the application layer

### Notes

- The application layer never assumes the underlying transaction mechanism  
- Infrastructure implementations may wrap:
  - Database transactions  
  - Distributed transaction scopes  
  - Atomic write batches  
  - Event‑sourced commit boundaries  
- Identity command handlers (e.g., session creation or revocation) depend on this
  interface to ensure consistent state transitions

---

# Why a Specialized Identity Unit of Work?

Although the core `IUnitOfWork` abstraction is generic, Identity benefits from a
specialized version because:

- It provides a clear subsystem boundary  
- It enables Identity‑specific DI registration and scoping  
- It avoids leaking infrastructure concerns into other subsystems  
- It supports future Identity‑specific transactional behaviors without modifying
  the core abstraction

This specialization keeps the Identity subsystem clean, isolated, and
architecturally consistent.

---

# Summary

The UnitOfWork folder defines the Identity subsystem’s transactional boundary:

### **IFrankIdentityUnitOfWork**
A thin but important specialization of `IUnitOfWork` that ensures Identity
operations can be executed atomically and consistently, regardless of the
underlying persistence mechanism.

This abstraction helps maintain a clean separation between application logic and
infrastructure concerns across the Identity subsystem.

---
