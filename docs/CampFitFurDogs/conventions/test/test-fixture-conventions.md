# Test Fixture Conventions (CampFitFurDogs)

Test fixtures provide reusable setup for integration and unit tests.  
Fixtures must be deterministic and must not leak state between tests.

---

## Fixture Rules

Fixtures must:

- create fresh state for each test  
- avoid static state  
- avoid global caches  
- avoid shared DbContext instances  
- use in‑memory database for integration tests  

---

## ApiFactory Fixtures

ApiFactory fixtures must:

- create a new ApiFactory per test class  
- override services deterministically  
- replace external integrations with fakes  
- replace builders with fakes  

---

## Database Fixtures

Database fixtures must:

- use in‑memory database  
- seed deterministic data  
- avoid migrations  
- avoid real connection strings  

---

## Prohibitions

Fixtures must not:

- share state between tests  
- depend on environment variables  
- depend on real hosting providers  
- depend on real HttpClient  
