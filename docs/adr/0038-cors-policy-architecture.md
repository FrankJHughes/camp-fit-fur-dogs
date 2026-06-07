# ADR‑0038 — CORS Policy Architecture

## Status  
Accepted

## Context  
The initial implementation of CORS relied on permissive defaults (`AllowAnyOrigin`, `AllowAnyHeader`, `AllowAnyMethod`) to simplify early development.  
As the system matured, several issues emerged:

- Wildcard CORS behavior created unnecessary security exposure.  
- Origins differed across local, preview, and production environments.  
- PR Preview environments required dynamic origin resolution.  
- Preflight requests were inconsistently handled across endpoints.  
- Some error responses did not include CORS headers because middleware ordering was incorrect.  
- Browsers exhibited inconsistent behavior when wildcard rules interacted with credentials.

CORS became a cross‑cutting architectural concern requiring deterministic, environment‑aware behavior.

## Decision  
The system now uses a **hardened, explicit CORS policy** with the following characteristics:

1. **Explicit allow‑lists**  
   - Origins, methods, and headers are explicitly enumerated.  
   - No wildcard allowances remain.

2. **Environment‑aware origin resolution**  
   - Local development: `http://localhost:*`  
   - Preview: dynamically resolved Vercel preview URL  
   - Production: canonical production frontend URL  

3. **Deterministic preflight handling**  
   - Explicit validation of requested methods and headers  
   - Explicit `Access-Control-Max-Age` for caching  

4. **Correct middleware ordering**  
   - CORS executes **before** authentication and authorization  
   - Ensures error responses include correct CORS headers  

5. **Structured logging**  
   - Blocked origins are logged with structured metadata for observability.

## Consequences  
### Positive  
- Stronger security posture by eliminating wildcard CORS behavior.  
- Predictable browser behavior across all environments.  
- PR Preview environments work seamlessly with dynamic origins.  
- Error responses consistently include CORS headers.  
- Improved debugging and observability for blocked origins.

### Negative  
- Requires ongoing maintenance of allow‑lists as environments evolve.  
- Misconfigured origins now fail fast instead of silently succeeding.  
