# Testing — Contexts

The **Contexts** folder contains the mutation‑based configuration primitives used
by the Frank testing harness.  
These contexts allow test suites to declaratively shape the behavior of the
test web application — including environment settings, authentication modes,
database usage, configuration overrides, service overrides, cookie options,
endpoint discovery, and fake service injection.

All contexts follow an immutable, record‑based design with fluent mutation
helpers that return new instances, ensuring deterministic and side‑effect‑free
test setup.

---

## Purpose

The Contexts subsystem provides:

- A unified way to configure test application behavior  
- Immutable, mutation‑friendly test contexts  
- Support for database containers (PostgreSQL)  
- Support for cookie‑only authentication scenarios  
- Configuration and DI overrides  
- Endpoint assembly discovery  
- Fake service injection for test doubles  
- Clean separation between test setup and test execution  

These contexts are consumed by the test harness when constructing the test
application host.

---

## Files

### **MutatedWebApplicationContext\<TSelf>**

The core test application context.

Responsibilities:

- Defines all configurable aspects of the test host:
  - Environment name  
  - Database enable/disable + PostgreSQL container  
  - Cookie‑only authentication  
  - Cookie override behavior  
  - Configuration overrides  
  - Service overrides  
  - Cookie authentication option overrides  
  - Endpoint assemblies  
  - Fake service registry  
- Provides fluent mutation helpers for all properties  
- Ensures immutability via record‑based cloning  
- Supports test doubles through the `Fakes` dictionary  

Used during:

- Test host construction  
- Environment simulation  
- Authentication mode switching  
- Database container orchestration  
- DI customization  
- Endpoint discovery  

---

### **MutatedWebApplicationContextExtensions**

Fluent mutation helpers for `MutatedWebApplicationContext`.

Responsibilities:

- Adds:
  - `WithEnvironment`  
  - `WithDatabase`  
  - `WithCookieAuthOnly`  
  - `WithCookieHttpOverride`  
  - `WithConfigOverride`  
  - `WithServiceOverride`  
  - `WithCookieOptionsOverride`  
- Ensures each mutation returns a new immutable context  
- Provides ergonomic, readable test setup code  

Example:

```csharp
ctx
    .WithEnvironment("Integration")
    .WithDatabase(true, postgres)
    .WithCookieAuthOnly()
    .WithConfigOverride(cfg => cfg.AddInMemoryCollection(values));
```

---

### **MutatedWebApplicationClientContext\<TSelf>**

A lightweight client‑side context used when issuing HTTP requests from the test
client.

Responsibilities:

- Defines:
  - Simulated current user subject (`sub`)  
  - Sign‑in scheme override  
  - Default request headers  
- Supports immutable mutation via record cloning  
- Used by the test client to simulate authenticated or customized requests  

Used during:

- Request‑level identity simulation  
- Header injection  
- Client‑side authentication overrides  

---

### **MutatedWebApplicationClientContextExtensions**

Fluent mutation helpers for the client context.

Responsibilities:

- Adds:
  - `WithCurrentUser`  
  - `WithHeader`  
  - `WithSignInScheme`  
- Enables ergonomic request customization  
- Ensures immutability and deterministic behavior  

Example:

```csharp
clientCtx
    .WithCurrentUser("auth0|123")
    .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString())
    .WithSignInScheme("TestScheme");
```

---

## Design Principles

The Contexts subsystem follows these principles:

- **Immutable by default**  
  Every mutation returns a new instance.

- **Fluent configuration**  
  Test setup reads like a declarative scenario.

- **Separation of concerns**  
  Application setup, client setup, and fake injection are isolated.

- **Deterministic behavior**  
  No shared mutable state across tests.

- **Extensible architecture**  
  New overrides or mutation helpers can be added without breaking existing tests.

---

## Example Usage

```csharp
var ctx = new MyTestContext()
    .WithEnvironment("Integration")
    .WithDatabase(true, postgres)
    .WithConfigOverride(cfg => cfg.AddJsonFile("testsettings.json"))
    .WithServiceOverride(services => services.AddSingleton<IMyFake, MyFake>())
    .WithCookieAuthOnly()
    .WithEndpointAssembly(typeof(MyApi.AssemblyMarker).Assembly);
```

---

## Summary

The **Contexts** folder provides the foundational mutation‑based configuration
system for the Frank testing harness:

- Application context  
- Client context  
- Fluent mutation helpers  
- Database + authentication configuration  
- DI + configuration overrides  
- Fake service injection  
- Endpoint assembly discovery  

It ensures that test environments are expressive, deterministic, and easy to
compose.

