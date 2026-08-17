# Identity Application — Sessions Subsystem

The **Sessions** folder contains the application‑layer components responsible for
managing authenticated sessions within the Identity subsystem.  
This includes generating secure session tokens, creating sessions, retrieving
sessions, and revoking sessions.

The subsystem follows the CQRS pattern:

- **Commands** for mutating session state  
- **Queries** for retrieving session state  
- **Token generation** for secure cookie issuance  
- **Unit of Work** for transactional persistence  

This structure ensures clarity, testability, and a clean separation between
application logic and persistence.

---

## Folder Structure

```
Sessions/
├── ServiceCollectionExtensions.cs
├── SessionTokenGenerator.cs
├── GetSession/
│   └── GetSessionByIdHandler.cs
└── RevokeSession/
    └── RevokeSessionHandler.cs
```

(Additional abstractions live in `Identity.Application.Abstractions.Sessions`.)

---

# SessionTokenGenerator

Generates secure session tokens and their corresponding SHA‑256 hashes.

### Responsibilities

- Produce a 256‑bit random plaintext token (hex‑encoded)
- Produce a SHA‑256 hash of the token for DB storage
- Provide hashing functionality for cookie validation

### Notes

- Plaintext token → cookie  
- Hashed token → database  
- Used by the Save pipeline and authentication middleware

---

# GetSession

Contains the query handler for retrieving sessions by token hash.

### **GetSessionByIdHandler**

Retrieves a session using its hashed token.

#### Responsibilities

- Accept a `GetSessionQuery`
- Use `IGetSessionReader` to load the session
- Return `GetSessionResponse`
- Throw `SessionNotFoundException` if not found

#### Used During

- Cookie validation  
- Authentication middleware  
- Session introspection  

---

# RevokeSession

Contains the command handler for revoking sessions.

### **RevokeSessionHandler**

Revokes a session using its hashed token.

#### Responsibilities

- Accept a `RevokeSessionCommand`
- Convert raw token hash → `SessionTokenHash`
- Persist revocation via `IRevokeSessionWriter`
- Commit via `IFrankIdentityUnitOfWork`

#### Used During

- Logout flows  
- Forced session invalidation  
- Administrative session cleanup  

---

# ServiceCollectionExtensions

Registers all session‑related services into DI.

### Adds:

- `ISessionTokenGenerator`
- All CQRS command handlers in the Sessions assembly
- All CQRS query handlers in the Sessions assembly

### Notes

- Uses assembly scanning with `DiscoveryOptions`
- Only registers handlers in the `Frank.Identity.Application.Sessions` namespace

---

# Session Lifecycle Overview

```
[ Generate Token ]
        ↓
SessionTokenGenerator
        ↓
[ Plaintext Token → Cookie ]
[ Hashed Token → DB ]
        ↓
CreateSession (Save Pipeline)
        ↓
[ Session Created ]
        ↓
GetSessionQuery
        ↓
GetSessionByIdHandler
        ↓
[ Session Retrieved ]
        ↓
RevokeSessionCommand
        ↓
RevokeSessionHandler
        ↓
[ Session Revoked ]
```

This lifecycle ensures:

- Cryptographically secure token generation  
- Immutable session creation  
- Deterministic session retrieval  
- Safe and auditable session revocation  

---

# Summary

The Sessions subsystem provides the full authenticated‑session lifecycle:

### Token Generation
- `SessionTokenGenerator`

### Session Retrieval
- `GetSessionByIdHandler`

### Session Revocation
- `RevokeSessionHandler`

### DI Registration
- `ServiceCollectionExtensions`

Together, these components form a clean, testable, and predictable session‑management subsystem within the Identity architecture.

---
