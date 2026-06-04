# Copilot Instructions

This file provides top‑level guidance for how Copilot must behave in this repository.
All detailed conventions live in `docs/conventions`.
All governance documents live in `docs/governance`.

Copilot must always follow the repository’s
**[conventions](ca://s?q=Show_repository_conventions)**,
**[governance](ca://s?q=Show_governance_rules)**, and
**[guardrails](ca://s?q=Show_guardrail_tests)**.

Copilot must also respect the governance enforcement system:

- **[Governance Enforcement Checklist](ca://s?q=Open_governance_enforcement_checklist)**
- **[Governance Enforcement Matrix](ca://s?q=Open_governance_enforcement_matrix)**

These documents define *how governance is enforced* across roles, CI, scripts, and Frank.

---

# Purpose

Copilot must follow the established architecture, workflow, coding, documentation, and governance rules defined in this repository.
This file defines how Copilot interprets and applies those rules, how it generates files, and how it avoids corruption or drift.

Copilot’s behavior must always align with:

- the **[architectural boundaries](ca://s?q=Explain_architecture_boundaries)**
- the **[workflow and deployment model](ca://s?q=Explain_workflow_and_deployment_model)**
- the **[code and documentation conventions](ca://s?q=Explain_code_and_docs_conventions)**
- the **[script‑first and patch‑first rules](ca://s?q=Explain_script_first_and_patch_first_rules)**
- the **[guardrail tests](ca://s?q=List_guardrail_tests)** that enforce repository expectations
- the **[governance rules](ca://s?q=Explain_governance_rules)** that define process, responsibilities, and enforcement
- the governance enforcement system (checklist + matrix)

---

# Conventions Index (Canonical)

All conventions are defined in the following files:

- `docs/conventions/architecture.md`
- `docs/conventions/workflow.md`
- `docs/conventions/code.md`
- `docs/conventions/docs.md`

These four files are the **single source of truth** for all repository conventions.
Copilot must treat them as canonical.

---

# Governance Index (Reference Only)

Governance defines **process**, **responsibilities**, **boundaries**, and **enforcement**.
Governance overrides conventions when they conflict.

All governance documents live in:

```
docs/governance/product/
docs/governance/technical/
docs/governance/enforcement/
```

## Product Governance
- `docs/governance/product/story-governance.md`
- `docs/governance/product/changelog-governance.md`
- `docs/governance/product/repo-hygiene.md`
- `docs/governance/product/multi-product-governance.md`
- `docs/governance/product/contributor-governance.md`

## Technical Governance
- `docs/governance/technical/api-governance.md`
- `docs/governance/technical/architecture-governance.md`
- `docs/governance/technical/ci-governance.md`
- `docs/governance/technical/security-governance.md`
- `docs/governance/technical/operations-governance.md`

## Enforcement Governance
- `docs/governance/enforcement/governance-enforcement-checklist.md`
- `docs/governance/enforcement/governance-enforcement-matrix.md`
- `docs/governance/enforcement/governance-process.md`

Copilot must respect governance rules but must not reinterpret or rewrite them unless explicitly instructed.

---

# How Copilot Must Use the Conventions

- Copilot must always consult the conventions files before generating or modifying code, documentation, or scripts.
- Copilot must not invent new patterns or workflows that contradict the conventions.
- When conventions appear to conflict, Copilot must ask for clarification rather than guessing.
- When generating files, Copilot must follow the rules defined in the conventions, including:
  - **[script‑first rules](ca://s?q=Explain_script_first_rules)**
  - **[quoting and fencing rules](ca://s?q=Explain_fencing_rules)**
  - **[workflow rules](ca://s?q=Explain_workflow_rules)**
  - **[architectural boundaries](ca://s?q=Explain_architecture_boundaries)**
  - **[TDD expectations](ca://s?q=Explain_TDD_expectations)**

Additional requirements:

- Guardrail tests exist to ensure Copilot aligns with established rules.
- Copilot must not modify governance files unless explicitly instructed.
- Copilot must respect the **Governance Enforcement Checklist** and **Governance Enforcement Matrix** when generating or validating content.

---

# Hosting & Deployment Guidance

Copilot must follow the hosting and deployment conventions defined in:

- `docs/conventions/architecture.md` (Hosting & Deployment Architecture)
- `docs/conventions/workflow.md` (API Deployment Workflow and PR Preview Workflow)
- `docs/guides/developer/api-hosting.md` (Developer API Hosting Guide)

These documents define:

- API hosting platform: **Render**
- API service name: **`campfitfurdogsapi`**
- Database hosting platform: **Neon**
- PR Preview model: Git‑backed Render PR Previews + Neon ephemeral branches
- Required environment variables:
  - `ConnectionStrings__DefaultConnection`
  - `PREVIEW_DB_CONNECTION_STRING`
- Health check conventions (`/api/health`, `/api/dogs`)
- Deployment triggers and expectations

Copilot must not propose alternative hosting platforms or deployment models unless explicitly requested.

---

# File Generation Rules

Copilot must:

- Regenerate **entire files** when applying patches (**[Universal Patch Rule](ca://s?q=Explain_Universal_Patch_Rule)**)
- Use script‑first generation for any file creation or updates
- Respect quoting, fencing, and here‑string safety rules
- Avoid partial edits, inline diffs, or search‑and‑replace instructions
- Ensure generated files use `utf8NoBOM` encoding

These rules prevent corruption, drift, and ambiguity.

---

# Lessons Learned

| Number | Sprint | Lesson | Mitigation |
|--------|--------|--------|------------|
| 1 | 3 | Accidental direct push to main | Added branch protection rule and local pre‑push hook |
| 2 | 3 | `git update-index --chmod` fails on hooks directory | Moved hooks into tracked directory with `core.hooksPath` |
| 3 | 3 | Changelog used PR numbers instead of issue numbers | Always use issue numbers |
| 4 | 3 | `gh pr create` body bypasses PR template | Embed merge checklist manually |
| 5 | 4 | CRLF line endings broke pre‑push hook | Added `.gitattributes` LF enforcement |
| 6 | 4 | Double‑quoted here‑strings corrupted backticks | Use single‑quoted here‑strings |
| 7 | 4 | Makefile targets only covered backend | Added stack‑scoped naming conventions |
| 8 | 5 | Version drift across csproj files | Introduced Central Package Management |
| 9 | 5 | Guardrail tests mixed reflection + DI | Split into Architecture.Tests + Api.Tests/Guardrails |
| 10 | 6 | Documentation duplicated | Added canonical ownership map |
| 11 | 7 | Quote characters inside here‑strings caused corruption | Added quote‑safety rules |
| 12 | 8 | Manual copy‑paste corrupted generated files | Added script‑first rule |
| 13 | 9 | DI tests required separate assembly | Added debugging discipline rule |
| 14 | 10 | Partial file edits caused drift | Added Universal Patch Rule |
| 15 | 6 | PowerShell treated `[id]` as wildcard | Use `-LiteralPath` |
| 16 | 6 | PR body bypassed template | Always follow PR template |
| 17 | 6 | Missing changelog entry | Add under `[Unreleased]` |
| 18 | 11 | Shared frontend files placed under `components/shared/` | Use `lib/components/` instead |
| 19 | 11 | Vitest config matched wrong test dirs | Updated include/exclude patterns |
| 20 | 11 | `.Replace()` matched all headings | Use `.IndexOf()` + `.Insert()` |
| 21 | 11 | `gh pr create --body` bypassed template | Load template manually |
| 22 | 12 | Renaming Render service broke previews | Never rename Git‑backed preview services |
| 23 | 12 | Enabling previews after creation broke linkage | Enable previews at creation time |
| 24 | 12 | Workflow scraped GitHub Checks for URLs | Use deterministic preview URL patterns |
| 25 | 12 | Legacy image‑backed preview logic conflicted | Remove all image‑backed preview logic |
| 26 | 12 | Manual Render API calls conflicted | Never manually create preview instances |
| 27 | 12 | Preview URL detection failed on reopened PRs | Compute URLs deterministically |
| 28 | 12 | Misunderstanding of Render env var injection | Document Render’s environment variable rules |
| 29 | 13 | Preview callback URLs drifted between Auth0 and Render | Add script‑first generator for callback URL matrix |
| 30 | 13 | Missing Auth0 secrets caused silent 500s in preview | Add CI guardrail to fail preview deploy if secrets missing |
| 31 | 13 | Cookie flags differed between local and preview | Document cookie flag matrix + add guardrail tests |
| 32 | 13 | Form validation tests drifted from schema messages | Require tests to import messages from schema/validator |
| 33 | 13 | Preview teardown failed on reopened PRs | Always destroy previews before provisioning |
| 34 | 13 | Neon branch not ready before migrations | Add Neon readiness probe before migrations |
| 35 | 13 | Vitest config drifted from folder structure | Add guardrail asserting test dirs match Vitest globs |
| 36 | 13 | Authentication tests reused stale cookies | Add shared helper to clear cookies between tests |

