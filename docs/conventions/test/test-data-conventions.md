# Test Data Conventions (CampFitFurDogs)

Test data must be deterministic, minimal, and explicit.  
Tests must not rely on implicit or hidden data.

---

## Test Data Rules

Test data must:

- be created explicitly in each test  
- avoid global shared state  
- avoid random values  
- avoid timestamps unless using time abstractions  

---

## Seed Data

Seed data must:

- be minimal  
- be deterministic  
- be created via helper methods  
- not depend on environment variables  

---

## Builders

Test data builders must:

- produce valid domain objects  
- avoid side effects  
- avoid persistence  
- avoid randomness  

---

## Prohibitions

Test data must not:

- use random GUIDs  
- use DateTime.UtcNow directly  
- depend on real repositories  
