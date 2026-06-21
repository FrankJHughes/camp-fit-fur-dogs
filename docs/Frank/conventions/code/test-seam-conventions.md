# Test Seam Conventions (Frank)

Frank provides seams that allow products to test hosting, environment, identity,
and external integrations without touching real systems.

---

## HttpClient Seam

All external HTTP calls must use **named HttpClients**.

Tests replace them using:

- `FakeHttpMessageHandler`  
- `WithServiceOverride`  

No real network calls are allowed in tests.

---

## Hosting Provider Seam

Hosting providers depend on abstractions:

- `IEnvironment`  
- `IRenderPrParser`  
- `IGitHubArtifactClient`  
- `IRenderConfigurationWriter`  

Tests supply fakes for all of these.

---

## Identity Seam

Identity is resolved via:

````csharp
ICurrentUserService
````

Tests use `FakeCurrentUser`.

---

## Audit Logging Seam

Audit logging is abstracted behind an interface.

Tests use `FakeAuditLogger`.

---

## Prohibitions

Frank must not:

- require real environment variables in tests  
- require real HTTP calls  
- require real hosting providers  

Products must not:

- bypass Frank’s seams  
- mock HttpClient directly (must use handlers)  
