---
id: US-210
title: "Domain Event Dispatch Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
  - US-198
  - US-207
---

# US‑211 — Domain Event Dispatch Observability

## Intent
As an **admin**, I must observe domain event dispatch behavior so that I can diagnose event cascades, dispatch failures, and event-driven workflows.

As a **developer**, I must have visibility into domain event dispatch so that I can debug event propagation, ordering, and failures without relying on manual logging.

## Value
Domain event dispatch is a critical but often invisible part of the system.  
Without observability, event cascades, dispatch failures, and ordering issues become extremely difficult to diagnose.

## Acceptance Criteria
- [ ] AC‑1: Emit `[domain.event.dispatch.start]` with correlation ID, event type, aggregate type
- [ ] AC‑2: Emit `[domain.event.dispatch.handler_resolved]` with handler type
- [ ] AC‑3: Emit `[domain.event.dispatch.no_handlers]` when no handlers exist
- [ ] AC‑4: Emit `[domain.event.dispatch.failed]` with sanitized exception info
- [ ] AC‑5: Emit `[domain.event.dispatch.end]` with duration and handler count
- [ ] AC‑6: Domain event cascades preserve correlation ID across all dispatch cycles
- [ ] AC‑7: Dispatch participates in the handler execution envelope (US‑205 → US‑209)
- [ ] AC‑8: Test harness can assert dispatch‑level observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — Domain event propagation is visible and predictable.
- **EG‑06 Developer Confidence** — Event cascades and dispatch failures are diagnosable.

## Notes
This story covers the *dispatch* phase of domain events.  
Domain event *handler* execution is covered separately in **US‑207**.
