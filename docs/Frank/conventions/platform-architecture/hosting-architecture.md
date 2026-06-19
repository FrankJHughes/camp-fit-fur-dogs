# hosting-architecture.md

# Hosting Architecture (Frank)

Frank provides the **hosting foundation** for all products, including
CampFitFurDogs.  
Hosting is centralized, deterministic, and fully governed — products do not
define their own hosting models.

Frank’s hosting architecture ensures:

- consistent startup behavior  
- predictable module loading  
- deterministic environment handling  
- unified logging and observability  
- hardened security defaults  

Frank is the **platform**, not the application.

---

## Hosting Responsibilities

Frank owns:

- the hosting provider abstraction  
- the startup engine  
- module discovery and loading  
- lifecycle events (startup, shutdown, reload)  
- environment detection  
- configuration loading  
- DI container initialization  
- global middleware registration  

Products must not override or re‑implement hosting.

---

## Hosting Provider Model

Frank defines a hosting provider abstraction that:

- wraps ASP.NET Core hosting  
- applies hardened defaults  
- configures Kestrel  
- enforces HTTPS  
- enables HSTS  
- configures request limits  
- applies security headers  
- wires global middleware  

Hosting providers must not:

- contain business logic  
- reference product code  
- depend on slices  
- read product configuration keys  

Hosting is **platform‑level only**.

---

## Startup Engine

The startup engine:

- discovers modules  
- loads module manifests  
- registers module services  
- applies auto‑registration (unless opted out)  
- validates required services  
- emits lifecycle events  
- initializes observability  
- configures global middleware  

Startup must be:

- deterministic  
- ordered  
- testable  
- product‑agnostic  

Products must not define their own startup pipelines.

---

## Module Loading

Modules:

- declare their services  
- declare their dependencies  
- may opt out of auto‑registration  
- must not perform hosting logic  
- must not modify global middleware  

Frank ensures modules load in a deterministic order.

---

## Environment & Configuration

Frank provides:

- environment detection (`Development`, `Staging`, `Production`)  
- configuration binding  
- secrets resolution  
- environment variable access  

Products must not read environment variables directly.

---

## Middleware Governance

Frank registers global middleware:

- security headers  
- CORS  
- rate limiting  
- correlation IDs  
- exception handling  
- request logging  

Products may add slice‑specific middleware **only after** global middleware.

Products must not:

- override global middleware  
- weaken security defaults  
- reorder the pipeline  

---

## Enforcement

Hosting architecture rules are enforced through:

- guardrail tests  
- dependency analysis  
- module validation  
- conventions governance  

Frank hosting is **the single source of truth** for application startup.
