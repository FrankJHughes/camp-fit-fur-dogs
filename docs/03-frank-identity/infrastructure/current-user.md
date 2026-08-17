# Frank.Identity.Infrastructure — Current User

This document describes the `/docs/03-frank-identity/infrastructure` area and maps it back to the implementation under `/src/Frank/Identity/Infrastructure`.

## Purpose

The **Current User** subsystem provides the runtime‑level mechanics for accessing the authenticated identity within the platform. While the Identity Application layer performs authentication, session validation, and claim mapping, the Infrastructure layer exposes a safe, environment‑aware way for API endpoints, middleware, and application services to retrieve the *current authenticated identity user*.

This subsystem contains **no identity logic**, **no domain invariants**, and **no persistence rules** — it is strictly operational support for accessing identity context at runtime.

---

## Source Alignment

- Primary implementation area: `/src/Frank/Identity/Infrastructure`
- Current folder: `/docs/03-frank-identity/infrastructure`

The Current User subsystem is implemented as part of Identity Infrastructure and consumed by Identity Application and API layers.

---

## Responsibilities of the Subsystem

### [Identity Context Access](ca://s?q=Explain_identity_context_access)
Infrastructure provides a unified abstraction for retrieving the current authenticated identity.

Responsibilities:

- expose the current identity user as a domain model  
- expose the current identity ID  
- expose provider subject and issuer  
- expose normalized identity claims  
- ensure access is safe, consistent, and environment‑aware  

This abstraction prevents API endpoints from directly parsing tokens or claims.

---

### [Session‑Backed Identity Resolution](ca://s?q=Explain_identity_session_resolution)
Current User is resolved from validated session state.

Responsibilities:

- retrieve session state from middleware  
- map session metadata to domain identity user  
- ensure revoked or expired sessions never produce a current user  
- ensure resolution is consistent across API and Application layers  

Session validation occurs before Current User resolution.

---

### [Claims Principal Integration](ca://s?q=Explain_identity_claimsprincipal_integration)
Infrastructure integrates with ASP.NET Core’s `ClaimsPrincipal`.

Responsibilities:

- extract identity claims from the authenticated principal  
- normalize provider‑specific claim formats  
- ensure domain identity user is constructed from validated claims  
- prevent direct claim parsing in API endpoints  

ClaimsPrincipal is a transport mechanism, not a domain model.

---

### [Environment‑Aware Behavior](ca://s?q=Explain_identity_environment_detection)
Current User behavior adapts to the hosting environment.

Responsibilities:

- enable diagnostic metadata in development  
- enforce strict validation in production  
- support mock identity providers for local development  
- ensure consistent behavior across environments  

Environment detection influences resolution, not domain identity models.

---

### [Observability & Logging](ca://s?q=Explain_identity_observability)
Current User resolution emits structured logs.

Responsibilities:

- log identity resolution events  
- log missing or invalid session state  
- log provider claim inconsistencies  
- attach correlation and causation metadata  
- integrate with platform‑wide observability (US‑183)  

Observability ensures identity context issues are diagnosable.

---

## How Current User Connects to the Broader Platform

Current User collaborates with:

- **Frank.Identity.Application**  
  - application services consume the current identity user  
  - session validation precedes identity resolution  

- **Frank.Identity.Domain**  
  - domain identity user models represent the resolved identity  
  - domain invariants ensure identity state is valid  

- **Frank.Identity.EntityFrameworkCore**  
  - persistence stores identity users and sessions  
  - Current User resolution may trigger session reads indirectly  

- **Frank.Core.Infrastructure**  
  - provides ClaimsPrincipal, logging, environment detection  
  - integrates identity context with hosting engine  

- **Frank.Core.Api**  
  - middleware resolves session and identity before endpoint execution  
  - API endpoints consume Current User abstraction  

Current User is the runtime bridge between authentication and application logic.

---

## Runtime Collaboration Points

Current User interacts with the runtime by:

- resolving identity from validated session state  
- mapping claims to domain identity user  
- emitting logs for identity resolution  
- supporting environment‑specific behavior  
- surfacing identity context failures to observability  
- providing identity metadata to API endpoints and application services  

It ensures identity context is safe, consistent, and diagnosable.

---

## Composition Flow (Session → Middleware → Infrastructure → Application → API)

```
Session Validation (Middleware)
    ↓
ClaimsPrincipal Created
    ↓
Current User Resolved (Infrastructure)
        - IdentityId
        - ProviderSubject
        - ProviderIssuer
        - Claims
    ↓
Identity Application Services
    ↓
API Endpoint Execution
```

Current User provides the runtime identity context for all identity‑aware operations.

---

## What Belongs in This Document

- identity context access  
- session‑backed identity resolution  
- ClaimsPrincipal integration  
- environment‑aware behavior  
- observability and logging  
- runtime collaboration points  

This document does **not** include:

- authentication logic  
- claim mapping rules  
- session issuance  
- lockout evaluation  
- HTTP endpoints  
- domain invariants  
- persistence logic  

Those belong in the application, domain, or EF Core layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity Infrastructure implementation.  
Whenever identity resolution, session behavior, or environment rules evolve, update this section to reflect the current platform architecture.
