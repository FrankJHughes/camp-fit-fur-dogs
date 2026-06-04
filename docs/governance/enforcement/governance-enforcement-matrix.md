# Governance Enforcement Matrix  
Roles × Responsibilities

This matrix defines **who enforces what** across the entire governance system.  
It operationalizes the Governance Hub and ensures every rule has a clear owner.

Governance defines **process, boundaries, responsibilities, and enforcement**.  
This matrix defines **who is accountable for each enforcement area**.

---

# Legend

- **PO** — Product Owner  
- **REV** — Reviewers  
- **CI** — Continuous Integration  
- **SCR** — Scripts & Automation  
- **SK** — Frank Guardrails  
- **OPS** — Operations / Hosting Providers  

Guided links point to the relevant governance area for deeper review.

---

# Governance Enforcement Matrix

| Governance Area | PO | REV | CI | SCR | SK | OPS |
|-----------------|----|-----|----|-----|----|-----|
| **[Story Governance](ca://s?q=Open_story_governance)** | ✔ | ✔ | ✔ | ✔ | — | — |
| **[Changelog Governance](ca://s?q=Open_changelog_governance)** | ✔ | ✔ | ✔ | — | — | — |
| **[Repository Hygiene](ca://s?q=Open_repo_hygiene_governance)** | — | ✔ | ✔ | ✔ | — | — |
| **[Multi‑Product Governance](ca://s?q=Open_multi_product_governance)** | ✔ | ✔ | — | — | ✔ | — |
| **[CI Governance](ca://s?q=Open_ci_governance)** | — | ✔ | ✔ | ✔ | — | — |
| **[Security Governance](ca://s?q=Open_security_governance)** | ✔ | ✔ | ✔ | — | ✔ | ✔ |
| **[Contributor Governance](ca://s?q=Open_contributor_governance)** | ✔ | ✔ | ✔ | — | — | — |
| **[Operations Governance](ca://s?q=Open_operations_governance)** | ✔ | ✔ | ✔ | — | ✔ | ✔ |
| **[Governance Process](ca://s?q=Open_governance_process)** | ✔ | ✔ | — | — | — | — |

---

# Role Responsibilities (Expanded)

## Product Owner (PO)
- Approves governance changes  
- Owns story governance, milestone governance, EG/LG alignment  
- Ensures changelog correctness  
- Enforces multi‑product boundaries at the strategic level  
- Approves security‑sensitive and operational governance changes  
- Final authority on governance conflicts  

## Reviewers (REV)
- Enforce repository hygiene  
- Validate CI governance compliance  
- Enforce contributor governance  
- Enforce product boundaries  
- Validate story correctness and acceptance criteria  
- Ensure conventions align with governance  

## Continuous Integration (CI)
- Enforces structural rules  
- Validates metadata, frontmatter, catalog, changelog  
- Enforces dependency direction  
- Enforces path‑based test selection  
- Enforces preview‑safe behavior  
- Enforces security scanning and operational safety  

## Scripts & Automation (SCR)
- Prevent drift across catalog, changelog, stories, docs  
- Enforce deterministic file generation  
- Enforce governance‑driven metadata rules  
- Enforce CI dependency graph correctness  
- Support operational safety (e.g., teardown probes, readiness probes)  

## Frank Guardrails (SK)
- Enforce architectural boundaries  
- Enforce layering rules  
- Enforce endpoint discovery rules  
- Enforce test seams  
- Enforce domain purity  
- Enforce hosting provider selection and validation primitives  
- Enforce DI auto‑registration and EF Core scanning  

## Operations / Hosting Providers (OPS)
- Enforce hosting provider hardening  
- Enforce configuration validation  
- Enforce fail‑fast startup  
- Enforce environment isolation  
- Enforce preview environment lifecycle  
- Surface operational failures deterministically  

---

# Enforcement Coverage Summary

| Area | Primary Enforcer | Secondary Enforcers |
|------|------------------|----------------------|
| Story Governance | PO | REV, CI |
| Changelog Governance | PO | REV, CI |
| Repository Hygiene | REV | CI, SCR |
| Multi‑Product Governance | PO | REV, SK |
| CI Governance | CI | REV, SCR |
| Security Governance | PO | REV, CI, SK, OPS |
| Contributor Governance | REV | PO, CI |
| Operations Governance | OPS | PO, REV, CI, SK |
| Governance Process | PO | REV |

---

# How to Use This Matrix

- **Reviewers** use it during PR review  
- **CI** uses it to validate structural rules  
- **Product Owner** uses it to approve governance changes  
- **Scripts** use it to enforce deterministic behavior  
- **Frank** uses it to enforce architectural guardrails  
- **Operations** uses it to enforce hosting and configuration safety  

This matrix ensures **every governance rule has a clear enforcement owner**.

---

# Summary

The Governance Enforcement Matrix ensures:

- No governance rule is unenforced  
- Responsibilities are explicit and non‑overlapping  
- CI and scripts enforce structural rules  
- Reviewers enforce boundaries and hygiene  
- Product Owner maintains strategic control  
- Frank enforces architecture  
- Operations enforces hosting and configuration safety  

Governance defines the rules.  
This matrix defines **who enforces them**.
