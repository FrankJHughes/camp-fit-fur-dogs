# CampFitFurDogs Observability Test Conventions

These conventions define how CampFitFurDogs must test its usage of the Frank Observability platform.

## 1. Test Harness Usage
CampFitFurDogs tests must:
- Use the Frank test harness
- Use deterministic event and metric sinks
- Use deterministic correlation IDs
- Avoid mocking observability interfaces

## 2. Correlation Tests
Tests must verify:
- Correlation ID is propagated through:
  - API handlers
  - domain services
  - infrastructure adapters
  - outbox handlers
- Correlation ID appears in:
  - events
  - metrics

Tests must not generate correlation IDs manually.

## 3. Event Tests
Tests must verify:
- Events are emitted using `ITraceEvents`
- Event names follow conventions
- Payloads are structured
- Correlation ID is included

Tests must not:
- Assert against vendor‑specific exporters
- Use ad‑hoc logging

## 4. Metric Tests
Tests must verify:
- Metrics are emitted using `IMetrics`
- Metric names follow conventions
- Timers use Frank abstractions
- Counters increment deterministically

Tests must not:
- Use Stopwatch
- Depend on real time

## 5. Outbox Tests
Tests must verify:
- Outbox handlers emit events
- Outbox handlers emit metrics
- Correlation ID flows from domain → outbox → handler

## 6. Forbidden Testing Practices
- Manual correlation ID creation  
- Stopwatch usage  
- Vendor‑specific assertions  
- Mocking Frank observability interfaces  
