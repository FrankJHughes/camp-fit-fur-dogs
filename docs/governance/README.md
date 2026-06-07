# Governance Hub

This directory contains the complete governance system for the repository.  
Governance defines **process**, **responsibilities**, **boundaries**, and **enforcement** across all products.

Governance is the **highest‑level rule system**.  
Conventions define *how work is implemented*; governance defines *how decisions are made, constrained, and upheld*.

Governance is intentionally **stable, minimal, and slow‑changing**.  
Conventions and guides evolve more frequently.

---

# Governance Documents

Governance documents are organized into three categories:

```
docs/governance/product/
docs/governance/technical/
docs/governance/enforcement/
```

Below is the canonical index.

---

## 🟦 Product Governance  
Rules that govern product‑level processes, story lifecycle, changelog behavior, contributor expectations, and cross‑product boundaries.

- [story-governance.md](product/story-governance.md)  
- [changelog-governance.md](product/changelog-governance.md)  
- [repo-hygiene.md](product/repo-hygiene.md)  
- [multi-product-governance.md](product/multi-product-governance.md)  
- [contributor-governance.md](product/contributor-governance.md)  

---

## 🟩 Technical Governance  
Rules that govern architecture, API behavior, CI behavior, security, and operational constraints.

- [api-governance.md](technical/api-governance.md)  
- [architecture-governance.md](technical/architecture-governance.md)  
- [ci-governance.md](technical/ci-governance.md)  
- [security-governance.md](technical/security-governance.md)  
- [operations-governance.md](technical/operations-governance.md)  

---

## 🟥 Enforcement Governance  
Rules that define **how governance is enforced**, who enforces it, and how governance evolves.

- [governance-enforcement-checklist.md](enforcement/governance-enforcement-checklist.md)  
- [governance-enforcement-matrix.md](enforcement/governance-enforcement-matrix.md)  
- [governance-process.md](enforcement/governance-process.md)  

---

# Governance Scope

Governance applies to:

- All products in the repository  
- All contributors  
- All CI/CD workflows  
- All documentation  
- All architectural boundaries  
- All story and changelog processes  
- All operational and security requirements  

Governance is **binding** and overrides conventions when conflicts arise.

---

# Governance Hierarchy

Governance follows a strict hierarchy:

1. **Product Governance** — story lifecycle, contributor rules, changelog rules  
2. **Technical Governance** — architecture, API, CI, security, operations  
3. **Enforcement Governance** — how governance is applied and evolved  
4. **Conventions** — implementation rules  
5. **Guides** — how‑to documents  

Higher levels override lower levels.

---

# Governance Invariants

All governance documents must remain:

- **Stable** — changes are rare and intentional  
- **Minimal** — only rules, no implementation details  
- **Enforceable** — must be testable or reviewable  
- **Non‑overlapping** — no duplication across governance files  
- **Cross‑product** — rules apply to all products unless explicitly scoped  

Governance must never contain:

- Code conventions  
- Workflow implementation details  
- How‑to instructions  
- Examples or tutorials  
- ADR‑style rationale  

Governance defines **rules**, not explanations.

---

# Governance Drift Detection

Governance drift is any mismatch between:

- Governance rules  
- Conventions  
- CI behavior  
- Documentation  
- Product code  
- Story lifecycle behavior  

Drift must be:

- Detected by CI guardrails  
- Corrected immediately  
- Documented in the PR  
- Reviewed by governance owners  

---

# Governance Review Cadence

Governance must be reviewed:

- At major milestones  
- After architectural changes  
- After CI/CD model changes  
- After security incidents  
- After new product surfaces  
- After convention evolution  
- When guardrails detect drift  

Reviews must follow the governance process.

---

# Governance Change Lifecycle

Governance changes require:

1. A PR modifying the relevant governance file  
2. Clear rationale and consequences  
3. Full reviewer discussion  
4. Product Owner approval  
5. Updates to conventions, guides, or scripts if required  
6. Guardrail updates if enforcement changes  

Governance changes must be:

- Intentional  
- Minimal  
- Backwards‑compatible unless explicitly breaking  
- Fully documented  

---

# Governance Retirement Rules

A governance rule may be retired when:

- It is no longer applicable  
- It is replaced by a convention  
- It is superseded by architecture  
- It becomes redundant  
- It conflicts with updated guardrails  
- It conflicts with updated workflows  

Retirement requires:

- A PR removing the rule  
- Product Owner approval  
- Updated references  
- Updated enforcement matrix  

---

# Relationship to Conventions

Governance defines:

- Process  
- Responsibilities  
- Enforcement  
- Boundaries  
- Stability guarantees  
- Cross‑product rules  

Conventions define:

- Architecture implementation rules  
- Workflow implementation rules  
- Code implementation rules  
- Documentation implementation rules  

Conventions live in:

- `docs/conventions/architecture.md`  
- `docs/conventions/workflow.md`  
- `docs/conventions/code.md`  
- `docs/conventions/docs.md`

**Governance overrides conventions** when they conflict.  
Conventions must remain aligned with governance.

---

# Canonical Ownership

- **Product Owner** — story governance, milestone governance, EG/LG alignment  
- **Reviewers** — hygiene, CI, contributor governance, product boundaries  
- **CI** — structural enforcement, metadata validation, drift detection  
- **Scripts** — deterministic file generation and governance enforcement  

Governance changes require **Product Owner approval**.

---

# Updating Governance

Governance must be updated via:

1. A PR modifying the relevant governance file  
2. Clear rationale and consequences  
3. Full reviewer discussion  
4. Product Owner approval  
5. Updates to conventions, guides, or scripts if required  

Governance must remain:

- Stable  
- Intentional  
- Minimal  
- Enforceable  
- Aligned with product boundaries  

---

# Summary

The Governance Hub defines the **rules that govern the rules**.  
It ensures:

- Stable decision‑making  
- Clear responsibilities  
- Cross‑product boundaries  
- Deterministic CI and operational behavior  
- Alignment between stories, code, CI, and documentation  

Governance is the backbone of the repository’s long‑term maintainability.
