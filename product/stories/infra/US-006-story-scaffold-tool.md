# Story Scaffold Tool

## User Story

As a Product Owner, I want a scaffold tool that generates a product
story file from minimal inputs — so that every story follows the
established format, lands in the correct directory, and is ready for
refinement without hand-authoring boilerplate.

## Context

The project follows a story-first development model: every feature
begins as a product story under `product/stories/<domain>/` before
becoming a GitHub Issue, sprint board item, or PR. Over Sprint 2,
the team hand-authored eight product stories and discovered a stable
format through repetition:

- **User Story** — As a [role], I want [capability] so that [benefit]
- **Context** — Why this story exists, what problem it solves
- **Scope** — What files/locations this story produces
- **Acceptance Criteria** — Specific, testable, binary (pass/fail)
- **Dependencies** — What must exist before this work begins
- **Open Questions** — Unresolved decisions to address during refinement
- **Decision Record** — Link to ADR (if applicable)

A scaffold tool codifies this format so it cannot drift, and reduces
the friction of creating a new story to a single command.

### Directory Convention

- `product/stories/infra/` — infrastructure stories
- `product/stories/docs/` — documentation stories
- `product/stories/features/` — feature stories
- `product/stories/<domain>/` — new domains as needed

File naming: `US-{NNN}-{kebab-title}.md` — a zero-padded three-digit
story number followed by the kebab-case title
(e.g., `US-005-one-command-local-bootstrap.md`).

### Prior Art

The bootstrap scripts (ADR-0007) established a two-script pattern
— `bootstrap.sh` (bash) and `bootstrap.ps1` (PowerShell) — that
this tool should follow for consistency.

## Scope

| Deliverable | Path |
|-------------|------|
| Bash scaffold script | `scripts/new-story.sh` |
| PowerShell scaffold script | `scripts/New-Story.ps1` |
| Story template | `product/stories/_template.md` |
| Makefile target | `make new-story` (wraps bash script) |
| Product story | `product/stories/infra/US-006-story-scaffold-tool.md` |

## Acceptance Criteria

### Inputs and Defaults

- [ ] Accepts `--title` (required) — used for file name and User
      Story heading.
- [ ] Accepts `--id` (required) — zero-padded three-digit story
      number (e.g., `045`). Used as the `US-{NNN}` prefix in the
      generated file name.
- [ ] Accepts `--domain` (required) — target subdirectory under
      `product/stories/` (e.g., `infra`, `docs`, `features`).
- [ ] Accepts `--role` (optional, default: `developer`) — pre-fills
      the "As a [role]" clause.
- [ ] Accepts `--issue` (optional) — when provided, adds
      `**Issue:** #<number>` to the file header.
- [ ] Accepts `--adr` (optional flag) — when provided, generates
      an ADR stub alongside the story.

### Story Generation

- [ ] Creates `product/stories/<domain>/US-<id>-<kebab-title>.md`
      with all standard sections populated as fill-in prompts.
- [ ] Converts title to kebab-case and prepends the `US-<id>-`
      prefix (e.g., `--id 045 --title "My Feature"` →
      `US-045-my-feature.md`).
- [ ] Creates the domain directory if it does not exist.
- [ ] Refuses to overwrite an existing file unless `--force` is
      provided — prints a clear message with the existing file path.
- [ ] Prints the created file path on completion.

### ADR Stub (Optional)

- [ ] When `--adr` is provided, generates
      `docs/adr/<NNNN>-<kebab-title>.md` with standard ADR sections
      (Status: Proposed, Date, Context, Decision, Alternatives
      Considered, Consequences).
- [ ] Auto-increments ADR number from the highest existing entry
      in `docs/adr/README.md`.
- [ ] Appends the new ADR to the index in `docs/adr/README.md`.
- [ ] Adds a Decision Record link in the generated story pointing
      to the new ADR.

### Template

- [ ] `product/stories/_template.md` contains the canonical story
      format — the scaffold scripts read from this template, not
      from hardcoded strings.
- [ ] Modifying the template changes future scaffold output without
      touching the scripts.

### Makefile Integration

- [ ] `make new-story ID=045 TITLE="My Feature" DOMAIN=features`
      invokes the bash script with the provided arguments.
- [ ] `make new-story` with no arguments prints usage help.

## Dependencies

- Diamond Model L4 — Makefile (Issue #57, merged) for `make` target.
- Established story format from Sprint 2 shipped stories.
- US-012 naming convention (PRs #78, #79, merged) for `US-{NNN}` pattern.

## Open Questions

- Should the tool validate that `--domain` matches an existing
  directory, or create any domain on the fly?
- Should the tool offer an interactive mode (prompt for each field)
  in addition to flag-based input?
- Should the template support optional sections that the scaffold
  conditionally includes (e.g., Decision Record only when `--adr`)?
