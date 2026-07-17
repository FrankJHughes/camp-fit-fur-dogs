# Frank — Conventions — Platform Architecture — Cross‑Cutting Responsibilities  
*The platform‑level primitives that every product relies on.*

Frank provides the **platform‑level primitives** that every product — including CampFitFurDogs — relies on.  
These responsibilities are **cross‑cutting** by nature: they apply uniformly across all modules, slices, and layers.

Frank exists to **centralize**, **standardize**, and **harden** these concerns so that products remain clean, consistent, and maintainable.

---

# Purpose

Cross‑cutting responsibilities exist to:

- eliminate duplication  
- enforce consistent behavior  
- provide hardened, reusable primitives  
- isolate external dependencies  
- ensure deterministic behavior across environments  

Frank is the **single source of truth** for these concerns.

---

# Categories of Cross‑Cutting Responsibilities

## 1. Hosting & Startup

Frank owns:

- hosting providers  
- HostingEngine  
- StartupEngine  
- module loader  
- lifecycle events  
- environment detection  
- configuration loading  

Products must **not** implement their own hosting logic.

---

## 2. Configuration & Environment

Frank provides:

- strongly‑typed configuration binding  
- environment variable access  
- secrets resolution  
- environment detection (`Development`, `Staging`, `Production`)  

Products must **not** read environment variables directly.

---

## 3. Logging & Observability

Frank defines:

- structured logging primitives  
- correlation ID propagation  
- execution tracing  
- lifecycle event logging  
- error context enrichment  
- observability test harness (ADR‑0060)  

Products must **not** implement their own logging frameworks.

---

## 4. HTTP & External Calls

Frank provides:

- HTTP client abstractions  
- retry policies  
- circuit breakers  
- timeout policies  
- typed HTTP responses  
- outbound trace context propagation  

Products must **not** use `HttpClient` directly.

---

## 5. Time, IDs, and Deterministic Utilities

Frank provides:

- clock abstraction  
- deterministic ID generator  
- random number abstraction  
- deterministic time provider for tests  

Products must **not** call:

- `DateTime.UtcNow`  
- `Guid.NewGuid()`  
- `Random()`  

directly.

---

## 6. Security & Hardening

Frank enforces:

- security headers  
- CORS policy primitives  
- rate limiting primitives  
- anti‑abuse protections  
- safe defaults for hosting  

Products must **not** weaken or override these defaults.

---

## 7. Serialization & Formatting

Frank provides:

- JSON serialization settings  
- converters  
- safe defaults  
- deterministic formatting  

Products must **not** define their own global JSON settings.

---

# Prohibitions

Frank must **not**:

- contain business logic  
- contain slice‑specific logic  
- depend on any product  
- expose product‑specific DTOs  
- embed CampFitFurDogs conventions  

Frank is a **platform**, not an application.

---

# Enforcement

Cross‑cutting responsibilities are enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

Frank is the **foundation** — stable, hardened, and product‑agnostic.
