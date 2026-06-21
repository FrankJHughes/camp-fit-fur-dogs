---
id: US-204
title: "AutoRegistration Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
---

# US‑204 — AutoRegistration Observability

## Intent
As an **admin**, I must see which services are auto‑registered so that I can diagnose DI issues.

As a **developer**, I must understand why services were registered or skipped so that I can debug registration behavior.

## Value
AutoRegistration is powerful but opaque; observability ensures correctness and safety.

## Acceptance Criteria
- [ ] AC‑1: Emit `[autoreg.start]`
- [ ] AC‑2: Emit `[autoreg.service.registered]` with service type and lifetime
- [ ] AC‑3: Emit `[autoreg.service.skipped]` with reason
- [ ] AC‑4: Emit `[autoreg.end]` with duration
- [ ] AC‑5: Test harness can assert autoreg events

## Emotional Guarantees
- **EG‑01 No Surprises** — AutoRegistration behavior is visible.
- **EG‑06 Developer Confidence** — DI issues are diagnosable.

## Notes
AutoRegistration observability prevents silent DI failures.
