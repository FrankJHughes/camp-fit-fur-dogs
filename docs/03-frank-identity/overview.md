# Frank.Identity — Overview

The **Frank.Identity** subsystem provides authentication, authorization, user resolution, and session management for the platform. It integrates external identity providers (Auth0 via OIDC), manages identity users and sessions, and exposes the authenticated user to the rest of the application through a consistent runtime abstraction.

Identity is composed of five architectural layers:

- **[API](ca://s?q=Explain_identity_api_layer)** — HTTP endpoints, middleware, and request‑level identity handling  
- **[Application](ca://s?q=Explain_identity_application_layer)** — authentication flows, session issuance, user creation, orchestration  
- **[Domain](ca://s?q=Explain_identity_domain_layer)** — pure identity models and invariants  
- **[EntityFrameworkCore](ca://s?q=Explain_identity_efcore_layer)** — persistence for users, sessions, and lockout state  
- **[Infrastructure](ca://s?q=Explain_identity_infrastructure_layer)** — provider integration, configuration, logging, current user resolution  

This document describes the Identity subsystem under:

```
/docs/03-frank-identity
```

and maps it back to its implementation under:

```
/src/Frank/Identity
```

---

## Key Responsibilities

### [Authentication](ca://s?q=Explain_identity_authentication)
Validates user identity using external OIDC providers (Auth0).

Responsibilities:

- redirect to provider login  
- process OIDC callback  
- validate tokens and claims  
- construct domain identity user  
- issue session state  

Authentication is orchestrated in the Application layer and configured in Infrastructure.

---

### [Sessions](ca://s?q=Explain_identity_session_management)
Creates, retrieves, validates, and revokes user sessions.

Responsibilities:

- issue new sessions after authentication  
- validate session tokens on each request  
- support expiration and revocation  
- persist session state in EF Core  

Sessions are domain models persisted through EF Core and used by API middleware.

---

### [Users](ca://s?q=Explain_identity_user_model)
Manages identity‑level users (not customer domain users).

Responsibilities:

- create identity users on first login  
- update identity metadata when claims change  
- look up users by provider subject or identity ID  
- persist identity users in EF Core  

Identity users represent authentication subjects within the platform.

---

### [OIDC Flow](ca://s?q=Explain_identity_oidc_flow)
Handles OAuth2/OIDC callback processing.

Responsibilities:

- retrieve provider metadata  
- validate tokens using JWKS  
- normalize provider claims  
- map claims to domain identity user  
- orchestrate session issuance  

OIDC flow is implemented in Application and configured in Infrastructure.

---

### [Authorization](ca://s?q=Explain_identity_authorization)
Enforces access control on protected endpoints.

Responsibilities:

- validate session state  
- resolve current user  
- enforce policies and role‑based rules  
- integrate with platform authorization middleware  

Authorization is performed in the API layer using domain identity metadata.

---

## Architectural Layers

### API Layer — `/src/Frank/Identity/Api`

Responsibilities:

- authentication middleware  
- authorization policies  
- OIDC callback endpoints  
- login/logout endpoints  
- request‑scoped identity resolution  
- platform‑level service composition  

The API layer is the entry point for all identity‑aware HTTP requests.

---

### Application Layer — `/src/Frank/Identity/Application`

Responsibilities:

- commands for creating sessions and users  
- queries for session and user lookup  
- OIDC callback orchestration  
- lockout evaluation (if enabled)  
- unit of work for atomic identity operations  

The Application layer orchestrates identity flows using domain models and infrastructure services.

---

### Domain Layer — `/src/Frank/Identity/Domain`

Responsibilities:

- `User` aggregate  
- `Session` aggregate  
- value objects (IdentityId, ProviderSubject, SessionId, etc.)  
- domain invariants and validation  
- domain exceptions  

The Domain layer contains pure identity logic with no infrastructure dependencies.

---

### EntityFrameworkCore Layer — `/src/Frank/Identity/EntityFrameworkCore`

Responsibilities:

- DbContext for identity entities  
- entity configurations  
- EF Core readers and writers  
- migrations  
- unit of work implementation  

EF Core provides durable persistence for identity users, sessions, and lockout state.

---

### Infrastructure Layer — `/src/Frank/Identity/Infrastructure`

Responsibilities:

- Auth0/OIDC provider integration  
- configuration binding and validation  
- current user resolution  
- audit logging  
- environment detection  
- runtime services (clock, ID generation, HTTP clients)  

Infrastructure supplies the runtime mechanics required by Application and API layers.

---

## Integration Points

Product code integrates with Identity through the `ICurrentUser` abstraction:

```csharp
[FromServices] ICurrentUser currentUser
```

`ICurrentUser` provides:

- `Id` — the authenticated identity ID  
- `Session` — the current session state  
- `IsAuthenticated` — whether the request has a valid identity  

This abstraction ensures API endpoints never parse tokens or claims directly.

---

## Notes

Keep this document grounded in the actual Frank.Identity implementation.  
Whenever authentication flows, provider behavior, or persistence rules evolve, update this overview to reflect the current architecture.
