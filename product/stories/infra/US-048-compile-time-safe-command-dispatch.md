---
id: US-048
title: "Compile Time Safe Command Dispatch"
epic: ""
milestone: ""
status: backlog
domain: infra
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# US-048 — Compile-Time-Safe Command Dispatch

## Intent

As a developer, I want the command dispatcher to use compile-time-safe resolution so that missing or misconfigured handlers are caught at build time, not at runtime.

## Value

The current `CommandDispatcher` uses reflection and `dynamic` to resolve handlers. This is invisible to the compiler — a missing handler registration silently compiles and explodes at runtime. Replacing it with a source-generated or explicit registration approach makes the system deterministic and refactor-safe.

## Acceptance Criteria

- [ ] `CommandDispatcher` no longer uses reflection or `dynamic` for handler resolution
- [ ] A missing handler registration produces a compile-time error, not a runtime exception
- [ ] All existing command/handler pairs continue to work with no behavior change
- [ ] No new external dependencies required (source generators or manual registration are both acceptable)
- [ ] Existing tests pass without modification

## Emotional Guarantees

- EG-01 No surprises

