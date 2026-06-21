---
id: US-212
title: "CORS Middleware Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
  - US-198
---

# US‑212 — CORS Middleware Observability

## Intent
As an **admin**, I must observe CORS evaluation behavior so that I can diagnose cross‑origin request failures and misconfigurations.

As a **developer**, I must have visibility into CORS decisions so that I can debug allowed origins, methods, and headers without relying on browser console errors.

## Value
CORS failures are notoriously opaque.  
Without observability, diagnosing them requires guesswork and trial‑and‑error.

## Acceptance Criteria
- [ ] AC‑1: Emit `[middleware.cors.start]` with correlation ID and request origin
- [ ] AC‑2: Emit `[middleware.cors.policy_resolved]` with policy name and allowed origins
- [ ] AC‑3: Emit `[middleware.cors.allowed]` when the request is permitted
- [ ] AC‑4: Emit `[middleware.cors.rejected]` with reason when the request is denied
- [ ] AC‑5: Emit `[middleware.cors.end]` with duration
- [ ] AC‑6: Middleware participates in correlation ID propagation
- [ ] AC‑7: Test harness can assert CORS middleware observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — CORS behavior is visible and predictable.
- **EG‑06 Developer Confidence** — Cross‑origin issues are diagnosable.

## Notes
This story covers the CORS middleware only.  
Other middleware types are covered in separate stories.
