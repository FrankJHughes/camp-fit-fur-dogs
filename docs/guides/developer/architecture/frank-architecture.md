# Frank Architecture  
*The shared architectural backbone that powers CampFitFurDogs.*

Frank is a **product**, not a layer.  
It provides the **cross‑cutting architectural primitives** that make the CampFitFurDogs system consistent, testable, deterministic, and maintainable.

Frank is the **shared kernel replacement** — but unlike a traditional shared kernel, Frank is:

- Explicit  
- Modular  
- Guarded  
- Versioned  
- Testable  
- Free of product‑specific logic  

This guide explains **what Frank is**, **what belongs in it**, **how it is structured**, and **how CampFitFurDogs consumes it**.

For enforceable rules, see:  
`docs/governance/technical/architecture-governance.md`

---

# 1. Purpose of Frank

Frank exists to:

- Eliminate boilerplate  
- Enforce architectural purity  
- Provide cross‑cutting primitives  
- Provide deterministic pipelines  
- Provide hosting provider infrastructure  
- Provide DI auto‑registration  
- Provide endpoint discovery  
- Provide validator scanning  
- Provide environment abstraction  
- Provide security headers  
- Provide error boundaries  
- Provide testing seams  

Frank is the **architectural foundation** of the system.

CampFitFurDogs is built **on top of Frank**, not inside it.

---

# 2. What Belongs in Frank

Frank contains **reusable architectural primitives** that:

- Apply across all products  
- Are not domain‑specific  
- Are not business‑specific  
- Are not tied to CampFitFurDogs  
- Are deterministic  
- Are testable  
- Are stable across versions  

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
- Product‑specific hosting behavior  
- Product‑specific configuration  
- Product‑specific abstractions  
- Business rules  
- Aggregates  
- Entities  
- Value objects  
- Repositories  
- Readers  
- Email flows  
- Authentication business logic  

Frank is **pure architecture**, not business logic.

---

# 4. Frank Folder Structure

Frank is organized into several sub‑products:

```
src/
  Frank/
  Frank.Api/
  Frank.Infrastructure/
  Frank.Infrastructure.EntityFrameworkCore/
  Frank.Testing/
```

Each sub‑product has a clear purpose.

---

# 5. Frank (Core)

```
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
```

## **5.1 Abstractions**
Cross‑cutting interfaces:

- `IEnvironment`  
- `ITimeProvider`  
- `IAuditLogger`  
- `IStartupModule`  
- `IHostingProvider`  

## **5.2 AutoRegistration**
The DI auto‑registration engine:

- Scans assemblies  
- Registers services based on attributes  
- Enforces purity  
- Prevents manual DI drift  

## **5.3 DependencyInjection**
DI helpers and guardrails.

## **5.4 Domain**
Domain‑agnostic primitives:

- Domain event base types  
- Result types  
- Error types  

## **5.5 Events**
Domain event dispatch pipeline.

## **5.6 ImmutableContext**
The ImmutableContextBuilder:

- Multi‑stage transformations  
- Pure, deterministic pipelines  
- Used heavily in authentication callback flows  

## **5.7 Modules**
Startup modules:

- Ordered startup execution  
- Deterministic initialization  
- Hosting provider integration  

## **5.8 Settings**
Configuration primitives.

---

# 6. Frank.Api

```
Frank.Api/
  ExceptionHandling/
  Hosting/
  SecurityHeaders/
  Startup/
```

## **6.1 ExceptionHandling**
Error boundary middleware:

- Shapes errors  
- Prevents leaking internals  
- Integrates with correlation IDs  
- Applies consistent error format  

## **6.2 Hosting**
HostingEngine:

- Selects hosting providers  
- Validates configuration  
- Applies environment seams  
- Ensures deterministic startup  

## **6.3 SecurityHeaders**
Security headers middleware:

- HSTS  
- X‑Frame‑Options  
- X‑Content‑Type‑Options  
- Referrer‑Policy  
- Permissions‑Policy  

## **6.4 Startup**
StartupEngine:

- Runs startup modules  
- Validates ordering  
- Ensures all modules complete  
- Fails fast on misconfiguration  

---

# 7. Frank.Infrastructure

```
Frank.Infrastructure/
  Environment/
```

## **7.1 Environment**
Environment abstraction:

- Safe access to environment variables  
- No direct `Environment.GetEnvironmentVariable`  
- Deterministic behavior  
- Testable seams  

---

# 8. Frank.Infrastructure.EntityFrameworkCore

```
Frank.Infrastructure.EntityFrameworkCore/
  Configurations/
```

Provides:

- EF Core configuration scanning  
- Guardrails for missing configurations  
- Deterministic model building  

---

# 9. Frank.Testing

```
Frank.Testing/
  Contexts/
  Endpoints/
  Factories/
```

Provides:

- Fake environment  
- Fake hosting provider  
- Fake time provider  
- Fake external services  
- Endpoint testing utilities  
- Test harness primitives  

Frank.Testing enables:

- Deterministic tests  
- No real infrastructure  
- No real hosting  
- No real identity providers  

---

# 10. How CampFitFurDogs Consumes Frank

CampFitFurDogs uses Frank for:

- DI auto‑registration  
- Endpoint discovery  
- Validation pipeline  
- Dispatcher pipeline  
- Domain event pipeline  
- ImmutableContextBuilder  
- Hosting provider selection  
- Environment abstraction  
- Security headers  
- Error boundaries  
- EF Core configuration scanning  
- Test harness utilities  

CampFitFurDogs **does not modify Frank**.  
It only **consumes** Frank.

---

# 11. Versioning Frank

Frank must be:

- Backward compatible  
- Stable  
- Versioned  
- Tested independently  
- Updated intentionally  

Breaking changes require:

- Migration documentation  
- Updated tests  
- Updated governance  
- Updated developer guides  

Frank is a **product**, not a folder.

---

# 12. Testing Frank

Frank is tested through:

- Unit tests  
- Integration tests  
- Architecture tests  
- Hosting provider seam tests  
- DI auto‑registration tests  
- Endpoint discovery tests  
- Validation scanning tests  
- Error boundary tests  
- Security header tests  

Frank failures block backend merges.

---

# 13. Summary

Frank is the architectural backbone of the system.  
It provides:

- Cross‑cutting primitives  
- Deterministic pipelines  
- Hosting provider infrastructure  
- Environment abstraction  
- DI auto‑registration  
- Endpoint discovery  
- Validation scanning  
- Security headers  
- Error boundaries  
- Testing seams  

Frank must remain:

- Pure  
- Reusable  
- Deterministic  
- Testable  
- Free of product logic  

CampFitFurDogs is built **on Frank**, not inside it.

For enforceable rules, see:

```
docs/governance/technical/architecture-governance.md
```
