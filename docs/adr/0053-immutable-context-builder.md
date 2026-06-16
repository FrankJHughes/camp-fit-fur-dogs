# ADR‑0053 — Adoption of ImmutableContextBuilder as a Core Architectural Primitive

## Status  
Accepted

## Context  
CampFitFurDogs previously implemented multi‑stage workflows (e.g., authentication callback, identity resolution, session creation) using a **step‑engine pattern**:

- Mutable context objects  
- Ordered step lists  
- Dispatcher‑driven execution  
- Implicit state transitions  
- High coupling between steps  
- Difficult testability  
- Weak invariant enforcement  

As the system grew, this pattern produced several architectural issues:

- Steps mutated shared state, making flows hard to reason about  
- Ordering bugs were common and difficult to detect  
- Steps leaked cross‑layer concerns (protocol logic inside business logic, etc.)  
- Testing required complex harnesses and deep mocks  
- Guardrails could not reliably enforce purity or immutability  
- Authentication callback logic became increasingly fragile  

The authentication refactor (US‑110, US‑111) introduced a new pattern:

> **ImmutableContextBuilder<TRequest, TContext, TResult>**  
> A deterministic, pure, strongly‑typed pipeline primitive for multi‑stage transformations.

This pattern proved superior in every dimension:

- Immutable state  
- Deterministic transitions  
- Strong typing  
- Clear separation of protocol vs business logic  
- Full testability at unit, pipeline, and integration levels  
- Perfect alignment with Architecture Governance  
- No mutable context, no step engine, no dispatcher pipeline  

The builder pattern is now used in:

- Frank Auth Callback Pipeline (protocol)  
- Application Auth Callback Pipeline (business)  
- Identity mapping  
- Session creation  
- Redirect computation  

Given its success and architectural clarity, the builder must be formalized as a **core primitive**.

## Decision  
CampFitFurDogs adopts **ImmutableContextBuilder** as the **standard mechanism** for implementing multi‑stage, deterministic, pure transformations across the system.

### 1. ImmutableContextBuilder is now a first‑class architectural primitive  
It joins:

- StartupEngine  
- HostingEngine  
- Endpoint discovery  
- Validator scanning  
- Environment abstraction  

as a core Frank‑level construct.

### 2. All new multi‑stage flows must use ImmutableContextBuilder  
This includes:

- Authentication flows  
- Payment provider callbacks  
- Webhook processing  
- Multi‑stage normalization  
- Multi‑stage validation  
- Multi‑stage redirect logic  

### 3. Builders must follow strict layering rules  
- **Frank builders**: protocol logic only  
- **Application builders**: business logic only  
- **Domain**: no builders  
- **Api**: orchestrates builders, issues cookies, shapes responses  

### 4. Builders must be pure and immutable  
- No mutation  
- No overwriting  
- No clearing  
- No side effects  
- No Infrastructure access in Application builders  
- No business logic in Frank builders  

### 5. Builders replace the step engine  
The following are now deprecated:

- `IAuthCallbackStep`  
- Step lists  
- Mutable callback contexts  
- Dispatcher‑driven step execution  

### 6. Builders must be tested at three levels  
- Unit tests (individual transformations)  
- Pipeline tests (full builder)  
- Guardrail tests (immutability, purity, invariants)  

### 7. Authentication callback architecture is permanently builder‑based  
The three‑layer model is now canonical:

1. **Frank pipeline** — protocol  
2. **Application pipeline** — business  
3. **Api endpoint** — boundary  

## Consequences  

### Positive  
- Deterministic, predictable flows  
- Strong typing across all stages  
- Immutable state eliminates ordering bugs  
- Clear separation of concerns  
- Perfect alignment with Architecture Governance  
- Builders are trivial to test  
- Authentication callback is now fully composable and safe  
- Guardrails can enforce invariants reliably  
- No more mutable context objects  
- No more step engine complexity  

### Neutral  
- Requires developers to learn the builder pattern  
- Requires updates to documentation and conventions  

### Negative  
- Existing step‑engine‑based flows must be migrated over time  
- Builders require more upfront structure (TRequest, TContext, TResult)  

---

## Summary  
CampFitFurDogs standardizes **ImmutableContextBuilder** as the architectural foundation for all multi‑stage transformations.  
It replaces the step engine, enforces purity and immutability, and provides a deterministic, testable, governance‑aligned mechanism for complex flows.

This ADR formalizes the pattern and ensures consistent usage across the entire system.
