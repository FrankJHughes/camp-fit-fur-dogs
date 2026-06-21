---
id: US-221
title: "Dependency Registration Engine Refactor"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-176
  - US-185
---

# US‑221 — Dependency Registration Engine Refactor

## Intent

As an **admin**, I must have a clean, deterministic, and well‑named dependency registration engine so that the system’s DI graph is predictable, discoverable, and governed without relying on ambiguous or misleading terminology.

## Value

The current registration subsystem uses terminology such as “auto‑registration,” which implies implicit or magical behavior.  
However, Frank’s model is **explicit, deterministic registration with discovery**, not auto‑magic.

This refactor:

- clarifies the conceptual model  
- improves developer understanding  
- strengthens governance  
- prepares the system for future attribute‑based contract/implementation metadata  
- aligns naming with the rest of the Frank Engine architecture  

This story lays the foundation for a future rename (DependencyContract / DependencyImplementation) without requiring the rename to ship immediately.

## Acceptance Criteria

- [ ] AC‑1: The existing registration engine is refactored for clarity, determinism, and consistency with Frank’s engine architecture  
- [ ] AC‑2: All internal components use the canonical name **Dependency Registration Engine**  
- [ ] AC‑3: Documentation updated to reflect the correct conceptual model: *registration with discovery*, not auto‑registration  
- [ ] AC‑4: Developer Guide, Tester Guide, and User Guide regenerated with current names plus a “Future Direction” note  
- [ ] AC‑5: No breaking changes to public APIs (rename is non‑breaking at this stage)  
- [ ] AC‑6: Internal code comments and architecture notes reference the planned future rename (DependencyContractAttribute / DependencyImplementationAttribute)  
- [ ] AC‑7: Tests updated to reflect the clarified terminology  
- [ ] AC‑8: No functional behavior changes — this is a conceptual and naming refactor only  

## Emotional Guarantees

- **EG‑01 No Surprises** — Developers understand exactly how registration works and what is being registered  
- **EG‑03 Calm Protection** — Clear naming prevents accidental misuse or misunderstanding of DI behavior  
- **EG‑05 Responsible Partner** — The system communicates its behavior honestly and transparently  

## Notes

- This story **does not** introduce the new attributes — it documents the future direction and prepares the codebase for the rename  
- Future rename direction (not part of this story):  
  - `DependencyContractAttribute` or `ServiceContractAttribute`  
  - `DependencyImplementationAttribute` or `ServiceImplementationAttribute`  
  - Namespace: `Frank.DependencyRegistration` or `Frank.ServiceRegistration`  
- This story ensures all guides (developer/tester/user) reflect the **current** names while acknowledging the **planned** rename  
- This refactor aligns the subsystem with the Hosting Engine, Startup Engine, and Testing Engine naming conventions  
- **Demo:** Show before/after documentation and internal naming, demonstrating clarity and consistency  
