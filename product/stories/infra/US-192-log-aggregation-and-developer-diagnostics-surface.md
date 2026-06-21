---
id: US-192
title: "Log Aggregation & Developer Diagnostics Surface"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies: []
---

# US‑192 — Log Aggregation & Developer Diagnostics Surface

## Intent
As an **admin**, I must be able to access and query system logs so that I can diagnose authentication and operational issues.

As a **developer**, I must have a structured, queryable log and metrics surface so that I can debug issues efficiently across local, staging, and production environments.

## Value
Without a log aggregation and diagnostics surface, observability data cannot be consumed.  
This story provides the foundation required for all other observability stories to deliver value.

## Acceptance Criteria
- A log aggregation system is available in production  
- Logs are queryable by:
  - Correlation ID  
  - Event name  
  - Layer (api/protocol/application)  
- Metrics are viewable in a dashboard or metrics surface  
- Local development includes:
  - Structured console logs  
  - Optional local log viewer (e.g., Seq)  
- Production includes:
  - Structured logs emitted to stdout or a hosted sink  
  - Metrics emitted to the hosting abstraction  
- Documentation provided for:
  - How Admins access logs  
  - How Developers access logs  
  - How to trace a request end‑to‑end  

## Emotional Guarantees
- **EG‑01 No Surprises** — Admins know where to find logs and how to use them.  
- **EG‑06 Developer Confidence** — Developers have a reliable debugging surface.  
- **EG‑05 Clear Boundaries** — Logs and metrics are separated by layer.

## Example Log Consumption
Admin querying logs by correlation ID:

```
query logs
| where correlationId == "c-9f3a1d"
| order by timestamp asc
```

Developer viewing local logs:

```
info: oidc.token.exchange.start { correlationId=c-9f3a1d }
info: oidc.token.exchange.end { correlationId=c-9f3a1d, status=success }
```

## Actor Consumption
Admins consume logs through the production log aggregation system. They use dashboards, queries, and correlation‑ID traces to diagnose issues.

Developers consume logs through local console output, container logs, and integration test traces. They use correlation‑ID traces to debug authentication flows.

## Notes
- This story is a prerequisite for US‑193, US‑194, US‑195, and US‑196.  
