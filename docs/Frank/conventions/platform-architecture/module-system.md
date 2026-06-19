# module-system.md

# Module System (Frank)

The **Module System** is the foundation of extensibility, composition, and
deterministic initialization in Frank‑based applications.  
Modules are the **unit of capability** across Hosting, Startup, and Application
pipelines.

Modules are:

- discoverable  
- ordered  
- isolated  
- deterministic  
- declarative  

Modules must never contain product‑specific logic.  
They are **platform‑level components**.

---

## Purpose

The Module System exists to:

- provide a unified model for extensibility  
- enforce deterministic ordering  
- isolate concerns  
- support opt‑in and opt‑out DI registration  
- enable test harness integration  
- unify Hosting and Startup behavior  
- ensure platform‑level purity  

Modules are the **building blocks** of the Frank platform.

---

## Module Types

Frank defines three categories of modules:

### 1. Hosting Modules  
Executed by the **Hosting Engine**.  
They configure:

- hosting providers  
- environment behavior  
- global hosting settings  
- hosting context contributions  

### 2. Startup Modules  
Executed by the **Startup Engine**.  
They configure:

- DI registration  
- configuration binding  
- observability initialization  
- startup context contributions  

### 3. Application Modules (Optional)  
Executed by application‑level pipelines.  
They configure:

- handlers  
- validators  
- application‑level capabilities  

All modules follow the same structural rules.

---

## Module Manifest

Each module includes a manifest that declares:

- module name  
- version  
- dependencies  
- optional capabilities  
- opt‑out flags (e.g., auto‑registration)  

Manifests must be:

- immutable  
- declarative  
- strongly typed  
- validated at startup  

Modules must not compute their own dependencies dynamically.

---

## Module Ordering

Modules are ordered deterministically using:

- declared dependencies  
- topological sorting  
- stable ordering rules  

Ordering must be:

- deterministic  
- reproducible  
- validated  

Modules must not:

- override ordering  
- introduce circular dependencies  
- depend on product code  

---

## Module Execution

Each module implements:

````csharp
Task<IContext> ExecuteAsync(IContext context);
````

Execution rules:

- modules receive an immutable context  
- modules return a **new** immutable context  
- modules must not mutate previous state  
- modules must not perform I/O (Startup modules)  
- modules must not contain business logic  

Modules are **pure transformers** of context.

---

## Auto‑Registration & Opt‑Out

Modules participate in **auto‑registration** unless they explicitly opt out.

Auto‑registration includes:

- service discovery  
- handler discovery  
- validator discovery  
- configuration binding  

Opt‑out modules must:

- register all services manually  
- pass validation  
- fail fast if required services are missing  

(See US‑185 for governing rules.)

---

## Module Validation

The Module System validates:

- dependency graphs  
- required services  
- configuration bindings  
- context integrity  
- lifecycle event emission  

Validation failures must:

- fail fast  
- provide actionable diagnostics  
- never allow partial startup  

Modules must never silently fail.

---

## Lifecycle Events

Modules emit structured lifecycle events:

- `Module.Discovered`  
- `Module.Loaded`  
- `Module.Initialized`  
- `Module.Completed`  

These events support:

- observability  
- debugging  
- environment diagnostics  

(See US‑183 for observability requirements.)

---

## Prohibitions

Modules must not:

- contain business logic  
- reference product code  
- read environment variables directly  
- perform HTTP calls (Startup modules)  
- weaken security defaults  
- modify global middleware ordering  
- bypass the Hosting or Startup Engines  

Modules are **platform‑level only**.

---

## Enforcement

The Module System is enforced through:

- guardrail tests  
- dependency analysis  
- module validation  
- conventions governance  

Modules must remain **pure**, **deterministic**, and **product‑agnostic**.
