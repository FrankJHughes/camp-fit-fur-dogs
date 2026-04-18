# Product Story Naming Convention

## User Story

As a Product Owner, I want every product story file to follow a
consistent naming convention with a type prefix, sequential identifier,
and descriptive kebab-case name — so that stories are uniquely
referenceable in conversation, sortable by type, and immediately
recognizable in a directory listing.

## Context

Over two sprints the project accumulated product story files under
four different naming patterns:

- `story.doc-architecture.md` (dot-prefix era)
- `story-scaffold-script.md` (bare kebab-case, no identifier)
- `validate-planning-workflow.md` (verb-first, no identifier)
- `consistent-editor-experience.md` (noun-phrase, no identifier)

None of these carry a stable, speakable identifier. In standup or a
PR description you can't say "US-007" — you have to spell out the
full file name. There is no way to distinguish a user story from a
spike or chore at a glance. And when the scaffold tool (US-006)
ships, it needs a naming rule to enforce.

### Decision

**Pattern:** `{TYPE}-{NNN}-{kebab-name}.md`

| Component | Rule |
|-----------|------|
| `TYPE` | Work item type: `US` (user story), `SP` (spike), `CH` (chore) |
| `NNN` | Three-digit, zero-padded, sequential per type across all domains |
| `kebab-name` | Lowercase, hyphen-delimited, descriptive — matches story title |

**Numbering is per-type and global.** Each type owns its own counter.
The combination of TYPE + NNN is globally unique across all domain
directories. The scaffold tool scans all directories under
`product/stories/` to find the highest existing number for the
given type before assigning the next.

### Examples

| File | Meaning |
|------|---------|
| `US-001-infrastructure-services.md` | 1st user story |
| `US-011-scrum-master-contributor-guide.md` | 11th user story |
| `SP-001-evaluate-auth-providers.md` | 1st spike |
| `CH-001-upgrade-dotnet-dependencies.md` | 1st chore |

### Why Per-Type Numbering

- The number carries meaning: "US-011" means "the 11th user story,"
  not just "the 11th thing we created."
- When the first spike arrives, it starts fresh at SP-001 instead of
  continuing from whatever the global counter reached.
- TYPE + NNN is globally unique because the prefix differentiates
  (US-001 ≠ SP-001 ≠ CH-001).
- The scaffold tool auto-increments by scanning files matching the
  requested type prefix — trivial to implement.

## Scope

| Deliverable | Path |
|-------------|------|
| This product story | `product/stories/docs/US-012-story-naming-convention.md` |
| Rename all existing stories | `product/stories/infra/`, `product/stories/docs/` |
| ADR documenting the decision | `docs/adr/0009-story-naming-convention.md` |

## Acceptance Criteria

### Convention Definition

- [x] The naming pattern `{TYPE}-{NNN}-{kebab-name}.md` is documented
      in an ADR (0009) with status Accepted.
- [x] The ADR defines exactly three type prefixes: `US` (user story),
      `SP` (spike), `CH` (chore).
- [x] The ADR specifies that numbering is three-digit, zero-padded,
      sequential per type, and global across all domain directories.

### Retroactive Rename

- [x] Every existing product story under `product/stories/` is
      renamed to follow the convention.
- [x] The rename mapping is:

      | Current Name | New Name |
      |---|---|
      | `infrastructure-services.md` | `US-001-infrastructure-services.md` |
      | `containerized-dev-environment.md` | `US-002-containerized-dev-environment.md` |
      | `consistent-editor-experience.md` | `US-003-consistent-editor-experience.md` |
      | `standardized-developer-commands.md` | `US-004-standardized-developer-commands.md` |
      | `one-command-local-bootstrap.md` | `US-005-one-command-local-bootstrap.md` |
      | `story-scaffold-tool.md` | `US-006-story-scaffold-tool.md` |
      | `planning-artifact-validation.md` | `US-007-planning-artifact-validation.md` |
      | `documentation-audit-and-defragmentation.md` | `US-008-doc-audit-and-defragmentation.md` |
      | `developer-contributor-guide.md` | `US-009-developer-contributor-guide.md` |
      | `product-owner-contributor-guide.md` | `US-010-product-owner-contributor-guide.md` |
      | `scrum-master-contributor-guide.md` | `US-011-scrum-master-contributor-guide.md` |

- [x] No story file exists under `product/stories/` that does not
      match the `{TYPE}-{NNN}-{kebab-name}.md` pattern (excluding
      `_template.md`).

### Scaffold Tool Alignment

- [x] The scaffold tool story (US-006) is updated to include a
      `--type` flag accepting `US`, `SP`, or `CH` (default: `US`).
- [x] The scaffold tool story specifies that the tool scans all
      domain directories to find the highest existing number for
      the requested type before assigning the next.

### Cross-References

- [x] Any internal references to renamed story files (in other
      stories, ADRs, or documentation) are updated to use the
      new file names.
- [x] The ADR index (`docs/adr/README.md`) includes ADR-0009.

## Dependencies

- Established 7-section story format from Sprint 2.
- Doc audit and defragmentation story (US-008) — renames are
  coordinated with the broader cleanup.
- Scaffold tool story (US-006) — convention informs the tool's
  `--type` flag behavior.

## Open Questions

- Should the convention allow additional type prefixes beyond
  US/SP/CH in the future (e.g., BG for bug)? If so, should the
  ADR define an extension mechanism?
- Should the `_template.md` file also carry a naming prefix, or
  remain exempt as a meta-artifact?
- Should the numbering sequence account for deleted or archived
  stories (i.e., never reuse a number), or allow gaps?

## Decision Record

`docs/adr/0009-story-naming-convention.md` — to be created as part
of this story's implementation.