# CampFitFurDogs.Application.Exceptions

The `CampFitFurDogs.Application.Exceptions` namespace contains all custom
application‑layer exceptions used to signal structural authentication failures,
identity inconsistencies, and standardized error conditions. These exceptions
provide clear, intentional failure modes that align with the vertical‑slice
architecture and identity model used throughout the application.

This folder contains lightweight, purpose‑specific exception types. They do not
encode domain rules, persistence errors, or infrastructure concerns. Instead,
they capture **application‑level invariants** related to authentication and
identity resolution.

---

## 🎯 Purpose

The exceptions in this namespace serve three primary roles:

- **Authentication correctness**  
  Ensuring that operations requiring an authenticated user fail fast when no
  user is present.

- **Identity consistency**  
  Ensuring that the authenticated user’s identity contains the required claims
  (e.g., `UserId`) before application‑layer workflows proceed.

- **Standardized error signaling**  
  Providing stable, machine‑readable error codes via `ErrorCode` for consistent
  API responses, logging, and telemetry.

---

## 📦 Included Exceptions

### `ErrorCode`
A strongly‑typed catalog of machine‑readable error identifiers used throughout
the application layer. Each error code provides:

- A stable string identifier  
- An optional human‑readable description  
- A consistent mechanism for mapping exceptions to API error responses

Common error codes include:

- `external_auth_provider_failure`
- `user_not_authenticated`
- `invalid_user_identity`
- `validation_failed`
- `domain_error`
- `unexpected`

---

### `UserNotAuthenticatedException`
Thrown when an operation requiring authentication is invoked without an
authenticated user context. This typically indicates:

- Missing or misconfigured authentication middleware  
- A caller attempting an action without being signed in  
- An empty or invalid authentication principal

This exception represents a **structural authentication failure**, not a domain
error.

---

### `UserIdClaimNotFoundException`
Thrown when the authenticated user’s identity does not contain the required
`UserId` claim. This exception is raised when:

- The identity provider omits required claims  
- The authentication token is malformed  
- The application cannot resolve the current user’s identifier

This exception signals an **identity consistency failure**.

---

## 🚫 What Does *Not* Belong Here

This namespace intentionally excludes:

- Domain exceptions  
- Infrastructure exceptions  
- Persistence errors  
- Validation exceptions (handled by FluentValidation)  
- Business rule violations (handled by domain aggregates)

Only **application‑layer identity and authentication failures** belong here.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Application.Abstractions.Exceptions` — shared abstractions  
- `CampFitFurDogs.Domain` — domain invariants and domain exceptions  
- `CampFitFurDogs.Infrastructure` — persistence and external system exceptions  
- `CampFitFurDogs.Api` — HTTP‑level error mapping and response formatting

---
