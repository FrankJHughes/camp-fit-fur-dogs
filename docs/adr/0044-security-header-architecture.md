# ADR‑0044 — Security Header Architecture

## Status  
Accepted

## Context  
Early versions of the API returned minimal or inconsistent security headers.  
As the system matured, several requirements emerged:

- Browsers must enforce modern security protections by default.  
- Headers must be applied consistently across all endpoints, including errors.  
- Security headers must not conflict with CORS or authentication flows.  
- PR Preview environments must behave identically to production.  
- Middleware ordering must ensure headers are applied after routing.

Security headers became a cross‑cutting architectural concern requiring a unified model.

## Decision  
The system now applies a **standardized security‑header set** across all API responses.

### Header Set

1. **Strict‑Transport‑Security (HSTS)**  
   - Enabled in all non‑local environments  
   - Ensures HTTPS‑only communication

2. **X‑Content‑Type‑Options: nosniff**  
   - Prevents MIME‑type sniffing

3. **X‑Frame‑Options: DENY**  
   - Prevents clickjacking

4. **Referrer‑Policy: strict‑origin‑when‑cross‑origin**  
   - Limits referrer leakage

5. **X‑XSS‑Protection: 0**  
   - Disables legacy, unsafe browser XSS filters

6. **Content‑Security‑Policy (CSP)**  
   - Minimal baseline CSP applied  
   - Avoids interfering with frontend frameworks

### Middleware Ordering  
Security headers are applied:

1. Once early in the pipeline (baseline)  
2. Once after routing (final pass)  

This ensures all responses—including errors—receive the correct headers.

## Consequences  

### Positive  
- Stronger browser‑level security posture.  
- Consistent behavior across all environments.  
- Predictable interaction with CORS and authentication.  
- Reduced risk of clickjacking, MIME sniffing, and referrer leakage.

### Negative  
- CSP must be maintained carefully to avoid breaking frontend behavior.  
- Additional middleware passes increase pipeline complexity.  
