# Integration Test Conventions (CampFitFurDogs)

Integration tests verify end‑to‑end behavior across Api, Application, Domain, and
Infrastructure slices using the ApiFactory harness.

---

## Scope

Integration tests must:

- use the full pipeline  
- use in‑memory database  
- use fake external services  
- use fake builders  
- assert real HTTP responses  
- assert cookies, redirects, and DTO shapes  

Integration tests must not:

- hit real databases  
- hit real identity providers  
- hit real external services  

---

## Authentication Callback Integration Tests

Tests must assert:

- missing `code` → 400  
- missing `state` → 400  
- protocol builder invoked  
- application builder invoked  
- cookie issued  
- cookie flags correct  
- redirect correct  

---

## Repository Integration Tests

Tests must:

- use in‑memory database  
- seed deterministic data  
- assert repository behavior through Application handlers  

---

## Prohibitions

Integration tests must not:

- bypass Api endpoints  
- bypass the dispatcher  
- use Infrastructure directly  
