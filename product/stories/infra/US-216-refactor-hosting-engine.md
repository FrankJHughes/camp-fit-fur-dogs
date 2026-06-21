---
id: US-216
title: "Refactor HostingEngine to use ImmutabilityContext"
domain: infra
status: backlog
vertical_slice: false
dependencies:
  - US-185
---

# US‑216 — Refactor HostingEngine to use ImmutabilityContext

## Intent
As an **admin**, I must be able to rely on the HostingEngine using an immutable context model so that hosting modules execute deterministically without hidden state mutation or side effects.

## Value
Refactoring HostingEngine to use `ImmutabilityContext` aligns hosting behavior with the existing immutable callback contexts used for authentication.  
This makes hosting pipelines easier to reason about, safer to extend, and more testable, especially when multiple hosting modules contribute configuration.

## Acceptance Criteria
- [ ] HostingEngine is refactored to use an `ImmutabilityContext`-based hosting context
- [ ] Hosting modules receive an immutable context and return a new context instance
- [ ] No hosting module mutates shared state or previously created context instances
- [ ] Guardrail tests verify deterministic behavior given the same inputs
- [ ] Existing hosting behavior (Kestrel, HTTPS, HSTS, limits, etc.) is preserved
- [ ] Test harness can construct and inspect hosting context snapshots
- [ ] Documentation is updated to describe HostingEngine’s use of `ImmutabilityContext`

## Notes
- Current `ImmutabilityContext` usage in `ApplicationAuthCallbackContext` and `OidcAuthCallbackCallbackContext` should be treated as the reference pattern
- This story does not introduce new hosting features; it is a structural refactor
