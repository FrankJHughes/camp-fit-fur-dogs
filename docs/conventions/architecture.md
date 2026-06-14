# Architecture Conventions

The architecture of Camp Fit Fur Dogs defines the structural boundaries, layering rules, hosting model, and cross‑cutting primitives that shape the entire system.  
Architecture conventions describe **how the system is organized**, not how code is written or how workflows operate.

Frank is the system’s cross‑cutting backbone.  
Application, Domain, Infrastructure, and Api form the vertical slices of behavior.

---

# 1. Layering Model

The system uses a strict layered architecture:

- **Domain** — business rules, aggregates, value objects, domain events  
- **Application** — use‑case orchestration, CQRS handlers, validation, domain interaction  
- **Infrastructure** — persistence, external integrations, hosting provider implementations  
- **Api** — endpoints, DTO binding, authorization, request/response shaping  
- **Frank** — cross‑cutting primitives, DI auto‑registration engine, endpoint discovery, hosting abstractions, environment abstraction, startup engine, hosting engine, test seams

## 1.1 Layering Rules

These rules are enforced by guardrail tests:

- Domain depends on **nothing** outside Domain  
- Application depends only on **Domain** and **Frank abstractions**  
- Infrastructure depends on **Application**, **Domain**, and **Frank abstractions**  
- Api depends on **Application**, **Domain**, and **Frank**  
- Frank depends on **nothing in the product code**  

Frank is the **only allowed cross‑layer dependency**.

## 1.2 Vertical Slice Boundaries

Each slice contains:

- Application handlers  
- Validators  
- Domain aggregates/entities  
- Infrastructure repositories/readers  
- API endpoints  

Slices must remain **self‑contained**:

- No slice may reference another slice’s Application or Domain types  
- Shared logic must move to Frank or a shared abstraction  
- Cross‑slice communication occurs only through Domain events or shared abstractions  

This prevents slice entanglement.

---

# 2. Frank (Cross‑Cutting Layer)

Frank provides the system’s cross‑cutting primitives and infrastructure.  
It is not a general DI container — it registers **core services** and **attributed interfaces**, nothing more.

Frank also provides the **StartupEngine** and **HostingEngine**, which CampFitFurDogs uses to compose the application.

## 2.1 Responsibilities

Frank provides:

### **DI auto‑registration engine**
- Registers interfaces marked with `[AutoRegister]`
- Discovers implementations
- Validates min/max implementation counts
- Registers services with the correct lifetime

### **Endpoint discovery**
- Scans assemblies for `IEndpoint` implementations
- Registers them automatically

### **Validator scanning**
- Discovers and registers validators

### **Security headers**
- `AddSecurityHeaders()` extension
- `SecurityHeadersMiddleware` with hardened defaults

### **Hosting provider abstractions**
- `IHostingModule`
- `IHostingEnvironment`
- `IEnvironment`
- Deterministic, testable hosting pipeline

### **Startup engine**
- Executes startup modules in two phases:
  - `AddAll(builder)` before the host is built
  - `UseAll(app)` after the host is built

### **Environment abstraction**
- Safe, testable environment variable access

### **Test seams**
- Hosting seam  
- Environment seam  
- Artifact seam  
- PR parser seam  
- Configuration writer seam  

Frank contains **no business logic** and **no slice‑specific logic**.

---

# 3. Hosting Model (Aligned With Actual Implementation)

CampFitFurDogs defines hosting modules; Frank provides the engine.

In `Program.cs`:

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

Where:

```csharp
var hostingModules = ConstructHostingModules();
var hostingEngine = new HostingEngine(hostingModules);
await hostingEngine.ApplyHostingEnvironmentConfigurationAsync(builder);
```

## 3.1 Provider Selection

- Hosting modules are evaluated **in order**
- The first module whose logic determines it is active is selected
- That module configures the hosting environment via the builder
- All other modules are skipped

This is deterministic and enforced by guardrails.

## 3.2 Provider Requirements

Hosting modules must:

- Use only Frank abstractions  
- Never read environment variables directly  
- Never perform HTTP calls directly  
- Never parse JSON/ZIP directly  
- Never write configuration directly  
- Be fully testable  

## 3.3 Hosting Provider Boundaries

Hosting modules configure:

