# Frank Hosting Engine — User Guide

The Hosting Engine capability lets your application load **hosting‑specific configuration overrides** from modular components called *hosting modules*.  
As a user of this capability, you do not build the engine — you **use** it by:

- registering hosting modules  
- optionally ordering them  
- letting the engine merge their configuration overrides  
- relying on the engine to activate modules only when appropriate  

This gives you a clean, predictable way to customize hosting behavior across environments.

---

# 1. What This Capability Does for You

When the application starts:

1. The Hosting Engine discovers all registered `IHostingModule` instances.
2. It sorts them by `[HostingModule(order)]`.
3. It asks each module whether it is active for the current environment.
4. Active modules return configuration overrides.
5. Overrides are merged — **later modules overwrite earlier ones**.
6. The merged overrides are injected into the hosting configuration.

You get:

- environment‑aware hosting behavior  
- deterministic override ordering  
- modular hosting logic  
- no manual configuration merging  

---

# 2. How to Use Hosting Modules

## 2.1 Register the Hosting Engine

Add the engine to DI:

````csharp
services.AddHostingEngine();
````

This registers the `HostingEngine` as a singleton.

---

## 2.2 Implement a Hosting Module

A hosting module is a class that implements `IHostingModule`:

````csharp
public sealed class MyHostingModule : IHostingModule
{
    public bool IsActive(WebApplicationBuilder builder)
        => builder.Environment.IsDevelopment();

    public Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder)
        => Task.FromResult<IDictionary<string, string?>>(
            new Dictionary<string, string?>
            {
                ["MyFeature:Enabled"] = "true"
            });
}
````

### What a module does:

- **IsActive** decides whether the module should run.
- **GetConfigurationOverridesAsync** returns configuration values.

### What a module does *not* do:

- It does not modify the builder directly.
- It does not write to DI.
- It does not apply configuration itself.

---

## 2.3 Control Module Ordering

Use `[HostingModule(order)]` to control execution order:

````csharp
[HostingModule(100)]
public sealed class ProductionOverridesModule : IHostingModule { ... }
````

- Lower numbers run first.
- Modules without the attribute default to `0`.

---

## 2.4 Apply Hosting Configuration

Call the engine during startup:

````csharp
await hostingEngine.ApplyHostingEnvironmentConfigurationAsync(builder);
````

After this runs:

- All active modules have contributed overrides.
- The final merged overrides are added to the configuration pipeline.

---

# 3. What You Should Expect in Configuration

If modules return overrides like:

```
ModuleA: { "FeatureX:Enabled" = "false" }
ModuleB: { "FeatureX:Enabled" = "true" }
```

And ModuleB runs after ModuleA, then:

```
builder.Configuration["FeatureX:Enabled"] == "true"
```

Later modules always win.

---

# 4. Best Practices

- Keep modules small and focused.
- Use `IsActive` to scope modules to environments.
- Use ordering when overrides must be layered.
- Return only the overrides you need — no more.
- Prefer simple key/value overrides for clarity.

---

# 5. Anti‑Patterns

Avoid:

- Doing heavy work inside `IsActive`.
- Throwing exceptions inside `GetConfigurationOverridesAsync`.
- Modifying the builder directly inside a module.
- Returning null override dictionaries.
- Relying on registration order instead of `[HostingModule(order)]`.

---

# 6. Summary

As a user of the Hosting Engine:

- You register hosting modules.
- You optionally assign them an order.
- The engine activates modules based on environment.
- Active modules return configuration overrides.
- Overrides are merged deterministically.
- The engine injects them into the hosting configuration.

This gives you a clean, modular, predictable hosting configuration system.
