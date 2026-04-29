---
id: US-050
title: "Unit Of Work Pattern"
epic: ""
milestone: ""
status: shipped
domain: infra
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# US-050 — Unit of Work Pattern

## Intent

As a developer, I want a Unit of Work abstraction so that cross-aggregate transactions are explicit, consistent, and testable.

## Value

Currently each repository calls `SaveChangesAsync` independently. This works for single-aggregate operations but breaks down when a use case spans multiple aggregates — partial commits become possible. A Unit of Work wraps the entire operation in a single transaction boundary.

## Acceptance Criteria

- [ ] `IUnitOfWork` interface is defined in the Application layer
- [ ] `SaveChangesAsync` is removed from individual repository implementations
- [ ] Command handlers call `IUnitOfWork.CommitAsync()` after all repository operations
- [ ] Infrastructure implements `IUnitOfWork` backed by EF Core's `DbContext`
- [ ] Existing `CreateCustomerHandler` is updated to use the new pattern
- [ ] All existing tests pass without behavior change

## Emotional Guarantees

- EG-01 No surprises