- URLs  
- Connection strings  
- Environment flags  
- Hosting metadata  

They must not:

- Access EF Core  
- Access Domain or Application  
- Access HttpContext  
- Perform business logic  

---

# 4. Startup Model (Aligned With Actual Implementation)

CampFitFurDogs defines startup modules; Frank provides the engine.

In `Program.cs`:

```csharp
Startup.AddAllServices(builder);
var app = builder.Build();
Startup.UseAllServices(app);
```

Where:

```csharp
var startupModules = ConstructStartupModules();
var startupEngine = new StartupEngine(startupModules);
startupEngine.AddAll(builder);

builder.Services.AddStartupModules();
builder.Services.AddStartupEngine();
```

And:

```csharp
var startupEngine = app.Services.GetRequiredService<StartupEngine>();
startupEngine.UseAll(app);
```

## 4.1 Startup Module Responsibilities

Startup modules encapsulate startup logic for a horizontal concern:

Examples:

- `ApiStartupModule`  
- `ApplicationStartupModule`  
- `AuthenticationStartupModule`  
- `AuthorizationStartupModule`  
- `CorsStartupModule`  
- `ExceptionHandlingStartupModule`  
- `InfrastructureStartupModule`  
- `LoggingStartupModule`  
- `SecurityHeadersStartupModule`  
- `SwaggerStartupModule`  

Modules may:

- Register services  
- Register configuration  
- Register middleware dependencies  
- Configure middleware  
- Map endpoints  

Modules must not:

- Contain business logic  
- Perform persistence  
- Access HttpContext directly  
- Depend on other modules  

## 4.2 StartupEngine Responsibilities

StartupEngine:

- Executes modules in the order provided  
- Runs `AddAll(builder)` before DI is built  
- Runs `UseAll(app)` after DI is built  
- Ensures deterministic startup  
- Ensures consistent module ordering  
- Provides a stable startup lifecycle  

StartupEngine does **not**:

- Perform DI scanning  
- Register handlers, repositories, readers  
- Apply EF Core configurations  
- Select hosting providers  

---

# 5. Test Harness (Aligned With Actual System)

The test harness uses:

- Frank’s environment seam  
- Frank’s hosting seam  
- Frank’s startup seam  
- ApiFactory / ApiContext pattern  

Tests simulate:

- Local development  
- Render PR Preview  
- Production  
- Failure modes  

Tests do **not**:

- Touch real environment variables  
- Touch real hosting providers  
- Touch real external services  

Everything is mocked through Frank’s seams.

---

# 6. Api Architecture

Api is responsible for:

- Endpoint definitions  
- DTO binding  
- Authorization  
- Dispatching commands/queries  
- Applying security headers  
- Mapping endpoints via Frank’s endpoint discovery

Api must not:

- Contain business logic  
- Access Infrastructure directly  
- Return domain entities  
- Read identity from request bodies

---

# 7. Application Architecture

Application orchestrates use cases.

Responsibilities:

- CQRS handlers  
- Validation  
- Domain interaction  
- Transaction boundaries via Unit of Work  
- Publishing domain events

Application must not:

- Access HttpContext  
- Access Infrastructure directly  
- Perform persistence logic  
- Contain business rules

---

# 8. Domain Architecture

Domain contains:

- Aggregates  
- Entities  
- Value objects  
- Domain events  
- Invariants  
- Business rules

Domain must not depend on:

- Application  
- Infrastructure  
- Api  
- Frank

---

# 9. Infrastructure Architecture

Infrastructure provides:

- EF Core persistence  
- Repositories  
- Unit of Work  
- External integrations  
- Hosting provider implementations  
- Environment access via Frank abstractions

Infrastructure must not:

- Expose EF Core types to Application  
- Contain business logic  
- Depend on Api

---

# 10. Security Architecture

Security headers are mandatory.

Api must apply:

- `AddSecurityHeaders()`  
- `SecurityHeadersMiddleware`  

Guardrails enforce presence.

---

# Summary

The architecture ensures:

- Strict layering  
- Deterministic hosting behavior  
- Deterministic startup behavior  
- Testable, composable providers  
- Strong security posture  
- Clear cross‑cutting boundaries  
- Frank as the backbone of shared behavior  
- Predictable CI/CD behavior  
- PR Preview safety  

All contributors must follow these conventions.
