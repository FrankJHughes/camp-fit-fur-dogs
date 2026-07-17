# Frank Security Headers Middleware — User Guide

This guide explains how users of the Frank platform should work with the **Security Headers Middleware** today, and how the experience will evolve in the future.

The Security Headers Middleware provides a **hardened, OWASP‑aligned baseline** of HTTP response headers.  
It protects APIs by ensuring every response includes modern security headers unless explicitly overridden.

This guide focuses on how to *use* the middleware — not how to implement or extend it.

---

# 1. Current State (Before Future Enhancements)

The Security Headers Middleware currently provides:

- A strict set of modern security headers  
- A hardened Content‑Security‑Policy (CSP)  
- A `SetIfMissing` pattern that prevents overriding upstream headers  
- A simple DI‑friendly registration method (`AddSecurityHeaders`)  
- No configuration required — safe defaults out of the box  

### What this means for you today

As a user of this capability:

- You register the middleware in DI using `AddSecurityHeaders()`  
- You add the middleware to the pipeline using `UseMiddleware<SecurityHeadersMiddleware>()`  
- You get a strict, modern security header baseline automatically  
- You can override any header manually before the middleware runs  
- You do not configure anything — the defaults are intentionally strict  

### What the middleware does today

- Adds OWASP‑recommended headers  
- Adds a strict CSP suitable for APIs  
- Ensures headers are only added if missing  
- Leaves existing headers untouched  
- Works for any API built on ASP.NET Core  

### What the middleware does *not* do today

- No configuration API  
- No dynamic CSP generation  
- No nonce or hash support  
- No per‑environment profiles  
- No reporting endpoints  
- No metadata‑driven behavior  
- No diagnostics or logging  

The current capability is intentionally simple and secure.

---

# 2. Future Intent (After Capability Expansion)

As the platform evolves, the Security Headers Middleware will become more flexible and more powerful.

### Future enhancements may include:

### **2.1 Configurable Security Profiles**
- Strict API (current default)  
- Relaxed UI  
- Development mode  
- Zero‑trust hardened  

### **2.2 Dynamic CSP Generation**
- Nonce‑based script/style support  
- Hash‑based inline script support  
- Per‑request CSP mutation  
- Automatic nonce injection into UI frameworks  

### **2.3 Reporting and Monitoring**
- CSP violation reporting  
- Logging of blocked resources  
- Integration with observability pipelines  

### **2.4 Metadata‑Driven Behavior**
- Endpoint attributes that modify CSP  
- Automatic relaxation for file uploads or streaming endpoints  
- Per‑endpoint header overrides  

### **2.5 Middleware Pipeline Integration**
- Automatic registration via the Startup Engine  
- Ordering guarantees  
- Conflict detection with other middleware  

These enhancements will transform the subsystem into a full **Security Headers Engine**.

---

# 3. How You Use the Middleware Today

## 3.1 Register the middleware

```csharp
builder.Services.AddSecurityHeaders();
```

## 3.2 Add it to the pipeline

```csharp
app.UseMiddleware<SecurityHeadersMiddleware>();
```

## 3.3 Override a header manually (optional)

If you need to relax a header for a specific scenario:

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self' https://cdn.example.com";

    await next();
});
```

### What you get:

- Strict defaults  
- Predictable behavior  
- No configuration complexity  
- A hardened API surface  

---

# 4. What the Middleware Guarantees

### **Consistency**
Every response includes a hardened set of security headers.

### **Safety**
Headers are only added if missing — upstream overrides are respected.

### **Modern Defaults**
The middleware applies:

- OWASP‑aligned headers  
- Strict CSP  
- COOP / COEP / CORP  
- Permissions‑Policy restrictions  

### **Simplicity**
No configuration required.

---

# 5. What the Middleware Does *Not* Guarantee

### **No UI‑friendly CSP**
This middleware is designed for APIs, not complex frontends.

### **No dynamic behavior**
CSP is static.

### **No per‑environment behavior**
Development and production behave the same.

### **No diagnostics**
The middleware does not log or report anything.

### **No metadata integration**
Endpoints cannot modify CSP automatically.

---

# 6. Example: A Hardened API Using the Middleware

```
/Api
    Program.cs
    /Security
        SecurityHeadersMiddleware.cs
```

Startup:

```csharp
builder.Services.AddSecurityHeaders();
app.UseMiddleware<SecurityHeadersMiddleware>();
```

This gives you:

- A hardened API surface  
- Strict defaults  
- No configuration overhead  
- Predictable behavior across environments  

---

# 7. Summary

**Current State:**  
You use the Security Headers Middleware to:

- Add strict, modern security headers  
- Enforce a hardened CSP  
- Protect APIs with minimal effort  
- Override headers manually when needed  

**Future Intent:**  
The middleware will evolve to support:

- Configurable profiles  
- Dynamic CSP generation  
- Reporting and diagnostics  
- Metadata‑driven behavior  
- Startup Engine integration  

As a user of this capability:

- Today, you register the middleware and rely on strict defaults  
- In the future, Frank will provide a full Security Headers Engine with dynamic and configurable behavior  

