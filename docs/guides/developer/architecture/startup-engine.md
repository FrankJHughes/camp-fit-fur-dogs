# StartupEngine Guide

The **StartupEngine** is provided by **Frank** and is responsible for orchestrating the startup lifecycle using a set of **startup modules** supplied by CampFitFurDogs.

CampFitFurDogs does **not** implement the engine — it **implements modules** and **invokes** the engine.

---

# 1. Purpose

StartupEngine provides a structured, deterministic way to:

- Register services across all layers  
- Apply cross‑cutting startup behavior  
- Keep startup logic modular and composable  
- Ensure consistent ordering of startup modules  
- Centralize all application initialization  

StartupEngine itself is **app‑agnostic**.  
All application‑specific behavior lives in **modules**.

---

# 2. How StartupEngine Works

StartupEngine is constructed with an ordered list of **IStartupModule** instances:

```csharp
var startupModules = ConstructStartupModules();
var startupEngine = new StartupEngine(startupModules);
```

It exposes two phases:

### **AddAll(WebApplicationBuilder builder)**  
Executed *before* the host is built.

Modules use this phase to:

- Register services  
- Register configuration  
- Register middleware dependencies  
- Register cross‑cutting infrastructure  

### **UseAll(WebApplication app)**  
Executed *after* the host is built.

Modules use this phase to:

- Add middleware  
- Map endpoints  
- Finalize the request pipeline  

---

# 3. CampFitFurDogs Startup Modules

CampFitFurDogs defines its own startup modules:

```csharp
public static IStartupModule[] ConstructStartupModules()
{
    return
    [
        new ApiStartupModule(),
        new ApplicationStartupModule(),
        new AuthenticationStartupModule(),
        new AuthorizationStartupModule(),
        new CorsStartupModule(),
        new ExceptionHandlingStartupModule(),
        new InfrastructureStartupModule(),
        new LoggingStartupModule(),
        new SecurityHeadersStartupModule(),
        new SwaggerStartupModule()
    ];
}
```

Each module encapsulates startup logic for a specific horizontal concern.

Examples:

- **ApiStartupModule** → endpoints, routing  
- **ApplicationStartupModule** → AddApplication(), pipeline steps  
- **InfrastructureStartupModule** → AddInfrastructure(), DbContext  
- **SecurityHeadersStartupModule** → security headers  
- **CorsStartupModule** → CORS policy  
- **SwaggerStartupModule** → OpenAPI  

---

# 4. How StartupEngine Is Invoked

In `Program.cs`:

```csharp
Startup.AddAllServices(builder);

var app = builder.Build();

Startup.UseAllServices(app);
```

Where:

### AddAllServices
```csharp
var startupModules = ConstructStartupModules();
var startupEngine = new StartupEngine(startupModules);
startupEngine.AddAll(builder);

builder.Services.AddStartupModules();
builder.Services.AddStartupEngine();
```

### UseAllServices
```csharp
var startupEngine = app.Services.GetRequiredService<StartupEngine>();
startupEngine.UseAll(app);
```

This ensures:

- The same StartupEngine instance is used for both phases  
- Modules run in the correct order  
- Modules have access to DI and hosting context  

---

# 5. What StartupEngine Does NOT Do

StartupEngine does **not**:

- Perform DI scanning (Frank handles this)  
- Register handlers, repositories, readers (auto‑registration)  
- Apply EF Core configurations (Frank handles this)  
- Select hosting providers (HostingEngine handles this)  
- Contain application logic  

StartupEngine is purely an **orchestrator**.

---

# 6. Summary

- Frank provides the **StartupEngine**  
- CampFitFurDogs provides **startup modules**  
- StartupEngine runs modules in two phases: AddAll + UseAll  
- StartupEngine is stable, generic, and app‑agnostic  
- All application‑specific startup behavior lives in modules  
