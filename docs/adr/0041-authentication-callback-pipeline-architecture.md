# ADR‑0041 — Authentication Callback Pipeline Architecture

## Status  
Accepted

## Context  
The authentication callback originally consisted of a single service that performed multiple responsibilities:

- Exchanging the authorization code  
- Fetching userinfo  
- Validating identity data  
- Mapping external identities to internal Owners  
- Creating new Owners when necessary  
- Issuing session cookies  
- Building the redirect response  

As authentication requirements expanded—session management, identity mapping, error handling, observability—the callback logic became increasingly complex.  
A monolithic implementation made it difficult to test, reason about, and extend.

A structured, deterministic pipeline was needed.

## Decision  
The system now uses a **step‑based authentication callback pipeline**, where each step performs one responsibility and mutates a shared context object.

### Pipeline Characteristics

1. **Deterministic ordering**  
   Steps execute in a fixed, predictable sequence.

2. **Single‑responsibility steps**  
   Each step performs exactly one operation:
   - Validate configuration  
   - Exchange authorization code  
   - Fetch userinfo  
   - Validate userinfo  
   - Resolve or create Owner identity  
   - Issue session cookie  
   - Create session record  
   - Build redirect response  
   - Audit login  

3. **Shared context object**  
   Steps read and write to a strongly‑typed `AuthCallbackContext`.

4. **Short‑circuiting**  
   Any step may fail early with a typed error.

5. **Auto‑registration**  
   Steps are registered explicitly in `AddApplication()` to ensure ordering clarity.

6. **Purity alignment**  
   - No domain logic in the callback endpoint  
   - All domain interactions occur inside the pipeline steps  
   - Endpoint remains thin and pure

## Consequences  

### Positive  
- Highly testable: each step can be tested independently.  
- Predictable behavior: deterministic ordering eliminates ambiguity.  
- Extensible: new steps can be added without modifying existing ones.  
- Clear separation of concerns.  
- Improved observability and error reporting.

### Negative  
- Requires careful maintenance of step ordering.  
- More files and types compared to a monolithic implementation.  
