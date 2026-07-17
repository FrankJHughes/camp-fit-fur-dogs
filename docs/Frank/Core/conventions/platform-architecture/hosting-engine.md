# hosting-engine.md

# Hosting Engine (Frank)

The **Hosting Engine** is the deterministic execution pipeline responsible for
initializing, configuring, and running the hosting layer for all Frank‑based
applications.  
It mirrors the Startup Engine in structure and behavior, but operates at the
**hosting** level rather than the **application startup** level.

The Hosting Engine ensures:

- deterministic hosting initialization  
- consistent module loading  
- hardened hosting defaults  
- environment‑safe configuration  
- unified logging and observability  
- predictable hosting behavior across all products  

Hosting is not ad‑hoc — it is an **engine**.

---

## Responsibilities

The Hosting Engine owns:

- hosting module discovery  
- hosting module ordering  
- hosting provider selection  
- hosting context creation  
- hosting pipeline execution  
- environment detection  
- configuration loading  
- lifecycle event emission  
- global hosting‑level hardening  

The Hosting Engine is the **foundation** of the runtime environment.

---

## Hosting Modules

Hosting modules are discovered and executed in a deterministic order.

Each hosting module may:

- register hosting‑level services  
- configure hosting providers  
- contribute to hosting context  
- declare dependencies on other hosting modules  

Hosting modules must not:

- contain business logic  
- reference product code  
- modify application‑level middleware  
- read environment variables directly  
- override platform security defaults  

Hosting modules are **platform‑level**, not application‑level.

---

## Hosting Context

The Hosting Engine constructs an immutable hosting context that includes:

- environment information  
- hosting provider configuration  
- resolved secrets  
- global hosting settings  
- module contributions  

The context is:

- immutable  
- strongly typed  
- deterministic  
- validated  

Modules receive the context and return a **new** context — never mutating state.

---

## Hosting Provider Selection

The Hosting Engine selects and configures the hosting provider:

- Kestrel  
- reverse proxy mode  
- container‑optimized mode  
- development mode  

Providers must:

- enforce HTTPS  
- enable HSTS  
- apply hardened defaults  
- configure request limits  
- integrate with Frank observability  

Providers must not:

- contain business logic  
- depend on product code  
- weaken security defaults  

---

## Lifecycle Events

The Hosting Engine emits structured lifecycle events:

- `Hosting.Begin`  
- `Hosting.Module.Loaded`  
- `Hosting.Module.Initialized`  
- `Hosting.Provider.Selected`  
- `Hosting.Complete`  
- `Shutdown.Begin`  
- `Shutdown.Complete`  

These events support:

- observability  
- diagnostics  
- environment validation  
- module health verification  

---

## Configuration & Environment

The Hosting Engine:

- loads configuration from all supported sources  
- binds strongly‑typed hosting configuration  
- resolves secrets  
- detects environment (`Development`, `Staging`, `Production`)  
- validates required hosting configuration keys  

Products must not read environment variables directly.

---

## Hardening

The Hosting Engine applies global hosting‑level hardening:

- HTTPS enforcement  
- HSTS  
- request size limits  
- connection limits  
- thread pool configuration  
- security header defaults  
- CORS defaults  
- rate limiting defaults  

Products may extend hosting behavior but must not weaken it.

---

## Enforcement

Hosting Engine rules are enforced through:

- guardrail tests  
- module validation  
- dependency analysis  
- conventions governance  

The Hosting Engine is the **runtime backbone** of every Frank‑based application.
