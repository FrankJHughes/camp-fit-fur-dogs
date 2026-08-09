# Identity Application — Users / CreateUser

The **CreateUser** folder contains the application‑layer abstractions required to
provision a new user within the Identity subsystem.

User creation is a foundational workflow that establishes an owner’s identity,
contact information, and external identity provider linkage.  
This folder defines the contracts and immutable command model used by the
application layer, while delegating all persistence and infrastructure concerns
to the infrastructure layer.

---

## Folder Structure

```
CreateUser/
├── CreateUserCommand.cs
└── ICreateUserWriter.cs
```

---

# CreateUserCommand

Represents the CQRS command used to create a new user.

### Responsibilities

- Carry all required user‑creation data:
  - First name  
  - Last name  
  - Email  
  - External identity provider ID  
  - Optional phone number  
- Trigger the user‑creation pipeline  
- Return the newly created user’s `Guid` identifier  

### Notes

- The command handler performs validation and uniqueness checks  
- The handler constructs the domain `User` entity  
- Persistence is delegated to `ICreateUserWriter`  
- Email and external ID must be unique within the system  

---

# ICreateUserWriter

Defines the contract for persisting a newly created domain `User` entity.

### Responsibilities

- Store the constructed `User` in durable storage  
- Enforce uniqueness constraints (email, external ID)  
- Support cancellation  
- Integrate with `IFrankIdentityUnitOfWork` for transactional guarantees  

### Notes

- The writer abstracts away all infrastructure details  
- It must persist the `User` exactly as provided by the command handler  
- Constraint violations should result in domain‑appropriate exceptions  
- Infrastructure implementations may apply normalization rules (email, phone)  

---

# User Creation Pipeline Overview

```
[ CreateUserCommand ]
        ↓
Command Handler
        ↓
Domain User Construction
        ↓
ICreateUserWriter
        ↓
[ User Persisted ]
        ↓
Returns UserId (Guid)
```

This pipeline ensures:

- Clean separation of concerns  
- Deterministic user creation  
- Immutable command modeling  
- Infrastructure‑agnostic persistence  
- Transactional consistency via UnitOfWork  

---

# Summary

The CreateUser folder defines the complete application‑layer abstraction for
user provisioning:

### **CreateUserCommand**
Carries all user‑creation data and returns the new user’s identifier.

### **ICreateUserWriter**
Persists the constructed domain `User` entity with transactional guarantees.

Together, these abstractions form a clean, testable, and deterministic
user‑creation subsystem within Identity.

---
