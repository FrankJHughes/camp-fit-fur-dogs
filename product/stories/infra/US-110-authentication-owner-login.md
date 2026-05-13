---
id: US-110
title: "Authentication Owner Login"
epic: 
milestone: M1+
status: backlog
domain: infra
urgency: high
importance: high
covey_quadrant: Q1
vertical_slice: false
emotional_guarantees:
legal_guarantees:
dependencies:
---

# US‑110 — Authentication: Owner Login (OIDC)

## Intent
As an **owner**, I must be able to **log in securely using an external identity provider**, so that I can access my dog’s information without managing a password.

## Value
- Establishes the foundation for all authenticated owner experiences.
- Reduces security risk by delegating identity to a trusted provider.
- Eliminates password management friction for owners.
- Enables future verticals (Reservations, Grooming, Dog Management).

## Acceptance Criteria

### OIDC Login Flow
- Owner can initiate an OIDC login flow from the frontend.
- Successful login returns an authenticated session token (US‑111).
- Failed login returns correct HTTP error semantics (401, 403).
- No identity provider tokens are persisted server‑side.
- Login event is recorded for audit and security monitoring.

### System Behavior
- Endpoint remains pure (no domain logic).
- Identity provider configuration is environment‑specific (dev/staging/prod).
- Login flow supports future expansion to multiple providers.

### Observability
- Audit logs include: timestamp, provider, success/failure, and correlation ID.
- No sensitive identity provider payloads are logged.

## Emotional Guarantees
- **EG‑01 — No Surprises**  
  Owners understand why login is required and what happens next.
- **EG‑03 — Calm Protection**  
  Authentication feels safe, predictable, and secure.

## Notes
- Must follow API Endpoint Purity rules.
- Must follow Dispatcher Pipeline rules.
- Does not include authorization or onboarding.
- Pairs with US‑111 (Session Management) for token validation.
