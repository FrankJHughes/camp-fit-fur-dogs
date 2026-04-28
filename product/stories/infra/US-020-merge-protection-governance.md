---
id: US-020
title: "Merge Protection Governance"
epic: ""
milestone: ""
status: shipped
domain: infra
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# Merge Protection Governance

## Intent
Configure GitHub branch protection rules on main so that no code lands without passing CI and at least one approval, enforcing the quality contract.

## Value
Prevents accidental force-pushes, ensures every merge is reviewed, and makes the quality gate non-negotiable.

## Acceptance Criteria
- [x] main branch requires pull request with at least 1 approval
- [x] Status checks (CI workflow) are required to pass before merge
- [x] Force-push and deletion are disabled on main
- [x] Admin enforcement is enabled (no bypass)
- [x] Branch protection rules are documented in CONTRIBUTING.md

## Emotional Guarantees: N/A

