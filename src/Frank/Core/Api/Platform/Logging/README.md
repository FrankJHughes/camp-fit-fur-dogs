# Platform Logging

The **Platform Logging** subsystem provides environment‑aware HTTP logging for the
Frank.Core API.  
It registers ASP.NET Core’s built‑in HTTP logging services and conditionally
enables them at runtime based on the hosting environment.

This subsystem ensures that development environments have rich diagnostic
visibility, while production environments remain optimized and free of verbose
logging.

---

## Files

```
Logging/
├── ServiceCollectionExtensions.cs
└── ApplicationBuilderExtensions.cs
```

---

## ServiceCollectionExtensions

`ServiceCollectionExtensions` registers the HTTP logging infrastructure used by
the Frank.Core API.

### Responsibilities

- Registers ASP.NET Core’s `HttpLoggingMiddleware` services.
- Configures which HTTP fields are logged:
  - **RequestPath**
  - **RequestMethod**
  - **ResponseStatusCode**
- Provides a minimal, high‑value logging configuration suitable for development.

### Why this matters

This subsystem ensures:

- HTTP logging is **centralized** and **consistent**.
- Only essential fields are logged, avoiding excessive verbosity.
- Logging infrastructure is available for conditional activation at runtime.

### Usage

```csharp
services.AddFrankCoreApiPlatformLogging();
```

This prepares the logging services for use by the application pipeline.

---

## ApplicationBuilderExtensions

`ApplicationBuilderExtensions` activates HTTP logging at runtime when the API is
running in the **Development** environment.

### Responsibilities

- Detects the current hosting environment.
- Enables `UseHttpLogging()` only when `env.IsDevelopment()` is true.
- Emits a startup log entry confirming activation.

### Why this matters

- Development environments gain full request/response visibility.
- Production environments remain fast, secure, and uncluttered.
- Logging behavior is predictable and environment‑driven.

### Usage

```csharp
app.UseFrankCoreApiPlatformLogging();
```

This activates HTTP logging only when appropriate.

---

## How Platform Logging Fits Into the Architecture

Platform Logging is part of the API’s hosting and diagnostics layer.  
It complements:

- **[Observations](ca://s?q=Tell_me_more_about_Observations_middleware)**  
- **[Security Headers](ca://s?q=Tell_me_more_about_Security_Headers_middleware)**  
- **[CORS](ca://s?q=Tell_me_more_about_CORS_configuration)**  
- **[Exceptions](ca://s?q=Explain_exception_handling_middleware)**  

Together, these subsystems provide a secure, observable, and predictable API
platform.

---

## Design Principles

- **Environment‑aware**  
  Logging is enabled only where it is safe and useful.

- **Minimal but meaningful**  
  Only high‑value fields are logged.

- **Centralized configuration**  
  Logging is defined once and applied consistently.

- **Zero production overhead**  
  No verbose logging in production environments.

---

## Notes

- HTTP logging is intentionally limited to essential fields.
- Additional logging fields can be added if needed for debugging.
- Works seamlessly with Observations and other diagnostics subsystems.

---
