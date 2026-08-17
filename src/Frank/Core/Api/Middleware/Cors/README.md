# CORS Middleware

The **CORS Middleware** subsystem provides enhanced visibility into Cross-Origin
Resource Sharing (CORS) behavior within the Frank.Core API.  
It logs origin activity, evaluates CORS policy decisions, and helps diagnose
blocked or misconfigured cross-origin requests.

This folder contains middleware components that extend ASP.NET Core’s built-in
CORS pipeline with detailed logging and diagnostics.

---

## Files

```
Cors/
├── OriginLoggingMiddleware.cs
└── ApplicationBuilderExtensions.cs
```

---

## OriginLoggingMiddleware

`OriginLoggingMiddleware` inspects incoming requests for an `Origin` header,
evaluates the active CORS policy, and logs whether the request is allowed or
blocked.

### Responsibilities

- Reads the request’s `Origin` header.
- Retrieves the active CORS policy via `ICorsPolicyProvider`.
- Determines whether the origin is allowed.
- Logs detailed information for:
  - simple CORS requests  
  - preflight (OPTIONS) requests  
- Logs both allowed and blocked requests for diagnostic clarity.

### Why this matters

CORS issues are notoriously difficult to debug.  
This middleware provides transparent, structured logging that helps identify:

- unexpected blocked origins  
- missing or misconfigured CORS policies  
- incorrect preflight headers  
- frontend misconfigurations  

### Typical behavior

For simple requests:

```
CORS request allowed. Origin=https://example.com, Method=GET, Path=/dogs
```

For preflight requests:

```
CORS preflight blocked. Origin=https://evil.com, Method=OPTIONS, RequestMethod=POST, RequestHeaders=Content-Type, Path=/dogs
```

---

## ApplicationBuilderExtensions

`ApplicationBuilderExtensions` provides a single extension method for adding the
CORS logging middleware to the ASP.NET Core pipeline.

### Responsibilities

- Registers `OriginLoggingMiddleware` via `UseMiddleware`.
- Provides a clean, discoverable API for enabling CORS logging.

### Usage

```csharp
app.UseFrankCoreApiMiddlewareOriginLogging();
```

This should be placed **after** CORS configuration but **before** routing to
ensure all CORS-related requests are logged.

---

## How CORS Middleware Fits Into the Architecture

The CORS middleware is part of the API’s diagnostics layer.  
It enhances observability by providing:

- insight into cross-origin traffic  
- visibility into blocked requests  
- clarity around preflight behavior  
- support for debugging frontend/backend integration issues  

It does **not** modify CORS behavior — it only logs decisions made by the
configured CORS policy.

---

## Design Principles

- **Non-invasive**  
  Middleware logs decisions but does not alter CORS outcomes.

- **High visibility**  
  Both allowed and blocked requests are logged.

- **Preflight-aware**  
  Special handling for OPTIONS requests with `Access-Control-Request-Method`.

- **Consistent logging**  
  Uses structured logging for easy filtering and analysis.

---

## Notes

- Middleware requires a configured CORS policy to evaluate origins.
- Logging output depends on the configured log level (Information/Warning).
- This subsystem is safe to enable in all environments, including production.

---
