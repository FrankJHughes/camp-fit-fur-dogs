# Documentation Conventions

Documentation is a first‑class artifact in the Camp Fit Fur Dogs system.  
It defines how architecture, workflow, code, and processes are communicated, maintained, and evolved.  
All contributors must treat documentation with the same rigor as code.

---

# Purpose

Documentation exists to:

- Make decisions explicit  
- Reduce ambiguity  
- Keep the team aligned  
- Onboard new contributors quickly  
- Preserve architectural and workflow intent  
- Prevent drift between code and conventions  

Documentation is part of the product and must always reflect **current behavior**, not historical behavior.

---

# Canonical Structure

The repository uses four canonical convention documents. These are the single source of truth for repository rules and must remain synchronized:

- **Architecture** — layers, boundaries, hosting model, PR Preview architecture, core building blocks  
- **Workflow** — CI/CD structure, composite actions, PR Preview lifecycle, script‑first rules  
- **Code** — backend, frontend, CQRS, endpoints, EF Core, SharedKernel usage  
- **Docs** — documentation rules, patching rules, fencing rules, ownership  

Other documents (ADRs, guides, READMEs) may reference these conventions but must not redefine them.

**Location**

Canonical files live under:

```
docs/conventions/
  architecture.md
  workflow.md
  code.md
  docs.md   ← this file
```

---

# ADRs (Architecture Decision Records)

Significant architectural decisions must be captured as ADRs.

## Requirements

Each ADR must:

- Describe the **context**  
- State the **decision** clearly  
- Explain **consequences** (positive and negative)  
- Reference relevant conventions and guardrails  
- Clarify interactions with SharedKernel and other systems  

## Placement

- ADRs may live next to the code they affect or in `docs/adr/`  
- ADRs must be updated or superseded when decisions evolve  

## Style

- Concise and factual  
- Use the ADR template: Status, Date, Context, Decision, Rationale, Consequences, Alternatives, Notes  

---

# Documentation Lifecycle Rules

Documentation must evolve with the system.

- PRs that change behavior, architecture, or workflow **must** update documentation in the same PR  
- Outdated documentation must be corrected immediately  
- Breaking changes must be reflected in both code and docs  
- Conventions must remain internally consistent across the four canonical files  
- Documentation changes must follow the **Universal Patch Rule**  
- Documentation is part of the definition of done  

---

# Universal Patch Rule

When updating a documentation file, regenerate and return the **entire file** with the patch already applied.

- No partial edits  
- No diffs  
- No search‑and‑replace instructions  

## Rationale

- Prevents corruption  
- Eliminates ambiguity  
- Ensures deterministic updates  
- Aligns with script‑first and debugging discipline  

---

# Style and Tone

Documentation must be:

- Direct  
- Technical  
- Implementation‑ready  
- Free of fluff  
- Free of narrative or opinionated language  

## Guidelines

- Use active voice and present tense  
- Prefer short paragraphs and bullet lists  
- Use examples sparingly and ensure they are copy‑pasteable  
- Avoid humor, speculation, or historical commentary  

---

# Fencing, Quoting, and Script‑First Rules

Documentation must be compatible with script‑first automation and safe for programmatic consumption.

## General Rules

- Prefer PowerShell examples when a shell is required  
- Generated files must use `utf8NoBOM` encoding  
- Scripts must be copy‑pasteable without manual fixes  

## Fencing and Quoting

- Avoid nested triple‑backtick fences  
- When inner fenced blocks are required, wrap the entire document with **four backticks**  
- PowerShell here‑strings for PR bodies must be **single‑quoted**  
- Avoid literal double‑quote characters inside here‑strings  
- Avoid the sequences `@'` and `'@` inside here‑strings  
- Avoid backticks inside here‑strings  
- Avoid nested here‑strings  
- When syntax is unsafe to show literally, describe it in prose  

## File Generation

