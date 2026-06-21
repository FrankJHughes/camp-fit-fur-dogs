---
id: US-196
title: "Authentication Metrics"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-192
---

# US‑196 — Authentication Metrics

## Intent
As an **admin**, I must be able to see high‑level authentication metrics so that I can monitor system health and detect anomalies.

As a **developer**, I must have metrics that help me identify authentication issues quickly without relying solely on logs.

## Value
Metrics provide operational insight into login success rates, failures, and session creation trends.  
This enables proactive detection of outages, misconfiguration, or abuse.

## Acceptance Criteria
- Metric emitted for login attempts  
- Metric emitted for login failures (protocol vs business)  
- Metric emitted for sessions created  
- Metric emitted for sessions expired  
- Metrics follow hosting abstraction conventions  
- No PII included in metric dimensions  
- Developer can use metrics to identify failure patterns  

## Emotional Guarantees
- **EG‑03 Calm Protection** — Metrics help detect issues early without exposing sensitive data.  
- **EG‑05 Clear Boundaries** — Metrics reflect system behavior, not user identity.  
- **EG‑06 Developer Confidence** — Metrics make debugging faster and more reliable.

## Example Metrics
Successful login:

```
metric: auth.login.attempt
value: 1
labels: { provider: "auth0" }

metric: auth.login.success
value: 1
labels: { provider: "auth0" }

metric: auth.session.created
value: 1
labels: { sessionType: "owner" }
```

Failed login:

```
metric: auth.login.failure
value: 1
labels: { provider: "auth0", reason: "protocol" }
```

## Actor Consumption
Admins consume metrics through the hosting abstraction’s metrics surface.

Developers consume metrics through local dashboards or console output.

## Notes
- No dashboards or external sinks included in this story.  
