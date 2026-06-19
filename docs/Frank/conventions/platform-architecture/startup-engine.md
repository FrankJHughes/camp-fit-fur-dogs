# startup-engine.md

# Startup Engine (Frank)

The **Startup Engine** is the deterministic, centralized initialization pipeline
for all Frank‑based products.  
It is responsible for discovering modules, loading configuration, wiring
dependencies, applying global middleware, and emitting lifecycle events.

Products — including CampFitFurDogs — must not define their own startup logic.

The Startup Engine ensures:

- consistent initialization across environments  
- deterministic module loading  
- strict DI governance  
- hardened security defaults  
- unified observability  
- predictable hosting behavior  

---

## Responsibilities

The Startup Engine owns:

- module discovery  
- module manifest loading  
- DI registration (with opt‑out support)  
- configuration binding  
- environment detection  
- lifecycle event emission  
- global middleware registration  
- observability initialization  
- validation of required services  

The engine is the **single source of truth** for application startup.

---

## Module Discovery

Modules are discovered by:

- scanning assemblies  
- reading module manifests  
- validating dependencies  
- ordering modules deterministically  

Modules must not:

- perform hosting logic  
- modify global middleware  
- read environment variables  
- override platform defaults  

Modules are **units of capability**, not hosting components.

---

## Auto‑Registration & Opt‑Out

By default, modules participate in **auto‑registration**:

- services are registered automatically  
- handlers are discovered  
- configuration sections are bound  
- validators are wired  

Modules may opt out when:

- explicit DI wiring is required  
- safety or clarity demands manual registration  
- performance considerations apply  

Opt‑out modules must:

- register all required services explicitly  
- pass Startup Engine validation  
- fail fast if dependencies are missing  

(See US‑185 for the governing story.)

---

## Lifecycle Events

The Startup Engine emits structured lifecycle events:

- `Startup.Begin`  
- `Module.Loaded`  
- `Module.Initialized`  
- `Startup.Complete`  
- `Shutdown.Begin`  
- `Shutdown.Complete`  

These events support:

- observability  
- debugging  
- environment diagnostics  
- module health verification  

(See US‑183 for observability requirements.)

---

## Global Middleware Registration

The Startup Engine registers global middleware in a fixed, hardened order:

1. correlation IDs  
2. structured request logging  
3. exception handling  
4. security headers  
5. CORS  
6. rate limiting  
7. routing  
8. endpoint mapping  

Products may add slice‑specific middleware **after** global middleware.

Products must not:

- reorder global middleware  
- weaken security defaults  
- bypass exception handling  

---

## Configuration & Environment

The Startup Engine:

- loads configuration from all supported sources  
- binds strongly‑typed configuration objects  
- resolves secrets  
- detects environment (`Development`, `Staging`, `Production`)  
- validates required configuration keys  

Products must not read environment variables directly.

---

## Enforcement

Startup Engine rules are enforced through:

- guardrail tests  
- module validation  
- dependency analysis  
- conventions governance  

The Startup Engine is the **foundation** of every Frank‑based application.

