# Frank Security Headers Middleware — Developer Guide

This guide documents the **current state** of the Frank Security Headers Middleware and the **intended future state** as the platform evolves.

The Security Headers Middleware provides a hardened, OWASP‑aligned baseline for HTTP response headers.  
It ensures that all API responses include modern security headers unless explicitly overridden upstream.

This guide focuses on how to *use and extend* the middleware — not how to consume it as an end user.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides the following implementation:

```csharp
public sealed class SecurityHeadersMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = context.Response.Headers;

        SetIfMissing(headers, "X-Content-Type-Options", "nosniff");
        SetIfMissing(headers, "X-Frame-Options", "DENY");
        SetIfMissing(headers, "X-XSS-Protection", "0");
        SetIfMissing(headers, "Referrer-Policy", "strict-origin-when-cross-origin");
        SetIfMissing(headers, "Permissions-Policy", "geolocation=(), microphone=(), camera=(), payment=(), usb=()");
        SetIfMissing(headers, "Cross-Origin-Opener-Policy", "same-origin");
        SetIfMissing(headers, "Cross-Origin-Embedder-Policy", "require-corp");
        SetIfMissing(headers, "Cross-Origin-Resource-Policy", "same-origin");

        var sb = new StringBuilder();
        sb.Append("default-src 'self'; ");
        sb.Append("script-src 'self'; ");
        sb.Append("style-src 'self'; ");
        sb.Append("img-src 'self' data:; ");
        sb.Append("font-src 'self'; ");
        sb.Append("connect-src 'self'; ");
        sb.Append("frame-ancestors 'none'; ");
        sb.Append("object-src 'none'; ");
        sb.Append("base-uri 'self'; ");
        sb.Append("form-action 'self'");
        SetIfMissing(headers, "Content-Security-Policy", sb.ToString());

        return next(context);
    }

    private static void SetIfMissing(IHeaderDictionary headers, string key, string value)
    {
        if (!headers.ContainsKey(key))
        {
            headers[key] = value;
        }
    }
}
```

And the registration extension:

```csharp
public static class SecurityHeadersExtensions
{
    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services)
    {
        services.AddTransient<SecurityHeadersMiddleware>();
        return services;
    }
}
```

### What exists today

- A hardened set of OWASP‑aligned security headers  
- A strict Content‑Security‑Policy baseline  
- A `SetIfMissing` pattern that prevents overriding upstream headers  
- Middleware implemented using `IMiddleware` (DI‑friendly and testable)  
- A clean registration extension (`AddSecurityHeaders`)  
- No configuration required — safe defaults out of the box  

### What does *not* exist today

- No customization API  
- No per‑environment overrides  
- No dynamic CSP generation  
- No nonce or hash support  
- No reporting endpoints  
- No integration with endpoint metadata  
- No diagnostics or logging  

### Developer implications today

- Middleware is stateless and safe to register as transient  
- Middleware must be added to the pipeline manually  
- CSP is static and cannot be modified at runtime  
- Developers must override headers manually if needed  
- Middleware is intended for APIs, not complex web UIs  

The current capability is intentionally strict and minimal.

---

# 2. Future Intent (After Capability Expansion)

As the platform evolves, the Security Headers Middleware will support richer configuration and dynamic behavior.

### 2.1 Configurable Security Header Profiles

Future enhancements may include:

- “Strict API” profile (current default)  
- “Relaxed UI” profile  
- “Development mode” profile  
- “Zero‑trust hardened” profile  

### 2.2 Dynamic CSP Generation

Potential improvements:

- Nonce‑based script/style support  
- Hash‑based inline script support  
- Per‑request CSP mutation  
- Automatic nonce injection into Razor/Blazor  

### 2.3 Reporting and Monitoring

Future support may include:

- `report-uri` / `report-to` integration  
- CSP violation logging  
- Security header diagnostics  
- Integration with observability pipelines  

### 2.4 Metadata‑Driven Behavior

Potential additions:

- Endpoint metadata that modifies CSP  
- Attribute‑based header overrides  
- Automatic relaxation for file uploads or streaming endpoints  

### 2.5 Middleware Pipeline Integration

Future capabilities may include:

- Automatic registration via the Startup Engine  
- Ordering guarantees  
- Conflict detection with other middleware  

These enhancements will transform the subsystem into a configurable, dynamic **Security Headers Engine**.

---

# 3. Developer Responsibilities (Current vs Future)

## Current Responsibilities

Developers must:

- Register the middleware via `AddSecurityHeaders`  
- Add the middleware to the pipeline  
- Override headers manually if needed  
- Ensure CSP does not conflict with frontend requirements  
- Keep endpoints and middleware compatible with strict defaults  

## Future Responsibilities

Once the capability expands, developers will:

- Select or configure security profiles  
- Use metadata to adjust header behavior  
- Integrate CSP nonces into UI frameworks  
- Monitor CSP violations  
- Use diagnostics to validate header correctness  

---

# 4. Architecture and Invariants

### 4.1 Middleware Invariants

- Middleware must remain stateless  
- Middleware must not override existing headers  
- Middleware must apply headers *after* upstream logic  
- Middleware must not break streaming responses  
- Middleware must not mutate request headers  

### 4.2 Security Invariants

- CSP must always include a `default-src` directive  
- No inline scripts or styles allowed by default  
- No external origins allowed by default  
- Frame embedding must remain disabled  
- Dangerous features (camera, mic, USB, etc.) must remain opt‑in  

### 4.3 DI and Pipeline Invariants

- Middleware must be registered as transient  
- Middleware must be added early in the pipeline  
- Middleware must not depend on scoped services  

---

# 5. Example Usage (Developer Perspective)

### 5.1 Register the middleware

```csharp
builder.Services.AddSecurityHeaders();
```

### 5.2 Add to the pipeline

```csharp
app.UseMiddleware<SecurityHeadersMiddleware>();
```

### 5.3 Override a header manually (if needed)

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self' https://cdn.example.com";

    await next();
});
```

---

# 6. Summary

**Current State:**  
The Security Headers Middleware provides:

- A strict, modern, OWASP‑aligned security header baseline  
- A static CSP suitable for APIs  
- A safe `SetIfMissing` pattern  
- A simple DI‑friendly middleware implementation  

**Future Intent:**  
The capability will evolve to support:

- Configurable profiles  
- Dynamic CSP generation  
- Reporting and diagnostics  
- Metadata‑driven behavior  
- Integration with the Startup Engine  

As a developer:

- Today, you register the middleware and rely on strict defaults  
- In the future, Frank will provide a full Security Headers Engine with dynamic and configurable behavior  

