---
id: US-175
title: "Migrate Auth Callback Engine to Frank"
epic: Infrastructure
milestone: M1+
status: ready
domain: infra
vertical_slice: false
dependencies:
  - US-110
  - US-111
---

# US‑175 — Migrate Auth Callback Engine to Frank

## Intent

As an **admin**, I must migrate the authentication callback engine into the Frank architecture so that identity processing, token validation, and callback flows are deterministic, testable, and consistent across the platform.

## Acceptance Criteria

- [ ] AC‑1: Auth callback engine is extracted into a Frank module with a single public entry point  
- [ ] AC‑2: Callback engine validates tokens and identity payloads using Frank conventions  
- [ ] AC‑3: Callback engine integrates cleanly with the OIDC login flow  
- [ ] AC‑4: Callback engine exposes a deterministic API for tests via the test harness  
- [ ] AC‑5: Legacy callback logic is removed from the API project  
- [ ] AC‑6: Errors are surfaced with structured, testable failure modes  

## Emotional Guarantees

- **EG‑03 — Calm Protection:** Identity callbacks must be processed safely and predictably.  
- **EG‑05 — Clear Ownership:** The owner’s identity must always be unambiguous after callback processing.

## Notes

This story prepares the foundation for US‑179 (Authenticated User Service).
