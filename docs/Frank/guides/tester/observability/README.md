# Frank Observability Tester Guide

This guide explains how testers validate the correctness, determinism, and integration behavior of the Frank Observability subsystem across hosting, startup, modules, domain, and infrastructure.

---

## 1. Purpose of Observability Testing

Testers ensure:
- Correlation is deterministic and correctly propagated
- Events are emitted with correct names, payloads, and correlation
- Metrics are emitted with correct names and deterministic timing
- Engines (hosting, startup, modules) integrate correctly
- No forbidden patterns exist in the codebase

---

## 2. Test Harness Requirements

Frank.Testing provides:
- Deterministic correlation ID generation  
- Deterministic event sinks  
- Deterministic metric sinks  
- Real hosting pipeline execution  
- Real startup engine execution  

Testers must **not** mock observability interfaces unless testing the interfaces themselves.

---

## 3. Correlation Tests

Tests must verify:
- Correlation ID creation is deterministic  
- Correlation flows through:
  - Hosting engine  
  - Startup engine  
  - Modules  
  - Domain services  
  - Infrastructure adapters  
  - Outbox handlers  
- Correlation ID appears in all events and metrics  

Tests must **not** generate correlation IDs manually.

---

## 4. Event Tests

Tests must verify:
- Events are emitted using `ITraceEvents`
- Event names follow `slice.module.action`
- Payloads are structured and serializable
- Correlation ID is injected automatically

Tests must **not**:
- Assert against vendor-specific exporters  
- Use ad-hoc logging  

---

## 5. Metric Tests

Tests must verify:
- Metrics are emitted using `IMetrics`
- Metric names follow `slice.module.metric_name`
- Timers use Frank abstractions (no Stopwatch)
- Counters increment deterministically

Tests must **not**:
- Depend on real time  
- Use Stopwatch  

---

## 6. Engine Integration Tests

Testers must validate:
- Hosting engine creates and propagates context  
- Startup engine emits events and metrics  
- Modules propagate correlation and emit events  
- Infrastructure adapters propagate correlation and emit metrics  

---

## 7. Forbidden Testing Practices

- Manual correlation ID creation  
- Stopwatch usage  
- Vendor-specific assertions  
- Mocking Frank observability interfaces  
- Asserting against real exporters  

---

## 8. Tester Responsibilities Summary

Testers must:
- Validate determinism  
- Validate naming conventions  
- Validate propagation  
- Validate event/metric correctness  
- Ensure no forbidden patterns exist  

Frank Observability defines the rules; testers ensure they are followed.
