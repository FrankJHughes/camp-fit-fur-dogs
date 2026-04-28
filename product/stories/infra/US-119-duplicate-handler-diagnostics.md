---
id: US-119
title: "Duplicate Handler Diagnostics"
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
# US-119 — US‑119 — Receive Clear Diagnostics for Duplicate Handlers

## Intent
_As a product engineer, I must receive a clear startup error when multiple handlers exist for the same command or query, so that I can resolve ambiguity before the application runs._

## Value
Prevents ambiguous dispatch behavior and improves developer confidence.

## Acceptance Criteria
- Startup fails if multiple command handlers exist for the same command.
- Startup fails if multiple query handlers exist for the same query.
- Error message lists all conflicting handler types.

## Emotional Guarantees
- EG‑01 No Surprises
- EG‑05 Confident Clarity

## Notes
Add architectural constraints, purity rules, or cross‑slice considerations.

