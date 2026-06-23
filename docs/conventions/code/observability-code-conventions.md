# CampFitFurDogs Observability Code Conventions

These conventions define how CampFitFurDogs code must use the Frank Observability platform.

## 1. Event Naming
CampFitFurDogs must follow:
````  
slice.module.action  
````
Examples:
- `customer.registration.created`
- `infra.email.sent`
- `api.owner.login_failed`

## 2. Metric Naming
CampFitFurDogs must follow:
````  
slice.module.metric_name  
````
Examples:
- `api.login.duration_ms`
- `infra.outbox.retry_count`
- `domain.customer.created_count`

## 3. Event Emission Rules
CampFitFurDogs must:
- Emit events using `ITraceEvents`
- Use structured payloads (no strings with embedded JSON)
- Include correlation ID automatically (Frank handles this)
- Include:
  - slice
  - module
  - action
  - severity
  - payload

CampFitFurDogs must not:
- Use ad‑hoc logging
- Use `Console.WriteLine`
- Use vendor‑specific logging APIs

## 4. Metric Emission Rules
CampFitFurDogs must:
- Use `IMetrics` for all metrics
- Use timers from Frank (no Stopwatch)
- Emit:
  - counters
  - gauges
  - timers

CampFitFurDogs must not:
- Use Stopwatch
- Use vendor‑specific metric libraries

## 5. Correlation Rules
CampFitFurDogs must:
- Accept correlation ID from Frank
- Propagate correlation ID through:
  - domain services
  - infrastructure calls
  - outbox handlers
- Never generate correlation IDs manually

## 6. Forbidden Code Practices
- Manual correlation ID creation  
- Stopwatch timing  
- Ad‑hoc logging  
- Vendor‑specific APIs  
- Mutable observability context  
