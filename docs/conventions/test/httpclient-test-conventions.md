# HttpClient Test Conventions (CampFitFurDogs)

All external HTTP calls must use named HttpClients.  
Tests must replace these with fake handlers.

---

## Fake HttpClient Rules

Tests must:

- use `FakeHttpMessageHandler`  
- override HttpClient via `WithServiceOverride`  
- simulate all external responses  
- avoid real network calls  

Fake handlers must:

- be deterministic  
- return stable responses  
- simulate error conditions  
- never throw raw HttpRequestException unless testing error paths  

---

## Prohibitions

Tests must not:

- use `new HttpClient()`  
- hit real external services  
- depend on network availability  
- depend on environment variables  
