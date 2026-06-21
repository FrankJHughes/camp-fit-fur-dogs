# Frank Endpoint Registration Engine — Tester Guide

This guide documents the **current state** of the Frank Endpoint Registration Engine and the **intended future state** as the platform evolves.

The Endpoint Registration Engine provides a **convention‑based mechanism** for discovering, instantiating, and mapping API endpoints.  
Testers validate the *behavior* of the engine — not the behavior of the endpoints themselves.

---

# 1. Current State (Before Future Enhancements)

The engine currently consists of:

- `IEndpoint` — the contract for endpoint modules  
- `EndpointRegistrationEngine.AddEndpoints(assembly)` — discovers endpoint types  
- `EndpointRegistrationEngine.MapEndpoints(app)` — instantiates and maps endpoints  
- `app.MapEndpoints()` — convenience extension  

### What exists today (testable behavior)

- **Discovery**  
  - Only non‑abstract types implementing `IEndpoint` are discovered  
  - Discovery is assembly‑based  
  - Discovery is explicit (developer must call `AddEndpoints`)  
  - Discovered types are cached in a thread‑safe dictionary  

- **Instantiation**  
  - Endpoints are instantiated via `Activator.CreateInstance`  
  - Endpoints must have parameterless constructors  
  - Endpoints must be stateless  

- **Mapping**  
  - Each discovered endpoint has its `Map` method invoked  
  - Mapping order is not guaranteed  
  - Mapping is synchronous  
  - Mapping is idempotent (calling twice does not break the system)  

### What does *not* exist today (not testable)

- No DI activation  
- No constructor injection  
- No endpoint ordering  
- No grouping or versioning  
- No metadata conventions  
- No automatic assembly scanning  
- No diagnostics or logging  
- No filtering by namespace or attribute  

### What testers validate today

- The engine discovers the correct endpoint types  
- The engine instantiates endpoints successfully  
- The engine invokes `Map` on each endpoint  
- The engine ignores abstract types  
- The engine ignores non‑`IEndpoint` types  
- The engine handles duplicate discovery safely  
- The engine does not throw when mapping endpoints  
- The engine does not persist state between runs (beyond the static cache)  

The current capability is intentionally minimal and deterministic.

---

# 2. Future Intent (After Capability Expansion)

As the platform evolves, testers will validate richer engine behavior.

### 2.1 Dependency Injection Support

Future tests will validate:

- Constructor injection  
- Scoped and transient endpoint lifetimes  
- DI‑aware activation failures  
- DI‑based endpoint dependencies  

### 2.2 Endpoint Grouping and Versioning

Future tests will validate:

- Namespace‑based grouping  
- Versioning conventions  
- Route prefix conventions  
- Tag conventions  

### 2.3 Assembly Scanning Enhancements

Future tests will validate:

- Automatic scanning of all loaded assemblies  
- Attribute‑based filtering  
- Module‑based endpoint registration  

### 2.4 Metadata and Conventions

Future tests will validate:

- Automatic OpenAPI metadata  
- Automatic authorization conventions  
- Automatic validation conventions  
- Automatic logging conventions  

### 2.5 Diagnostics and Observability

Future tests will validate:

- Discovery diagnostics  
- Mapping logs  
- Endpoint registration metrics  
- Failure reporting  

These enhancements will expand the testing surface significantly.

---

# 3. Required Test Types (Current State)

## 3.1 Discovery Tests

Validate:

- Only types implementing `IEndpoint` are discovered  
- Abstract classes are ignored  
- Types without parameterless constructors are ignored or fail predictably  
- Duplicate types do not cause errors  
- Discovery is deterministic  

### Example

```csharp
EndpointRegistrationEngine.AddEndpoints(typeof(TestEndpoint).Assembly);
Assert.Contains(typeof(TestEndpoint), DiscoveredTypes());
```

---

## 3.2 Instantiation Tests

Validate:

- Endpoints are instantiated via `Activator.CreateInstance`  
- Missing parameterless constructors cause predictable failures  
- Endpoints remain stateless  

### Example

```csharp
var endpoint = Activator.CreateInstance(typeof(TestEndpoint));
Assert.NotNull(endpoint);
```

---

## 3.3 Mapping Invocation Tests

Validate:

- `Map` is called exactly once per endpoint  
- Mapping order is not guaranteed  
- Mapping does not throw  
- Mapping is idempotent  

### Example

```csharp
var endpoint = new TestEndpoint();
endpoint.Map(app);
Assert.True(endpoint.WasMapped);
```

---

## 3.4 Integration Tests (Engine + Minimal API)

Validate:

- Endpoints register routes correctly  
- Routes appear in the endpoint data source  
- No duplicate routes are created  
- Mapping does not break other middleware  

---

# 4. Anti‑Patterns (Tests Must Reject)

- Tests that assume DI activation  
- Tests that assume constructor injection  
- Tests that assume endpoint ordering  
- Tests that assume grouping or versioning  
- Tests that assume metadata conventions  
- Tests that assume automatic assembly scanning  
- Tests that assume logging or diagnostics  
- Tests that assume endpoint statefulness  

These features do **not** exist today.

---

# 5. Summary

**Current State:**  
Testers validate:

- Endpoint discovery  
- Endpoint instantiation  
- Endpoint mapping  
- Idempotency  
- Statelessness  
- Correct handling of invalid endpoint types  

**Future Intent:**  
Testers will validate:

- DI activation  
- Grouping and versioning  
- Automatic assembly scanning  
- Metadata conventions  
- Diagnostics and observability  

This Tester Guide prepares testers for both the current minimal engine and the richer future Endpoint Registration Engine.

