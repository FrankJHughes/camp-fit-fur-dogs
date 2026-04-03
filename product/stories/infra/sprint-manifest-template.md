# Sprint Manifest

## Intent
A reusable manifest format exists for each sprint that declares the sprint
goal, time boundaries, capacity target, and references to every story in
scope — so that sprint scope is version-controlled, auditable, and
machine-readable.

## Value
Sprint 2 has stories assigned to it but no artifact declaring what the
sprint contains, when it runs, or what the goal is. Without a manifest,
sprint scope lives in someone's head. A manifest makes planning decisions
explicit and enables future automation such as burndown generation and
scope-change detection.

## Acceptance Criteria
- [ ] A reusable manifest template exists and is documented
- [ ] The manifest declares: sprint goal, time boundaries, capacity target, and story references
- [ ] Stories are referenced, not duplicated — each story artifact remains in its canonical location
- [ ] A manifest exists for Sprint 2
- [ ] The manifest format is documented in planning or contributor documentation
- [ ] Prior sprints are either backfilled or noted as pre-convention

## Out of Scope
- Automating manifest generation
- Burndown or velocity tracking tooling
- Prescribing file format, location, or field names

## Emotional Guarantees: N/A
