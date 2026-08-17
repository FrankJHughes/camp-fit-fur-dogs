# Frank.Testing — Overview

The **Frank.Testing** module provides the infrastructure required for realistic, end‑to‑end and integration‑level testing of the platform. It enables tests to exercise the full HTTP pipeline — routing, middleware, Identity flows, persistence, and infrastructure — without relying on external services or production configuration.

Testing is built around three core subsystems:

- **[Application Contexts](ca://s?q=Explain_testing_application_contexts)** — deterministic test web application builders  
- **[Endpoints](ca://s?q=Explain_testing_endpoints)** — helpers for issuing HTTP requests and validating responses  
- **[Factories](ca://s?q=Explain_testing_factories)** — builders for domain models, EF Core entities, and test data  

This document describes the testing subsystem under:

```
/docs/04-testing
```

and maps it back to its implementation under:

```
/src/Frank/Testing
```

---

## Key Components

### Application Contexts — `/src/Frank/Testing/Contexts`

The **Contexts** subsystem provides test‑friendly web application builders that allow tests to mutate the runtime environment safely and deterministically.

Key components:

- **`MutatedWebApplicationBuilder`** — constructs a test web app with overridden services  
- **`MutatedWebApplicationContext`** — manages the lifecycle of the test host  
- factory methods for common test scenarios (authenticated user, invalid configuration, fake providers)

Contexts allow tests to override DI, configuration, middleware, and environment flags without touching production code.

---

### Endpoints — `/src/Frank/Testing/Endpoints`

The **Endpoints** subsystem provides helpers for issuing HTTP requests and validating responses against the test host.

Capabilities:

- request/response builders  
- HTTP helper methods for common operations  
- authentication and authorization stubs  
- utilities for simulating session tokens, headers, and correlation IDs  

Endpoint helpers ensure tests remain expressive and avoid boilerplate.

---

### Factories — `/src/Frank/Testing/Factories`

The **Factories** subsystem provides deterministic object construction helpers.

Capabilities:

- default entity factories  
- aggregate root constructors  
- domain model builders  
- test data generators for common identity scenarios  

Factories ensure tests can create realistic objects without duplicating construction logic.

---

## Typical Test Flow

```csharp
var context = MutatedWebApplicationBuilder
    .Create()
    .WithService<ITimeProvider>(testClock)
    .BuildAsync();

using var client = context.CreateHttpClient();

var response = await client.PostAsJsonAsync("/api/dogs", 
    new { name = "Buddy", breed = "Golden" });

assert(response.IsSuccessStatusCode);
```

This pattern exercises the full HTTP pipeline:

```
Test → Mutated Context → Test Host → Middleware → Endpoint → Application → Domain → EF Core
```

---

## Testing Patterns

### [Isolated Test Apps](ca://s?q=Explain_testing_isolated_apps)
Use `MutatedWebApplicationContext` to create isolated test hosts with custom DI, configuration, and middleware.

### [Selective Dependency Overrides](ca://s?q=Explain_testing_dependency_overrides)
Override only the services relevant to the test scenario (e.g., clock, provider metadata, logging sinks).

### [Full Pipeline Execution](ca://s?q=Explain_testing_full_pipeline)
Prefer exercising the entire HTTP pipeline instead of mocking layers — this validates real behavior.

### [Domain‑Aligned Test Data](ca://s?q=Explain_testing_domain_data)
Keep test data builders close to the domain models they construct to ensure invariants remain valid.

---

## How Testing Connects to the Broader Platform

Testing collaborates with:

- **Identity API** — middleware, routing, endpoint handlers  
- **Identity Application** — authentication/session orchestration  
- **Identity Domain** — invariants surfaced through test flows  
- **Identity EF Core** — persistence tested through real or in‑memory databases  
- **Identity Infrastructure** — provider integration, configuration binding, logging  
- **Frank Test Harness (US‑176)** — deterministic setup/teardown, DI overrides, fake providers  

Testing ensures the entire vertical behaves correctly under realistic runtime conditions.

---

## Notes

Keep this document grounded in the actual Frank.Testing implementation.  
As new testing utilities, identity features, or runtime behaviors are added, update this overview to reflect the evolving architecture.
