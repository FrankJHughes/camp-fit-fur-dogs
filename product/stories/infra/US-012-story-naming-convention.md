# Story Naming Convention

## User Story

As a contributor, I want every product story, planning YAML, and
GitHub issue to follow a single, predictable naming convention — so
that I can locate any artifact by its ID, cross-reference stories
across systems without ambiguity, and trust that the backlog has
no orphaned or duplicate entries.

## Context

By the end of Sprint 2, the project had accumulated three naming
patterns across its 44 product stories:

- **Infra stories (Sprint 0-2):** bare kebab-case
  (e.g., `ci-baseline-build-and-test.md`)
- **Customer stories:** `US-C{NN}` prefix
  (e.g., `US-C01-create-customer-account.md`)
- **Infra stories (PR #78):** `US-{NNN}` prefix
  (e.g., `US-002-containerized-dev-environment.md`)

Planning YAMLs, sprint manifests, and GitHub issue titles each
followed their own subset of these patterns. The inconsistency made
it impossible to search by ID, validate cross-references
programmatically, or onboard new contributors without a decoder ring.

### Decisions Made

During backlog grooming (April 2026), the team resolved every open
question and implemented the convention in a single migration:

1. **Canonical pattern:** `US-{NNN}-{kebab-name}` — a `US-` prefix,
   three-digit zero-padded number, hyphen, then kebab-case title.
2. **US-001 retired:** Reserved as a gap. The phantom entry from
   PR #78 (no source file ever existed) was not backfilled.
3. **Customer stories renumbered:** `US-C{NN}` to `US-027` through
   `US-044`, bringing all stories into one contiguous sequence.
4. **Planning YAMLs aligned:** Filenames prefixed with `US-{NNN}`,
   internal `id` and `source.productFile` fields updated,
   `.yaml` extension normalized to `.yml`.
5. **GitHub issue titles prefixed:** All 18 linked issues updated
   to `US-{NNN}: Title` format.
6. **Next available number:** `US-045`.

## Scope

| Deliverable | Path / Location | Status |
|-------------|-----------------|--------|
| Product story renames (32 files) | `product/stories/infra/`, `product/stories/customer/` | Done (PR #79) |
| Planning YAML renames + updates (36 files) | `planning/stories/sprint-{0,1,2}/`, `planning/stories/` | Done (PR #79) |
| Sprint manifest updates (3 files) | `planning/sprints/sprint-{0,1,2}.yml` | Done (PR #79) |
| GitHub issue title prefixes (18 issues) | GitHub Issues | Done (gh issue edit) |
| Stale cross-reference fixes (20 refs) | Various | Done (PR #79) |
| Duplicate/superseded file deletions (3 files) | Various | Done (PR #79) |
| ADR-0009 — naming convention decision record | `docs/adr/0009-story-naming-convention.md` | Pending |
| US-006 scaffold tool — teach `US-{NNN}` pattern | `product/stories/infra/US-006-story-scaffold-tool.md` | Pending |
| US-007 validation — enforce `US-{NNN}` pattern | `product/stories/infra/US-007-planning-artifact-validation.md` | Pending |

## Acceptance Criteria

### Naming Convention

- [x] Every product story file follows `US-{NNN}-{kebab-name}.md`.
- [x] Every planning YAML file follows `US-{NNN}-{kebab-name}.yml`.
- [x] Every GitHub issue title follows `US-{NNN}: Title`.
- [x] `US-001` is retired as a reserved gap — no file exists.
- [x] Customer stories use the unified `US-{NNN}` sequence (no
      separate `US-C{NN}` namespace).
- [x] The next available story number (`US-045`) is documented.

### Cross-System Integrity

- [x] Every planning YAML `source.productFile` path resolves to an
      existing product story file.
- [x] Every planning YAML `id` field matches its filename prefix.
- [x] Every sprint manifest path resolves to an existing planning
      YAML file.
- [x] No stale `US-C` references remain in any file.
- [x] No stale `.yaml` extensions remain in `planning/`.

### Documentation

- [ ] ADR-0009 formalizes the naming convention decision, including
      the pattern, the US-001 gap rationale, and the customer story
      renumbering rationale.
- [ ] US-006 (Story Scaffold Tool) spec includes `--id` parameter
      and generates files with the `US-{NNN}` prefix.
- [ ] US-007 (Planning Artifact Validation) spec validates the
      `US-{NNN}-{kebab-name}` file naming pattern.

## Dependencies

- PR #78 (initial infra renames, merged Sprint 2).
- PR #79 (full migration, merged April 2026).

## Open Questions

None — all questions resolved during backlog grooming.

## Decision Record

Formal record pending as ADR-0009.
