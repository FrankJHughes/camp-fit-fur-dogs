---
id: US-173
title: "Native AOT Migration Plan"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-107
  - US-141
  - US-140
---

# US-173: Native AOT Migration Plan

## Intent

As an **owner**, I must be able to understand the feasibility, risks, and required architectural changes for migrating the backend to **.NET Native AOT** so that I can make an informed decision about whether the product should pursue AOT for faster cold starts, smaller deployment size, and improved operational efficiency.

## Value

Native AOT promises significant performance and startup improvements, but it is not a drop‑in replacement for the current reflection‑heavy architecture.  
Before committing engineering time, the business needs a clear, human‑readable plan that explains:

- what would break  
- what must change  
- what the migration would cost  
- what benefits we would gain  
- whether the migration aligns with product goals  

This story delivers the **decision‑making clarity** needed before any technical work begins.

## Acceptance Criteria

- [ ] A written migration plan exists in `docs/adr/` or `docs/architecture/` describing:
  - AOT benefits relevant to our product (startup time, memory, cold‑start behavior)
  - AOT blockers in the current architecture (reflection, EF Core, DI, JSON, validators)
  - Required changes to become AOT‑compatible
  - Risks and tradeoffs (loss of dynamic behaviors, trimming constraints)
  - Estimated engineering effort and sequencing
  - Recommendation: pursue, defer, or reject AOT
- [ ] The plan clearly identifies which SharedKernel components require annotations or redesign
- [ ] The plan identifies which Infrastructure components are incompatible with AOT (EF Core, migrations)
- [ ] The plan includes a proposed spike story if further investigation is needed
- [ ] The plan is written for **humans**, not engineers — no implementation details, no code

## Emotional Guarantees

- **EG‑01 — No Surprises:** The plan must clearly explain what breaks under AOT so the team is never blindsided.
- **EG‑03 — Calm Protection:** The plan must reduce fear and uncertainty by showing a safe, reversible path.
- **EG‑05 — Confidence:** The owner must feel confident that the decision is grounded in facts, not assumptions.

## Notes

- This story produces **documentation**, not code.
- EF Core is currently the largest AOT blocker; the plan must address EF Core’s AOT limitations.
- Reflection‑based DI, FluentValidation, MediatR‑style pipelines, and JSON polymorphism must be evaluated.
- The plan should reference Microsoft’s official AOT guidance for ASP.NET Core and EF Core.
- If the plan recommends pursuing AOT, follow‑up stories will be created for:
  - AOT‑safe DI
  - AOT‑safe validation
  - AOT‑safe JSON serialization
  - EF Core provider evaluation
  - Build pipeline changes
