# StartupEngine Architecture Guide  
*A developer‑facing explanation of how application startup works using Frank’s StartupEngine.*

The **StartupEngine** is part of **Frank**, the shared kernel.  
It provides a structured, deterministic way to compose application startup using a set of **startup modules** supplied by CampFitFurDogs.

StartupEngine itself is **generic and app‑agnostic**.  
CampFitFurDogs provides **modules**, not engines.

This guide explains how StartupEngine works, how modules fit into the architecture, and how the startup lifecycle is structured.

---

# 1. Purpose

StartupEngine exists to solve a specific architectural need:

> **Provide a modular, deterministic, layer‑agnostic way to configure application startup.**

It replaces:

- Ad‑hoc startup logic in `Program.cs`  
- Large monolithic startup methods  
- Unpredictable ordering of middleware and DI setup  
- Scattered cross‑cutting configuration  

StartupEngine ensures:

- Consistent startup ordering  
- Clear separation of concerns  
- Composable startup behavior  
- A unified startup lifecycle  
- Predictable DI and middleware configuration  

---

# 2. High‑Level Model

StartupEngine coordinates two phases:

```
AddAll (before host build)
UseAll (after host build)
```

Each phase runs across all startup modules in the order they were provided.

```
Startup Modules
    ↓
StartupEngine.AddAll(builder)
    ↓
builder.Build()
    ↓
StartupEngine.UseAll(app)
```

---

# 3. Startup Modules

A **startup module** encapsulates startup logic for a specific horizontal concern.

Examples:

- API setup  
- Application setup  
- Authentication  
- Authorization  
- CORS  
- Logging  
- Infrastructure  
- Security headers  
- Swagger  

Each module implements:

```csharp
public interface IStartupModule
{
    void Add(WebApplicationBuilder builder);
    void Use(WebApplication app);
}
```

Modules are:

- Deterministic  
- Composable  
- Focused  
- Layer‑agnostic  
- Free of business logic  

Modules configure startup — they do not execute use cases.

---

# 4. The Two Startup Phases

## 4.1 Add Phase  
**Executed before the host is built.**

Modules use this phase to:

- Register services  
- Register configuration  
- Register middleware dependencies  
- Register EF Core, authentication, authorization, logging, etc.  

This phase is DI‑only.

Example:

```csharp
public void Add(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();
}
```

---

## 4.2 Use Phase  
**Executed after the host is built.**

Modules use this phase to:

- Add middleware  
- Map endpoints  
- Finalize the request pipeline  

This phase is pipeline‑only.

Example:

```csharp
public void Use(WebApplication app)
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}
```

---

# 5. Constructing Startup Modules

CampFitFurDogs constructs its startup modules in a single place:

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

This list defines the **startup order**.

---

# 6. Invoking StartupEngine

In `Program.cs`:

```csharp
Startup.AddAllServices(builder);

var app = builder.Build();

Startup.UseAllServices(app);
```

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

# 7. Interaction With HostingEngine

StartupEngine runs **after** HostingEngine.

Sequence:

```
HostingEngine → StartupEngine.AddAll → Host Build → StartupEngine.UseAll
```

HostingEngine configures:

- Environment  
- Hosting provider  
- Hosting metadata  

StartupEngine configures:

- DI  
- Middleware  
- Endpoints  
- Cross‑cutting pipelines  

They are complementary but separate.

---

# 8. Interaction With DI Auto‑Registration

StartupEngine does **not**:

- Scan for handlers  
- Scan for validators  
- Scan for repositories  
- Scan for readers  
- Scan for EF Core configurations  

Frank’s DI auto‑registration engine handles all scanning.

StartupEngine simply orchestrates module execution.

---

# 9. Interaction With the Test Harness

The test harness uses StartupEngine to build a realistic test host.

Sequence:

```
HostingEngine
StartupEngine.AddAll
Test overrides
Host Build
StartupEngine.UseAll
```

Test overrides must always run **after** both engines.

StartupEngine itself contains no test‑specific logic.

---

# 10. What Belongs in a Startup Module

Put it in a startup module if it:

- Configures DI  
- Configures middleware  
- Configures endpoints  
- Configures cross‑cutting infrastructure  
- Configures authentication/authorization  
- Configures logging  
- Configures CORS  
- Configures security headers  
- Configures Swagger  

Startup modules configure startup — nothing more.

---

# 11. What Does NOT Belong in StartupEngine or Modules

Do **not** put the following in StartupEngine or modules:

- Business logic  
- Domain logic  
- Application logic  
- Persistence  
- HTTP calls  
- Request‑specific logic  
- Environment‑specific logic (HostingEngine handles this)  
- Test logic  
- Slice‑specific behavior  

StartupEngine is a **startup orchestrator**, not a logic container.

---

# 12. Summary

- StartupEngine is part of **Frank**, the shared kernel  
- It orchestrates startup using **startup modules**  
- Modules run in two phases: AddAll + UseAll  
- StartupEngine is deterministic, modular, and app‑agnostic  
- All application‑specific startup behavior lives in modules  
- StartupEngine interacts cleanly with HostingEngine and DI auto‑registration  
- StartupEngine is a core part of the application’s startup architecture  

