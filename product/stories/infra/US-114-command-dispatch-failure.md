---
id: US-114
title: "Command Dispatch Failure"
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
# US-114 — US‑114 — Dispatch Commands With Clear Failure When No Handler Exists

## Intent
_As a product engineer, I must be able to dispatch a command and receive a clear, actionable error when no handler exists, so that I can quickly diagnose missing application logic and avoid silent failures._

## Value
Prevents silent failures, improves debugging clarity, and ensures commands are never ignored.

## Acceptance Criteria
- Dispatching a command with no registered handler throws a descriptive exception.
- Exception message identifies the missing handler type.
- No exception is thrown at startup for missing command handlers.

## Emotional Guarantees
- EG‑01 No Surprises
- EG‑03 Calm Protection

## Notes
Add architectural constraints, purity rules, or cross‑slice considerations.

