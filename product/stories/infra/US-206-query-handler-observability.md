---
id: US-206
title: "Query Handler Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
  - US-198
---

# US‑206 — Query Handler Observability

## Intent
As an **admin**, I must observe query handler execution so that I can diagnose read‑model issues.

As a **developer**, I must have automatic instrumentation for query handlers so that I do not manually implement logging or metrics.

## Value
Query handlers power all read operations. Their observability is essential for diagnosing performance and correctness issues.

## Acceptance Criteria
- [ ] AC‑1: Emit `[query.exec.start]` with correlation ID, handler name, slice
- [ ] AC‑2: Emit `[query.exec.end]` with duration
- [ ] AC‑3: Emit `[query.exec.failed]` with sanitized exception info
- [ ] AC‑4: Query handlers are wrapped automatically by the handler envelope
- [ ] AC‑5: Validation failures emit `[query.exec.validation_failed]`
- [ ] AC‑6: Metrics emitted for query duration
- [ ] AC‑7: Test harness can assert query handler observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — Query execution is visible and consistent.
- **EG‑06 Developer Confidence** — No manual instrumentation required.

## Notes
Query handlers are critical for performance diagnostics.
