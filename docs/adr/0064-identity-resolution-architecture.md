# ADR‑0064 — Identity Resolution Architecture v2

## Status  
Accepted  
Supersedes: ADR‑0013 (Server‑Side Identity Resolution)

## Context  
Identity resolution in the original architecture (ADR‑0013) relied on:

- ad‑hoc identity resolution inside endpoints and middleware  
- fake sign‑in endpoints for tests  
- Testcontainers‑backed identity tests  
- mixed responsibility between API, infrastructure, and test harness  
- implicit coupling between authentication, session, and identity mapping  

As the platform evolved with:

- Exclusive OIDC authentication (ADR‑0059)  
- Session token + cookie architecture (ADR‑0056, ADR‑0058)  
- Authentication callback pipeline (ADR‑0041, ADR‑0055)  
- Observation abstractions (ADR‑0060)  
- Registration Engine (ADR‑0061)  
- StartupEngine (ADR‑0062)  
- Endpoint Engine (ADR‑0063)  
- strengthened purity rules and guardrails  

the original identity resolution model became:

- too tightly coupled to HTTP endpoints  
- too dependent on test‑only endpoints  
- too hard to test deterministically  
- too leaky across layers (API, infrastructure, domain)  

A new, cleaner, **v2 identity resolution architecture** was required.

---

## Decision  

We introduce a **v2 identity resolution architecture** centered on:

- `ICurrentUser`  
- `ICorrelationContext`  
- `IRequestObservationContext`  
- `DefaultHttpContext` + `IHttpContextAccessor`  
- exclusive OIDC authentication  
- session token + cookie model  

### 1. ICurrentUser — canonical identity abstraction

````csharp
public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
````

Rules:

- `ICurrentUser` is the **only** way to access the current user identity in application code.
- Endpoints, handlers, and infrastructure must not read identity directly from `HttpContext`.
- Domain code must not depend on `ICurrentUser`.

### 2. HttpContext‑backed identity resolution

Identity is resolved from `HttpContext` via `IHttpContextAccessor`:

- OIDC authentication populates claims.
- Session token + cookie architecture ensures continuity.
- A dedicated identity resolver maps claims → `ICurrentUser`.

No fake sign‑in endpoints.  
No test‑only identity routes.

### 3. Integration with Observation

Identity is propagated into Observation contexts:

- `RequestObservationContext` includes `UserId` when available.
- `OutboundObservationContextHandler` propagates correlation + channel + agent.
- Observation guardrails assert identity + correlation behavior.

This ties identity resolution to:

- correlation  
- structured events  
- metrics  

### 4. Guardrails

Api.Tests/Guardrails enforce:

- `ICurrentUser` is resolvable via DI.
- identity resolution does not require Testcontainers.
- identity guardrails use `DefaultHttpContext` + `IHttpContextAccessor`.
- no endpoint or handler reads identity directly from `HttpContext`.

Architecture.Tests enforce:

- identity abstractions live in Abstractions, not Domain.
- no domain type depends on `ICurrentUser`.

### 5. Test Harness Integration

Identity tests use:

- `DefaultHttpContext`  
- `IHttpContextAccessor`  
- `ICurrentUser`  

No fake sign‑in endpoints.  
No Testcontainers for identity guardrails.

Identity behavior is tested via:

- claims injection into `HttpContext`  
- resolution into `ICurrentUser`  
- propagation into `RequestObservationContext`.

---

## Consequences  

### Positive  
- Clean separation between identity resolution and business logic.  
- Deterministic identity behavior in tests.  
- No reliance on fake sign‑in endpoints.  
- No Testcontainers dependency for identity guardrails.  
- Stronger purity enforcement (no direct `HttpContext` usage).  
- Tight integration with Observation and correlation.  
- Consistent identity model across API, infrastructure, and test harness.

### Negative  
- Contributors must use `ICurrentUser` instead of `HttpContext.User`.  
- Migration requires updating existing identity resolution code.  
- Tests must be updated to use `DefaultHttpContext` + `IHttpContextAccessor`.

### Neutral  
- Does not change OIDC provider choice.  
- Does not change session token/cookie architecture (already defined in ADR‑0056, ADR‑0058).  

---

## Summary  
Identity Resolution v2 replaces the original server‑side identity resolution model (ADR‑0013) with a cleaner architecture based on:

- `ICurrentUser`  
- `IHttpContextAccessor`  
- OIDC authentication  
- session token + cookie model  
- Observation integration  

It removes fake sign‑in endpoints, removes Testcontainers from identity guardrails, and enforces purity by making `ICurrentUser` the single, governed identity abstraction for application code.

This ADR formalizes the v2 identity resolution architecture and supersedes ADR‑0013.

