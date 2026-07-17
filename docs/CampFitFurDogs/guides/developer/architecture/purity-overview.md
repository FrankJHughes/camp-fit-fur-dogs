# Purity Overview — Developer Guide  
*A developer‑friendly explanation of architectural purity in CampFitFurDogs and Frank.*

Purity is one of the core architectural values of the CampFitFurDogs system.  
It ensures that each layer, slice, and component has a **single, clear responsibility**, enabling:

- predictable behavior  
- deterministic testing  
- safe refactoring  
- stable hosting behavior  
- clear operational boundaries  
- high developer velocity  

This document explains **what purity means**, **why it matters**, and **how to apply it** — without defining rules.  
For rules, see **Architecture Governance → Purity Rules**.

---

# 1. What “Purity” Means in This Architecture

Purity means:

- each layer does only what it is designed to do  
- no layer leaks responsibilities into another  
- no layer depends on details it shouldn’t know about  
- no layer performs work that belongs elsewhere  
- behavior is deterministic and testable  

Purity is not “functional programming purity.”  
It is **architectural cleanliness**.

Purity ensures that:

- API stays thin  
- Application stays orchestration‑focused  
- Domain stays business‑focused  
- Infrastructure stays integration‑focused  
- Frank stays cross‑cutting and reusable  

---

# 2. Why Purity Matters

Purity is the backbone of the system’s reliability and maintainability.

---

## 2.1 Testability

Pure layers are easy to test because:

- they have no hidden dependencies  
- they don’t reach across boundaries  
- they don’t mutate global state  
- they don’t depend on infrastructure  

This enables:

- deterministic unit tests  
- fast integration tests  
- predictable CI behavior  
- reliable PR preview environments  

---

## 2.2 Replaceability

Pure components can be replaced without cascade failures.

Examples:

- swap a repository implementation  
- replace a hosting provider  
- change authentication flows  
- add new slices without touching existing ones  

---

## 2.3 Developer Velocity

Purity reduces cognitive load:

- developers know where code belongs  
- slices are self‑contained  
- layers behave consistently  
- onboarding is faster  
- refactoring is safer  

---

## 2.4 Operational Safety

Pure layers prevent:

- leaking secrets  
- leaking internal exceptions  
- inconsistent error shapes  
- infrastructure failures affecting domain logic  

---

# 3. Purity Across Layers

Purity is applied differently in each layer.

---

## 3.1 API Purity

The API layer is a **boundary**, not a business layer.

It should:

- map HTTP → Application  
- validate requests  
- shape responses  
- apply authentication & authorization  
- use dispatchers  
- use Frank middleware  

It should **not**:

- contain business logic  
- access the database  
- construct domain entities  
- call external services  

---

## 3.2 Application Purity

The Application layer is the **orchestrator**.

It should:

- coordinate use cases  
- invoke domain logic  
- dispatch domain events  
- use abstractions (readers, repositories)  
- remain deterministic  

It should **not**:

- perform persistence  
- perform HTTP calls  
- contain UI logic  
- contain hosting logic  
- contain protocol logic (e.g., OIDC)  

---

## 3.3 Domain Purity

The Domain layer is the **heart of the business**.

It should:

- contain business rules  
- contain invariants  
- contain aggregates, entities, value objects  
- raise domain events  

It should **not**:

- know about HTTP  
- know about databases  
- know about hosting  
- know about authentication providers  
- know about infrastructure  

---

## 3.4 Infrastructure Purity

Infrastructure is the **integration layer**.

It should:

- implement repositories  
- implement readers  
- integrate with databases  
- integrate with external services  
- implement hosting providers  

It should **not**:

- contain business rules  
- construct domain entities incorrectly  
- leak infrastructure types into Application or Domain  

---

## 3.5 Frank Purity

Frank is the **cross‑cutting backbone**.

It should:

- provide reusable primitives  
- provide DI auto‑registration  
- provide endpoint discovery  
- provide validator scanning  
- provide hosting provider abstractions  
- provide security headers  
- provide error boundaries  
- provide dispatcher pipeline  

It should **not**:

- contain product‑specific logic  
- depend on CampFitFurDogs  
- contain domain rules  
- contain business logic  

---

# 4. Purity and Vertical Slices

Purity enables slices to be:

- self‑contained  
- predictable  
- easy to navigate  
- easy to test  
- easy to extend  

A pure slice spans:

````text
Api → Application → Domain → Infrastructure
````

Each layer contributes only what it is responsible for.

Purity prevents:

- API doing Application work  
- Application doing Domain work  
- Domain doing Infrastructure work  
- Infrastructure leaking into Domain  

---

# 5. Purity and Pipelines

Purity is enforced through:

- dispatcher pipeline  
- validation pipeline  
- domain event pipeline  
- ImmutableContextBuilder pipelines  
- hosting provider pipeline  
- StartupModule engine  

Each pipeline isolates responsibilities and ensures:

- deterministic behavior  
- clear ordering  
- clear failure semantics  
- testability  

---

# 6. Purity and Testing

Pure architecture enables:

- unit tests for Domain  
- handler tests for Application  
- endpoint tests for API  
- integration tests for Infrastructure  
- full‑stack tests for slices  

Because layers are pure:

- tests don’t require real infrastructure  
- tests don’t require real HTTP  
- tests don’t require real hosting  
- tests don’t require real identity providers  

---

# 7. Purity and Developer Experience

Purity improves DX by:

- making code predictable  
- making slices consistent  
- making layers clear  
- making onboarding faster  
- making refactoring safer  
- making debugging easier  

Developers always know:

- where code belongs  
- where code does *not* belong  
- how layers interact  
- how slices are structured  

---

# 8. Purity in Practice: A Quick Checklist

A developer can use this mental checklist:

- **API**: Am I doing anything other than mapping HTTP to Application?  
- **Application**: Am I orchestrating, not implementing business rules?  
- **Domain**: Am I expressing business rules without infrastructure concerns?  
- **Infrastructure**: Am I integrating external systems without leaking them upward?  
- **Frank**: Am I building reusable primitives, not product logic?  

If the answer is “yes,” purity is preserved.

---

# 9. Summary

Purity is the foundation of:

- testability  
- replaceability  
- developer velocity  
- operational safety  
- architectural clarity  

This guide explains **why purity matters** and **how to think about it**.  
For enforceable rules, see:

````text
docs/governance/technical/purity-rules.md
````

Purity is not a restriction — it’s a superpower.
