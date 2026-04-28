# US-116 — US‑116 — Dispatch Queries With Clear Failure When No Handler Exists

## Intent
_As a product engineer, I must be able to dispatch a query and receive a clear, actionable error when no handler exists, so that I can quickly identify missing read‑side logic._

## Value
Prevents null/default responses and ensures queries are never silently ignored.

## Acceptance Criteria
- Dispatching a query with no handler throws a descriptive exception.
- Exception message identifies the missing handler type.
- No exception is thrown at startup for missing query handlers.

## Emotional Guarantees
- EG‑01 No Surprises

## Notes
Add architectural constraints, purity rules, or cross‑slice considerations.
