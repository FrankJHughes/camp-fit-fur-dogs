# CampFitFurDogs Observability Architecture Conventions

CampFitFurDogs uses the Frank Observability platform.  
These conventions define how CampFitFurDogs must integrate with the platform at the architectural level.

## 1. Observability Context Usage
CampFitFurDogs must:
- Accept the `IObservabilityContext` provided by Frank.Hosting
- Never create its own observability context
- Propagate the context through:
  - API handlers
  - Domain services
  - Infrastructure adapters
  - Outbox handlers

The context is immutable and must not be modified.

## 2. Correlation Propagation
CampFitFurDogs must:
- Use the correlation ID provided by Frank
- Include correlation IDs in:
  - Domain events
  - Outbox messages
  - External calls
  - Error events

CampFitFurDogs must not generate correlation IDs manually.

## 3. Event Architecture
CampFitFurDogs must:
- Emit structured events for:
  - API request handling
  - Domain actions
  - Outbox processing
  - External system interactions
- Use Frank’s `ITraceEvents` abstraction
- Follow the naming pattern: `slice.module.action`

## 4. Metric Architecture
CampFitFurDogs must:
- Emit metrics for:
  - Handler execution duration
  - Domain operation duration
  - External call latency
  - Success/failure counters
- Use Frank’s `IMetrics` abstraction
- Follow the naming pattern: `slice.module.metric_name`

## 5. External System Integration
CampFitFurDogs infrastructure components must:
- Propagate correlation IDs to external systems
- Emit events for:
  - External call start/end
  - Retries
  - Failures
- Emit metrics for:
  - Latency
  - Success/failure

## 6. Forbidden Architectural Practices
- Manual correlation ID creation  
- Ad‑hoc logging  
- Vendor‑specific logging/metrics APIs  
- Mutable observability context  
