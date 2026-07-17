# layering-rules.md

# Layering Rules (Frank)

Frank provides the cross‑cutting primitives, abstractions, and hosting model used
by CampFitFurDogs and other products.  
To remain reusable and product‑agnostic, Frank enforces strict layering rules.

Frank is **not** an application layer.  
Frank is a **platform layer**.

---

## Purpose of Frank’s Layering Rules

Frank exists to:

- provide stable abstractions  
- isolate external dependencies  
- enforce deterministic behavior  
- prevent product‑specific leakage  
- ensure platform‑level consistency  

Frank must remain **pure**, **predictable**, and **independent** of any product.

---

## Layer Definitions

### Core  
- fundamental primitives  
- functional helpers  
- immutable types  
- low‑level utilities  
- zero product knowledge  

### Abstractions  
- interfaces for hosting, configuration, HTTP, logging, environment  
- no implementations  
- no external dependencies  

### Implementations  
- hosting providers  
- HTTP clients  
- environment readers  
- configuration loaders  
- logging adapters  

### Extensions  
- optional helpers  
- syntactic sugar  
- convenience APIs  

Frank’s layers must not depend on CampFitFurDogs or any other product.

---

## Dependency Rules

### Core  
- depends on nothing  
- must not reference abstractions or implementations  

### Abstractions  
- may depend on Core  
- must not depend on implementations  

### Implementations  
- may depend on Abstractions + Core  
- must not depend on Extensions  

### Extensions  
- may depend on Abstractions + Core  
- may depend on Implementations  
- must not introduce product‑specific behavior  

---

## Prohibitions

Frank must not:

- reference CampFitFurDogs  
- reference any product’s Domain, Application, Infrastructure, or Api  
- contain business logic  
- contain slice‑specific logic  
- contain persistence logic  
- contain EF Core  
- contain DTOs from any product  
- contain product‑specific configuration keys  

Frank is a **platform**, not a feature layer.

---

## Enforcement

Frank layering rules are enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

Frank must remain **clean**, **stable**, and **product‑agnostic**.
