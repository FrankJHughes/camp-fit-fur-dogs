---
id: US-177
title: "Stabilize Test Harness"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-176
---

# US‑177 — Stabilize Test Harness

## Intent

As an **admin**, I must be able to execute all application handlers through a deterministic, isolated test harness so that the system can be validated without relying on API hosting, external infrastructure, or non-deterministic runtime behavior.

## Acceptance Criteria

- [ ] AC‑1: Test harness can execute any handler in isolation without requiring API or hosting layers  
- [ ] AC‑2: Test harness provides deterministic setup/teardown with no cross‑test state leakage  
- [ ] AC‑3: Test harness supports injecting fake infrastructure (email, outbox, clock, ID generator)  
- [ ] AC‑4: Test harness exposes a single entry point for executing commands and queries  
- [ ] AC‑5: Test harness logs failures with full context (handler, payload, exception, inner exception)  
- [ ] AC‑6: Test harness supports module‑scoped overrides for DI registrations  
- [ ] AC‑7: All existing tests run under the new harness without modification or flakiness  

## Notes

This story ensures the Frank architecture has a stable, deterministic foundation for all future test development.
