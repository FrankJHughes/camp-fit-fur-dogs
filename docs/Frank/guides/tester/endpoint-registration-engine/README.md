# Frank — Tester Guide — Endpoint Registration Engine  
*How to test the current and future Endpoint Registration Engine.*

This guide documents the **current state** of the Frank Endpoint Registration Engine and the **intended future state** as the platform evolves.

The Endpoint Registration Engine provides a **convention‑based mechanism** for discovering, instantiating, and mapping API endpoints.  
Testers validate the *behavior of the engine itself* — **not** the behavior of the endpoints.

---

# 1. Current State (Before Future Enhancements)

The engine currently consists of:

- `IEndpoint` — the contract for endpoint modules  
- `EndpointRegistrationEngine.AddEndpoints(assembly)` — discovers endpoint types  
- `EndpointRegistrationEngine.MapEndpoints(app)` — instantiates and maps endpoints  
- `app.MapEndpoints()` — convenience extension  

---

## What exists today (testable behavior)

### **Discovery**
- only non‑abstract types implementing `IEndpoint` are discovered  
- discovery is assembly‑based  
- discovery is explicit (developer must call `AddEndpoints`)  
- discovered types are cached in a thread‑safe dictionary  

### **Instantiation**
- endpoints are instantiated via `Activator.CreateInstance`  
- endpoints must have parameterless constructors  
- endpoints must be stateless  

### **Mapping**
- each discovered endpoint has its `Map` method invoked  
- mapping order is not guaranteed  
- mapping is synchronous  
- mapping is idempotent (calling twice does not break the system)  

---

## What does *not* exist today (not testable)

- no DI activation  
- no constructor injection  
- no endpoint ordering  
- no grouping or versioning  
- no metadata conventions  
- no automatic assembly scanning  
- no diagnostics or logging  
- no namespace/attribute filtering  

---

## What testers validate today

Testers validate:

- correct endpoint discovery  
- correct instantiation  
- correct invocation of `Map`  
- ignoring abstract types  
- ignoring non‑`IEndpoint` types  
- safe handling of duplicate discovery  
- mapping does not throw  
- mapping does not persist state between runs (beyond static cache)  

The current capability is intentionally minimal and deterministic.

---

# 2. Future Intent (After Capability Expansion)

As the platform evolves, testers will validate richer engine behavior.

---

## 2.1 Dependency Injection Support

Future tests will validate:

- constructor injection  
- scoped and transient endpoint lifetimes  
- DI‑aware activation failures  
- DI‑based endpoint dependencies  

---

## 2.2 Endpoint Grouping and Versioning

Future tests will validate:

- namespace‑based grouping  
- versioning conventions  
- route prefix conventions  
- tag conventions  

---

## 2.3 Assembly Scanning Enhancements

Future tests will validate:

- automatic scanning of all loaded assemblies  
- attribute‑based filtering  
- module‑based endpoint registration  

---

## 2.4 Metadata and Conventions

Future tests will validate:

- automatic OpenAPI metadata  
- automatic authorization conventions  
- automatic validation conventions  
- automatic logging conventions  

---

## 2.5 Diagnostics and Observability

Future tests will validate:

- discovery diagnostics  
- mapping logs  
- endpoint registration metrics  
- failure reporting  

These enhancements will significantly expand the testing surface.

---

# 3. Required Test Types (Current State)

## 3.1 Discovery Tests

Validate:

- only types implementing `IEndpoint` are discovered  
- abstract classes are ignored  
- types without parameterless constructors are ignored or fail predictably  
- duplicate types do not cause errors  
- discovery is deterministic  

### Example

````csharp
EndpointRegistrationEngine.AddEndpoints(typeof(TestEndpoint).Assembly);
Assert.Contains(typeof(TestEndpoint), DiscoveredTypes());
````

---

## 3.2 Instantiation Tests

Validate:

- endpoints are instantiated via `Activator.CreateInstance`  
- missing parameterless constructors cause predictable failures  
- endpoints remain stateless  

### Example

````csharp
var endpoint = Activator.CreateInstance(typeof(TestEndpoint));
Assert.NotNull(endpoint);
````

---

## 3.3 Mapping Invocation Tests

Validate:

- `Map` is called exactly once per endpoint  
- mapping order is not guaranteed  
- mapping does not throw  
- mapping is idempotent  

### Example

````csharp
var endpoint = new TestEndpoint();
endpoint.Map(app);
Assert.True(endpoint.WasMapped);
````

---

## 3.4 Integration Tests (Engine + Minimal API)

Validate:

- endpoints register routes correctly  
- routes appear in the endpoint data source  
- no duplicate routes are created  
- mapping does not break other middleware  

---

# 4. Anti‑Patterns (Tests Must Reject)

Tests must reject assumptions about features that **do not exist today**:

- DI activation  
- constructor injection  
- endpoint ordering  
- grouping or versioning  
- metadata conventions  
- automatic assembly scanning  
- logging or diagnostics  
- endpoint statefulness  

Tests must reflect the **current minimal engine**, not the future one.

---

# 5. Summary

**Current State — Testers validate:**

- endpoint discovery  
- endpoint instantiation  
- endpoint mapping  
- idempotency  
- statelessness  
- correct handling of invalid endpoint types  

**Future Intent — Testers will validate:**

- DI activation  
- grouping and versioning  
- automatic assembly scanning  
- metadata conventions  
- diagnostics and observability  

This Tester Guide prepares testers for both the current minimal engine and the richer future Endpoint Registration Engine.
