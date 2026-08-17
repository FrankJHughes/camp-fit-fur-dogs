# Frank — Testing Module

The **Testing** module provides a deterministic, mutation‑driven foundation for
integration testing across the Frank platform.  
It is designed to make test environments **predictable**, **expressive**, and
**fully configurable**, without leaking test‑specific behavior into production
code.

This module consists of three major subsystems:

- **Contexts** — immutable configuration objects that shape the test host  
- **Factories** — host builders that apply context mutations  
- **Endpoints** — lightweight diagnostic endpoints for test scenarios  
- **AssemblyMarker** — stable anchor for assembly discovery  

Together, these components form a unified testing harness that supports
environment simulation, authentication flows, database containers, DI overrides,
and controlled failure scenarios.

---

## Goals

The Testing module exists to provide:

- Deterministic test host construction  
- Clear separation between test logic and production logic  
- Mutation‑based configuration for both host and client  
- Support for PostgreSQL test containers  
- Support for cookie‑based authentication in HTTP environments  
- Fake service injection for test doubles  
- Diagnostic endpoints for verifying authentication, routing, and error handling  
- A stable assembly marker for endpoint discovery  

This module is intentionally **framework‑level**, not application‑level.

---

## Folder Structure

```
Testing/
│
├── AssemblyMarker.cs
│
├── Contexts/
│   ├── MutatedWebApplicationContext.cs
│   ├── MutatedWebApplicationContextExtensions.cs
│   ├── MutatedWebApplicationClientContext.cs
│   └── MutatedWebApplicationClientContextExtensions.cs
│
├── Factories/
│   ├── MutatedWebApplicationFactory.cs
│   └── CookieRewriteStartupFilter.cs
│
└── Endpoints/
    ├── CurrentUserIdEndpoint.cs
    ├── HealthCheckEndpoint.cs
    └── ThrowEndpoint.cs
```

---

## Subsystems

### **Contexts**

Immutable, mutation‑friendly configuration objects used to shape the test host
and test client.

Capabilities include:

- Environment selection  
- Database enable/disable + PostgreSQL container  
- Cookie‑only authentication  
- Cookie rewrite behavior  
- Configuration overrides  
- Service overrides  
- Cookie options overrides  
- Endpoint assembly discovery  
- Fake service injection  
- Request‑level identity simulation  
- Default header injection  

Contexts are the **source of truth** for test configuration.

---

### **Factories**

Factories apply context mutations to construct a fully configured test host.

Key features:

- Integrates with ASP.NET Core’s `WebApplicationFactory`  
- Applies all context mutations (environment, config, DI, cookies, endpoints)  
- Supports PostgreSQL lifecycle via `WithDatabaseAsync`  
- Provides extension points:
  - `ConfigureMutations`
  - `ConfigureDatabase`
  - `ConfigureDatabaseDisabled`
  - `ApplyAuthenticationAsync`
- Exposes the underlying `IServiceCollection` for introspection  
- Includes a startup filter to rewrite `Set-Cookie` headers for HTTP testing  

Factories are the **execution engine** of the testing harness.

---

### **Endpoints**

Lightweight, deterministic endpoints used exclusively for testing.

Included endpoints:

- `/__test__/current-user-id` — exposes the authenticated user ID  
- `/__test__/health` — verifies host availability  
- `/__test__/throw` — triggers a controlled exception  

These endpoints help validate:

- Authentication flows  
- Middleware behavior  
- Error handling  
- Routing  
- Host startup  

Endpoints are the **diagnostic tools** of the testing harness.

---

### **AssemblyMarker**

A simple marker type used for:

- Assembly scanning  
- Endpoint discovery  
- Resource lookup  
- Test harness configuration  

It provides a stable anchor for referencing the Testing assembly.

---

## Design Principles

The Testing module follows these principles:

- **Immutable by default**  
  All contexts return new instances on mutation.

- **Mutation‑driven configuration**  
  Tests describe *what* they need; the harness applies it.

- **Deterministic behavior**  
  No shared mutable state across tests.

- **Separation of concerns**  
  Contexts define configuration; factories apply it; endpoints validate it.

- **Minimal test‑only surface area**  
  No production logic leaks into the testing layer.

- **Extensibility**  
  New contexts, factories, or endpoints can be added without breaking existing tests.

---

## Example Workflow

```csharp
var appCtx = new MyAppContext()
    .WithEnvironment("Integration")
    .WithDatabase(true, postgres)
    .WithServiceOverride(services => services.AddSingleton<IMyFake, MyFake>())
    .WithEndpointAssembly(typeof(MyApi.AssemblyMarker).Assembly);

var clientCtx = new MyClientContext()
    .WithCurrentUser("auth0|123")
    .WithHeader("X-Test", "true");

var factory = new MyFactory(appCtx);
var client = factory.CreateClient(clientCtx);

var health = await client.GetStringAsync("/__test__/health");
var userId = await client.GetStringAsync("/__test__/current-user-id");
```

---

## Summary

The **Testing** module provides a complete, mutation‑driven testing harness for
the Frank platform:

- Contexts define configuration  
- Factories apply configuration  
- Endpoints validate behavior  
- AssemblyMarker anchors discovery  

It ensures that integration tests are expressive, deterministic, and deeply
configurable — without compromising production purity.

