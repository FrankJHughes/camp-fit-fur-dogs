# security-boundaries-architecture.md

# CampFitFurDogs — Security Boundaries Architecture

The **Security Boundaries Architecture** defines the trust boundaries and
responsibility splits between:

- external identity providers
- Frank’s OIDC protocol handling
- CampFitFurDogs authentication/session
- CampFitFurDogs application logic and data

It documents what is trusted where, and what must never cross layers.

---

## Trust Boundaries

### 1. External Identity Provider → Frank OIDC

- The IdP is external and untrusted by default.
- Frank’s OIDC layer:
  - validates tokens
  - enforces issuer/audience
  - enforces required claims
- Raw IdP responses must not be passed directly into CampFitFurDogs.

### 2. Frank OIDC → CampFitFurDogs Authentication

- Frank passes a **normalized, validated protocol context** to CampFitFurDogs.
- CampFitFurDogs trusts:
  - that tokens are valid
  - that claims are structurally correct
- CampFitFurDogs does not:
  - re‑validate tokens
  - call the IdP directly

### 3. Authentication/Session → Application Logic

- Application logic trusts the identity/session abstraction:
  - User is authenticated
  - Session is valid
  - Claims/roles are consistent
- Application logic must not:
  - parse tokens
  - read cookies
  - call the IdP

### 4. Application Logic → Data Layer

- Data layer trusts:
  - that authorization decisions have been made before sensitive operations
- Data layer may still enforce additional invariants (e.g., tenant scoping).

---

## Boundary Responsibilities

- **Frank OIDC**
  - protocol correctness
  - token validation
  - claim normalization

- **CampFitFurDogs Authentication/Session**
  - identity resolution (User)
  - session lifecycle
  - principal construction

- **CampFitFurDogs Application**
  - business rules
  - business‑level authorization

- **Endpoints**
  - thin delivery boundary
  - no security logic beyond attributes/configuration

---

## Prohibitions

- No direct IdP calls from CampFitFurDogs.
- No token parsing outside Frank’s OIDC layer.
- No cookie manipulation outside authentication/session middleware.
- No business logic in Frank’s OIDC layer.
- No security‑critical decisions in endpoints (beyond declarative config).
