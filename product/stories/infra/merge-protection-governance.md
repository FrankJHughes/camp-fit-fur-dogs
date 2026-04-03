# Merge Protection Governance

## Intent
Configure GitHub branch protection rules on main so that no code lands without passing CI and at least one approval, enforcing the quality contract.

## Value
Prevents accidental force-pushes, ensures every merge is reviewed, and makes the quality gate non-negotiable.

## Acceptance Criteria
- [ ] main branch requires pull request with at least 1 approval
- [ ] Status checks (CI workflow) are required to pass before merge
- [ ] Force-push and deletion are disabled on main
- [ ] Admin enforcement is enabled (no bypass)
- [ ] Branch protection rules are documented in CONTRIBUTING.md

## Emotional Guarantees: N/A
