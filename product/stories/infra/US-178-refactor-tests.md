---
id: US-178
title: "Refactor Tests"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-177
---

# US‑178 — Refactor Tests

## Intent

As an **admin**, I must have a clean, deterministic, and maintainable test suite so that regressions are caught early, tests are easy to understand, and the system remains stable as new features are added.

## Acceptance Criteria

- [ ] AC‑1: All tests follow the Arrange‑Act‑Assert pattern with no inline setup duplication  
- [ ] AC‑2: All tests use the unified test harness entry point (no direct handler instantiation)  
- [ ] AC‑3: All tests run deterministically and pass when executed in parallel  
- [ ] AC‑4: All tests use fake infrastructure provided by the harness (no real email, no real clock)  
- [ ] AC‑5: Test names follow the convention `<Unit>_<Behavior>_<Expectation>`  
- [ ] AC‑6: All obsolete or redundant tests are removed or consolidated  

## Notes

This story ensures the test suite reflects the Frank architecture and remains maintainable long‑term.
