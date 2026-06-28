
# Frank — Guides — User — Registration Engine Guide  
*How to use the Registration Engine directly.*

Frank’s Registration Engine provides a **governed, deterministic, explicit** mechanism for discovering and registering services into DI.

Unlike older systems, **Frank does not auto‑register anything.**  
The `[Registration]` attribute does **not** trigger registration — it only provides **parameters** used *if* the interface is selected by `DiscoveryOptions`.

The Registration Engine is **explicit**:

- You choose which interfaces are governed.  
- You choose which implementations are eligible.  
- You call `Orchestrator.Orchestrate`.  
- The engine performs scanning, planning, validation, and DI registration.

This guide covers **only the Registration Engine**, not any capability wrappers.

---

# 1. What the Registration Engine Does

When you invoke the engine:

1. You define governed interfaces using `DiscoveryOptions.IncludeInterface`.
2. You define eligible implementations using `IncludeImplementation`.
3. The engine:
   - scans assemblies  
   - finds governed interfaces  
   - finds implementing classes  
   - reads `[Registration]` metadata  
   - validates min/max constraints  
   - registers implementations into DI  

You get:

- deterministic DI registration  
- architectural rule enforcement  
- fast failure when contracts are violated  
- zero manual registration boilerplate  

---

# 2. How to Use the Registration Engine

## 2.1 Mark an Interface

```csharp
[Registration(ServiceLifetime.Scoped)]
public interface IMyService { }
```

This tells the engine:

- “If this interface is governed, use Scoped lifetime.”

You can also specify constraints:

```csharp
[Registration(ServiceLifetime.Singleton, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
public interface IValidator { }
```

This enforces exactly one implementation — **but only if the interface is selected by `DiscoveryOptions`**.

---

## 2.2 Implement the Interface

```csharp
public sealed class MyService : IMyService { }
```

You may have multiple implementations unless you restrict them.

---

## 2.3 Invoke the Registration Engine Directly  
This is the **canonical**, **direct**, **framework‑level** entry point:

```csharp
var options = new DiscoveryOptions();

// Governed interfaces
options.IncludeInterface(iface =>
    iface.IsInterface &&
    iface.GetCustomAttribute<RegistrationAttribute>() is not null);

// Eligible implementations
options.IncludeImplementation(impl =>
    impl.ImplementedInterfaces.Any(i =>
        i.GetCustomAttribute<RegistrationAttribute>() is not null));

Orchestrator.Orchestrate(services, assemblies, options);
```

### ✔ No auto‑registration  
### ✔ No implicit scanning  
### ✔ No capability wrappers required  
### ✔ You control everything through `DiscoveryOptions`

---

# 3. What Happens Inside the Engine

When you call `Orchestrator.Orchestrate`:

1. **Scanner**  
   - Finds governed interfaces  
   - Finds implementing classes  
   - Applies `DiscoveryOptions` filters  

2. **Planner**  
   - Builds a registration plan per governed interface  
   - Reads `[Registration]` metadata  
   - Determines lifetime and concrete type rules  

3. **Validator**  
   - Enforces min/max implementation constraints  
   - Produces violations (does not throw)  

4. **Registrar**  
   - Registers implementations into DI  
   - Applies lifetime  
   - Registers concrete types if requested  

5. **Orchestrator**  
   - Coordinates the pipeline  
   - Throws `InvalidOperationException` if violations exist  
   - Ensures deterministic behavior  

This ensures your DI container is always in a valid state.

---

# 4. How to Control Registration Behavior

### Lifetime  
Set via the attribute:

```csharp
[Registration(ServiceLifetime.Transient)]
```

### Min/Max Implementations  
Enforce architectural rules:

```csharp
[Registration(ServiceLifetime.Scoped, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
```

### Register Concrete Types  
If you want the concrete class registered as itself:

```csharp
[Registration(ServiceLifetime.Singleton, RegisterConcreteType = true)]
```

This produces two registrations:

- `IMyService -> MyService`
- `MyService -> MyService`

### DiscoveryOptions  
You fully control discovery:

```csharp
var options = new DiscoveryOptions();

options.IncludeInterface(iface => /* governed interface */);
options.IncludeImplementation(impl => /* implementing class */);

Orchestrator.Orchestrate(services, assemblies, options);
```

The engine does **only** what you tell it to.

---

# 5. What You Should Expect When Things Go Wrong

If constraints are violated, startup fails with a message like:

```
IMyService
requires between 1 and 1 implementations. It has 0:
```

This is intentional — the engine enforces architectural correctness.

Other failures include:

- missing implementations  
- multiple implementations when only one is allowed  
- invalid lifetimes  
- DI resolution failures  
- concrete type registration conflicts  

All failures are surfaced clearly and deterministically.

---

# 6. Best Practices

- Prefer direct use of `Orchestrator.Orchestrate` when building framework‑level features.
- Use `DiscoveryOptions` to explicitly govern interfaces and implementations.
- Use min/max constraints to enforce architectural rules.
- Prefer one implementation per interface unless polymorphism is intended.
- Use `RegisterConcreteType` only when you need direct resolution of the class.
- Keep interfaces small and focused.
- Avoid open generics unless explicitly supported.
- Keep implementations stateless and DI‑friendly.

---

# 7. Anti‑Patterns

Avoid:

- Expecting `[Registration]` to auto‑register anything.  
- Forgetting to include assemblies in the orchestrator call.  
- Setting `MinRegistrationCount > 0` without providing implementations.  
- Relying on manual DI registration for governed interfaces.  
- Using open generic classes (Registration ignores them).  
- Expecting abstract classes to be registered (they are ignored).  
- Registering governed interfaces without `[Registration]`.

---

# 8. Summary

As a user of the Registration Engine:

- You mark interfaces with `[Registration]`.  
- You implement those interfaces.  
- You call `Orchestrator.Orchestrate`.  
- You define discovery rules via `DiscoveryOptions`.  
- The engine discovers, validates, and registers everything automatically.  
- If something is wrong, the engine fails fast with a clear message.  

This gives you a clean, governed, deterministic DI experience.

