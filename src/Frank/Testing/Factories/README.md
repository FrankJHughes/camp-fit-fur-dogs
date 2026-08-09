# Testing — Factories

The **Factories** folder contains the core host‑construction machinery used by the
Frank testing harness.  
These factories combine the mutation‑based contexts from the **Contexts** folder
with ASP.NET Core’s `WebApplicationFactory` to produce fully customized,
environment‑aware, authentication‑aware, database‑aware test hosts.

This folder currently contains:

- `MutatedWebApplicationFactory` — the main extensible test host factory  
- `CookieRewriteStartupFilter` — a startup filter enabling HTTP cookie usage in tests  

Together, these components form the backbone of your integration testing
infrastructure.

---

## Purpose

The Factories subsystem provides:

- A configurable test host that respects all mutations from:
  - Application context (`MutatedWebApplicationContext`)
  - Client context (`MutatedWebApplicationClientContext`)
- Support for:
  - Environment overrides  
  - Configuration overrides  
  - Service overrides  
  - Fake service injection  
  - Endpoint assembly discovery  
  - Cookie authentication downgrade  
  - Optional PostgreSQL test containers  
- A clean extension model via virtual methods  

It ensures that test hosts behave exactly as specified by your mutated contexts.

---

## Files

### **MutatedWebApplicationFactory\<TEntryPoint, TContext, TClientContext>**

The central test host factory.

Responsibilities:

- Builds the test host using the mutated application context  
- Applies:
  - Environment selection  
  - Configuration overrides  
  - Service overrides  
  - Fake service injection  
  - Database enable/disable logic  
  - Endpoint assembly registration  
  - Cookie rewrite startup filter  
- Provides extension points:
  - `ConfigureMutations`  
  - `ConfigureDatabase`  
  - `ConfigureDatabaseDisabled`  
  - `ApplyAuthenticationAsync`  
- Supports PostgreSQL test containers via `WithDatabaseAsync`  
- Exposes the underlying `IServiceCollection` for introspection  

Used during:

- Integration test host construction  
- Database‑enabled test scenarios  
- Authentication simulation  
- Endpoint discovery  
- DI customization  

Example usage:

```csharp
var factory = new MyFactory(ctx)
    .WithDatabaseAsync()
    .Result;

var client = factory.CreateClient(clientCtx);
```

---

### **CookieRewriteStartupFilter**

A startup filter that rewrites `Set-Cookie` headers to remove the `Secure`
attribute.

Responsibilities:

- Enables cookie usage over HTTP in test environments  
- Rewrites all outgoing cookie headers  
- Ensures compatibility with cookie‑based authentication flows during testing  

Used during:

- Cookie authentication scenarios  
- Local HTTP test environments  
- Any test requiring cookies without HTTPS  

---

## Design Principles

The Factories subsystem follows these principles:

- **Mutation‑driven configuration**  
  All behavior originates from the application and client contexts.

- **Extensibility**  
  Virtual methods allow targeted customization without modifying the core factory.

- **Deterministic behavior**  
  Database lifecycle, configuration overrides, and cookie rewriting behave
  predictably across tests.

- **Separation of concerns**  
  Contexts define *what* should happen; factories define *how* it is applied.

- **Full control over the test host**  
  Every part of the ASP.NET Core pipeline can be mutated or overridden.

---

## Example Workflow

```csharp
var ctx = new MyAppContext()
    .WithEnvironment("Integration")
    .WithDatabase(true, postgres)
    .WithServiceOverride(services => services.AddSingleton<IMyFake, MyFake>());

var clientCtx = new MyClientContext()
    .WithCurrentUser("auth0|123")
    .WithHeader("X-Test", "true");

var factory = new MyFactory(ctx);
var client = factory.CreateClient(clientCtx);
```

---

## Summary

The **Factories** folder provides the core machinery for constructing fully
customized test hosts:

- Mutation‑aware host construction  
- Database lifecycle management  
- Authentication simulation  
- Cookie rewrite support  
- Endpoint assembly registration  
- DI and configuration overrides  

It is the foundation of the Frank testing harness, enabling expressive,
deterministic, and deeply configurable integration tests.

