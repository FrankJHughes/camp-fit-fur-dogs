# US-115 — US‑115 — Allow Commands Without Handlers Until Dispatched

## Intent
_As a product engineer, I should be able to define commands without immediately providing handlers, so that I can incrementally build vertical slices without breaking application startup._

## Value
Supports iterative development and reduces friction during slice creation.

## Acceptance Criteria
- Application startup succeeds even if commands exist with no handlers.
- No warnings or errors are emitted at startup.
- Dispatching such a command still throws a descriptive exception.

## Emotional Guarantees
- EG‑02 Predictable Progress

## Notes
Add architectural constraints, purity rules, or cross‑slice considerations.
