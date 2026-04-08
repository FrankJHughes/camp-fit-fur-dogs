# ADR-0009: Story Naming Convention

| Field     | Value              |
|-----------|--------------------|
| Status    | Accepted           |
| Date      | 2026-04-08         |
| Deciders  | Frank Hughes       |

## Context

By the end of Sprint 2, the Camp Fit Fur Dogs repository contained
44 product stories spread across three competing naming patterns:

- **Bare kebab-case** (Sprint 0-2 infra stories):
  `ci-baseline-build-and-test.md`
- **US-C{NN} prefix** (customer stories):
  `US-C01-create-customer-account.md`
- **US-{NNN} prefix** (PR #78 infra renames):
  `US-002-containerized-dev-environment.md`

Planning YAMLs, sprint manifests, and GitHub issue titles each used
their own variation. The inconsistency made it impossible to:

- Search for a story by its ID across systems (files, issues, board)
- Validate cross-references programmatically
- Onboard new contributors without explaining which pattern applied
  where

The project needed a single, enforced convention before the backlog
could grow further.

## Decision

All product stories, planning YAMLs, and GitHub issue titles follow
a unified naming convention:

**Product stories:** `US-{NNN}-{kebab-name}.md`
- `US-` prefix (constant)
- Three-digit, zero-padded sequence number
- Hyphen separator
- Kebab-case descriptive name
- Example: `US-005-one-command-local-bootstrap.md`

**Planning YAMLs:** `US-{NNN}-{kebab-name}.yml`
- Same pattern, `.yml` extension (not `.yaml`)

**GitHub issue titles:** `US-{NNN}: Title`
- Example: `US-005: One-Command Local Bootstrap`

**Numbering rules:**
- US-001 is permanently retired (phantom entry from PR #78 — source
  file never existed). It is reserved as a gap and will not be
  backfilled.
- US-002 through US-026 are infra/docs stories.
- US-027 through US-044 are customer stories (renumbered from the
  former `US-C01` through `US-C18` sequence).
- The next available number is US-045.
- Numbers are never reused after retirement or deletion.

**Enforcement:**
- The scaffold tool (US-006) will generate files with the correct
  prefix automatically.
- The CI validation gate (US-007) will reject files that do not
  match the pattern.

## Consequences

### Positive

- Every story has a globally unique, grep-friendly identifier that
  works across files, issues, and the project board.
- Cross-reference validation can be fully automated — broken links
  are caught before merge.
- Onboarding friction is eliminated: one pattern, documented in one
  place.
- Customer and infra stories share a single contiguous sequence,
  removing the cognitive overhead of parallel numbering schemes.

### Negative

- The three-digit format caps the sequence at 999 stories. This is
  unlikely to be a constraint for this project, but a future ADR
  could extend to four digits if needed.
- The migration (PR #79) touched 76 files in a single commit,
  creating a large diff that complicates `git blame` for those
  files. Use `git log --follow` to trace pre-rename history.

### Neutral

- The US-001 gap is a permanent historical artifact. It carries no
  functional cost but may prompt questions from new contributors.
  This ADR serves as the canonical explanation.
- Planning YAMLs retain their own filenames (not shared with product
  stories) — they are parallel artifacts, not duplicates.
