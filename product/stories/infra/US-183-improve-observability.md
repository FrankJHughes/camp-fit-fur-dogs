---
id: US-183
title: "Improve Observability"
epic: Infrastructure
milestone: M1+
status: ready
domain: infra
vertical_slice: false
dependencies:
  - US-180
  - US-181
---

# US‑183 — Improve Observability

## Intent

As an **admin**, I must have unified, structured observability across the Frank hosting and startup engines so that system behavior can be monitored, debugged, and analyzed consistently across environments.

## Acceptance Criteria

- [ ] AC‑1: Structured logging is enabled across all Frank modules  
- [ ] AC‑2: Hosting and startup engines emit lifecycle events (startup, shutdown, module load)  
- [ ] AC‑3: Handler execution logs include correlation IDs and execution duration  
- [ ] AC‑4: Errors include structured context (module, handler, payload)  
- [ ] AC‑5: Observability is testable through the test harness  
- [ ] AC‑6: No legacy logging remains in API or infrastructure projects  

## Notes

This story ensures the Frank platform is diagnosable and production‑ready.
