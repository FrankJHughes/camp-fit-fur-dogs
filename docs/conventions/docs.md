# Documentation Conventions

Documentation is a first‑class artifact in the Camp Fit Fur Dogs system.  
It defines how architecture, workflow, code, and processes are communicated, maintained, and evolved.  
All contributors must treat documentation with the same rigor as code.

Documentation describes **how information is structured, maintained, and authored** — not governance, not architecture, and not implementation details.

---

# 1. Purpose

Documentation exists to:

- Make decisions explicit  
- Reduce ambiguity  
- Keep the team aligned  
- Onboard new contributors quickly  
- Preserve architectural and workflow intent  
- Prevent drift between code and conventions  

Documentation must always reflect **current behavior**, not historical behavior.

Documentation must never redefine governance.  
Governance lives in:

- **Architecture Governance**  
- **API Governance**  
- **Security Governance**  
- **Operations Governance**

---

# 2. Canonical Structure

The repository uses four canonical convention documents. These are the single source of truth for repository rules and must remain synchronized:

- **Architecture** — layering, boundaries, hosting model, PR Preview architecture, cross‑cutting primitives, Configurator Engine lifecycle  
- **Workflow** — CI/CD structure, composite actions, PR Preview lifecycle, script‑first rules  
- **Code** — backend, frontend, CQRS, endpoints, EF Core, Frank usage, DI usage, Configurator patterns  
- **Docs** — documentation rules, patching rules, fencing rules, ownership  

Other documents (ADRs, guides, READMEs) may reference these conventions but must not redefine them.

## 2.1 Location

```
docs/conventions/
  architecture.md
  workflow.md
  code.md
  docs.md   ← this file
```

## 2.2 Allowed Document Types

Documentation may take the following forms:

- **Conventions** — the four canonical files  
- **Governance** — rules and boundaries  
- **Guides** — how‑to documents  
- **ADRs** — architectural decisions  
- **Reference docs** — slice‑specific or feature‑specific explanations  
- **Examples** — safe, minimal patterns  

Documentation must not introduce new categories.

---

# 3. Governance vs. Conventions vs. Guides

Documentation must respect the system’s separation of concerns.

## 3.1 Governance (What must be true)

Governance defines:

- Boundaries  
- Responsibilities  
- Enforcement  
- Stability guarantees  
- Security posture  
- Operational rules  

Governance documents must **never** contain implementation details.

## 3.2 Conventions (How we write code)

Conventions define:

- Coding patterns  
- Architectural implementation  
- CQRS usage  
- Endpoint structure  
- Form architecture  
- Test seams  
- Folder structure  
- Dependency injection usage (Frank auto‑registration)  
- Configurator Engine usage and ordering  
- Hosting provider integration rules  
- Middleware ordering rules  

Conventions must **never** redefine governance.

## 3.3 Guides (How to do things)

Guides define:

- How to run the system locally  
- How to configure Auth0  
- How to debug hosting providers  
- How to test endpoints  
- How to run migrations  
- How Frank auto‑registration works (reference only; rules live in conventions)  
- How to write configurators (reference only; rules live in conventions)  

Guides must **never** define rules or boundaries.

---

# 4. ADRs (Architecture Decision Records)

Significant architectural decisions must be captured as ADRs.

## 4.1 Requirements

Each ADR must:

- Describe the **context**  
- State the **decision** clearly  
- Explain **consequences** (positive and negative)  
- Reference relevant conventions and guardrails  
- Clarify interactions with Frank and other systems  

## 4.2 Placement

- ADRs may live next to the code they affect or in `docs/adr/`  
- ADRs must be updated or superseded when decisions evolve  

## 4.3 Style

- Concise and factual  
- Use the ADR template: Status, Date, Context, Decision, Rationale, Consequences, Alternatives, Notes  

---

# 5. Documentation Lifecycle Rules

Documentation must evolve with the system.

- PRs that change behavior, architecture, or workflow **must** update documentation in the same PR  
- Outdated documentation must be corrected immediately  
- Breaking changes must be reflected in both code and docs  
- Conventions must remain internally consistent across the four canonical files  
- Documentation changes must follow the **Universal Patch Rule**  
- Documentation is part of the definition of done  

---

# 6. Universal Patch Rule

When updating a documentation file, regenerate and return the **entire file** with the patch already applied.

- No partial edits  
- No diffs  
- No search‑and‑replace instructions  

## 6.1 Rationale

- Prevents corruption  
- Eliminates ambiguity  
- Ensures deterministic updates  
- Aligns with script‑first and debugging discipline  

---

# 7. Style and Tone

Documentation must be:

- Direct  
- Technical  
- Implementation‑ready  
- Free of fluff  
- Free of narrative or opinionated language  

## 7.1 Guidelines

- Use active voice and present tense  
- Prefer short paragraphs and bullet lists  
- Use examples sparingly and ensure they are copy‑pasteable  
- Avoid humor, speculation, or historical commentary  

---

