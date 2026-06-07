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

## Decision  
The system now uses a **centralized API Error Boundary** that standardizes all error responses.

### Characteristics

1. **Single error shape**  
   All errors conform to a unified structure:
   ```json
   {
     "type": "validation|domain|authentication|authorization|invariant|unknown",
     "message": "Human‑readable message",
     "details": { ... }
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

## Consequences  

### Positive  
- Predictable error responses across all endpoints.  
- Simplified frontend error handling.  
- Improved observability and logging.  
- Clean separation between domain logic and error formatting.  
- Consistent behavior across local, preview, and production.

### Negative  
- Requires ongoing maintenance of exception mappings.  
- All new exception types must be integrated into the boundary.
