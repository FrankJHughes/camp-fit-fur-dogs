# Identity Application — Users Subsystem

The **Users** subsystem defines all application‑layer abstractions involved in
creating, resolving, and retrieving user records within the Identity domain.

This subsystem provides a clean separation between:

- **Domain logic** (user entity, invariants)
- **Application logic** (commands, queries, readers, writers, resolvers)
- **Infrastructure logic** (database, caching, normalization, uniqueness)

All files in this folder are **pure application‑layer contracts** or **immutable
response models**.  
No persistence, normalization, or storage concerns appear here.

---

## Folder Structure

```
Users/
├── CreateUser/
│   ├── CreateUserCommand.cs
│   └── ICreateUserWriter.cs
│
├── GetUserByExternalId/
│   ├── GetUserByExternalIdResponse.cs
│   └── IGetUserByExternalIdReader.cs
│
├── GetUserById/
│   ├── GetUserByIdResponse.cs
│   └── IGetUserByIdReader.cs
│
├── ICurrentUser.cs
└── IUserResolver.cs
```

---

# CreateUser

### CreateUserCommand
Carries all required data to provision a new user:

- First name  
- Last name  
- Email  
- External identity provider ID  
- Optional phone number  

Returns the new user’s internal `Guid` identifier.

### ICreateUserWriter
Persists the constructed domain `User` entity.

Responsibilities:

- Enforce uniqueness constraints (email, external ID)  
- Support transactional consistency  
- Abstract away infrastructure concerns  

---

# GetUserByExternalId

### GetUserByExternalIdResponse
Minimal model exposing only the internal user ID resolved from an external
identity provider subject.

### IGetUserByExternalIdReader
Lookup contract for resolving users by external identity provider ID.

Responsibilities:

- Perform lookup by external ID  
- Return `GetUserByExternalIdResponse?`  
- Support cancellation  
- Abstract away persistence and normalization  

---

# GetUserById

### GetUserByIdResponse
Minimal model exposing:

- Internal user ID  
- First name  
- Last name  

Used for profile display, authorization, and owner‑centric workflows.

### IGetUserByIdReader
Lookup contract for resolving users by internal `Guid` identifier.

Responsibilities:

- Perform lookup by internal ID  
- Return `GetUserByIdResponse?`  
- Support cancellation  
- Abstract away persistence concerns  

---

# CurrentUser

### ICurrentUser
Represents the authenticated user associated with the current request.

Exposes:

- `IsAuthenticated`  
- `Id` (internal user ID)  
- `Name` (display name)

This abstraction is populated by authentication/session middleware and used by
application flows requiring user context.

---

# UserResolver

### IUserResolver
Maps an OIDC authentication callback result to an internal user ID.

Responsibilities:

- Extract external identity provider subject  
- Resolve existing user via external ID  
- Create new user if none exists  
- Return internal user ID for session creation  

Encapsulates the full OIDC → user mapping pipeline.

---

# User Subsystem Overview

```
[ OIDC Callback ]
        ↓
IUserResolver
        ↓
[ Internal UserId ]
        ↓
CreateUser / GetUserByExternalId
        ↓
GetUserById (profile, authorization)
        ↓
ICurrentUser (request context)
```

This subsystem ensures:

- Deterministic user creation  
- Clean external‑ID → internal‑ID mapping  
- Minimal, safe exposure of user information  
- Infrastructure‑agnostic design  
- Consistent abstractions across all user workflows  

---

# Summary

The Users subsystem defines the complete application‑layer abstraction for user
management:

### Creation
- `CreateUserCommand`  
- `ICreateUserWriter`

### External‑ID Lookup
- `GetUserByExternalIdResponse`  
- `IGetUserByExternalIdReader`

### Internal‑ID Lookup
- `GetUserByIdResponse`  
- `IGetUserByIdReader`

### Current User Context
- `ICurrentUser`

### OIDC User Resolution
- `IUserResolver`

Together, these abstractions form a clean, testable, and deterministic user
management subsystem within Identity.

---
