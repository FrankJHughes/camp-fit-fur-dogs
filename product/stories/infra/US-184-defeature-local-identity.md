---
id: US-184
title: "De-feature Local Identity"
epic: Infrastructure
milestone: M1+
status: ready
domain: infra
vertical_slice: false
dependencies:
  - US-110
  - US-175
  - US-179
---

# US‑184 — De‑feature Local Identity

## Intent

As an **admin**, I must remove all local identity mechanisms so that the system relies exclusively on OIDC‑based authentication, reducing security risk and eliminating unused code paths.

## Acceptance Criteria

- [ ] AC‑1: All local identity endpoints are removed or disabled  
- [ ] AC‑2: All local password storage and validation code is removed  
- [ ] AC‑3: All handlers requiring identity use the Authenticated User Service (US‑179)  
- [ ] AC‑4: No local identity configuration remains in appsettings  
- [ ] AC‑5: Test harness uses OIDC‑based identity fakes exclusively  
- [ ] AC‑6: System fails safely if local identity is referenced  

## Emotional Guarantees

- **EG‑03 — Calm Protection:** Removing local identity must not introduce ambiguity or risk in authentication flows.

## Notes

This story completes the transition to a single identity model.
