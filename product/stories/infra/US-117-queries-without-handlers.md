# US-117 — US‑117 — Allow Queries Without Handlers Until Dispatched

## Intent
_As a product engineer, I should be able to define queries without immediately providing handlers, so that I can design read models incrementally without breaking startup._

## Value
Supports slice‑by‑slice development and reduces pressure to complete read models prematurely.

## Acceptance Criteria
- Application startup succeeds even if queries exist with no handlers.
- No warnings or errors at startup.
- Dispatching such a query still throws a descriptive exception.

## Emotional Guarantees
- EG‑02 Predictable Progress

## Notes
Add architectural constraints, purity rules, or cross‑slice considerations.
