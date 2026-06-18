---
id: US-208
title: "CommandDispatcher Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
  - US-198
---

# US‑208 — CommandDispatcher Observability

## Intent
As an **admin**, I must observe command dispatch behavior so that I can diagnose handler resolution, validation, and execution issues.

As a **developer**, I must have visibility into dispatch‑level failures so that I can debug command workflows.

## Value
The CommandDispatcher is the root of command execution. Its observability is essential for diagnosing resolution and validation failures.

## Acceptance Criteria
- [ ] AC‑1: Emit `[command.dispatch.start]` with correlation ID and command type
- [ ] AC‑2: Emit `[command.dispatch.handler_resolved]` with handler type
- [ ] AC‑3: Emit `[command.dispatch.validation_failed]` with validation errors
- [ ] AC‑4: Emit `[command.dispatch.failed]` with sanitized exception info
- [ ] AC‑5: Emit `[command.dispatch.end]` with duration
- [ ] AC‑6: Dispatcher participates in the handler execution envelope
- [ ] AC‑7: Test harness can assert dispatcher observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — Dispatch behavior is visible and consistent.
- **EG‑06 Developer Confidence** — Resolution and validation issues are diagnosable.

## Notes
Dispatcher observability is required for cohesive command execution traces.
