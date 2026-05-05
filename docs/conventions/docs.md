# Documentation Conventions

Documentation is a first‑class artifact in the Camp Fit Fur Dogs system.  
It defines how architecture, workflow, code, and processes are communicated, maintained, and evolved.  
All contributors must treat documentation with the same rigor as code.

---

## Purpose

Documentation exists to:

- Make decisions explicit.  
- Reduce ambiguity.  
- Keep the team aligned.  
- Onboard new contributors quickly.  
- Preserve architectural and workflow intent.  
- Prevent drift between code and conventions.

Docs are part of the product and must be kept up to date.

---

## Canonical Structure

The repository uses four canonical convention documents. These are the single source of truth for repository rules and must be kept synchronized.

- **Architecture** — layers, boundaries, hosting, PR Preview model, core building blocks.  
- **Workflow** — how work is done (TDD, debugging, patching, script‑first, PR Preview lifecycle).  
- **Code** — backend, frontend, CQRS, endpoint, and SharedKernel coding rules.  
- **Docs** — how documentation itself is structured and maintained.

Other documents (ADRs, guides, READMEs) may reference these conventions but must not redefine them.

Location:

- Canonical files live under `docs/conventions/` with stable filenames and anchors.

---

## ADRs (Architecture Decision Records)

Significant architectural decisions must be captured as ADRs.

**Each ADR must:**

- Describe the **context**.  
- State the **decision** clearly.  
- Explain **consequences** (positive and negative).  
- Reference relevant conventions and guardrails.  
- Clarify interactions with SharedKernel and other systems.

**Placement**

- ADRs may live next to the code they affect or in `docs/adr/`.  
- ADRs must be updated or superseded when decisions evolve.

**Style**

- Keep ADRs concise and factual.  
- Use the ADR template: Status, Date, Context, Decision, Rationale, Consequences, Alternatives, Notes.

---

## Documentation Lifecycle Rules

Documentation must evolve with the system.

- PRs that change behavior, architecture, or workflow **must** update documentation in the same PR.  
- Outdated documentation must be corrected immediately.  
- Breaking changes must be reflected in both code and docs.  
- Conventions must remain internally consistent across the four canonical files.  
- Documentation changes must follow the **Universal Patch Rule** (full‑file regeneration).  
- Docs are part of the definition of done.

---

## Universal Patch Rule

When a patch is required, regenerate and return the **entire file** with the patch already applied.

- No partial edits.  
- No diffs.  
- No search‑and‑replace instructions.

**Rationale**

- Prevents corruption.  
- Eliminates ambiguity.  
- Aligns with script‑first and debugging discipline.

---

## Style and Tone

- Write for clarity and future readers.  
- Use active voice and present tense for conventions.  
- Prefer short paragraphs and bullet lists.  
- Use examples sparingly and ensure they are copy‑pasteable.  
- Avoid opinionated language; document facts, rationale, and tradeoffs.

---

## Fencing, Quoting, and Script‑First Rules

Documentation must be compatible with script‑first automation and safe for programmatic consumption.

**General rules**

- Prefer PowerShell examples when a shell is required.  
- Generated files must use `utf8NoBOM` encoding.  
- Scripts must be copy‑pasteable without manual fixes.

**Fencing and quoting**

- Avoid nested triple‑backtick fences in Markdown.  
- When inner fenced blocks are required, wrap the whole document with **four backticks**.  
- PowerShell here‑strings for PR bodies must be **single‑quoted**.  
- Avoid literal double‑quote characters inside here‑strings.  
- Avoid the sequences `@'` and `'@` inside here‑strings.  
- Avoid backticks inside here‑strings.  
- Avoid nested here‑strings.  
- When documentation requires showing tricky syntax, describe it in words rather than using literal characters.

**File generation**

- When generating files via scripts, write content using single‑quoted here‑strings.  
- Avoid fenced code blocks inside here‑strings.  
- Prefer script‑driven, idempotent file generation.

---

## Safe Examples

- Provide minimal, copy‑pasteable examples.  
- Do not include examples that violate fencing or here‑string rules.  
- If an example would require problematic characters, explain the pattern in prose and provide a sanitized snippet.

---

## Navigation and Ownership

**Navigation**

- `docs/conventions/` is the canonical hub.  
- Each file must have clear headings and stable anchors.  
- Cross‑references must be explicit and correct.

**Ownership**

- **Architecture & SharedKernel conventions** — platform/architecture maintainers.  
- **Workflow & tooling conventions** — automation/build maintainers.  
- **Code conventions** — backend, frontend, and infra maintainers.  
- **Docs conventions** — shared responsibility with a clear maintainer.

Ownership must be explicit in the file header.

---

## Updating Conventions

When updating conventions:

- Follow the **Universal Patch Rule** (full‑file regeneration).  
- Keep changes cohesive and well‑scoped.  
- Explain rationale in the PR description.  
- Ensure examples remain valid under fencing and script‑first rules.  
- Update all four canonical documents if the change affects multiple areas.  
- Ensure guardrail tests remain aligned with the updated conventions.

---

## Relationship to SharedKernel

SharedKernel is the authoritative source for cross‑cutting behavior.

Documentation must:

- Indicate when a rule is enforced by SharedKernel types or helpers.  
- Encourage product code to use SharedKernel instead of duplicating patterns.  
- Highlight SharedKernel as the canonical home for:
  - CQRS abstractions  
  - Domain primitives  
  - Endpoint discovery  
  - DI conventions  
  - EF Core base classes  
  - Guardrail enforcement

SharedKernel and documentation must remain synchronized.

---

## PR Preview Documentation Requirements

Because the system uses Neon + Render PR Previews, documentation must:

- Describe the preview lifecycle (Neon branch creation, migrations, Render preview deployment).  
- Document preview‑safe coding rules.  
- Clarify environment variable expectations (`PREVIEW_DB_CONNECTION_STRING`, `ConnectionStrings__DefaultConnection`).  
- Ensure all examples reflect Git‑backed Render PR Previews and label‑driven deployment.  
- Avoid references to image‑backed or manually triggered preview flows.

**Minimum content for preview docs**

- Exact workflow sequence (label removal → teardown probe → DB provisioning → migrations → infra tests → label add → readiness probe → API tests).  
- Probe endpoints and thresholds (teardown `/health` → `404` ×3; startup `/api/dogs` → `200,400,401` ×3; timeout 300s; poll 5s).  
- Artifact handling (db-conn.txt) and sensitivity guidance.  
- Secrets handling and where to find them (GitHub Secrets, Render environment).  
- How to reproduce locally (recommended local commands and environment variables).

---

## ADR and Docs Cross‑References

- ADRs must reference the canonical docs they affect.  
- When an ADR changes a convention, update the relevant canonical document(s) in the same PR.  
- Keep a changelog entry for documentation changes that affect developer workflows.

---

## Accessibility and Discoverability

- Use clear headings and short paragraphs.  
- Provide a short summary at the top of each convention file.  
- Include a "Quick Reference" section for common tasks (e.g., how to run previews locally, how to add the render label).  
- Keep examples minimal and link to full examples in a `docs/examples/` folder.

---

## Enforcement and Tests

- Guardrail tests should validate layering, dependency rules, and other enforceable conventions.  
- Documentation must include a section describing which guardrail tests exist and where they run.  
- When a convention is enforced by tests, document the test name and location.

---

## Summary

Documentation ensures:

- Architectural clarity.  
- Workflow consistency.  
- Coding discipline.  
- Preview‑safe behavior.  
- SharedKernel alignment.  
- Long‑term maintainability.

All contributors must follow these conventions and keep documentation accurate, complete, and synchronized with the system.
