# startup-architecture.md

# Startup Architecture (Frank)

The **Startup Architecture** defines the conceptual model, boundaries, and
responsibilities of application startup in Frank‑based systems.  
Where the **Startup Engine** is the concrete implementation, the Startup
Architecture describes the **philosophy**, **constraints**, and **governance**
that shape how startup must behave across all products.

Startup is not an ad‑hoc sequence of registrations.  
It is a **governed architectural layer** with strict rules.

---

## Purpose

The Startup Architecture exists to:

- ensure deterministic initialization  
- enforce module boundaries  
- centralize configuration and environment handling  
- guarantee hardened defaults  
- unify observability  
- prevent product‑specific startup logic  
- ensure startup is testable and predictable  

Startup is a **platform concern**, not an application concern.

---

## Architectural Principles

### 1. Determinism  
Startup must produce the same result given the same inputs:

- same modules  
- same configuration  
- same environment  

No randomness, no implicit ordering, no side effects.

### 2. Immutability  
Startup context objects must be:

- immutable  
- strongly typed  
- validated  

Modules return **new** contexts, never mutate existing ones.

### 3. Separation of Concerns  
Startup must not:

- contain business logic  
- reference product code  
- perform HTTP calls  
- read environment variables directly  
- modify hosting behavior outside approved seams  

Startup is **initialization**, not execution.

### 4. Module‑Driven  
Startup is composed of:

- startup modules  
- module manifests  
- module dependency graphs  
- deterministic ordering  

Modules are the **unit of startup capability**.

---

## Startup Layers

Startup consists of three conceptual layers:

### **1. Configuration Layer**  
Loads and binds:

- configuration files  
- environment variables  
- secrets  
- typed configuration objects  

### **2. Module Layer**  
Discovers and executes:

- startup modules  
- module manifests  
- dependency ordering  
- opt‑out modules  
- validation rules  

### **3. Global Initialization Layer**  
Applies:

- observability initialization  
- DI container validation  
- global middleware registration (delegated to Hosting Engine)  
- lifecycle event emission  

Startup is the **bridge** between hosting and application execution.

---

## Responsibilities

Startup Architecture defines:

- how modules are discovered  
- how modules declare dependencies  
- how modules contribute to the startup context  
- how configuration is loaded and validated  
- how lifecycle events are emitted  
- how DI is governed  
- how auto‑registration works (and how opt‑out works)  

Startup Architecture does **not** define:

- hosting behavior  
- application behavior  
- business logic  
- slice logic  

---

## Prohibitions

Startup must not:

- depend on product code  
- depend on slices  
- contain business rules  
- perform persistence  
- perform HTTP calls  
- read environment variables directly  
- bypass the Hosting Engine  
- weaken security defaults  
- reorder global middleware  

Startup is **platform‑level only**.

---

## Relationship to the Startup Engine

The **Startup Architecture** defines:

- the rules  
- the boundaries  
- the responsibilities  
- the invariants  
- the conceptual model  

The **Startup Engine** implements:

- module discovery  
- module ordering  
- DI registration  
- configuration binding  
- lifecycle events  
- validation  

Architecture is the **contract**.  
The engine is the **executor**.

---

## Enforcement

Startup Architecture is enforced through:

- guardrail tests  
- module validation  
- dependency analysis  
- conventions governance  
- CI enforcement  

Startup must remain **pure**, **deterministic**, and **product‑agnostic**.
