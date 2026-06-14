---
id: US-185
title: "Auto-registration Opt-Out"
epic: Infrastructure
milestone: M1+
status: ready
domain: infra
vertical_slice: false
dependencies:
  - US-181
---

# US‑185 — Auto‑registration Opt-Out

## Intent

As an **admin**, I must be able to opt out of automatic DI registration for specific modules or components so that the system can enforce explicit wiring where required for safety, clarity, or performance.

## Acceptance Criteria

- [ ] AC‑1: Modules can declare opt‑out of auto‑registration  
- [ ] AC‑2: Opt‑out modules require explicit DI registration  
- [ ] AC‑3: Startup engine validates that all required services are registered  
- [ ] AC‑4: System fails with a clear error if a required service is missing  
- [ ] AC‑5: Test harness supports opt‑out modules  
- [ ] AC‑6: Documentation updated for module authors  

## Notes

This story ensures DI governance and prevents accidental service exposure.
