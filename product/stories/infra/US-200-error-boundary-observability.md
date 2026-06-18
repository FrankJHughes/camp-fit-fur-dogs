---
id: US-200
title: "Error Boundary Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
  - US-198
---

# US‑200 — Error Boundary Observability

## Intent
As an **admin**, I must see structured error events when failures occur so that I can diagnose issues without relying on stack traces or console output.

As a **developer**, I must rely on a consistent error boundary that emits safe, structured, correlated error events.

## Value
A unified error boundary ensures all exceptions are captured and logged consistently across the platform.

## Acceptance Criteria
- [ ] AC‑1: All unhandled exceptions emit `[error.unhandled]`
- [ ] AC‑2: Events include:
      - correlation ID  
      - slice  
      - module  
      - handler  
      - exception type  
      - sanitized message  
- [ ] AC‑3: No PII or sensitive data is logged
- [ ] AC‑4: Error boundary integrates with handler envelope
- [ ] AC‑5: Error boundary integrates with HostingEngine
- [ ] AC‑6: Test harness can assert error boundary behavior

## Emotional Guarantees
- **EG‑03 Calm Protection** — Sensitive data is never exposed.
- **EG‑06 Developer Confidence** — Errors are always captured and traceable.

## Notes
This is the safety net for the entire platform.
