# ADR‑0047 — API Error Boundary Architecture

## Status  
Accepted

## Context  
As the API surface expanded, error handling became inconsistent across slices:

- Some endpoints returned raw exceptions.  
- Some returned inconsistent error shapes.  
- Validation errors were not always distinguishable from domain errors.  
- Authentication and CORS failures produced different response formats.  
- Frontend forms required predictable error structures for FormCommand integration.  

A unified error boundary was required to ensure consistent, predictable error responses across all API endpoints.

### Additional Architectural Context  
The team evaluated ASP.NET Core’s built‑in exception handling (`UseExceptionHandler`, `ProblemDetails`, `ProblemDetailsFactory`) and determined it could not satisfy the platform’s requirements.

The built‑in system is:

- **HTTP‑only** — cannot be used in workers, background modules, or dispatchers.  
- **Not domain‑aware** — cannot classify domain exceptions or map them to platform error codes.  
- **Not observable** — cannot attach structured metadata (module, handler, correlation IDs, payload context).  
- **Not composable** — no handler pipeline, no ordering, no fallback behavior.  
- **Not testable** — cannot be invoked deterministically in the test harness.  
- **Not unifiable** — cannot provide a single error model across all transports.

Because the platform requires **cross‑transport consistency**, **structured observability**, and **domain‑aware classification**, the built‑in ASP.NET Core exception system was insufficient.

This necessitated a custom error boundary and a custom `ProblemDetails` model.

## Decision  
The system now uses a **centralized API Error Boundary** that standardizes all error responses.

### Characteristics

1. **Single error shape**  
   All errors conform to a unified structure:
   ```json
   {
     "type": "validation|domain|authentication|authorization|invariant|unknown",
     "message": "Human‑readable message",
     "details": { }
   }
   ```

2. **Exception‑to‑error mapping**  
   - Validation exceptions → `validation`  
   - Domain exceptions → `domain`  
   - Authentication/session failures → `authentication`  
   - Authorization failures → `authorization`  
   - Invariant violations → `invariant`  
   - All other exceptions → `unknown`

3. **Middleware‑based implementation**  
   - Wraps the entire pipeline  
   - Ensures all errors pass through the boundary  
   - Ensures CORS and security headers are applied

4. **Purity alignment**  
   - Endpoints remain thin and pure  
   - No endpoint contains try/catch logic  
   - Error boundary handles all exception translation

5. **Frontend compatibility**  
   - FormCommand state machine consumes standardized error shapes  
   - Client‑side error merging is deterministic

### Additional Decision Justification  
The platform requires:

- A **transport‑agnostic** error model (API + workers + modules).  
- **Structured observability** (handler name, module name, correlation ID, payload metadata).  
- **Deterministic classification** of domain exceptions.  
- **Composable handler pipelines** with ordering and fallback behavior.  
- **Testability** within the Frank test harness.  

ASP.NET Core’s built‑in exception handling cannot meet these requirements.  
Therefore, a custom `IExceptionHandler` and custom `ProblemDetails` were implemented.

## Consequences  

### Positive  
- Predictable error responses across all endpoints.  
- Simplified frontend error handling.  
- Improved observability and logging.  
- Clean separation between domain logic and error formatting.  
- Consistent behavior across local, preview, and production.  
- Unified error model across API and background workers.  
- Deterministic, testable error classification.  
- Extensible handler pipeline for future domain needs.

### Negative  
- Requires ongoing maintenance of exception mappings.  
- All new exception types must be integrated into the boundary.  
- Slightly more code than relying on built‑in ASP.NET Core features.  
- Requires discipline to keep domain exceptions mapped correctly.
