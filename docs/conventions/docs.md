# Documentation Conventions

Documentation is a first‑class artifact in the Camp Fit Fur Dogs system.  
It defines how architecture, workflow, code, and processes are communicated, maintained, and evolved.  
All contributors must treat documentation with the same rigor as code.

---

# Documentation Purpose

Documentation exists to:

- Make decisions explicit  
- Reduce ambiguity  
- Keep the team aligned  
- Onboard new contributors quickly  
- Preserve architectural and workflow intent  
- Prevent drift between code and conventions  

Docs are part of the product and must be kept up to date.

---

# Structure

Conventions are organized into four canonical documents:

- **Architecture** — layers, boundaries, hosting, PR Preview model, core building blocks  
- **Workflow** — how work is done (TDD, debugging, patching, script‑first, PR Preview lifecycle)  
- **Code** — backend, frontend, CQRS, endpoint, and SharedKernel coding rules  
- **Docs** — how documentation itself is structured and maintained  

These four documents form the **single source of truth** for all repository rules.

Other documents (e.g., ADRs, guides, READMEs) may reference these conventions but must not redefine them.

---

# Architecture Decision Records (ADRs)

Significant architectural decisions must be captured as ADRs.

Each ADR must:

- Describe the context  
- State the decision  
- Explain the consequences  
- Reference relevant conventions  
- Clarify how the decision interacts with SharedKernel or guardrails  

ADRs may live:

- Next to the code they affect, or  
- In a central `docs/adr/` folder  

ADRs must be updated or superseded when decisions evolve.

---

# Documentation Lifecycle

Documentation must evolve with the system.

Rules:

- PRs that change behavior, architecture, or workflow **must** update documentation in the same PR  
- Outdated documentation must be corrected immediately  
- Breaking changes must be reflected in both code and docs  
- Conventions must remain internally consistent across all four canonical files  
- Documentation changes must follow the Universal Patch Rule (full‑file regeneration)  

Docs are not optional; they are part of the definition of done.

---

# Safe Examples and Fencing

Documentation must respect the fencing and quoting rules defined in Workflow Conventions.

Rules:

- Avoid nested fences and complex quoting that can break scripts or PR bodies  
- When showing syntax that conflicts with script‑generation rules, describe it in words instead of using literal characters  
- Prefer simple, copy‑pasteable examples  
- Avoid examples that violate SharedKernel or guardrail rules  
- When demonstrating PowerShell, avoid backticks, double quotes, and nested here‑strings  

These rules ensure documentation remains compatible with script‑first workflows and automation.

---

# Navigation and Ownership

Conventions must be easy to find and maintain.

## Navigation

- `docs/conventions/` is the canonical hub  
- Each file must have clear headings and stable anchors  
- Cross‑references must be explicit and correct  

## Ownership

- **Architecture & SharedKernel conventions** — platform/architecture maintainers  
- **Workflow & tooling conventions** — automation/build maintainers  
- **Code conventions** — backend, frontend, and infra maintainers  
- **Docs conventions** — shared responsibility with a clear maintainer  

Ownership must be explicit to prevent drift.

---

# Updating Conventions

When updating conventions:

- Follow the Universal Patch Rule (full‑file regeneration)  
- Keep changes cohesive and well‑scoped  
- Explain rationale in the PR description  
- Ensure examples remain valid under fencing and script‑first rules  
- Update all four canonical documents if the change affects multiple areas  
- Ensure guardrail tests remain aligned with the updated conventions  

Conventions are living documents; they must evolve deliberately, not drift accidentally.

---

# Relationship to SharedKernel

SharedKernel is the authoritative source for cross‑cutting behavior.

Documentation must:

- Clearly indicate when a rule is enforced by SharedKernel types or helpers  
- Encourage product code to lean on SharedKernel instead of duplicating patterns  
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

- Describe the preview lifecycle (Neon branch creation, migrations, Render preview deployment)  
- Document preview‑safe coding rules  
- Clarify environment variable expectations (`PREVIEW_DB_CONNECTION_STRING`, `ConnectionStrings__DefaultConnection`)  
- Ensure all examples reflect Git‑backed Render PR Previews  
- Avoid references to image‑backed or manually triggered preview flows  

Documentation must reflect the current hosting and preview architecture at all times.

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
