# ADR-0009: Story Naming Convention

| Field     | Value              |
|-----------|--------------------|
| Status    | Accepted           |
| Date      | 2026-04-08         |
| Deciders  | Frank Hughes       |

## Context

Product stories accumulated across three sprints using inconsistent
naming: some files used descriptive slugs (`add-contributing-pr-template`),
others used short labels (`ci-baseline`), and none carried a stable
identifier that could survive renames or scope changes.

This made cross-referencing fragile. A story referenced in a sprint
manifest, a GitHub Issue title, or a commit message could not be
located by identifier alone — you had to know the current filename.
Planning YAMLs duplicated the story ID but diverged when files were
renamed independently.

The project needed a single, stable, human-readable identifier that
works in filenames, Issue titles, commit messages, branch names, and
conversation.

## Decision

Adopt the naming convention `US-{NNN}-{kebab-name}.md` for all
product story files, where:

- `US` = User Story prefix (constant).
- `{NNN}` = zero-padded three-digit sequential number, allocated in
  order of creation. Numbers are never reused.
- `{kebab-name}` = lowercase kebab-case summary of the story intent.

**US-001 is permanently reserved.** It was assigned during an early
prototype phase and is excluded from the active backlog to avoid
confusion. The next available number after the initial migration is
US-045.

GitHub Issue titles follow the same pattern: `US-NNN: Title`.

### Examples

| Before | After |
|--------|-------|
| `add-contributing-pr-template.md` | `US-013-add-contributing-pr-template.md` |
| `ci-baseline.md` | `US-015-ci-baseline-build-and-test.md` |
| `containerized-dev-environment.md` | `US-002-containerized-dev-environment.md` |

## Consequences

### Positive

- Every story has a stable, unique identifier that survives renames.
- Cross-references in commits, PRs, Issues, and conversation are
  unambiguous (`US-022` always means the planning runbook).
- Alphabetical directory listing groups stories by creation order,
  providing a natural chronological view.
- Scaffold tooling (US-006) can auto-allocate the next number.

### Negative

- The three-digit cap (999) is a theoretical ceiling. Unlikely to be
  reached for this project.
- Renaming 44 existing files in a single migration (PR #79) created
  a large diff that complicated blame history.

### Neutral

- The `US-` prefix was chosen over alternatives (`STORY-`, `S-`,
  bare numbers) for readability and grep-ability. It is a convention,
  not a technical constraint, and could be changed with a bulk rename
  if a better prefix emerges.
