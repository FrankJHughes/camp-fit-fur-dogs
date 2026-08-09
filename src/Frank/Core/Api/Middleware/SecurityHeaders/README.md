# Security Headers Middleware

The **Security Headers Middleware** subsystem applies a hardened set of modern
security headers to every HTTP response produced by the Frank.Core API.  
These headers follow OWASP recommendations and enforce strict browser‑side
protections against common attack vectors such as MIME sniffing, clickjacking,
XSS, cross‑origin resource leakage, and unsafe embedding.

This folder contains the middleware responsible for applying these headers and
the service registration used to make it available to the ASP.NET Core pipeline.

---

## Files

```
SecurityHeaders/
├── SecurityHeadersMiddleware.cs
└── ServiceCollectionExtensions.cs
```

---

## SecurityHeadersMiddleware

`SecurityHeadersMiddleware` applies a curated set of defensive headers to all
outgoing responses.  
It uses hardened defaults suitable for APIs and backend services, including a
strict Content Security Policy (CSP).

### Responsibilities

- Adds modern OWASP‑aligned security headers:
  - `X-Content-Type-Options: nosniff`
  - `X-Frame-Options: DENY`
  - `X-XSS-Protection: 0`
  - `Referrer-Policy: strict-origin-when-cross-origin`
  - `Permissions-Policy: geolocation=(), microphone=(), camera=(), payment=(), usb=()`
  - `Cross-Origin-Opener-Policy: same-origin`
  - `Cross-Origin-Embedder-Policy: require-corp`
  - `Cross-Origin-Resource-Policy: same-origin`
- Applies a strict CSP baseline:
  - `default-src 'self'`
  - `script-src 'self'`
  - `style-src 'self'`
  - `img-src 'self' data:`
  - `font-src 'self'`
  - `connect-src 'self'`
  - `frame-ancestors 'none'`
  - `object-src 'none'`
  - `base-uri 'self'`
  - `form-action 'self'`
- Ensures headers are only added if not already present.
- Provides consistent hardening across all vertical slices.

### Why this matters

These headers significantly reduce the attack surface by:

- preventing MIME sniffing  
- blocking clickjacking  
- disabling legacy XSS filters  
- restricting browser API access  
- enforcing cross‑origin isolation  
- preventing unsafe resource embedding  
- enforcing a strict CSP suitable for backend APIs  

This middleware ensures every response is protected without requiring slice‑level
configuration.

---

## ServiceCollectionExtensions

`ServiceCollectionExtensions` registers the security‑header middleware with the
DI container.

### Responsibilities

- Registers `SecurityHeadersMiddleware` as a transient service.
- Makes the middleware available for use in the ASP.NET Core pipeline.

### Usage

```csharp
services.AddFrankCoreApiSecurityHeaders();
```

This prepares the middleware for use via:

```csharp
app.UseMiddleware<SecurityHeadersMiddleware>();
```

(Or via your own extension method if you add one.)

---

## How Security Headers Fit Into the Architecture

Security headers are part of the API’s cross‑cutting security layer.  
They provide browser‑side protections that complement server‑side validation,
authentication, authorization, and CORS policies.

This subsystem ensures:

- consistent hardening across all endpoints  
- zero per‑slice configuration  
- alignment with modern OWASP guidance  
- safe defaults for production environments  

---

## Typical Flow

1. **Request enters API**  
2. Slice executes normally  
3. Before response is sent, middleware applies hardened headers  
4. Client receives a fully protected response  

---

## Design Principles

- **Secure by default**  
  Every response is hardened automatically.

- **Non‑intrusive**  
  Middleware does not modify response bodies or interfere with slice logic.

- **OWASP‑aligned**  
  Headers follow modern best practices.

- **Strict CSP**  
  Suitable for APIs and backend services.

- **Idempotent**  
  Headers are only added if missing.

---

## Notes

- Middleware is safe for all environments, including production.
- CSP is intentionally strict; adjust only if your frontend requires broader allowances.
- Works seamlessly with CORS, authentication, and other middleware.

---
