# Fake Builder Conventions (CampFitFurDogs)

CampFitFurDogs tests rely on fake builders to simulate:

- Frank’s OIDC protocol pipeline  
- Application authentication callback pipeline  

Fakes must implement the same generic signatures as the real builders.

---

## Requirements for Fake Builders

Fake builders must:

- be deterministic  
- be side‑effect‑free  
- return predictable results  
- simulate success and failure paths  
- expose configurable behavior for tests  

Fake builders must not:

- call external services  
- validate tokens  
- perform protocol logic  
- perform business logic  

---

## Usage in Tests

Tests must override real builders using:

````csharp
WithServiceOverride(services =>
{
    services.AddSingleton<IProtocolBuilder, FakeProtocolBuilder>();
    services.AddSingleton<IApplicationCallbackBuilder, FakeCallbackBuilder>();
});
````

---

## Prohibitions

Fake builders must not:

- depend on Infrastructure  
- depend on HttpContext  
- depend on environment variables  
