# CampFitFurDogs Observability Workflow Conventions

These conventions define how CampFitFurDogs developers must incorporate observability into their workflow.

## 1. Story Requirements
Stories that involve observability must:
- Reference Frank Observability conventions
- Include acceptance criteria for:
  - event emission
  - metric emission
  - correlation propagation
- Avoid implementation details

## 2. Task Requirements
Tasks must:
- Identify which events/metrics are added
- Include test plan for:
  - correlation propagation
  - event emission
  - metric emission
- Update documentation if new events/metrics are added

## 3. PR Requirements
PRs must:
- Include event/metric emission where appropriate
- Include tests for:
  - correlation propagation
  - event correctness
  - metric correctness
- Follow naming conventions
- Remove any ad‑hoc logging

## 4. CI Requirements
CI must validate:
- No forbidden logging APIs
- No Stopwatch usage
- All new events/metrics follow naming conventions
- All tests pass deterministically

## 5. Release Requirements
A feature may ship only when:
- All observability tests pass
- Event/metric catalog is updated
- No forbidden patterns remain
