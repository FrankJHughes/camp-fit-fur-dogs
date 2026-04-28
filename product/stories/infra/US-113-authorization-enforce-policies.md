---
id: US-113
title: "Authorization Enforce Policies"
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
# US‑113 — Authorization: Enforce Policies in API Endpoints

## Intent
As an **owner**, I must be **prevented from performing actions I am not authorized for**, so that the system remains fair, safe, and predictable.

## Value
- Protects all verticals (Dog Management, Reservations, Grooming).
- Ensures purity: endpoints enforce access, not business logic.
- Centralizes access control at the correct architectural boundary.

## Acceptance Criteria
- All protected endpoints call the policy engine.
- Unauthorized requests return 403.
- Policies enforced before dispatching commands/queries.
- No domain logic in authorization checks.

## Emotional Guarantees
- **EG‑03 — Calm Protection**
  Owners feel their data is protected.
- **EG‑05 — Clear Boundaries**
  Access rules are predictable and transparent.

## Notes
- Must follow API Endpoint Purity.
- Must follow Dispatcher Pipeline.

