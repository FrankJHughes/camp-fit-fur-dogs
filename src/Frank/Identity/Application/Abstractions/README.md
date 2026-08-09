# Identity Application — Abstractions

The **Abstractions** folder defines the public contract surface of the Identity
application layer.  
These abstractions describe *what* the Identity subsystem does — not *how* it is
implemented.  
All persistence, normalization, OIDC protocol handling, and infrastructure
concerns are delegated to the infrastructure layer.

This folder contains:

- Core authentication callback models  
- Error and exception types  
- Token wrappers  
- User‑related abstractions (creation, lookup, resolution, current user context)

Together, these abstractions form the stable API boundary consumed by handlers,
middleware, vertical slices, and other application‑layer components.

---

## Folder Structure

```
Abstractions/
├── AuthCallbackError.cs
├── AuthCallbackException.cs
├── AuthToken.cs
└── Users/
    ├── CreateUser/
    ├── GetUserByExternalId/
    ├── GetUserById/
    ├── ICurrentUser.cs
    └── IUserResolver.cs
```

---

# Authentication Callback Abstractions

These abstractions model the OIDC callback pipeline and its failure modes.

### AuthCallbackError
Enumerates all known error conditions that may occur during the OIDC callback
process:

- Missing authorization code  
- Incomplete configuration  
- Missing access token  
- Userinfo endpoint failure  
- Missing external ID  
- Missing callback result  

### AuthCallbackException
Strongly typed exception wrapping an `AuthCallbackError`.  
Used to surface deterministic callback failures to upstream components such as:

- OIDC callback handlers  
- User resolution logic  
- Session creation pipelines  
- Authentication middleware  

### AuthToken
Minimal wrapper around the raw access token returned by the identity provider.  
Used to safely pass the token between application‑layer components.

---

# Users Subsystem Abstractions

The **Users** folder contains all application‑layer contracts for user creation,
lookup, resolution, and request‑context representation.

### CreateUser
- `CreateUserCommand` — immutable command for provisioning a new user  
- `ICreateUserWriter` — persistence contract for storing the new user  

### GetUserByExternalId
- `GetUserByExternalIdResponse` — minimal model exposing internal user ID  
- `IGetUserByExternalIdReader` — lookup contract using external identity provider ID  

### GetUserById
- `GetUserByIdResponse` — minimal profile model (Id, FirstName, LastName)  
- `IGetUserByIdReader` — lookup contract using internal user ID  

### ICurrentUser
Represents the authenticated user associated with the current request:

- `IsAuthenticated`  
- `Id`  
- `Name`  

Populated by authentication/session middleware.

### IUserResolver
Maps an OIDC callback result to an internal user ID.  
Encapsulates the full external‑ID → internal‑ID resolution pipeline.

---

# Architectural Role of Abstractions

The Abstractions folder provides:

- **Stable contracts** for all Identity application workflows  
- **Clear separation of concerns** between application and infrastructure layers  
- **Deterministic behavior** for authentication, user resolution, and lookup  
- **Minimal, safe models** that avoid leaking sensitive identity provider details  
- **Composable building blocks** for vertical slices and middleware  

These abstractions ensure that the Identity subsystem remains:

- Testable  
- Predictable  
- Infrastructure‑agnostic  
- Easy to extend  
- Safe for cross‑boundary consumption  

---

# Summary

The Abstractions folder defines the complete application‑layer contract surface
for the Identity subsystem:

### Authentication Callback
- `AuthCallbackError`  
- `AuthCallbackException`  
- `AuthToken`

### Users
- Creation  
- External‑ID lookup  
- Internal‑ID lookup  
- Current user context  
- OIDC user resolution

Together, these abstractions form the backbone of the Identity application layer,
providing a clean, stable, and deterministic API for all identity‑related
workflows.

---
