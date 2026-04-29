# Governance Hub

This directory contains the complete governance system for the repository.  
Governance defines **process**, **responsibilities**, **boundaries**, and **enforcement** across all products.

Governance is the highest‑level rule system.  
Conventions define *how* work is done; governance defines *how decisions are made and upheld*.

---

# Governance Documents

## Story Governance
Rules for story creation, lifecycle, acceptance, EG/LG integration, and Covey prioritization.

- [story-governance.md](story-governance.md)

## Changelog Governance
Rules for user‑facing change documentation, release grouping, and multi‑product changelog separation.

- [changelog-governance.md](changelog-governance.md)

## Repository Hygiene Governance
Rules preventing drift across stories, catalog, changelog, CI, documentation, and product boundaries.

- [repo-hygiene.md](repo-hygiene.md)

## Multi‑Product Governance
Rules defining boundaries between Camp Fit Fur Dogs and Frank (SharedKernel), including dependency direction, milestones, and release independence.

- [multi-product-governance.md](multi-product-governance.md)

## CI Governance
Rules for path‑based test skipping, required checks, nightly safety nets, and deterministic workflows.

- [ci-governance.md](ci-governance.md)

## Security Governance
Rules for authentication, authorization, secrets, hosting security, dependency scanning, and incident response.

- [security-governance.md](security-governance.md)

## Contributor Governance
Rules for contributor responsibilities, PR requirements, story/catalog alignment, changelog expectations, and product boundary enforcement.

- [contributor-governance.md](contributor-governance.md)

## Operations Governance
Rules for hosting, deployment, configuration, monitoring, reliability, and operational safety.

- [operations-governance.md](operations-governance.md)

## Governance Process
Rules for how governance itself is created, changed, approved, enforced, and retired.

- [governance-process.md](governance-process.md)

---

# Relationship to Conventions

Governance defines:

- Process  
- Responsibilities  
- Enforcement  
- Boundaries  

Conventions define:

- Architecture rules  
- Workflow rules  
- Code rules  
- Documentation rules  

Conventions live in:

- `docs/conventions/architecture.md`  
- `docs/conventions/workflow.md`  
- `docs/conventions/code.md`  
- `docs/conventions/docs.md`

Governance overrides conventions when they conflict.

---

# Canonical Ownership

- **Product Owner** — story governance, milestone governance, EG/LG alignment  
- **Reviewers** — hygiene, CI, contributor governance, product boundaries  
- **CI** — structural enforcement, metadata validation  
- **Scripts** — deterministic file generation and drift prevention  

Governance changes require Product Owner approval.

---

# Updating Governance

Governance must be updated via:

1. A PR modifying the relevant governance file  
2. Clear rationale and consequences  
3. Full review  
4. Product Owner approval  
5. Updates to conventions or scripts if needed  

Governance must remain stable, intentional, and minimal.

