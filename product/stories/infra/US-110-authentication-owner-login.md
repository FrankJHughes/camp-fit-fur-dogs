---
id: US-110
title: "Authentication Owner Login"
epic: ""
milestone: ""
status: backlog
domain: infra
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# US‑110 — Authentication: Owner Login (OIDC)

## Intent
As an **owner**, I must be able to **log in securely using an external identity provider**, so that I can access my dog’s information without managing a password.

## Value
- Establishes the foundation for all authenticated owner experiences.
- Reduces security risk by delegating identity to a trusted provider.
- Enables future verticals (Reservations, Grooming, Dog Management).

## Acceptance Criteria
- Owner can initiate an OIDC login flow.
- Successful login returns an authenticated session token.
- Failed login returns correct HTTP error semantics.
- No identity provider tokens are persisted.
- Login event is recorded for audit.
- Endpoint remains pure (no domain logic).

## Emotional Guarantees
- **EG‑01 — No Surprises**
  Owners understand why login is required and what happens next.
- **EG‑03 — Calm Protection**
  Authentication feels safe, predictable, and secure.

## Notes
- Must follow API Endpoint Purity rules.
- Must follow Dispatcher Pipeline rules.
- Does not include authorization or onboarding.

