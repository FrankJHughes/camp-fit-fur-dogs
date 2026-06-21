# user-identity-architecture.md

# CampFitFurDogs — User Identity Architecture

The **User Identity Architecture** defines what it means to be a user in
CampFitFurDogs and how that identity participates in authentication, session,
and authorization.

CampFitFurDogs currently owns:

- the `User` domain model (Owner/Staff/Admin)
- EF configuration and migrations for User
- mapping from User → claims/principal
- product‑specific roles and permissions

Frank does **not** own the User model today.

---

## Purpose

This architecture exists to:

- define the canonical User representation
- describe how User participates in authentication and session
- define how roles and permissions are derived from User
- ensure identity behavior is consistent across the product

---

## User Model

The User model (conceptually) includes:

- a stable identifier (UserId)
- type/role (Owner, Staff, Admin)
- status flags (active, locked, etc.)
- contact information (e.g., email)
- security‑relevant fields (e.g., password hash if local, lockout counters)

The exact schema is defined in the CampFitFurDogs data model and migrations.

---

## Identity Construction

When a request is authenticated:

1. The Session is resolved and linked to a User.
2. The User is loaded from the database.
3. A principal is constructed with:
   - UserId
   - role(s) (Owner/Staff/Admin)
   - additional claims (e.g., owner ID, staff ID, permissions)

This principal is the identity used by:

- authorization policies
- domain logic that needs to know “who is acting”

---

## Roles and Permissions

CampFitFurDogs defines product‑specific roles and permissions, such as:

- Owner
- Staff
- Admin

Authorization policies and domain rules interpret these roles to decide:

- what endpoints can be called
- what operations can be performed
- what data can be accessed

Roles and permissions are derived from the User model and its relationships.

---

## Interaction with Authentication and Session

- Authentication establishes that a User is who they claim to be.
- Session maintains continuity of that User across requests.
- User is the identity root that both depend on.

User identity must remain:

- stable (IDs do not change)
- authoritative (source of truth for roles/permissions)
- consistent across authentication/session flows

---

## Security Considerations

- Lockout, disabled, or compromised Users must affect authentication/session:
  - login may be blocked
  - existing sessions may be invalidated
- Sensitive fields (password hashes, security tokens) must never be exposed
  outside the data layer.

---

## Prohibitions

- Endpoints and handlers must not:
  - bypass the User model with ad‑hoc identity constructs
  - hard‑code roles/permissions without going through the identity/authorization
    mechanisms
- No other entity may act as a “shadow User” for authentication.
