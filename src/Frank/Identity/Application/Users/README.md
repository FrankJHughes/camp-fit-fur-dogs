# Identity Application — Users Subsystem

The **Users** folder contains the application‑layer components responsible for
managing user identities within the Identity subsystem.  
This includes resolving users from external identity providers, creating new
users, and registering all user‑related CQRS handlers.

The subsystem follows the CQRS pattern:

- **Commands** for creating users  
- **Queries** for retrieving users  
- **Resolvers** for mapping external identities → internal users  
- **Writers/Readers** for persistence  
- **Unit of Work** for transactional consistency  

This structure ensures clarity, testability, and a clean separation between
application logic and domain invariants.

---

## Folder Structure

```
Users/
├── ServiceCollectionExtensions.cs
├── UserResolver.cs
└── CreateUser/
    ├── CreateUserCommandHandler.cs
    └── CreateUserCommandValidator.cs
```

(Additional abstractions live in `Identity.Application.Abstractions.Users`.)

---

# UserResolver

Resolves a user from an external identity provider subject ID.

### Responsibilities

- Accept a validated `CallbackOidcContextBuilderResult`
- Check if a user already exists for the external subject ID
- If found → return existing user ID
- If not found → create a new domain `User` using:
  - `FirstName`
  - `LastName`
  - `Email`
  - `ExternalId`
- Persist via `ICreateUserWriter`
- Return the new user’s ID

### Notes

- Domain value objects enforce syntactic and semantic correctness  
- Used during the Save pipeline after OIDC identity acquisition  
- Ensures deterministic user resolution across providers  

---

# CreateUser

Contains the command handler and validator for creating new users.

### **CreateUserCommandHandler**

Constructs and persists a new user.

#### Responsibilities

- Convert primitives → domain value objects  
- Create a `User` domain entity  
- Persist via `ICreateUserWriter`  
- Commit via `IFrankIdentityUnitOfWork`  
- Return the new user’s `Guid`  

### **CreateUserCommandValidator**

Validates identity‑source semantics.

#### Responsibilities

- Ensure `ExternalId` is present  
- Ensure `ExternalId` is in the format `"provider|id"`  

#### Notes

- Does **not** validate first name, last name, email, or phone  
- Domain value objects enforce syntactic rules  
- Request‑level validators enforce syntactic correctness  

---

# ServiceCollectionExtensions

Registers all user‑related services into DI.

### Adds:

- `IUserResolver`
- All CQRS command handlers in the Users assembly
- All CQRS query handlers in the Users assembly

### Notes

- Uses assembly scanning with `DiscoveryOptions`
- Only registers handlers in the `Frank.Identity.Application.Users` namespace

---

# User Lifecycle Overview

```
[ OIDC Callback Result ]
        ↓
UserResolver
        ↓
IGetUserByExternalIdReader
        ↓
[ Existing User? ]
   Yes → Return UserId
   No  → CreateUserCommandHandler
        ↓
User.Create()
        ↓
ICreateUserWriter.WriteAsync()
        ↓
UnitOfWork.CommitAsync()
        ↓
[ User Created or Resolved ]
```

This lifecycle ensures:

- Deterministic user resolution  
- Strong domain invariants  
- Clean separation of concerns  
- Full validation coverage (syntactic + semantic + domain)  

---

# Summary

The Users subsystem provides the full user‑identity lifecycle:

### User Resolution
- `UserResolver`

### User Creation
- `CreateUserCommandHandler`
- `CreateUserCommandValidator`

### DI Registration
- `ServiceCollectionExtensions`

### Abstractions (external to this folder)
- `IUserResolver`
- `ICreateUserWriter`
- `IGetUserByExternalIdReader`
- `IFrankIdentityUnitOfWork`
- Domain value objects (`FirstName`, `LastName`, `Email`, `ExternalId`, etc.)

Together, these components form a clean, testable, and predictable user‑identity
subsystem within the Identity architecture.

---
