# Governance Process

This document defines how governance rules are created, updated, approved, enforced, and retired.  
It establishes the meta‑process that keeps the governance system coherent, intentional, and aligned with product needs.

Governance exists to:

- Protect product quality  
- Prevent drift  
- Ensure predictable behavior  
- Maintain architectural integrity  
- Support long‑term maintainability  
- Provide clarity for contributors and reviewers  

Governance is not static — but it must never change casually.

---

# 1. Purpose of Governance

Governance provides:

- A stable foundation for decision‑making  
- A consistent framework for contributions  
- A shared understanding of expectations  
- A mechanism for preventing drift  
- A contract between contributors, reviewers, CI, automation, and the Product Owner  

Governance is the highest‑level rule system in the repository.  
All conventions, workflows, and scripts must align with it.

---

# 2. Scope of Governance

Governance applies to:

- Stories  
- Catalog  
- Changelog  
- CI  
- Security  
- Operations  
- Architecture boundaries  
- Multi‑product boundaries  
- Contributor behavior  
- Repository hygiene  
- Enforcement roles and responsibilities  

Governance does **not** define:

- Code style  
- Architecture implementation patterns  
- Testing techniques  
- Documentation formatting  

Those live in conventions.

Governance defines **process, boundaries, responsibilities, and enforcement**, not implementation.

---

# 3. Governance Artifacts

The governance system consists of:

- This file (`governance-process.md`)  
- Story governance  
- Changelog governance  
- Repo hygiene governance  
- Multi‑product governance  
- CI governance  
- Security governance  
- Operations governance  
- Contributor governance  
- Architecture governance  
- Governance Enforcement Checklist  
- Governance Enforcement Matrix  

These files form the canonical governance corpus.  
They must remain internally consistent and drift‑free.

---

# 4. Governance Change Process

Governance changes must follow this process:

1. **Identify the need**  
   - A gap, conflict, or ambiguity is discovered  
   - A new product requirement emerges  
   - A convention or workflow evolves  
   - A governance rule becomes obsolete or redundant  

2. **Draft the change**  
   - Create a PR modifying the relevant governance file(s)  
   - Include rationale, consequences, and alternatives  
   - Reference any related stories or ADRs  
   - Apply the Universal Patch Rule (full‑file regeneration)  

3. **Review**  
   - Reviewers evaluate clarity, consistency, and impact  
   - Product Owner evaluates alignment with product direction  
   - CI validates structure, metadata, and hygiene  
   - Scripts validate deterministic formatting  

4. **Approval**  
   - Product Owner must approve all governance changes  
   - Reviewers must confirm no conflicts with existing rules  
   - CI must pass all governance validations  

5. **Merge**  
   - Governance changes must not be merged without full review  
   - Governance changes must not be merged during active incidents  
   - Governance changes must not be merged with failing CI  

6. **Propagation**  
   - Update conventions if needed  
   - Update scripts if needed  
   - Update workflows if needed  
   - Update documentation if needed  
   - Update the Governance Enforcement Matrix if roles change  

Governance changes must be intentional, traceable, and consistent.

---

# 5. Governance Stability Rules

Governance must remain:

- **Stable** — not frequently changed  
- **Predictable** — contributors know what to expect  
- **Consistent** — no contradictions across files  
- **Minimal** — only rules that matter  
- **Explicit** — no implicit or tribal knowledge  
- **Aligned** — consistent with architecture, CI, security, and operations  

Governance must not:

- Drift  
- Duplicate conventions  
- Contain implementation details  
- Change without Product Owner approval  
- Conflict with the Governance Enforcement Matrix  

---

# 6. Governance Enforcement

Governance is enforced by:

- **Product Owner** — alignment, intent, approval  
- **Reviewers** — correctness, consistency, boundaries  
- **CI** — structural and metadata validation  
- **Scripts** — deterministic file generation and hygiene  
- **Frank Guardrails** — architectural boundaries, DI, EF Core, hosting provider rules  
- **Operations** — hosting provider hardening and configuration safety  

A PR must not merge if it violates governance.

---

# 7. Governance Violations

A governance violation occurs when:

- A PR bypasses required processes  
- A story is incomplete or inconsistent  
- A changelog entry is missing or incorrect  
- CI rules are violated  
- Product boundaries are crossed  
- Security posture is weakened  
- Operational rules are ignored  
- Repository hygiene is broken  
- DI auto‑registration or EF Core scanning fails  
- Hosting provider hardening rules are bypassed  
- Governance documents become inconsistent  

Violations must be corrected before merge.

---

# 8. Governance and ADRs

Governance changes may require an ADR when:

- Architecture changes  
- Product boundaries change  
- Hosting or deployment changes  
- Security posture changes  
- CI strategy changes  
- Operational model changes  
- Frank guardrail behavior changes  

ADRs document **why** governance changed.  
Governance documents **what** changed.

---

# 9. Governance and Conventions

Governance defines:

- Process  
- Responsibilities  
- Enforcement  
- Boundaries  

Conventions define:

- How code is written  
- How tests are structured  
- How documentation is formatted  
- How architecture is implemented  
- How workflows operate  

Governance overrides conventions when they conflict.  
Conventions must be updated when governance changes.

---

# 10. Governance Review Cadence

Governance must be reviewed:

- At the start of each major milestone  
- After major architectural changes  
- After significant incidents  
- When new product surfaces are added  
- When conventions evolve  
- When CI or operational models change  
- When Frank guardrails expand  

Reviews must be documented in PRs and must follow the Universal Patch Rule.

---

# 11. Governance Retirement

A governance rule may be retired when:

- It no longer applies  
- It has been replaced by a convention  
- It has been superseded by architecture  
- It is redundant or obsolete  
- It conflicts with updated guardrails or workflows  

Retirement requires:

- A PR removing the rule  
- Product Owner approval  
- Updated references across the repo  
- Updated enforcement matrix if roles change  

Governance must remain lean, intentional, and relevant.

---

# 12. Governance as a Living System

Governance is:

- A contract  
- A safety net  
- A quality mechanism  
- A long‑term investment  
- A shared responsibility across roles  

It evolves with the product — but only with intention, clarity, and discipline.

