---
id: US-018
title: "Domain Model Core Aggregates"
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
# Domain Model — Core Aggregates

## Intent
Implement the Dog and Guardian aggregate roots with their value objects, invariants, and domain events as the first real domain model in the system.

## Value
Proves the DDD architecture end-to-end and establishes the pattern every future aggregate will follow.

## Acceptance Criteria
- [x] Dog aggregate root with identity, name, breed, and weight value objects
- [x] Guardian aggregate root with identity, name, and contact value objects
- [x] Domain invariants enforced in constructors and methods
- [x] Domain events raised for creation and state transitions
- [x] Unit tests cover all invariants and event emissions

## Emotional Guarantees: N/A

