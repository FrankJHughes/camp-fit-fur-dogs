# ADR‑0049 — PR Preview URL Resolution Architecture

## Status  
Accepted

## Context  
PR Preview environments are dynamically created for each pull request.  
Early implementations relied on ad‑hoc environment variables and manual configuration, which caused:

- Incorrect callback URLs for authentication  
- Incorrect CORS origins  
- Inconsistent frontend → backend routing  
- Difficulty reproducing preview issues locally  
- Risk of leaking production URLs into preview environments  

A deterministic, provider‑agnostic model was required.

## Decision  
The system now uses a **unified PR Preview URL Resolution Architecture**.

### Characteristics

1. **Hosting provider abstraction**  
   - Vercel preview URLs are resolved through Frank’s hosting provider model  
   - No provider‑specific logic in product code

2. **Deterministic URL construction**  
   - Preview API URL  
   - Preview frontend URL  
   - Preview callback URL  
   - Preview CORS origin  
   All derived from a single preview identifier.

3. **Environment‑aware configuration**  
   - Preview URLs are injected into configuration objects  
   - No conditional logic in slices or endpoints

4. **Fail‑fast validation**  
   - Missing or malformed preview URLs cause startup failure  
   - Prevents silent misconfiguration

5. **Local reproduction**  
   - Preview URLs can be simulated locally using the hosting abstraction  
   - Enables debugging of preview‑specific issues

## Consequences  

### Positive  
- Predictable, stable PR Preview behavior.  
- Authentication, CORS, and routing all use the same resolved URLs.  
- No risk of production URL leakage.  
- Easy to reproduce preview issues locally.  
- Hosting provider differences are fully isolated.

### Negative  
- Requires maintenance of hosting provider implementations.  
- Preview URL resolution must remain consistent with provider behavior.
