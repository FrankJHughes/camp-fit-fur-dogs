# Planning Conventions README

## Intent
A planning/README.md exists at the root of the planning directory that
documents the directory structure, naming conventions, story lifecycle,
git hygiene rules, and atomic-PR convention — so that anyone creating or
reviewing planning artifacts knows the rules without asking.

## Value
Every planning session this team has run so far has re-derived conventions
from scratch. Specs land without YAMLs, YAMLs land with the wrong schema,
branches diverge because there is no documented clean-slate procedure.
A single conventions document eliminates these recurring costs.

## Acceptance Criteria
- [ ] planning/README.md exists and is discoverable from the repo root README
- [ ] Documents directory structure: sprint-N/, backlog/, epics/
- [ ] Documents story lifecycle: backlog → sprint manifest reference → execution
- [ ] Documents atomic-PR convention: product spec + story YAML ship together
- [ ] Documents naming conventions for story files
- [ ] Documents git hygiene: clean-slate branch creation, no amending pushed commits
- [ ] Documents when an ADR is required (architecture decisions gating multiple stories)
- [ ] Cross-referenced from CONTRIBUTING.md

## Out of Scope
- Sprint ceremony procedures (covered by Planning Runbook story)

## Emotional Guarantees: N/A
