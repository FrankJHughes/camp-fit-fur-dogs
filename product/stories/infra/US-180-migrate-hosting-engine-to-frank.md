---
id: US-180
title: "Migrate Hosting Engine to Frank"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-108
  - US-175
---

# US‑180 — Migrate Hosting Engine to Frank

## Intent

As an **admin**, I must have a unified hosting engine built on Frank so that the application starts consistently, loads modules deterministically, and follows the architectural conventions of the platform.

## Acceptance Criteria

- [ ] AC‑1: Hosting engine is extracted into a Frank module with a single public entry point  
- [ ] AC‑2: Hosting engine configures Kestrel, logging, and middleware using Frank conventions  
- [ ] AC‑3: Hosting engine loads modules in deterministic order (Foundation → Infra → App)  
- [ ] AC‑4: Hosting engine exposes a unified `BuildHost()` method used by API and tests  
- [ ] AC‑5: No legacy hosting code remains in the API project  
- [ ] AC‑6: Hosting engine is fully covered by integration tests using the test harness  

## Notes

This story ensures the hosting layer is consistent with the Frank architecture and fully testable.
