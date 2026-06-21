---
id: US-219
title: "Identity Platformization (Authentication + Session + User)"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-176   # Test Harness Migration
  - US-183   # Improve Observability
  - US-218   # Exception-to-Problem Migration (optional)
---

# US‑219 — Identity Platformization (Authentication + Session + User)

## Intent

As an **admin**, I must have a unified, platform‑level identity subsystem in
Frank that owns **Authentication**, **Session**, and **User** so that all
Frank‑based products share a consistent identity lifecycle, schema, and
authentication pipeline without re‑implementing identity logic in each product.

This includes migrating the **User domain model**, **Session domain model**,
**entity configuration**, **EF migrations**, **authentication middleware**,
**session middleware**, and **identity abstraction** from CampFitFurDogs into
Frank.

## Why This Is One Story

Authentication, Session, and User are not separable:

- Authentication establishes *who you are*
- Session establishes *that you remain who you are*
- User is the *identity root* that authentication and session depend on

These three form a **closed identity loop**.  
Migrating them separately would create circular dependencies and inconsistent
security boundaries.

They must be platformized **together**.

---

## Current State

CampFitFurDogs currently owns:

- `User` domain model (Owner/Staff/Admin)
- `Session` domain model (if persistent sessions are used)
- EF Core entity configuration for User/Session
- EF Core migrations for User/Session tables
- authentication middleware (cookie + OIDC glue)
- session validation and renewal logic
- identity and claims mapping
- authorization policies tied directly to the CampFitFurDogs User model
- login/logout flows
- security events (lockout, password reset, etc.)

Frank currently owns:

- OIDC protocol integration (token exchange, validation)
- nothing else related to identity

This creates:

- duplicated identity logic across products
- inconsistent authentication behavior
- product‑specific session handling
- product‑specific identity abstractions
- tight coupling between CampFitFurDogs and its identity schema
- no reusable identity subsystem for future Frank products

---

## Desired State

Frank becomes the **platform owner** of:

### 1. **User Identity**
- platform‑level `User` abstraction
- EF Core configuration for User
- EF Core migrations for User schema
- extension points for product‑specific roles/claims

### 2. **Session Lifecycle**
- session creation
- session validation
- session renewal
- session invalidation
- cookie configuration (secure, HTTP‑only, SameSite, etc.)
- session observability events

### 3. **Authentication Pipeline**
- OIDC integration (already in Frank)
- cookie/session authentication middleware
- principal construction
- identity enrichment hooks
- authentication failure handling (401 behavior)

### 4. **Authorization Integration**
- platform‑level authorization middleware
- extension points for product‑specific policies
- identity → claims → permissions mapping pipeline

### 5. **Observability**
- login events
- logout events
- session renewal events
- authentication failures
- correlation IDs

CampFitFurDogs becomes a **consumer** of Frank Identity and defines only:

- product‑specific roles
- product‑specific claims
- product‑specific authorization policies
- UI/UX for login/logout

---

## Acceptance Criteria

### Platformization (Frank)

- [ ] Frank defines a platform‑level `User` identity abstraction.
- [ ] Frank defines a platform‑level `Session` abstraction.
- [ ] Frank owns EF Core configuration for User/Session.
- [ ] Frank owns EF Core migrations for User/Session schema.
- [ ] Frank provides authentication middleware:
      - OIDC handler
      - cookie/session auth
      - principal construction
- [ ] Frank provides session lifecycle middleware:
      - validation
      - renewal
      - invalidation
- [ ] Frank exposes extension points for:
      - product‑specific claims
      - product‑specific roles
      - product‑specific identity enrichment
- [ ] Frank exposes a stable identity abstraction (`IUserIdentity`) to application code.
- [ ] Frank emits structured identity/session observability events (US‑183).

### Product Integration (CampFitFurDogs)

- [ ] CampFitFurDogs removes:
      - local User/Session domain models
      - local entity configuration
      - local EF migrations
      - local authentication middleware
      - local session middleware
- [ ] CampFitFurDogs adopts Frank Identity for:
      - schema
      - authentication
      - session lifecycle
      - identity abstraction
- [ ] CampFitFurDogs defines only:
      - product‑specific roles
      - product‑specific claims
      - product‑specific authorization policies
- [ ] All existing authentication/session tests pass using Frank Identity.
- [ ] Existing CampFitFurDogs data is preserved or migrated safely.

---

## Migration Constraints

- User IDs must remain stable or be migrated with a deterministic mapping.
- Session invalidation rules must be clearly defined during migration.
- Migration must be reversible or feature‑flagged until stable.
- Backward compatibility must be maintained during rollout.

---

## Emotional Guarantees

- **EG‑01 No Surprises**  
  Existing CampFitFurDogs users do not lose access or experience login failures.

- **EG‑03 Calm Protection**  
  Identity/session handling becomes more robust, secure, and predictable.

- **EG‑04 Future Confidence**  
  New Frank‑based products can adopt identity/session without reinventing the stack.

---

## Notes

- This story is intentionally large because Authentication, Session, and User
  cannot be separated without breaking the identity lifecycle.
- An ADR should accompany this story to define the platform‑level identity model
  and extension points.
