# HostingEngine Architecture

The HostingEngine is provided by Frank and is responsible for applying hosting‑specific configuration using a set of hosting modules supplied by CampFitFurDogs.  
It runs before dependency injection is built and ensures the application is correctly configured for the active hosting environment.

CampFitFurDogs does not implement the engine — it implements hosting modules and invokes the engine during startup.

---

# 1. Purpose

The HostingEngine provides a structured, deterministic way to:

- Detect the active hosting environment  
- Apply environment‑specific configuration  
- Configure the `WebApplicationBuilder` before DI is built  
- Provide a consistent hosting abstraction to the rest of the system  
- Keep environment logic isolated from application logic  

The engine itself is environment‑agnostic.  
All environment‑specific behavior lives in hosting modules.

---

# 2. HostingEngine Responsibilities

The HostingEngine coordinates:

1. Construction of hosting modules  
2. Execution of hosting modules in a defined order  
3. Application of environment‑specific configuration  
4. Early configuration of the `WebApplicationBuilder`  

The engine does not:

- Register services  
- Build the DI container  
- Configure middleware  
- Map endpoints  
- Perform DI scanning  
- Contain application logic  

It is purely an environment configuration orchestrator.

---

# 3. Hosting Modules

A hosting module encapsulates configuration for a specific hosting environment.

Each module implements:

```csharp
public interface IHostingModule
{
    Task<bool> AppliesToAsync(WebApplicationBuilder builder);
    Task ApplyAsync(WebApplicationBuilder builder);
}
```

## 3.1 Module Responsibilities

A hosting module:

- Detects whether it applies to the current environment  
- Applies environment‑specific configuration  
- Configures URLs, secrets, flags, and hosting metadata  
- Must not register services  
- Must not build the DI container  
- Must not configure middleware  

Modules are small, focused, and environment‑specific.

## 3.2 Module Ordering

Modules are executed in the order they are provided.

This allows:

- Local development overrides  
- Cloud‑specific overrides  
- Preview/production overrides  

Ordering is explicit and deterministic.

---

# 4. CampFitFurDogs Hosting Modules

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

### LocalDevelopmentHostingModule

Responsible for:

- Local environment variables  
- Local URLs  
- Local debugging configuration  
- Developer‑friendly defaults  

### RenderPrPreviewHostingModule

Responsible for:

- Render‑specific environment variables  
- Preview/production URLs  
- Cloud hosting configuration  
- Deployment metadata  

Each module encapsulates all logic for its environment.

---

# 5. HostingEngine Execution Flow

In `Program.cs`:

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

Which expands to:

```csharp
var hostingModules = Hosting.ConstructHostingModules();
var hostingEngine = new HostingEngine(hostingModules);
await hostingEngine.ApplyHostingEnvironmentConfigurationAsync(builder);
```

Execution flow:

1. Hosting modules are constructed  
2. HostingEngine iterates through modules  
3. Each module checks whether it applies  
4. The first matching module applies its configuration  
5. The builder is fully configured before DI is built  

This ensures:

- Correct environment detection  
- Correct configuration ordering  
- Correct builder state before DI registration  

---

# 6. Interaction With the Startup Pipeline

The HostingEngine runs **before**:

- DI registration  
- Auto‑registration  
- EF Core configuration  
- Middleware configuration  
- Endpoint mapping  

This ensures that:

- Environment variables are available  
- Hosting flags are set  
- URLs and ports are configured  
- Any environment‑specific configuration is applied early  

The HostingEngine does not interact with:

- Application pipelines  
- Authentication pipelines  
- Dispatcher pipelines  
- Domain events  
- Session management  

It is strictly a hosting‑level concern.

---

# 7. Purity Rules

The HostingEngine enforces strict purity boundaries:

## HostingEngine must not:
- Register services  
- Build the DI container  
- Configure middleware  
- Map endpoints  
- Perform scanning  
- Depend on Application, Domain, or Infrastructure  

## Hosting modules must not:
- Register services  
- Build the DI container  
- Access EF Core  
- Access HttpContext  
- Perform I/O  
- Contain business logic  

## Application, Domain, and Infrastructure must not:
- Detect hosting environments  
- Apply hosting configuration  
- Depend on hosting modules  

This ensures hosting logic remains isolated and testable.

---

# 8. Contributor Guidelines

When adding a new hosting module:

1. Implement `IHostingModule`  
2. Keep detection logic simple and deterministic  
3. Apply only environment‑specific configuration  
4. Do not register services  
5. Do not build the DI container  
6. Do not configure middleware  
7. Add the module to `ConstructHostingModules()`  
8. Ensure ordering is correct  
9. Test the module in isolation  

If a hosting module grows beyond ~30 lines, logic is leaking into the wrong layer.

---

# 9. Testing Guidelines

Hosting modules should be tested by:

- Constructing a `WebApplicationBuilder`  
- Invoking `AppliesToAsync`  
- Invoking `ApplyAsync`  
- Verifying builder configuration  

Tests should not:

- Build the DI container  
- Start the application  
- Invoke middleware  

Hosting logic must remain isolated and deterministic.

---

# 10. Summary

- Frank provides the HostingEngine  
- CampFitFurDogs provides hosting modules  
- HostingEngine applies environment configuration before DI is built  
- HostingEngine is stable, generic, and app‑agnostic  
- All environment‑specific behavior lives in modules  
- Hosting logic is isolated from application logic  

This architecture ensures predictable, maintainable hosting behavior across all environments.

