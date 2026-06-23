# Frank Observability User Guide

This guide explains observability from the perspective of **users of the Frank platform** — developers, operators, and maintainers who rely on observability outputs to understand system behavior.

This guide does *not* describe how to implement observability (developer guide) or how to test it (tester guide).  
It describes how to **use** the observability data produced by Frank.

---

## 1. What Observability Provides to Users

Frank Observability provides:
- End-to-end correlation IDs  
- Structured, queryable events  
- Vendor-agnostic metrics  
- Consistent naming across all slices and modules  
- Deterministic behavior in test and production environments  

Users consume this data through:
- Logs/event streams  
- Metrics dashboards  
- Tracing systems  
- CI/CD observability reports  

---

## 2. Understanding Correlation IDs

Every request, domain action, and infrastructure call is associated with a correlation ID.

Users can:
- Trace a request across API → domain → infrastructure  
- Identify where failures occur  
- Understand latency contributions  
- Reconstruct execution paths  

Correlation IDs appear in:
- Events  
- Metrics  
- Outbox messages  
- External system calls  

---

## 3. Understanding Structured Events

Events follow the naming pattern:

````  
slice.module.action  
````

Users can:
- Filter events by slice or module  
- Identify domain actions  
- Understand API behavior  
- Diagnose failures  
- Analyze retries and external call behavior  

Event payloads are structured and consistent across the platform.

---

## 4. Understanding Metrics

Metrics follow the naming pattern:

````  
slice.module.metric_name  
````

Users can:
- Monitor latency  
- Track success/failure counts  
- Observe retry behavior  
- Measure domain operation duration  
- Build dashboards and alerts  

Metrics are vendor-agnostic and consistent across all applications built on Frank.

---

## 5. Observability in CI/CD

Users can:
- Inspect event/metric output from test runs  
- Validate deterministic behavior  
- Identify regressions in event/metric emission  
- Ensure new features emit required observability data  

---

## 6. Observability in Production

Users can:
- Trace customer requests  
- Diagnose failures  
- Monitor performance  
- Understand system health  
- Investigate anomalies  

Frank Observability ensures consistent, structured, correlated data across all environments.

---

## 7. User Responsibilities Summary

Users must:
- Understand correlation IDs  
- Interpret structured events  
- Interpret metrics  
- Use observability data for debugging and monitoring  
- Report gaps in observability coverage  

Frank Observability provides the data; users interpret it.
