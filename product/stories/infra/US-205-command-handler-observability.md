---
id: US-205
title: "Command Handler Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
  - US-198
---

# US‑205 — Command Handler Observability

## Intent
As an **admin**, I must observe command handler execution so that I can diagnose issues within vertical slices.

As a **developer**, I must have automatic instrumentation for command handlers so that I do not manually implement logging or metrics.

## Value
Command handlers are the core of vertical slice execution. Their observability is essential for tracing business workflows.

## Acceptance Criteria
- [ ] AC‑1: Emit `[command.exec.start]` with correlation ID, handler name, slice
- [ ] AC‑2: Emit `[command.exec.end]` with duration
- [ ] AC‑3: Emit `[command.exec.failed]` with sanitized exception info
- [ ] AC‑4: Command handlers are wrapped automatically by the handler envelope
- [ ] AC‑5: Validation failures emit `[command.exec.validation_failed]`
- [ ] AC‑6: Metrics emitted for command duration
- [ ] AC‑7: Test harness can assert command handler observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — Command execution is visible and consistent.
- **EG‑06 Developer Confidence** — No manual instrumentation required.

## Notes
Command handlers are the primary entry point for business logic.
