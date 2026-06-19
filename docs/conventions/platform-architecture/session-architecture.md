# session-architecture.md

# CampFitFurDogs — Session Architecture

The **Session Architecture** in CampFitFurDogs defines how authenticated user
state is persisted, validated, renewed, and invalidated for the product.

CampFitFurDogs currently owns:

- the `Session` domain model (if persistent sessions are used)
- session cookie configuration
- session lifecycle rules (creation, renewal, expiration, invalidation)
- the link between Session and User
- all EF configuration and migrations for Session

Frank does **not** own sessions today.

---

## Purpose

The Session Architecture exists to:

- maintain authenticated continuity between requests
- enforce session lifetime and renewal rules
- tie authenticated requests to a specific User
- support logout and security‑driven invalidation
- integrate with CampFitFurDogs’ authentication and authorization behavior

---

## Session Lifecycle

1. **Creation**
   - Occurs after successful authentication in the Application Authentication
     pipeline.
   - A Session is created (in memory or persisted) and associated with a User.
   - A session cookie is issued to the client.

2. **Validation**
   - Each request passes through authentication middleware.
   - The session cookie is validated (signature, integrity, expiration).
   - If invalid, the request is treated as unauthenticated.

3. **Renewal**
   - Sliding expiration may extend the session lifetime.
   - Renewal rules are defined by CampFitFurDogs (not Frank).

4. **Usage**
   - A principal is constructed from the Session/User.
   - Authorization and domain logic rely on this identity.

5. **Invalidation**
   - Logout clears the session cookie and/or session record.
   - Security events (password reset, lockout, etc.) may invalidate sessions.
   - Expired sessions are rejected.

---

## Session Cookie Rules

CampFitFurDogs session cookies must:

- be HTTP‑only
- be `Secure` in production
- use an appropriate `SameSite` mode
- be signed and tamper‑resistant
- not contain PII or sensitive data

Endpoints must not:

- read or manipulate cookies directly
- embed business logic in cookie handling

All cookie handling is centralized in authentication/session middleware.

---

## Relationship to User

- Each Session is associated with a User.
- Identity and claims are derived from the User (and possibly Session metadata).
- Authorization decisions depend on the User identity resolved from the Session.

The Session layer must not:

- implement business rules unrelated to identity/lifecycle
- embed product‑specific domain logic beyond identity concerns

---

## Observability

Session events should be logged:

- session creation
- session renewal
- session invalidation
- session validation failures

These logs are used for diagnostics and security auditing.

---

## Prohibitions

- No endpoint or handler may implement its own session logic.
- No direct cookie manipulation outside the authentication/session middleware.
- No business data stored in sessions.
- No exposure of session identifiers or internals to clients beyond the cookie.
