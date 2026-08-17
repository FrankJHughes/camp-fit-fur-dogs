# Frank.Core.Api — Middleware

The Frank Core API middleware stack provides cross‑cutting concerns applied uniformly to every HTTP request and response. These middleware components enforce security, observability, consistency, and reliability across all products built on the Frank platform.

Middleware is applied in a strict, intentional order through `UseFrankCoreApiPlatform()`.

---

## Middleware Stack (Execution Order)

1. **CORS Middleware** — Allows cross‑origin requests from configured origins  
2. **Security Headers Middleware** — Applies hardened OWASP‑recommended headers  
3. **Observation Middleware** — Creates correlation context and tracks request lifecycle  
4. **Exception Handling Middleware** — Converts exceptions into structured responses  
5. **Logging Middleware** — Emits structured JSON logs with correlation metadata  

This ordering ensures that routing, security, observability, and error handling behave consistently across all environments.

---

## Security Headers Middleware

Frank applies a hardened set of security headers to every response:

- `X-Content-Type-Options: nosniff`  
- `X-Frame-Options: DENY`  
- `Content-Security-Policy: strict-baseline`  
- `Referrer-Policy: strict-origin-when-cross-origin`  

These defaults reduce attack surface by preventing MIME sniffing, clickjacking, unsafe script execution, and excessive referrer leakage.

---

## Observation Middleware

Observation is split into **inbound** and **outbound** phases.

### Inbound Observation
- Generates a correlation ID  
- Captures request start time  
- Establishes an `IObservationContext` for downstream components  

### Outbound Observation
- Adds correlation ID to response headers  
- Computes total request duration  
- Finalizes observation context for logging and tracing  

Handlers and other middleware can access correlation metadata via `IObservationContext`.

---

## Exception Handling Middleware

Frank’s exception middleware converts exceptions into structured, predictable responses:

### Application Exceptions
Mapped to appropriate HTTP status codes:
- `400` — validation failures  
- `403` — authorization failures  
- `404` — resource not found  

### Unhandled Exceptions
- Returned as `500 Internal Server Error`  
- Response includes correlation ID for traceability  
- Full details logged using the observation context  

This ensures that products never leak stack traces or internal details to clients.

---

## CORS Middleware

Frank’s CORS middleware:

- Resolves allowed origins from configuration  
- Normalizes and caches origin URLs  
- Allows credentials (cookies, auth headers)  
- Logs all cross‑origin requests for observability  

This enables secure cross‑origin access for frontends while preventing unauthorized origins.

---

## Logging Middleware

Every request produces a structured JSON log entry containing:

- correlation ID  
- HTTP method and path  
- response status code  
- total request duration  
- authenticated user ID (if present)  

Logs are formatted for cloud aggregation systems and can be correlated across services using the shared correlation ID.

---

This middleware stack forms the backbone of Frank’s API platform, ensuring that every product inherits consistent security, observability, and reliability.
