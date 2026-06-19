# oidc-protocol-architecture.md

# OIDC Protocol Architecture (Frank)

The **OIDC Protocol Architecture** defines how Frank handles OpenID Connect
(OIDC) as a **pure protocol layer**.

Frank’s OIDC layer:

- speaks to external identity providers (IdPs)
- performs all protocol‑level validation
- produces a normalized, validated authentication context

It does **not** perform:

- business logic
- persistence
- session creation
- redirects

Those concerns belong to consuming applications (e.g., CampFitFurDogs).

---

## Purpose

The OIDC protocol layer exists to:

- centralize OIDC mechanics in Frank
- ensure secure, correct token handling
- provide a normalized identity context to applications
- prevent products from re‑implementing OIDC flows
- clearly separate protocol from business authentication behavior

---

## High‑Level Flow

1. Application receives an OIDC callback (e.g., `/signin-oidc`).
2. Application delegates to the **Frank OIDC protocol pipeline**.
3. Frank:
   - exchanges the authorization code
   - retrieves tokens from the IdP
   - validates tokens (signature, issuer, audience, expiry)
   - extracts and normalizes claims
   - enforces required claims
4. Frank returns a **protocol context** to the application.
5. The application’s authentication pipeline:
   - resolves or creates a user
   - creates a session
   - decides the redirect target

Frank stops at the **protocol context** boundary.

---

## Responsibilities

Frank’s OIDC layer **must**:

- handle authorization‑code exchange
- call the IdP token endpoint
- validate ID/access tokens
- enforce issuer/audience/expiry
- extract claims from tokens
- normalize provider‑specific claims into a stable shape
- enforce required claims (e.g., subject, email if required)
- surface protocol errors in a controlled way

Frank’s OIDC layer **must not**:

- create or persist users
- create or manage sessions
- decide redirects
- perform product‑specific onboarding
- embed business rules

---

## Protocol Context

The OIDC protocol pipeline returns a **protocol context** to the application,
which includes:

- validated subject identifier
- normalized claims (e.g., email, name, provider ID)
- token metadata (e.g., issuer, audiences)
- protocol status (success/failure, error codes)

This context is:

- immutable
- validated
- free of raw, untrusted payloads

Applications consume this context to drive their **authentication pipeline**.

---

## Error Handling

On protocol failure (e.g., invalid code, token validation failure):

- Frank does **not** throw raw protocol exceptions to the application boundary.
- Frank returns a protocol error result (or throws mapped exceptions) that:
  - clearly indicates failure
  - does not expose sensitive details
- Applications decide:
  - how to surface the failure to the user
  - where to redirect

Frank owns **protocol correctness**, not UX.

---

## Security Boundaries

Between **IdP and Frank**:

- IdP is untrusted until tokens are validated.
- Frank must:
  - use HTTPS
  - validate TLS
  - validate all tokens and responses

Between **Frank and Application**:

- Application trusts:
  - that tokens are valid
  - that claims are normalized
- Application must not:
  - call the IdP directly
  - parse tokens itself
  - bypass the OIDC protocol layer

---

## Prohibitions

Frank’s OIDC layer must not:

- depend on product‑specific types (e.g., Owner, Staff, Admin)
- depend on product data stores
- embed product‑specific configuration beyond provider settings
- perform any persistence

Applications must not:

- re‑implement OIDC flows
- parse or validate tokens directly
- call IdP endpoints outside Frank

---

## Enforcement

The OIDC Protocol Architecture is enforced via:

- integration tests against IdPs (or test IdP)
- token validation tests
- claim normalization tests
- configuration guardrails
- conventions governance

Frank’s OIDC layer remains:

- **protocol‑only**
- **product‑agnostic**
- **security‑focused**