- Use single‑quoted here‑strings for file generation  
- Avoid fenced code blocks inside here‑strings  
- Prefer script‑driven, idempotent file generation  

---

# Safe Examples

Examples must be:

- Minimal  
- Copy‑pasteable  
- Free of problematic quoting or fencing  
- Sanitized when necessary  

If an example cannot be shown safely, describe the pattern in prose.

---

# Navigation and Ownership

## Navigation

- `docs/conventions/` is the canonical hub  
- Each file must have clear headings and stable anchors  
- Cross‑references must be explicit and correct  

## Ownership

- **Architecture & SharedKernel conventions** — platform/architecture maintainers  
- **Workflow & tooling conventions** — automation/build maintainers  
- **Code conventions** — backend, frontend, and infra maintainers  
- **Docs conventions** — shared responsibility with a designated maintainer  

Ownership must be explicit in the file header.

---

# Updating Conventions

When updating conventions:

- Follow the **Universal Patch Rule**  
- Keep changes cohesive and well‑scoped  
- Explain rationale in the PR description  
- Ensure examples remain valid under fencing and script‑first rules  
- Update all four canonical documents if the change affects multiple areas  
- Ensure guardrail tests remain aligned with the updated conventions  

---

# Relationship to SharedKernel

SharedKernel is the authoritative source for cross‑cutting behavior.

Documentation must:

- Indicate when a rule is enforced by SharedKernel types or helpers  
- Encourage product code to use SharedKernel instead of duplicating patterns  
- Highlight SharedKernel as the canonical home for:
  - CQRS abstractions  
  - Domain primitives  
  - Endpoint discovery  
  - DI conventions  
  - EF Core base classes  
  - Guardrail enforcement  

SharedKernel and documentation must remain synchronized.

---

# PR Preview Documentation Requirements

Because the system uses Neon + Render PR Previews, documentation must:

- Describe the preview lifecycle (Neon branch creation → migrations → infra tests → Render preview deployment → API tests)  
- Document preview‑safe coding rules  
- Clarify environment variable expectations (`PREVIEW_DB_CONNECTION_STRING`, `ConnectionStrings__DefaultConnection`)  
- Ensure examples reflect Git‑backed Render PR Previews and label‑driven deployment  
- Avoid references to image‑backed or manually triggered preview flows  

## Minimum Required Content

Documentation must include:

- Exact workflow sequence:  
  **label removal → teardown probe → DB provisioning → migrations → infra tests → label add → readiness probe → API tests**  
- Probe endpoints and thresholds:  
  - Teardown: `/health` → `404` ×3  
  - Startup: `/api/dogs` → `200,400,401` ×3  
  - Timeout: 300s  
  - Poll interval: 5s  
- Artifact handling (`db-conn.txt`) and sensitivity guidance  
- Secrets handling (GitHub Secrets, Render environment)  
- How to reproduce locally (recommended commands and environment variables)  

---

# ADR and Docs Cross‑References

- ADRs must reference the canonical docs they affect  
- When an ADR changes a convention, update the relevant canonical document(s) in the same PR  
- Maintain a changelog entry for documentation changes that affect developer workflows  

---

# Accessibility and Discoverability

- Use clear headings and short paragraphs  
- Provide a short summary at the top of each convention file  
- Include a “Quick Reference” section where appropriate  
- Keep examples minimal and link to full examples in `docs/examples/`  

---

# Enforcement and Tests

- Guardrail tests validate layering, dependency rules, and other enforceable conventions  
- Documentation must describe which guardrail tests exist and where they run  
- When a convention is enforced by tests, document the test name and location  

---

# Summary

Documentation ensures:

- Architectural clarity  
- Workflow consistency  
- Coding discipline  
- Preview‑safe behavior  
- SharedKernel alignment  
- Long‑term maintainability  

All contributors must follow these conventions and keep documentation accurate, complete, and synchronized with the system.
