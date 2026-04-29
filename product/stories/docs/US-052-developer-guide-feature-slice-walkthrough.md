---
id: US-052
title: "Developer Guide Feature Slice Walkthrough"
epic: ""
milestone: ""
status: shipped
domain: docs
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# US-052 — Developer Guide: Adding a New Feature Slice

## Intent

As a developer, I want a step-by-step walkthrough in the developer guide for adding a new feature slice so that I can follow the established CQRS + TDD pattern without guessing at conventions.

## Value

Every vertical slice shipped so far (Create Customer, Register Dog, View Dog Profile) followed the same red-green-refactor sequence across Domain → Application → Infrastructure → API layers. That sequence lives in tribal knowledge and PR history — not in any guide. A concrete walkthrough (using a representative example) turns implicit convention into explicit onboarding material, reducing ramp-up time and preventing pattern drift as the codebase grows.

## Acceptance Criteria

- `docs/guides/developer-guide.md` includes a new "Adding a new feature slice" section
- Section covers both command (write) and query (read) slice types
- Walkthrough follows the TDD red-green-refactor sequence: Domain → Application → Infrastructure → API
- Each layer step identifies the file(s) to create and the test to write first
- References ADR-0011 (CQRS) for architectural rationale
- References ADR-0002 (DDD) for layer boundaries
- Section is linked from the developer guide table of contents (if one exists)

## Emotional Guarantees

- EG-01 No surprises — the guide matches the actual patterns in the codebase
- EG-04 Always know where you stand — a developer can follow the checklist step by step without ambiguity

