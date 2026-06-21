---
id: US-198
title: "Correlation-ID Propagation"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
---

# US‑198 — Correlation-ID Propagation

## Intent
As an **admin**, I must be able to trace a request end‑to‑end so that I can diagnose failures across API, protocol, application, and vertical slices.

As a **developer**, I must have correlation IDs automatically propagated so that I can debug complex flows without manually wiring identifiers.

## Value
Correlation IDs unify logs across all layers, enabling full request tracing.

## Acceptance Criteria
- [ ] AC‑1: Correlation ID created at API boundary
- [ ] AC‑2: Propagated through:
      - Protocol layer  
      - Application layer  
      - Handler pipeline  
      - Vertical slices  
      - Outbound calls  
- [ ] AC‑3: Included in all structured events
- [ ] AC‑4: Included in all error boundary events
- [ ] AC‑5: Included in all metrics (as allowed by cardinality rules)
- [ ] AC‑6: Test harness can assert correlation propagation

## Emotional Guarantees
- **EG‑01 No Surprises** — Every log event is traceable.
- **EG‑06 Developer Confidence** — Debugging becomes deterministic.

## Notes
Correlation IDs are the backbone of cohesive observability.
