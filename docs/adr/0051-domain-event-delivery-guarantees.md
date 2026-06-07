# ADR‑0051 — Domain Event Delivery Guarantees

## Status  
Accepted

## Context  
Domain events are used to propagate changes within the system, particularly after aggregate mutations.  
As event usage increased, several questions emerged:

- When are events dispatched?  
- Are events guaranteed to run exactly once?  
- What happens if a handler fails?  
- How do events interact with the Unit of Work?  
- Should events be durable or in‑memory?  

A clear delivery model was required to ensure predictable behavior.

## Decision  
The system now uses an **in‑process, post‑commit domain event delivery model** with the following guarantees:

### 1. Delivery Timing  
- Events are dispatched **only after** the Unit of Work commits successfully.  
- No event is dispatched if the transaction fails.

### 2. Delivery Semantics  
- Delivery is **at‑least‑once** within the same process.  
- Handlers must be idempotent.  
- No cross‑process delivery guarantees (not a message bus).

### 3. Handler Execution  
- Handlers run synchronously after commit.  
- Handler failures are logged but do not roll back the transaction.  
- Handlers may dispatch additional commands or queries.

### 4. Event Storage  
- Events are not persisted.  
- Events exist only in memory during the request lifecycle.

### 5. Scope  
- Domain events are internal to the backend.  
- They are not exposed to external systems.

## Consequences  

### Positive  
- Predictable, deterministic event dispatching.  
- No risk of events firing before transaction commit.  
- Simple, testable event model.  
- No external infrastructure required.

### Negative  
- No cross‑process or durable delivery.  
- Handlers must be idempotent to avoid side effects.  
- Not suitable for long‑running or asynchronous workflows (future ADR).
