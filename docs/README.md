# Documentation

This directory contains all product, engineering, and operational documentation for the Camp Fit Fur Dogs system.  
Documentation is organized by purpose: **conventions**, **guides**, **runbooks**, **architecture decisions**, and **historical artifacts**.

The structure is intentionally simple, predictable, and AI‑friendly.

---

# Documentation Structure

## Conventions (`docs/conventions/`)
Authoritative, non‑negotiable rules that define how the system must be built and operated.

- **[Architecture](conventions/architecture.md)** — layering, boundaries, hosting model  
- **[Workflow](conventions/workflow.md)** — CI/CD, preview environments, selective testing  
- **[Code](conventions/code.md)** — coding standards, patterns, safety rules  

Conventions are the **rulebooks** of the system.

---

## Guides (`docs/guides/`)
Role‑specific or topic‑specific explanations.  
Guides teach *how to work within the system*, not what the rules are.

Examples include:

- Developer Guide  
- Product Owner Guide  
- Scrum Master Guide  
- Domain or feature walkthroughs  

Guides are **explanatory**, not normative.

---

## Runbooks (`docs/runbooks/`)
Operational, step‑by‑step procedures for real tasks.

Runbooks cover:

- Local development  
- Database migrations  
- Incident response  
- Preview environment recovery  
- Sprint planning  
- Release process  

Runbooks are **action playbooks**, not design documents.

---

## Architecture Decision Records (`docs/adr/`)
ADRs capture **significant, irreversible, or high‑impact decisions** in the system’s architecture, tooling, workflows, and documentation structure.

ADRs are:

- permanent historical records  
- numbered sequentially (`0001`, `0002`, …)  
- never deleted — only superseded  
- the source of truth for architectural intent  

Use the standard template:

- **[ADR Template](adr/TEMPLATE.md)**

See the ADR index:

- **[ADR Index](adr/README.md)**

---

## Sprint Reviews (`docs/sprint-reviews/`)
Historical summaries of each sprint.

Each review includes:

- What shipped  
- Completed stories  
- Incomplete stories  
- Key decisions  
- Learnings  
- Next sprint focus  

Use the standard template:

- **[Sprint Review Template](sprint-reviews/sprint-review-template.md)**

Sprint reviews are **historical artifacts**, not planning documents.

---

## Product Stories (`product/stories/`)
The product backlog.  
Each story is a Markdown file using Story Grammar:

> **As a <role>, I must/should be able to <verb>… so that <value>…**

Stories define **intent**, **value**, and **acceptance criteria**.

Tasks are created as GitHub Issues and reference stories.

---

## ADRs vs. Conventions vs. Runbooks vs. Guides

- **ADRs** → record *decisions*  
- **Conventions** → define *rules*  
- **Guides** → explain *how to work*  
- **Runbooks** → describe *how to execute*  
- **Sprint reviews** → capture *what happened*  
- **Stories** → define *product intent*  

This separation keeps documentation:

- discoverable  
- maintainable  
- stable  
- low‑friction  
- AI‑friendly  

---

# Documentation Philosophy

Documentation should be:

- **Minimal** — only what is needed  
- **Structured** — predictable locations and formats  
- **Durable** — stable over time  
- **Actionable** — runbooks and guides are practical  
- **Historical** — ADRs and sprint reviews preserve context  

This folder is the backbone of the system’s shared understanding.

