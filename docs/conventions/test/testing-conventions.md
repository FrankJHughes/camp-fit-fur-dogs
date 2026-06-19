# Testing Conventions (CampFitFurDogs)

CampFitFurDogs tests rely on Frank’s test seams and the ApiFactory test harness.
Tests must be deterministic, isolated, and free of external dependencies.

---

## ApiFactory Usage

Tests must:

- use `CampFitFurDogsApiFactory`
- override services using `WithServiceOverride`
- replace real builders with fakes
- replace HttpClient with fake handlers

Tests must not:

- hit real databases
- hit real identity providers
- hit real external services

---

## Callback Flow Tests

Tests must assert:

- missing `code` → 400
- missing `state` → 400
- cookie is issued
- cookie name is correct
- cookie is opaque
- cookie flags are correct
- redirect URL matches Application builder output

---

## Repository Tests

Repositories must be tested using:

- in‑memory database
- deterministic seed data
- no external dependencies

---

## Reader Tests

Readers must:

- use `AsNoTracking`
- return DTOs
- never mutate state

---

## Frontend Tests

Frontend tests must:

- mock API layer
- avoid real network calls
- test components as pure functions
- test hooks with mocked API clients

---

## Prohibitions

Tests must not:

- depend on environment variables
- depend on real hosting providers
- depend on real HttpClient
- depend on real cookies
