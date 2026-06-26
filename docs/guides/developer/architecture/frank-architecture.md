# Frank Architecture — Developer Guide  
*The shared architectural backbone that powers CampFitFurDogs.*

Frank is a **product**, not a layer.  
It provides the **cross‑cutting architectural primitives** that make the CampFitFurDogs system consistent, testable, deterministic, and maintainable.

Frank is the **shared‑kernel replacement**, but unlike a traditional shared kernel, Frank is:

- explicit  
- modular  
- guarded  
- versioned  
- testable  
- free of product‑specific logic  

This guide explains **what Frank is**, **what belongs in it**, **how it is structured**, and **how CampFitFurDogs consumes it**.

For enforceable rules, see:

````text
docs/governance/technical/architecture-governance.md
````

---

# 1. Purpose of Frank

Frank exists to:

- eliminate boilerplate  
- enforce architectural purity  
- provide cross‑cutting primitives  
- provide deterministic pipelines  
- provide hosting provider infrastructure  
- provide DI auto‑registration  
- provide endpoint discovery  
- provide validator scanning  
- provide environment abstraction  
- provide security headers  
- provide error boundaries  
- provide testing seams  

Frank is the **architectural foundation** of the system.

CampFitFurDogs is built **on top of Frank**, not inside it.

---

# 2. What Belongs in Frank

Frank contains **reusable architectural primitives** that:

- apply across all products  
- are not domain‑specific  
- are not business‑specific  
- are not tied to CampFitFurDogs  
- are deterministic  
- are testable  
- are stable across versions  

Examples:

- DI auto‑registration  
- Endpoint discovery  
- Validation pipeline  
- Dispatcher pipeline  
- Domain event pipeline  
- ImmutableContextBuilder  
- Hosting provider abstractions  
- Environment abstraction  
- Security headers middleware  
- Error boundary middleware  
- EF Core configuration scanning  
- Test harness utilities  

Frank is the **toolbox** for building clean, consistent products.

---

# 3. What Does *Not* Belong in Frank

Frank must never contain:

- CampFitFurDogs domain logic  
- CampFitFurDogs application logic  
- CampFitFurDogs infrastructure logic  
- product‑specific hosting behavior  
- product‑specific configuration  
- product‑specific abstractions  
- business rules  
- aggregates  
- entities  
- value objects  
- repositories  
- readers  
- email flows  
- authentication business logic  

Frank is **pure architecture**, not business logic.

---

# 4. Frank Folder Structure

Frank is organized into several sub‑products:

````text
src/
  Frank/
  Frank.Api/
  Frank.Infrastructure/
  Frank.Infrastructure.EntityFrameworkCore/
  Frank.Testing/
````

Each sub‑product has a clear purpose.

---

# 5. Frank (Core)

````text
Frank/
  Abstractions/
  Authentication/
  AutoRegistration/
  DependencyInjection/
  Domain/
  Events/
  ImmutableContext/
  Modules/
  Settings/
````

## 5.1 Abstractions

Cross‑cutting interfaces:

- `IEnvironment`  
- `ITimeProvider`  
- `IAuditLogger`  
- `IStartupModule`  
- `IHostingProvider`  

## 5.2 AutoRegistration

The DI auto‑registration engine:

- scans assemblies  
- registers services based on attributes  
- enforces purity  
- prevents manual DI drift  

## 5.3 DependencyInjection

DI helpers and guardrails.

## 5.4 Domain

Domain‑agnostic primitives:

- domain event base types  
- result types  
- error types  

## 5.5 Events

Domain event dispatch pipeline.

## 5.6 ImmutableContext

The ImmutableContextBuilder:

- multi‑stage transformations  
- pure, deterministic pipelines  
- used heavily in authentication callback flows  

## 5.7 Modules

Startup modules:

- ordered startup execution  
- deterministic initialization  
- hosting provider integration  

## 5.8 Settings

Configuration primitives.

---

# 6. Frank.Api

````text
Frank.Api/
  ExceptionHandling/
  Hosting/
  SecurityHeaders/
  Startup/
````

## 6.1 ExceptionHandling

Error boundary middleware:

- shapes errors  
- prevents leaking internals  
- integrates with correlation IDs  
- applies consistent error format  

## 6.2 Hosting

HostingEngine:

- selects hosting providers  
- validates configuration  
- applies environment seams  
- ensures deterministic startup  

## 6.3 SecurityHeaders

Security headers middleware:

- HSTS  
- X‑Frame‑Options  
- X‑Content‑Type‑Options  
- Referrer‑Policy  
- Permissions‑Policy  

## 6.4 Startup

StartupEngine:

- runs startup modules  
- validates ordering  
- ensures all modules complete  
- fails fast on misconfiguration  

---

# 7. Frank.Infrastructure

````text
Frank.Infrastructure/
  Environment/
````

## 7.1 Environment

Environment abstraction:

- safe access to environment variables  
- no direct `Environment.GetEnvironmentVariable`  
- deterministic behavior  
- testable seams  

---

# 8. Frank.Infrastructure.EntityFrameworkCore

````text
Frank.Infrastructure.EntityFrameworkCore/
  Configurations/
````

Provides:

- EF Core configuration scanning  
- guardrails for missing configurations  
- deterministic model building  

---

# 9. Frank.Testing

````text
Frank.Testing/
  Contexts/
  Endpoints/
  Factories/
````

Provides:

- fake environment  
- fake hosting provider  
- fake time provider  
- fake external services  
- endpoint testing utilities  
- test harness primitives  

Frank.Testing enables:

- deterministic tests  
- no real infrastructure  
- no real hosting  
- no real identity providers  

---

# 10. How CampFitFurDogs Consumes Frank

CampFitFurDogs uses Frank for:

- DI auto‑registration  
- endpoint discovery  
- validation pipeline  
- dispatcher pipeline  
- domain event pipeline  
- ImmutableContextBuilder  
- hosting provider selection  
- environment abstraction  
- security headers  
- error boundaries  
- EF Core configuration scanning  
- test harness utilities  

CampFitFurDogs **does not modify Frank**.  
It only **consumes** Frank.

---

# 11. Versioning Frank

Frank must be:

- backward compatible  
- stable  
- versioned  
- tested independently  
- updated intentionally  

Breaking changes require:

- migration documentation  
- updated tests  
- updated governance  
- updated developer guides  

Frank is a **product**, not a folder.

---

# 12. Testing Frank

Frank is tested through:

- unit tests  
- integration tests  
- architecture tests  
- hosting provider seam tests  
- DI auto‑registration tests  
- endpoint discovery tests  
- validation scanning tests  
- error boundary tests  
- security header tests  

Frank failures block backend merges.

---

# 13. Summary

Frank is the architectural backbone of the system.  
It provides:

- cross‑cutting primitives  
- deterministic pipelines  
- hosting provider infrastructure  
- environment abstraction  
- DI auto‑registration  
- endpoint discovery  
- validation scanning  
- security headers  
- error boundaries  
- testing seams  

Frank must remain:

- pure  
- reusable  
- deterministic  
- testable  
- free of product logic  

CampFitFurDogs is built **on Frank**, not inside it.

For enforceable rules, see:

````text
docs/governance/technical/architecture-governance.md
````
