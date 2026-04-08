# Planning Artifact Validation

## User Story

As a Scrum Master, I want a CI gate that validates planning artifact
integrity on every pull request — so that broken references, missing
sections, and format drift are caught before merge, and the team can
trust the backlog as a single source of truth.

## Context

The project maintains three categories of planning artifacts:

1. **Product stories** — markdown files under `product/stories/<domain>/`
   following a standard format (User Story, Context, Scope, Acceptance
   Criteria, Dependencies, Open Questions, Decision Record).
2. **ADRs** — markdown files under `docs/adr/` following a standard
   format (Status, Date, Context, Decision, Alternatives Considered,
   Consequences).
3. **ADR index** — `docs/adr/README.md`, a table linking to every ADR
   with its title and status.

These artifacts are hand-authored and cross-reference each other. Without
automated validation, references break silently, required sections get
omitted, and the index falls out of sync — all of which erode trust in
the planning system.

### What Has Changed

This story replaces an earlier version (Issue #51, Sprint 2) that
targeted YAML-based story artifacts and `source.productFile` references.
The planning workflow evolved during Sprint 2 to use markdown-based
product stories with a different format and linking model. Every
acceptance criterion in the original issue referenced artifacts that
no longer exist.

## Scope

| Deliverable | Path |
|-------------|------|
| Validation script | `scripts/validate-planning.sh` |
| CI workflow step | `.github/workflows/build-and-test.yml` (new job or step) |
| Product story | `product/stories/infra/US-007-planning-artifact-validation.md` |

## Acceptance Criteria

### Product Story Validation

- [ ] Every `.md` file under `product/stories/` (excluding
      `_template.md`) is checked for required sections: at minimum
      `## User Story` and `## Acceptance Criteria`.
- [ ] Files missing required sections produce a clear error message
      identifying the file and the missing section.
- [ ] File naming is validated as `US-{NNN}-{kebab-title}.md` — a
      `US-` prefix, three-digit zero-padded number, hyphen, then
      kebab-case title (lowercase, hyphens, no spaces or special
      characters). See US-012 for the canonical naming convention.

### ADR Validation

- [ ] Every `.md` file under `docs/adr/` (excluding `README.md`) is
      checked for required sections: `**Status:**`, `**Date:**`,
      `## Context`, `## Decision`, `## Consequences`.
- [ ] Files missing required sections produce a clear error message
      identifying the file and the missing section.
- [ ] ADR file naming is validated as `NNNN-kebab-title.md` (four-digit
      prefix, hyphen, kebab-case).

### ADR Index Integrity

- [ ] Every ADR file in `docs/adr/` has a corresponding row in
      `docs/adr/README.md`.
- [ ] Every row in the ADR index references a file that exists.
- [ ] Orphaned entries (index row without a file, or file without an
      index row) produce a clear error message.

### Cross-Reference Validation

- [ ] When a product story contains a `## Decision Record` section
      with a relative path to an ADR, the referenced ADR file is
      verified to exist.
- [ ] When an ADR references a product story path, the referenced
      story file is verified to exist.
- [ ] Broken cross-references produce a clear error message
      identifying both the source file and the missing target.

### CI Integration

- [ ] Validation runs as part of the existing `Build & Test` CI
      workflow on every push and pull request.
- [ ] Validation failures block merge with a clear summary of all
      errors (not just the first one).
- [ ] Validation passes when no planning artifacts have been
      modified (no false negatives on unrelated PRs).
- [ ] The validation script can be run locally:
      `./scripts/validate-planning.sh` (bash) or
      `make validate-planning` (Makefile target).

### Directory Convention

- [ ] `product/stories/` subdirectories are not validated for a fixed
      set — new domains can be created freely.
- [ ] Empty subdirectories do not cause validation errors.

## Dependencies

- Diamond Model L4 — Makefile (Issue #57, merged) for `make` target.
- CI workflow (`.github/workflows/build-and-test.yml`, merged).
- Established planning artifact format from Sprint 2 shipped stories
  and ADRs (ADR-0001 through ADR-0008).
- US-012 naming convention (PRs #78, #79, merged) for `US-{NNN}` pattern.

## Open Questions

- Should validation enforce a maximum ADR status set (Proposed,
  Accepted, Superseded, Deprecated), or allow freeform status values?
- Should the script produce structured output (JSON) in addition to
  human-readable messages, to support future dashboard tooling?
- Should PR description validation (e.g., "references an issue
  number") be part of this gate or a separate check?
