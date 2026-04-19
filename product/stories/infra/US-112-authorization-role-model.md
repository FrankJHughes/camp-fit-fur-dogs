# US‑112 — Authorization: Role Model & Policy Engine

## Intent
As an **owner**, I must be **restricted to actions I am allowed to perform**, so that my data and other owners’ data remain protected.

## Value
- Prevents authorization logic from leaking into endpoints or domain.
- Enables future multi‑role scenarios (Owner, Staff, Admin).
- Ensures consistent enforcement of access rules.

## Acceptance Criteria
- Role model defined (Owner, Staff, Admin).
- Policy engine implemented.
- Policies expressed declaratively.
- No domain logic in policy evaluation.
- Policy engine callable only from the API layer.

## Emotional Guarantees
- **EG‑04 — Fair Treatment**
  Access rules feel consistent and predictable.
- **EG‑05 — Clear Boundaries**
  Owners understand what they can and cannot do.

## Notes
- Must follow Abstractions Contract rules.
- Must not introduce cross‑layer coupling.
