# Unit Test Conventions (CampFitFurDogs)

Unit tests validate pure logic in isolation. They must be fast, deterministic,
and free of external dependencies.

---

## Scope

Unit tests must:

- test a single function, method, or behavior  
- use fakes or stubs for dependencies  
- avoid ApiFactory  
- avoid EF Core  
- avoid HttpClient  
- avoid environment variables  

Unit tests must not:

- hit databases  
- hit external services  
- depend on system time (use time abstractions)  
- depend on DI containers  

---

## Structure

Unit tests must:

- follow AAA (Arrange‑Act‑Assert)  
- use descriptive test names  
- assert behavior, not implementation details  
- avoid mocking internals  

---

## Prohibitions

Unit tests must not:

- use ApiFactory  
- use real HttpClient  
- use real repositories  
- use real builders  
