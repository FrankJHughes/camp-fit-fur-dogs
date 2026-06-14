# HostingEngine Guide

The **HostingEngine** is provided by **Frank** and is responsible for applying hosting‑specific configuration using a set of **hosting modules** supplied by CampFitFurDogs.

CampFitFurDogs does **not** implement the engine — it **implements hosting modules** and **invokes** the engine.

---

# 1. Purpose

HostingEngine provides a structured way to:

- Detect the active hosting environment  
- Apply environment‑specific configuration  
- Configure the WebApplicationBuilder before DI is built  
- Provide a consistent hosting abstraction to the rest of the system  

HostingEngine itself is **environment‑agnostic**.  
All environment logic lives in **modules**.

---

# 2. How HostingEngine Works

HostingEngine is constructed with an ordered list of **IHostingModule** instances:

```csharp
var hostingModules = ConstructHostingModules();
var hostingEngine = new HostingEngine(hostingModules);
```

It exposes:

### **ApplyHostingEnvironmentConfigurationAsync(WebApplicationBuilder builder)**  
Executed before any services are registered.

Hosting modules use this phase to:

- Detect whether they apply (e.g., Render, Local Dev, Test)  
- Apply environment‑specific configuration  
- Configure URLs, secrets, environment flags, etc.  

---

# 3. CampFitFurDogs Hosting Modules

CampFitFurDogs defines its own hosting modules:

```csharp
public static IHostingModule[] ConstructHostingModules()
{
    return
    [
        new LocalDevelopmentHostingModule(),
        new RenderPrPreviewHostingModule()
    ];
}
```

Examples:

- **LocalDevelopmentHostingModule**  
  - Local environment variables  
  - Local URLs  
  - Local debugging configuration  

- **RenderPrPreviewHostingModule**  
  - Render‑specific environment variables  
  - Preview/production URLs  
  - Cloud hosting configuration  

Each module encapsulates hosting logic for a specific environment.

---

# 4. How HostingEngine Is Invoked

In `Program.cs`:

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

Where:

```csharp
var hostingModules = Hosting.ConstructHostingModules();
var hostingEngine = new HostingEngine(hostingModules);
await hostingEngine.ApplyHostingEnvironmentConfigurationAsync(builder);
```

This ensures:

- Hosting configuration is applied **before** DI is built  
- The correct hosting module is selected  
- The builder is fully configured for the environment  

---

# 5. What HostingEngine Does NOT Do

HostingEngine does **not**:

- Register services  
- Build the DI container  
- Configure middleware  
- Map endpoints  
- Perform DI scanning  
- Contain application logic  

HostingEngine is purely an **environment configuration orchestrator**.

---

# 6. Summary

- Frank provides the **HostingEngine**  
- CampFitFurDogs provides **hosting modules**  
- HostingEngine applies environment configuration before DI is built  
- HostingEngine is stable, generic, and app‑agnostic  
- All environment‑specific behavior lives in modules  
