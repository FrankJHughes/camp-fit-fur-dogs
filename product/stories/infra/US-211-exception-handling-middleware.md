---
id: US-211
title: "Exception Handling Middleware Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
  - US-198
---

# US‑211 — Exception Handling Middleware Observability

## Intent
As an **admin**, I must observe exception handling middleware behavior so that I can diagnose unhandled exceptions, error boundary behavior, and failures that occur before the dispatch pipeline.

As a **developer**, I must have visibility into exception handling so that I can debug failures without relying on console output or ad‑hoc logging.

## Value
ExceptionHandlingMiddleware is the final safety net before the API boundary.  
Its observability is essential for diagnosing failures that occur outside the handler pipeline.

## Acceptance Criteria
- [ ] AC‑1: Emit `[middleware.exception.start]` with correlation ID and middleware type
- [ ] AC‑2: Emit `[middleware.exception.caught]` with sanitized exception info
- [ ] AC‑3: Emit `[middleware.exception.translated]` when an exception is mapped to a ProblemDetails response
- [ ] AC‑4: Emit `[middleware.exception.end]` with duration
- [ ] AC‑5: Middleware participates in correlation ID propagation
- [ ] AC‑6: No PII or sensitive data is logged
- [ ] AC‑7: Test harness can assert exception middleware observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — Exception behavior is visible and predictable.
- **EG‑06 Developer Confidence** — Failures are diagnosable without guesswork.

## Notes
This story covers the exception handling middleware only.  
Other middleware types are covered in separate stories.
