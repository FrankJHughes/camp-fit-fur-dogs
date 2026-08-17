# Frank.CrossCutting — Logging

Logging is configured at the **platform level** to provide operational visibility, structured diagnostics, and full request tracing across all vertical slices. It is a **cross‑cutting concern**: every subsystem (Identity, Dogs, Scheduling, Infrastructure) relies on consistent logging behavior.

This document describes the logging subsystem under:

```
/docs/05-cross-cutting
```

and maps it back to its implementation under:

```
/src/Frank/Core.Api
/src/Frank/Core.Infrastructure/Observations
```

Logging integrates with:

- structured observability (US‑183)  
- correlation and causation metadata  
- environment‑aware verbosity  
- platform‑level middleware  

---

## Configuration

Logging is configured through `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Frank.Core": "Debug",
      "CampFitFurDogs": "Information"
    }
  }
}
```

### How configuration works

- **Default** controls global verbosity  
- **Microsoft.AspNetCore** reduces noise from framework internals  
- **Frank.Core** enables detailed diagnostics for platform subsystems  
- **CampFitFurDogs** sets business‑level logging for product code  

Environment‑specific overrides (Development, Testing, Production) adjust verbosity automatically.

See also:  
- [Configuration Management](ca://s?q=Explain_crosscutting_configuration_management)  
- [Observability](ca://s?q=Explain_crosscutting_observability)

---

## Log Levels

The platform uses standard ASP.NET Core log levels:

- **Information** — business‑level events (user actions, domain changes)  
- **Debug** — internal flow (handler execution, query results)  
- **Warning** — recoverable issues (retries, degraded functionality)  
- **Error** — application errors requiring attention  
- **Critical** — system failures requiring immediate action  

### Environment behavior

- **Development** → Debug + Information  
- **Testing** → deterministic logging for assertions  
- **Production** → Information + Warning, minimal Debug  

This ensures logs remain useful without overwhelming storage or dashboards.

---

## Structured Logging

All log messages use **structured logging**, meaning logs are emitted as key/value pairs rather than raw strings.

Each request includes:

- correlation ID  
- causation ID  
- path  
- method  
- status code  
- latency  
- environment metadata  

This enables:

- filtering  
- analytics  
- distributed tracing  
- cross‑service correlation  

Structured logging is implemented through the **Observations subsystem**.

See:  
- [Observations](ca://s?q=Tell_me_more_about_Observations_middleware)  
- [Correlation IDs](ca://s?q=Explain_correlation_id_flow)

---

## Platform Registration

Logging is registered via:

```csharp
services.AddFrankCoreApiPlatformLogging();
```

This ensures:

- HTTP logging middleware is available  
- structured logging is enabled  
- environment‑aware verbosity is applied  
- request lifecycle events are captured  
- logs integrate with Observations and diagnostics subsystems  

At runtime, logging is activated via:

```csharp
app.UseFrankCoreApiPlatformLogging();
```

This applies logging to **all endpoints**.

---

## Runtime Collaboration Points

Logging interacts with:

- **Authentication** — logs login, callback, and session validation  
- **Authorization** — logs forbidden/unauthorized access  
- **Error Handling** — logs exceptions with correlation metadata  
- **Observability** — unified structured logs across layers  
- **Testing** — deterministic logs for integration tests  
- **Infrastructure** — environment detection and configuration binding  

Logging is foundational for debugging, monitoring, and operational insight.

---

## Notes

Keep this document grounded in the actual logging implementation.  
Whenever new middleware, observability features, or environment behaviors are added, update this section to reflect the current architecture.
