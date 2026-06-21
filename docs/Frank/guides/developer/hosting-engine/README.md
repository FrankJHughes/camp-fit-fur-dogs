# Frank Hosting Engine — Developer Guide

The Hosting Engine capability provides a **modular, ordered, override‑based hosting configuration pipeline**.  
It allows you to define *hosting modules* that:

- decide whether they are active  
- produce configuration overrides  
- run in a deterministic order  
- merge their overrides into the final hosting configuration  

This guide describes how the system works end‑to‑end.

---

# 1. Core Abstractions (Frank.Abstractions.Hosting)

## 1.1 HostingModuleAttribute

````csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class HostingModuleAttribute : Attribute
{
    public int Order { get; }

    public HostingModuleAttribute(int order)
    {
        Order = order;
    }
}
````

### Purpose

- Assigns a deterministic execution order to hosting modules.
- Lower `Order` runs first.
- Modules without the attribute default to `Order = 0`.

---

## 1.2 IHostingModule

````csharp
public interface IHostingModule
{
    bool IsActive(WebApplicationBuilder builder);

    Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder);
}
````

### Responsibilities

- **IsActive**  
  Determines whether the module should run for the current hosting environment.

- **GetConfigurationOverridesAsync**  
  Returns a dictionary of configuration key/value overrides.  
  Later modules overwrite earlier ones.

### What modules do *not* do

- They do **not** write to DI.  
- They do **not** modify the builder directly.  
- They do **not** apply configuration themselves.  

They only *produce* overrides.

---

# 2. Hosting Engine Runtime (Frank.Api.Hosting)

## 2.1 HostingConfigurationProvider

````csharp
public sealed class HostingConfigurationProvider : ConfigurationProvider
{
    private readonly IDictionary<string, string?> _values;

    public override void Load()
    {
        foreach (var kvp in _values)
            Data[kvp.Key] = kvp.Value;
    }
}
````

### Purpose

- Injects module‑produced overrides into the configuration system.

---

## 2.2 HostingOverridesConfigurationSource

````csharp
public sealed class HostingOverridesConfigurationSource : IConfigurationSource
{
    private readonly IDictionary<string, string?> _values;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new HostingConfigurationProvider(_values);
}
````

### Purpose

- Allows the Hosting Engine to add overrides to the configuration pipeline.

---

## 2.3 HostingEngine

````csharp
public sealed class HostingEngine
{
    private readonly IReadOnlyList<IHostingModule> _modules;

    public HostingEngine(IEnumerable<IHostingModule> modules)
    {
        _modules = modules
            .OrderBy(GetOrder)
            .ToArray();
    }

    private static int GetOrder(IHostingModule module)
    {
        var attr = module.GetType()
            .GetCustomAttributes(typeof(HostingModuleAttribute), false)
            .Cast<HostingModuleAttribute>()
            .FirstOrDefault();

        return attr?.Order ?? 0;
    }

    public async Task ApplyHostingEnvironmentConfigurationAsync(WebApplicationBuilder builder)
    {
        var merged = new Dictionary<string, string?>();

        foreach (var module in _modules)
        {
            if (!module.IsActive(builder))
                continue;

            var overrides = await module.GetConfigurationOverridesAsync(builder);

            foreach (var @override in overrides)
                merged[@override.Key] = @override.Value; // later modules win
        }

        if (merged.Count > 0)
        {
            builder.Configuration.Add(new HostingOverridesConfigurationSource(merged));
        }
    }
}
````

### Responsibilities

- Sort modules by `HostingModuleAttribute.Order`.
- Evaluate `IsActive` for each module.
- Collect configuration overrides from active modules.
- Merge overrides (later modules overwrite earlier ones).
- Add overrides to the hosting configuration pipeline.

### Execution Model

1. **Ordering**  
   Modules run in ascending `Order`.

2. **Activation**  
   `IsActive(builder)` determines whether a module participates.

3. **Override Collection**  
   Each module returns a dictionary of overrides.

4. **Override Merging**  
   Later modules overwrite earlier ones.

5. **Configuration Injection**  
   Overrides are added via `HostingOverridesConfigurationSource`.

---

# 3. DI Registration

````csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostingEngine(this IServiceCollection services)
    {
        services.AddSingleton<HostingEngine>();
        return services;
    }
}
````

### Purpose

- Registers the HostingEngine as a singleton.
- Modules themselves must be registered separately (typically via AutoRegistration).

---

# 4. Implementing a Hosting Module

To create a module:

1. Implement `IHostingModule`.
2. Add `[HostingModule(order)]`.
3. Register the module in DI.

Example:

````csharp
[HostingModule(100)]
public sealed class RenderHostingModule : IHostingModule
{
    public bool IsActive(WebApplicationBuilder builder)
        => builder.Environment.IsProduction();

    public Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder)
        => Task.FromResult<IDictionary<string, string?>>(
            new Dictionary<string, string?>
            {
                ["Render:Enabled"] = "true",
                ["Render:Region"] = "us-west"
            });
}
````

---

# 5. Invariants

Developers must ensure:

- `IsActive` never throws.
- `GetConfigurationOverridesAsync` never returns null.
- Override keys are unique within a module.
- Modules are deterministic and side‑effect free.
- A module must not modify the builder directly.
- Ordering must be explicit when ordering matters.

---

# 6. Anti‑Patterns

Avoid:

- Doing real work inside `IsActive`.
- Throwing exceptions inside `GetConfigurationOverridesAsync`.
- Writing to the builder directly inside modules.
- Using random or time‑dependent override values.
- Relying on registration order instead of `HostingModuleAttribute`.

---

# 7. Summary

The Hosting Engine provides:

- **Ordered hosting modules**  
- **Conditional activation**  
- **Configuration override merging**  
- **Deterministic hosting behavior**  
- **Separation of concerns**  

As a developer:

- You implement modules.  
- You assign order.  
- You return overrides.  
- The Hosting Engine handles the rest.  
