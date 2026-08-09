# Identity Application — CreateUser

The **CreateUser** folder contains the application‑layer components responsible
for creating new users within the Identity subsystem.  
User creation occurs during onboarding, external‑identity provisioning, or
administrative user management.

The subsystem follows the CQRS command pattern:

- A **command** (`CreateUserCommand`)
- A **validator** (`CreateUserCommandValidator`)
- A **handler** (`CreateUserCommandHandler`)
- A **writer abstraction** (`ICreateUserWriter`)
- A **unit of work** (`IFrankIdentityUnitOfWork`)

This structure ensures clarity, testability, and a clean separation between
application logic and domain invariants.

---

## Folder Structure

```
CreateUser/
├── CreateUserCommandHandler.cs
└── CreateUserCommandValidator.cs
```

(Additional abstractions live in `Identity.Application.Abstractions.Users.CreateUser`.)

---

# CreateUserCommandHandler

The handler responsible for constructing and persisting a new user.

### Responsibilities

- Accept a `CreateUserCommand`
- Convert primitives → domain value objects:
  - `FirstName`
  - `LastName`
  - `Email`
  - `ExternalId`
  - `PhoneNumber` (optional)
- Create a `User` domain entity (domain enforces invariants)
- Persist via `ICreateUserWriter`
- Commit via `IFrankIdentityUnitOfWork`
- Return the new user’s `Guid` identifier

### Notes

- Domain value objects enforce syntactic and semantic correctness  
- The handler performs external side effects (write + commit)  
- Cancellation is respected at the beginning of execution  

---

# CreateUserCommandValidator

Provides semantic validation for the identity source.

### Responsibilities

- Ensure `ExternalId` is present  
- Ensure `ExternalId` is in the format `"provider|id"`  

### Notes

- Does **not** validate first name, last name, email, or phone  
- Syntactic validation is handled by the request‑level validator  
- Domain invariants are enforced by value objects  

---

# Abstractions (in Identity.Application.Abstractions)

### **CreateUserCommand**
Represents a request to create a new user.

### **ICreateUserWriter**
Abstraction for persisting new users.

### **IFrankIdentityUnitOfWork**
Coordinates transactional persistence.

### **User Domain Entity**
Enforces identity invariants through value objects:
- `FirstName`
- `LastName`
- `Email`
- `ExternalId`
- `PhoneNumber`

---

# User Creation Flow

```
[ CreateUserCommand ]
        ↓
CreateUserCommandValidator
        ↓
CreateUserCommandHandler
        ↓
Value Object Construction
        ↓
User.Create()
        ↓
ICreateUserWriter.WriteAsync()
        ↓
UnitOfWork.CommitAsync()
        ↓
[ User Created (Guid) ]
```

This flow ensures:

- Strong domain invariants  
- Deterministic user creation  
- Clear separation of concerns  
- Full validation coverage (syntactic + semantic + domain)  

---

# Summary

The CreateUser folder defines the application‑layer user‑creation workflow:

### Core Components
- `CreateUserCommandHandler`
- `CreateUserCommandValidator`

### Abstractions (external to this folder)
- `CreateUserCommand`
- `ICreateUserWriter`
- `IFrankIdentityUnitOfWork`
- Domain value objects (`FirstName`, `LastName`, `Email`, etc.)

Together, these components form a clean, testable, and predictable user‑creation
subsystem within the Identity architecture.

---
