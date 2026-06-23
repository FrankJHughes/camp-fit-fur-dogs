---
id: US-225
title: "Align Documentation With DDD and Agile"
epic: Documentation
milestone: M3
status: backlog
domain: docs
vertical_slice: false
dependencies: []
---

# US‑225 — Align Documentation With DDD and Agile

## User Story

As an **admin**, I want the documentation reorganized into a coherent structure aligned with Domain‑Driven Design (DDD) and Agile principles so that contributors can easily understand the system, follow architectural patterns, and apply conventions consistently.

## Context

The current documentation structure mixes:

- architecture documents  
- patterns  
- conventions  
- how‑to guides  
- role responsibilities  
- subsystem deep dives  

This results in:

- architecture feeling like a heap of unrelated documents  
- patterns being scattered or missing  
- conventions appearing without the patterns they derive from  
- how‑tos mixed with architecture  
- role guides duplicating content  
- no clear conceptual hierarchy  

We have clarified the correct conceptual model:

- **Architecture** = domain‑driven structure of the system  
- **Patterns** = technical building blocks used to implement architecture  
- **Governance** = rules derived from patterns  
- **Guides** = task‑oriented how‑tos  
- **Roles** = responsibilities and workflows  

Agile shapes the structure (task‑oriented guides, role‑aligned responsibilities) without introducing an `agile/` folder.

The correct top‑level structure is:

````markdown
docs/
  architecture/
    patterns/
    bounded-contexts/
    aggregates/
    domain-concepts/
    domain-flows/
  governance/
  guides/
  roles/
````

Architecture is now **DDD‑aligned**, organized by domain boundaries rather than technical layers.  
Patterns now fit naturally *under* architecture as reusable architectural building blocks.

## Scope

This story includes:

- Creating the new top‑level documentation structure:
  ````markdown
  docs/
    architecture/
      patterns/
      bounded-contexts/
      aggregates/
      domain-concepts/
      domain-flows/
    governance/
    guides/
    roles/
  ````
- Moving existing documents into the correct conceptual layer  
- Creating missing pattern stubs  
- Creating architecture and pattern indexes  
- Ensuring conventions reference patterns, not architecture  
- Ensuring guides are procedural and role‑aligned  
- Ensuring architecture is domain‑driven (bounded contexts, aggregates, flows, concepts)  
- Updating internal links  

This story does **not** rewrite all content — it establishes the structure and moves documents into place.

## Acceptance Criteria

### Architecture
- [ ] `docs/architecture/` exists  
- [ ] Architecture is organized by:
  - bounded contexts  
  - aggregates  
  - domain flows  
  - domain concepts  
- [ ] No conventions or how‑tos appear in this folder  
- [ ] `architecture/index.md` provides a conceptual overview  

### Patterns
- [ ] `docs/architecture/patterns/` exists  
- [ ] Contains stubs for core patterns:
  - vertical slice  
  - endpoint purity  
  - command/query  
  - handler  
  - mapping  
  - validation layering  
  - authentication callback  
  - session establishment  
  - repository  
  - domain event  
- [ ] Patterns do not contain rules or how‑tos  
- [ ] `patterns/index.md` provides a conceptual overview  

### Governance
- [ ] `docs/governance/` exists  
- [ ] Conventions are moved here  
- [ ] Conventions reference the patterns they derive from  
- [ ] No architecture or how‑tos appear in this folder  

### Guides
- [ ] `docs/guides/` exists  
- [ ] Contains procedural how‑tos (e.g., implementing a vertical slice)  
- [ ] Guides reference patterns and conventions  
- [ ] Guides do not define architecture or rules  

### Roles
- [ ] `docs/roles/` exists  
- [ ] Each role guide contains:
  - responsibilities  
  - required patterns  
  - required architecture domains  
  - relevant guides  
- [ ] No architecture or conventions appear directly in role guides  

### Migration
- [ ] All existing documentation is moved into the correct conceptual layer  
- [ ] No orphaned documents remain  
- [ ] No duplicated documents remain  
- [ ] All internal links updated  

## Dependencies

None.

## Open Questions

- Should bounded contexts be documented at the aggregate level or capability level?  
- Should pattern documents include diagrams or remain text‑only until stabilized?  
- Should guides be grouped by role or by task category?