# 8. Fencing, Quoting, and Script‑First Rules

Documentation must be compatible with script‑first automation and safe for programmatic consumption.

## 8.1 General Rules

- Prefer PowerShell examples when a shell is required  
- Generated files must use `utf8NoBOM` encoding  
- Scripts must be copy‑pasteable without manual fixes  

## 8.2 Fencing and Quoting

- Avoid nested triple‑backtick fences  
- When inner fenced blocks are required, wrap the entire document with **four or more backticks**  
- PowerShell here‑strings for PR bodies must be **single‑quoted**  
- Avoid literal double‑quote characters inside here‑strings  
- Avoid the sequences `@'` and `'@` inside here‑strings  
- Avoid backticks inside here‑strings  
- Avoid nested here‑strings  
- When syntax is unsafe to show literally, describe it in prose  

## 8.3 File Generation

- Use single‑quoted here‑strings for file generation  
- Avoid fenced code blocks inside here‑strings  
- Prefer script‑driven, idempotent file generation  

---

# 9. Safe Examples

Examples must be:

- Minimal  
- Copy‑pasteable  
- Free of problematic quoting or fencing  
- Sanitized when necessary  

If an example cannot be shown safely, describe the pattern in prose.

---

# 10. Navigation and Ownership

## 10.1 Navigation

- `docs/conventions/` is the canonical hub  
- Each file must have clear headings and stable anchors  
- Cross‑references must be explicit and correct  
- Guides must link back to conventions when referencing rules  

## 10.2 Ownership

- **Architecture & Frank conventions** — platform/architecture maintainers  
- **Workflow & tooling conventions** — automation/build maintainers  
- **Code conventions** — backend, frontend, and infra maintainers  
- **Docs conventions** — shared responsibility with a designated maintainer  

Ownership must be explicit in the file header.

---

# 11. Updating Conventions

When updating conventions:

- Follow the **Universal Patch Rule**  
- Keep changes cohesive and well‑scoped  
- Explain rationale in the PR description  
- Ensure examples remain valid under fencing and script‑first rules  
- Update all four canonical documents if the change affects multiple areas  
- Ensure guardrail tests remain aligned with the updated conventions  

---

# 12. Relationship to Frank

Frank is the authoritative source for cross‑cutting behavior.

Documentation must:

- Indicate when a rule is enforced by Frank types or helpers  
- Encourage product code to use Frank instead of duplicating patterns  
- Highlight Frank as the canonical home for:
  - CQRS abstractions  
  - Domain primitives  
  - Endpoint discovery  
  - **DI auto‑registration engine**  
  - **Validator scanning**  
  - **Security headers middleware**  
  - **Hosting provider abstractions**  
  - **Environment abstraction**  
  - **GitHub artifact client abstraction**  
  - **PR parser abstraction**  
  - **Configuration writer abstraction**  
  - **Configurator Engine primitives**  
  - Guardrail enforcement  
  - Test seams  

Frank and documentation must remain synchronized.

---

# 13. PR Preview Documentation Requirements

Because the system uses Render PR Previews, documentation must:

- Describe the preview lifecycle  
- Document preview‑safe coding rules  
- Clarify environment variable expectations  
- Document hosting provider behavior  
- Document artifact naming conventions  
- Avoid references to image‑backed or manually triggered preview flows  

## 13.1 Minimum Required Content

Documentation must include:

- Exact workflow sequence:  
  **label removal → teardown probe → DB provisioning → migrations → infra tests → label add → readiness probe → API tests**  
- Probe endpoints and thresholds:  
  - Teardown: `/health` → `404` ×3  
  - Startup: `/api/dogs` → `200,400,401` ×3  
  - Timeout: 300s  
  - Poll interval: 5s  
- Artifact handling (`db-conn.txt`, `frontend-url.txt`)  
- Secrets handling (GitHub Secrets, Render environment)  
- How to reproduce locally (recommended commands and environment variables)  

---

# 14. ADR and Docs Cross‑References

- ADRs must reference the canonical docs they affect  
- When an ADR changes a convention, update the relevant canonical document(s) in the same PR  
- Maintain a changelog entry for documentation changes that affect developer workflows  

---

# 15. Accessibility and Discoverability

- Use clear headings and short paragraphs  
- Provide a short summary at the top of each convention file  
- Include a “Quick Reference” section where appropriate  
- Keep examples minimal and link to full examples in `docs/examples/`  

---

# 16. Enforcement and Tests

- Guardrail tests validate layering, dependency rules, hosting provider behavior, Configurator Engine ordering, and security headers  
- Documentation must describe which guardrail tests exist and where they run  
- When a convention is enforced by tests, document the test name and location  

---

# Summary

Documentation ensures:

- Architectural clarity  
- Workflow consistency  
- Coding discipline  
- Preview‑safe behavior  
- Frank alignment  
- Long‑term maintainability  

All contributors must follow these conventions and keep documentation accurate, complete, and synchronized with the system.
