# Sprint Manifest Template

## Intent
A sprint manifest YAML exists in planning/sprints/ for each sprint that
declares the sprint goal, date range, velocity target, and references to
every story in scope — so that sprint scope is version-controlled,
auditable, and machine-readable.

## Value
Sprint 2 has four stories in planning/stories/sprint-2/ but no manifest
declaring what the sprint contains, when it runs, or what the goal is.
Without a manifest, sprint scope lives in someone's head. A manifest makes
planning decisions explicit and enables future automation (burndown
generation, scope-change detection).

## Acceptance Criteria
- [ ] A manifest template exists (e.g., planning/sprints/_template.yml)
- [ ] Template includes fields: sprint number, goal, startDate, endDate, velocityTarget, stories (array of paths)
- [ ] A manifest exists for Sprint 2 using the template
- [ ] Stories are referenced by path, not duplicated — the YAML stays where it lives
- [ ] planning/README.md documents the manifest format and when to create one
- [ ] Existing sprint-0 and sprint-1 directories are backfilled with manifests or noted as pre-convention

## Out of Scope
- Automating manifest generation from story YAMLs
- Burndown or velocity tracking tooling

## Emotional Guarantees: N/A
