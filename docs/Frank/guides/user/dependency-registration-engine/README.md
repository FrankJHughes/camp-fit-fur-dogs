# Frank AutoRegistration — User Guide

The AutoRegistration capability automatically discovers and registers services into the DI container based on interfaces marked with `[AutoRegister]`.  
As a user of this capability, you do not write the engine — you **use** it by:

- marking interfaces with `[AutoRegister]`  
- providing implementations  
- calling `AddAutoRegistration` with the assemblies to scan  

The system handles the rest.

---

# 1. What This Capability Does for You

When you enable AutoRegistration:

1. The system scans your assemblies.
2. It finds all interfaces decorated with `[AutoRegister]`.
3. It finds all concrete classes that implement those interfaces.
4. It validates that the number of implementations meets your declared min/max requirements.
5. It registers the implementations into DI with the lifetime you specify.

You get:

- automatic DI registration  
- enforcement of architectural rules  
- fast failure when contracts are violated  
- no manual registration boilerplate  

---

# 2. How to Use AutoRegistration

## 2.1 Mark an Interface

````csharp
[AutoRegister(ServiceLifetime.Scoped)]
public interface IMyService { }
````

This tells the system:

- “This interface must be auto‑registered.”
- “Use Scoped lifetime for all implementations.”

You can also specify constraints:

````csharp
[AutoRegister(ServiceLifetime.Singleton, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
public interface IValidator { }
````

This enforces exactly one implementation.

---

## 2.2 Implement the Interface

````csharp
public sealed class MyService : IMyService { }
````

You may have multiple implementations unless you restrict them.

---

## 2.3 Enable AutoRegistration

Call the extension method:

````csharp
services.AddAutoRegistration(typeof(SomeType).Assembly);
````

You typically pass all assemblies that contain:

- your `[AutoRegister]` interfaces  
- their implementing classes  

---

# 3. What Happens at Startup

When the application starts:

1. **Scanner** finds all governed interfaces and implementations.  
2. **Planner** builds a registration plan for each interface.  
3. **Validator** checks min/max implementation counts.  
4. If any interface violates its constraints →  
   **startup fails with a clear error message**.  
5. **Registrar** registers all valid implementations into DI.

This ensures your DI container is always in a valid state.

---

# 4. How to Control Registration Behavior

### Lifetime  
Set via the attribute:

````csharp
[AutoRegister(ServiceLifetime.Transient)]
````

### Min/Max Implementations  
Enforce architectural rules:

````csharp
[AutoRegister(ServiceLifetime.Scoped, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
````

### Register Concrete Types  
If you want the concrete class registered as itself:

````csharp
[AutoRegister(ServiceLifetime.Singleton, RegisterConcreteType = true)]
````

This produces two registrations:

- `IMyService -> MyService`
- `MyService -> MyService`

---

# 5. What You Should Expect When Things Go Wrong

If constraints are violated, startup fails with a message like:

```
IMyService
requires between 1 and 1 implementations. It has 0:
```

This is intentional — AutoRegistration enforces architectural correctness.

---

# 6. Best Practices

- Always include all relevant assemblies in `AddAutoRegistration`.
- Use min/max constraints to enforce architectural rules.
- Prefer one implementation per interface unless polymorphism is intended.
- Use `RegisterConcreteType` only when you need direct resolution of the class.
- Keep interfaces small and focused.

---

# 7. Anti‑Patterns

Avoid:

- Marking interfaces with `[AutoRegister]` but forgetting to include the assembly.
- Setting `MinRegistrationCount > 0` without providing implementations.
- Relying on manual DI registration for governed interfaces.
- Using open generic classes (AutoRegistration ignores them).
- Expecting abstract classes to be registered (they are ignored).

---

# 8. Summary

As a user of AutoRegistration:

- You mark interfaces with `[AutoRegister]`.  
- You implement those interfaces.  
- You call `AddAutoRegistration`.  
- The system discovers, validates, and registers everything automatically.  
- If something is wrong, the system fails fast with a clear message.  

This gives you a clean, governed, boilerplate‑free DI experience.
