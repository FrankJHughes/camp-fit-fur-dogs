# Governance Hub  
The complete governance system for the repository

This directory contains the **full governance framework** for the repository.  
Governance defines **process**, **responsibilities**, **boundaries**, and **enforcement** across all products and all contributors.

Governance is the **highest‑level rule system**.  
Conventions define *how work is implemented*; governance defines *how decisions are made, constrained, and upheld*.

Governance is intentionally **stable, minimal, and slow‑changing**.  
Conventions and guides evolve more frequently.

---

# Governance Domains

Governance documents are organized into **five domains**, each with a distinct scope:

```
docs/governance/
├── repo/         # Cross-product, repository-wide governance
├── product/      # Product-specific governance (Camp Fit Fur Dogs)
├── technical/    # Architecture, API, CI, operations, purity
├── security/     # Security-specific governance
└── enforcement/  # How governance is enforced and evolved
```

Below is the canonical index.

---

## 🟦 Repo Governance  
Cross‑product rules that apply to **every product** in the repository (Frank, CFFD, SharedKernel, future products).

These govern:

- Story structure  
- Contributor expectations  
- Repository hygiene  
- Changelog behavior  
- Multi‑product boundaries  

**Documents:**

- [story-governance.md](repo/story-governance.md)  
- [repo-hygiene.md](repo/repo-hygiene.md)  
- [contributor-governance.md](repo/contributor-governance.md)  
- [changelog-governance.md](repo/changelog-governance.md)  
- [multi-product-governance.md](repo/multi-product-governance.md)  

Repo governance is **horizontal** — it applies everywhere.

---

## 🟩 Product Governance  
Rules that govern **how the Camp Fit Fur Dogs product is defined**, including:

- Capability governance  
- Milestone governance  
- Emotional guarantees  
- Story lifecycle  
- Backlog governance  
- Refinement governance  
- Product decision governance  

**Documents:**

- [README.md](product/README.md) *(Product Governance Guide)*

Product governance is **vertical** — it applies only to CFFD.

---

## 🟥 Technical Governance  
Rules that govern **how the system is architected and built**, including:

- Architecture boundaries  
- API behavior  
- CI/CD behavior  
- Operational constraints  
- Purity rules  
- Frank governance  

**Documents:**

- [api-governance.md](technical/api-governance.md)  
- [architecture-governance.md](technical/architecture-governance.md)  
- [ci-governance.md](technical/ci-governance.md)  
- [operations-governance.md](technical/operations-governance.md)  
- [purity-rules.md](technical/purity-rules.md)  
- [frank-governance.md](technical/frank-governance.md)  
- [security-governance.md](technical/security-governance.md)  

Technical governance is **cross‑cutting** — it applies to all code.

---

## 🟨 Security Governance  
Rules that govern **security‑specific behavior**, including:

- CORS  
- Authentication boundaries  
- Secrets handling  
- Session rules  
- Security headers  

**Documents:**

- [cors-governance.md](security/cors-governance.md)

Security governance is **cross‑cutting** — it applies to all products and all surfaces.

---

## 🟪 Enforcement Governance  
Rules that define **how governance is enforced**, who enforces it, and how governance evolves.

These govern:

- Governance enforcement matrix  
- Governance enforcement checklist  
- Governance change process  
- Drift detection  

**Documents:**

- [governance-enforcement-checklist.md](enforcement/governance-enforcement-checklist.md)  
- [governance-enforcement-matrix.md](enforcement/governance-enforcement-matrix.md)  
- [governance-process.md](enforcement/governance-process.md)  

Enforcement governance is **meta‑governance** — rules about rules.

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

1. **Repo Governance** — cross‑product rules  
2. **Product Governance** — product‑specific rules  
3. **Technical Governance** — architecture, API, CI, operations  
4. **Security Governance** — security constraints  
5. **Enforcement Governance** — how governance is applied  
6. **Conventions** — implementation rules  
7. **Guides** — how‑to documents  

Higher levels override lower levels.

---

# Governance Invariants

All governance documents must remain:

- **Stable** — changes are rare and intentional  
- **Minimal** — only rules, no implementation details  
- **Enforceable** — must be testable or reviewable  
- **Non‑overlapping** — no duplication across governance files  
- **Cross‑product** unless explicitly scoped  

Governance must never contain:

- Code conventions  
- Workflow implementation details  
- How‑to instructions  
- Tutorials or examples  
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

# Summary

The Governance Hub defines the **rules that govern the rules**.  
It ensures:

- Stable decision‑making  
- Clear responsibilities  
- Cross‑product boundaries  
- Deterministic CI and operational behavior  
- Alignment between stories, code, CI, and documentation  

Governance is the backbone of the repository’s long‑term maintainability.
