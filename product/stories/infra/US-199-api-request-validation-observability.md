---
id: US-199
title: "API Request Validation Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-192
  - US-197
  - US-198
---

# US‑210 — API Request Validation Observability

## Intent
As an **admin**, I must observe API request validation so that I can diagnose malformed or invalid client requests.

As a **developer**, I must have visibility into API‑level validation failures so that I can debug client‑side issues and ensure correct request shapes.

## Value
API validation occurs outside the dispatch pipeline. Without observability, malformed requests become invisible and difficult to diagnose.

## Acceptance Criteria
- [ ] AC‑1: Emit `[api.validation.start]` with correlation ID, route, and DTO type
- [ ] AC‑2: Emit `[api.validation.end]` with duration
- [ ] AC‑3: Emit `[api.validation.failed]` with validation errors
- [ ] AC‑4: API validation observability is distinct from handler validation observability
- [ ] AC‑5: No PII or sensitive data is logged
- [ ] AC‑6: Test harness can assert API validation observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — API validation is clearly separated from application validation.
- **EG‑06 Developer Confidence** — Client‑side issues are diagnosable.

## Notes
API validation is part of the API boundary, not the handler pipeline.
