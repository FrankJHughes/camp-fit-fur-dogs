# Purity Overview  
*A developer‑friendly explanation of architectural purity in CampFitFurDogs and Frank.*

Purity is one of the core architectural values of the CampFitFurDogs system.  
It ensures that each layer, slice, and component has a **single, clear responsibility**, enabling:

- Predictable behavior  
- Deterministic testing  
- Safe refactoring  
- Stable hosting behavior  
- Clear operational boundaries  
- High developer velocity  

This document explains **what purity means**, **why it matters**, and **how to apply it** — without defining rules.  
For rules, see **Architecture Governance → Purity Rules**.

---

# 1. What “Purity” Means in This Architecture

Purity means:

- Each layer does only what it is designed to do  
- No layer leaks responsibilities into another  
- No layer depends on details it shouldn’t know about  
- No layer performs work that belongs elsewhere  
- Behavior is deterministic and testable  

Purity is not about “functional programming purity.”  
It’s about **architectural cleanliness**.

Purity ensures that:

- API stays thin  
- Application stays orchestration‑focused  
- Domain stays business‑focused  
- Infrastructure stays integration‑focused  
- Frank stays cross‑cutting and reusable  

---

# 2. Why Purity Matters

Purity is the backbone of:

## **2.1 Testability**
Pure layers are easy to test because:

- They have no hidden dependencies  
- They don’t reach across boundaries  
- They don’t mutate global state  
- They don’t depend on infrastructure  

This enables:

- Deterministic unit tests  
- Fast integration tests  
- Predictable behavior in CI  
- Reliable PR preview environments  

## **2.2 Replaceability**
Pure components can be replaced without cascade failures.

Examples:

- Swap a repository implementation  
- Replace a hosting provider  
- Change authentication flows  
- Add new slices without touching existing ones  

## **2.3 Developer Velocity**
Purity reduces cognitive load:

- Developers know where code belongs  
- Slices are self‑contained  
- Layers behave consistently  
- Onboarding is faster  
- Refactoring is safer  

## **2.4 Operational Safety**
Pure layers prevent:

- Leaking secrets  
- Leaking internal exceptions  
- Inconsistent error shapes  
- Infrastructure failures affecting domain logic  

---

# 3. Purity Across Layers

Purity is applied differently in each layer.

## **3.1 API Purity**
The API layer is a **boundary**, not a business layer.

It should:

- Map HTTP → Application  
- Validate requests  
- Shape responses  
- Apply authentication & authorization  
- Use dispatchers  
- Use Frank middleware  

It should **not**:

- Contain business logic  
- Access the database  
- Construct domain entities  
- Call external services  

## **3.2 Application Purity**
The Application layer is the **orchestrator**.

It should:

- Coordinate use cases  
- Invoke domain logic  
- Dispatch domain events  
- Use abstractions (readers, repositories)  
- Remain deterministic  

It should **not**:

- Perform persistence  
- Perform HTTP calls  
- Contain UI logic  
- Contain hosting logic  
- Contain protocol logic (e.g., OIDC)  

## **3.3 Domain Purity**
The Domain layer is the **heart of the business**.

It should:

- Contain business rules  
- Contain invariants  
- Contain aggregates, entities, value objects  
- Raise domain events  

It should **not**:

- Know about HTTP  
- Know about databases  
- Know about hosting  
- Know about authentication providers  
- Know about infrastructure  

## **3.4 Infrastructure Purity**
Infrastructure is the **integration layer**.

It should:

- Implement repositories  
- Implement readers  
- Integrate with databases  
- Integrate with external services  
- Implement hosting providers  

It should **not**:

- Contain business rules  
- Construct domain entities incorrectly  
- Leak infrastructure types into Application or Domain  

## **3.5 Frank Purity**
Frank is the **cross‑cutting backbone**.

It should:

- Provide reusable primitives  
- Provide DI auto‑registration  
- Provide endpoint discovery  
- Provide validator scanning  
- Provide hosting provider abstractions  
- Provide security headers  
- Provide error boundaries  
- Provide dispatcher pipeline  

It should **not**:

- Contain product‑specific logic  
- Depend on CampFitFurDogs  
- Contain domain rules  
- Contain business logic  

---

# 4. Purity and Vertical Slices

Purity enables slices to be:

- Self‑contained  
- Predictable  
- Easy to navigate  
- Easy to test  
- Easy to extend  

A pure slice spans:

```
Api → Application → Domain → Infrastructure
```

Each layer contributes only what it is responsible for.

This prevents:

- API doing Application work  
- Application doing Domain work  
- Domain doing Infrastructure work  
- Infrastructure leaking into Domain  

---

# 5. Purity and Pipelines

Purity is enforced through:

- Dispatcher pipeline  
- Validation pipeline  
- Domain event pipeline  
- ImmutableContextBuilder pipelines  
- Hosting provider pipeline  
- StartupModule Engine  

Each pipeline isolates responsibilities and ensures:

- Deterministic behavior  
- Clear ordering  
- Clear failure semantics  
- Testability  

---

# 6. Purity and Testing

Pure architecture enables:

- Unit tests for Domain  
- Handler tests for Application  
- Endpoint tests for API  
- Integration tests for Infrastructure  
- Full‑stack tests for slices  

Because layers are pure:

- Tests don’t require real infrastructure  
- Tests don’t require real HTTP  
- Tests don’t require real hosting  
- Tests don’t require real identity providers  

---

# 7. Purity and Developer Experience

Purity improves DX by:

- Making code predictable  
- Making slices consistent  
- Making layers clear  
- Making onboarding faster  
- Making refactoring safer  
- Making debugging easier  

Developers always know:

- Where code belongs  
- Where code does *not* belong  
- How layers interact  
- How slices are structured  

---

# 8. Purity in Practice: A Quick Checklist

A developer can use this mental checklist:

- **API**: Am I doing anything other than mapping HTTP to Application?  
- **Application**: Am I orchestrating, not implementing business rules?  
- **Domain**: Am I expressing business rules without infrastructure concerns?  
- **Infrastructure**: Am I integrating external systems without leaking them upward?  
- **Frank**: Am I building reusable primitives, not product logic?  

If the answer is “no,” purity is preserved.

---

# 9. Summary

Purity is the foundation of:

- Testability  
- Replaceability  
- Developer velocity  
- Operational safety  
- Architectural clarity  

This guide explains **why purity matters** and **how to think about it**.  
For enforceable rules, see:

```
docs/governance/technical/purity-rules.md
```

Purity is not a restriction — it’s a superpower.
