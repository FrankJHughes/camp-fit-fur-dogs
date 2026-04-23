# US-120 — US‑120 — Receive Helpful Guidance When Handler Resolution Fails

## Intent
_As a product engineer, I should receive descriptive error messages when dispatching a command or query fails due to missing handlers, so that I can quickly correct the issue without digging through DI logs._

## Value
Reduces debugging time and improves onboarding.

## Acceptance Criteria
- Exception messages include the command/query type.
- Exception messages include the expected handler interface.
- Exception messages include a suggestion for how to fix the issue.

## Emotional Guarantees
- EG‑05 Confident Clarity

## Notes
Add architectural constraints, purity rules, or cross‑slice considerations.
