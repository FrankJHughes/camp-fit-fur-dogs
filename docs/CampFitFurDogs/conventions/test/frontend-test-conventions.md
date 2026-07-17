# Frontend Test Conventions (CampFitFurDogs)

Frontend tests validate UI components, hooks, and client‑side logic.  
They must be deterministic and must not hit real backend endpoints.

---

## Component Tests

Component tests must:

- treat components as pure functions  
- mock API layer calls  
- avoid real network calls  
- assert rendered output  
- assert state transitions  

Components must not:

- fetch data directly  
- embed business logic  

---

## Hook Tests

Hook tests must:

- mock API clients  
- simulate loading, success, and error states  
- assert state transitions  
- avoid real fetch calls  

---

## API Layer Tests

API layer tests must:

- mock fetch  
- assert correct request shape  
- assert correct response parsing  
- simulate error conditions  

---

## Prohibitions

Frontend tests must not:

- hit real backend endpoints  
- depend on browser APIs not supported by test environment  
- depend on real cookies  
