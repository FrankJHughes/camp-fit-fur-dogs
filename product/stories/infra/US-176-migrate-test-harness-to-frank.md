---
id: US-176
title: "Migrate Test Harness to Frank"
epic: Infrastructure
milestone: M1+
status: ready
domain: infra
vertical_slice: false
dependencies: []
---

# US‑176 — Migrate Test Harness to Frank

## Intent

As an **admin**, I must migrate the existing test harness into the Frank architecture so that all tests execute through a unified, deterministic, and modular testing foundation.

## Acceptance Criteria

- [ ] AC‑1: Test harness is extracted into a Frank module with a single public entry point  
- [ ] AC‑2: Harness supports module‑scoped DI overrides  
- [ ] AC‑3: Harness provides deterministic setup/teardown  
- [ ] AC‑4: Harness supports fake infrastructure (clock, email, outbox, ID generator)  
- [ ] AC‑5: Harness integrates with Frank’s module loader  
- [ ] AC‑6: Legacy harness code is removed from the test projects  

## Notes

This story enables US‑177 (Stabilize Test Harness) and US‑178 (Refactor Tests).
