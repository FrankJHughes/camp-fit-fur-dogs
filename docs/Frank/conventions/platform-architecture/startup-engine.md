# Conventions – Architecture – Startup Engine (Frank)

The **Startup Engine** is the deterministic, ordered module‑composition pipeline used by Frank‑based applications.  
It provides a consistent way for products to define startup behavior by composing **startup modules**.

Products — including CampFitFurDogs — must define startup logic **by composing startup modules**.

The Startup Engine ensures:

- predictable module ordering  
- consistent application initialization  
- separation of concerns across startup responsibilities  

---

# Responsibilities

The Startup Engine owns:

- resolving all `IStartupModule` instances from DI  
- ordering modules using `[StartupModuleAttribute(Order)]`  
- executing module `Add()` methods during builder configuration  
- executing module `Use()` methods during app configuration  

The engine does **not** perform discovery itself — modules are registered by the product.

---

# Startup Modules

A startup module is any class implementing:

```csharp
public interface IStartupModule
{
    void Add(WebApplicationBuilder builder);
    void Use(WebApplication app);
}
```

Modules may optionally specify ordering:

```csharp
[StartupModule(100)]
public sealed class LoggingStartupModule : IStartupModule { ... }
```

Modules with lower order values run earlier.  
Modules without an attribute default to order **1000**.

---

# Module Registration (Product‑Side)

Products register modules explicitly:

```csharp
services.AddSingleton<IStartupModule, LoggingStartupModule>();
services.AddSingleton<IStartupModule, CorsStartupModule>();
services.AddSingleton<IStartupModule, SwaggerStartupModule>();
```

Modules are **not** registered through Frank.Registration.  
Modules do **not** use `RegistrationAttribute`.  
Modules are **product‑owned**, not platform‑owned.

---

# Startup Engine Execution

The engine is registered via:

```csharp
services.AddStartupEngine();
```

During startup:

### 1. Add Phase

```csharp
startupEngine.AddAll(builder);
```

Executes:

```
module1.Add(builder)
module2.Add(builder)
...
```

### 2. Use Phase

```csharp
startupEngine.UseAll(app);
```

Executes:

```
module1.Use(app)
module2.Use(app)
...
```

Ordering is identical in both phases.

---

# Product Composition Pattern

A typical product startup looks like:

```csharp
var builder = WebApplication.CreateBuilder(args);

await Hosting.AdaptToHostingEnvironment(builder);

builder.Services.AddStartupModules();
builder.Services.AddStartupEngine();

var app = builder.Build();

var engine = app.Services.GetRequiredService<StartupEngine>();
engine.UseAll(app);

app.Run();
```

Products:

- register modules  
- register the engine  
- call `AddAll` and `UseAll` through their own composition helpers  

---

# Conventions

### Modules must:

- be stateless  
- be registered as `IStartupModule`  
- encapsulate a single startup concern  
- avoid side effects outside `Add()` and `Use()`  

### Modules must not:

- perform hosting environment adaptation  
- perform DI scanning or registration outside their scope  
- depend on product‑specific global state  
- assume ordering beyond what `[StartupModuleAttribute]` defines  

### Products must:

- compose startup exclusively through modules  
- register modules explicitly  
- avoid custom startup pipelines  

---

# Summary

The Startup Engine is a **simple, deterministic module runner**:

- modules are registered by the product  
- ordered via `[StartupModuleAttribute]`  
- executed through `Add()` and `Use()`  
- coordinated by `StartupEngine`  

This document reflects the **actual**, **current**, **implemented** behavior — no more, no less.
