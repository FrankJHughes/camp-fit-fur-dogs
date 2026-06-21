# Frank Security Headers Middleware — Tester Guide

This guide documents the **current state** of the Frank Security Headers Middleware and the **intended future state** as the platform evolves.

The Security Headers Middleware enforces a hardened, OWASP‑aligned set of HTTP response headers.  
Testers validate the *behavior* of the middleware — not the behavior of the application using it.

---

# 1. Current State (Before Future Enhancements)

The middleware currently:

- Adds a strict set of modern security headers  
- Applies a hardened Content‑Security‑Policy (CSP)  
- Uses a `SetIfMissing` pattern to avoid overriding upstream headers  
- Implements `IMiddleware` (DI‑friendly and testable)  
- Registers via `AddSecurityHeaders()`  

### What exists today (testable behavior)

- **Header Presence**
  - X‑Content‑Type‑Options = `nosniff`
  - X‑Frame‑Options = `DENY`
  - X‑XSS‑Protection = `0`
  - Referrer‑Policy = `strict-origin-when-cross-origin`
  - Permissions‑Policy = `geolocation=(), microphone=(), camera=(), payment=(), usb=()`
  - Cross‑Origin‑Opener‑Policy = `same-origin`
  - Cross‑Origin‑Embedder‑Policy = `require-corp`
  - Cross‑Origin‑Resource‑Policy = `same-origin`

- **CSP Presence**
  - Must include:  
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

- **SetIfMissing Behavior**
  - Middleware must NOT override headers already set by upstream components.

- **Pipeline Behavior**
  - Middleware must call `next(context)`  
  - Middleware must not throw  
  - Middleware must not mutate request headers  
  - Middleware must not break streaming responses  

### What does *not* exist today (not testable)

- No configuration API  
- No dynamic CSP generation  
- No nonce or hash support  
- No reporting endpoints  
- No metadata‑driven behavior  
- No diagnostics or logging  
- No per‑environment profiles  

### What testers validate today

- All expected headers are present  
- CSP is present and contains required directives  
- Middleware does not override existing headers  
- Middleware does not interfere with the pipeline  
- Middleware is stateless  
- Middleware is safe to register as transient  
- Middleware behaves consistently across requests  

The current capability is intentionally strict and predictable.

---

# 2. Future Intent (After Capability Expansion)

As the platform evolves, testers will validate richer and more dynamic behavior.

### 2.1 Configurable Security Profiles

Future tests will validate:

- Profile selection (strict API, relaxed UI, development mode, etc.)  
- Profile‑specific header sets  
- Profile‑specific CSP rules  

### 2.2 Dynamic CSP Generation

Future tests will validate:

- Nonce generation  
- Hash‑based inline script support  
- Per‑request CSP mutation  
- Automatic nonce injection into UI frameworks  

### 2.3 Reporting and Monitoring

Future tests will validate:

- CSP violation reporting  
- Logging of blocked resources  
- Integration with observability pipelines  

### 2.4 Metadata‑Driven Behavior

Future tests will validate:

- Endpoint attributes that modify CSP  
- Automatic relaxation for file uploads or streaming endpoints  
- Per‑endpoint header overrides  

### 2.5 Middleware Pipeline Integration

Future tests will validate:

- Automatic registration via the Startup Engine  
- Ordering guarantees  
- Conflict detection with other middleware  

These enhancements will significantly expand the testing surface.

---

# 3. Required Test Types (Current State)

## 3.1 Header Presence Tests

Validate that each expected header is present with the correct value.

### Example

```csharp
var response = await client.GetAsync("/test");
Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
```

---

## 3.2 CSP Tests

Validate:

- CSP header exists  
- Required directives are present  
- No forbidden directives appear  
- CSP is not empty  

### Example

```csharp
var csp = response.Headers.GetValues("Content-Security-Policy").Single();
Assert.Contains("default-src 'self'", csp);
Assert.Contains("frame-ancestors 'none'", csp);
```

---

## 3.3 SetIfMissing Tests

Validate:

- Middleware does NOT override existing headers  
- Upstream headers take precedence  

### Example

```csharp
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    await next();
});
```

Expected:

```
X-Frame-Options = SAMEORIGIN
```

---

## 3.4 Pipeline Continuation Tests

Validate:

- `next(context)` is always called  
- Middleware does not short‑circuit  
- Middleware does not throw  

---

## 3.5 Statelessness Tests

Validate:

- Middleware has no instance fields  
- Multiple requests behave identically  
- Middleware is safe to register as transient  

---

# 4. Anti‑Patterns (Tests Must Reject)

- Tests that assume configurable profiles  
- Tests that assume dynamic CSP generation  
- Tests that assume nonce or hash support  
- Tests that assume metadata‑driven behavior  
- Tests that assume diagnostics or logging  
- Tests that assume per‑environment behavior  
- Tests that assume UI‑friendly CSP rules  

These features do **not** exist today.

---

# 5. Summary

**Current State:**  
Testers validate:

- Security header presence  
- CSP correctness  
- Non‑overriding behavior  
- Pipeline continuation  
- Statelessness  
- Consistent behavior across requests  

**Future Intent:**  
Testers will validate:

- Configurable profiles  
- Dynamic CSP generation  
- Reporting and diagnostics  
- Metadata‑driven behavior  
- Startup Engine integration  

This Tester Guide prepares testers for both the current strict middleware and the future Security Headers Engine.

