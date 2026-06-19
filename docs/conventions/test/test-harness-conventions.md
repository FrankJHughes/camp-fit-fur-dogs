# Test Harness Conventions (CampFitFurDogs)

CampFitFurDogs uses a custom ApiFactory‑based test harness built on top of Frank’s
test seams. The harness ensures deterministic, isolated, environment‑free tests.

---

## ApiFactory Rules

The test harness must:

- use `CampFitFurDogsApiFactory` for all backend tests  
- override services using `WithServiceOverride`  
- replace real builders with fakes  
- replace HttpClient with fake handlers  
- run with no real environment variables  
- run with no real hosting providers  

ApiFactory must not:

- hit real databases  
- hit real identity providers  
- hit real external services  

---

## Determinism Requirements

Tests must:

- run in any order  
- not depend on system time (use time abstractions)  
- not depend on environment variables  
- not depend on network availability  

---

## Prohibitions

Tests must not:

- use `WebApplicationFactory` directly  
- use real HttpClient  
- use real cookies  
- use real hosting providers  
