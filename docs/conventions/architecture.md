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

### DI auto‑registration engine
- Registers interfaces marked with `[AutoRegister]`
- Discovers implementations
- Validates min/max implementation counts
- Registers services with the correct lifetime

### Endpoint discovery
- Scans assemblies for `IEndpoint` implementations
- Registers them automatically

### Validator scanning
- Discovers and registers validators

### Security headers
- `AddSecurityHeaders()` extension
- `SecurityHeadersMiddleware` with hardened defaults

### Hosting provider abstractions
- `IHostingModule`
- `IHostingEnvironment`
- `IEnvironment`
- Deterministic, testable hosting pipeline

### Startup engine
- Executes startup modules in two phases:
  - `AddAll(builder)` before the host is built
  - `UseAll(app)` after the host is built

### Environment abstraction
- Safe, testable environment variable access

### Test seams
- Hosting seam  
- Environment seam  
- Artifact seam  
- PR parser seam  
- Configuration writer seam  

Frank contains **no business logic** and **no slice‑specific logic**.

---

# 3. Hosting Model

CampFitFurDogs defines hosting modules; Frank provides the engine.

In Program.cs:

```
await Hosting.AdaptToHostingEnvironment(builder);
```

Where:

```
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

# 4. Startup Model

CampFitFurDogs defines startup modules; Frank provides the engine.

In Program.cs:

```
Startup.AddAllServices(builder);
var app = builder.Build();
Startup.UseAllServices(app);
```

Where:

```
var startupModules = ConstructStartupModules();
var startupEngine = new StartupEngine(startupModules);
startupEngine.AddAll(builder);

builder.Services.AddStartupModules();
builder.Services.AddStartupEngine();
```

And:

```
var startupEngine = app.Services.GetRequiredService<StartupEngine>();
startupEngine.UseAll(app);
```

## 4.1 Startup Module Responsibilities

Startup modules encapsulate startup logic for a horizontal concern:

Examples:

- ApiStartupModule  
- ApplicationStartupModule  
- AuthenticationStartupModule  
- AuthorizationStartupModule  
- CorsStartupModule  
- ExceptionHandlingStartupModule  
- InfrastructureStartupModule  
- LoggingStartupModule  
- SecurityHeadersStartupModule  
- SwaggerStartupModule  

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
- Runs AddAll(builder) before DI is built  
- Runs UseAll(app) after DI is built  
- Ensures deterministic startup  
- Ensures consistent module ordering  
- Provides a stable startup lifecycle  

StartupEngine does **not**:

- Perform DI scanning  
- Register handlers, repositories, readers  
- Apply EF Core configurations  
- Select hosting providers  

---

# 5. Test Harness

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

- AddSecurityHeaders()  
- SecurityHeadersMiddleware  

Guardrails enforce presence.

---

# 11. Authentication Callback Architecture

The authentication callback flow follows a **three‑layer structural pattern**:

## 11.1 Frank Pipeline (Protocol Layer)

Implemented as:

```
IImmutableContextBuilder<
    FrankAuthCallbackRequest,
    OidcAuthCallbackContext,
    FrankAuthCallbackResult>
```

Responsibilities:

- OIDC protocol handling  
- Authorization‑code exchange  
- Claims extraction  
- Provider normalization  

Contains **no business logic**.

## 11.2 Application Pipeline (Business Layer)

Implemented as:

```
IImmutableContextBuilder<
    ApplicationAuthCallbackRequest,
    ApplicationAuthCallbackContext,
    ApplicationAuthCallbackContextBuilderResult>
```

Responsibilities:

- Identity resolution  
- Customer creation  
- Session creation  
- Redirect URL selection  
- Cookie value generation  

Contains **no protocol logic**.

## 11.3 Api Endpoint (Infrastructure Boundary)

The API callback endpoint is a **thin orchestrator**:

- Extracts the `code` query parameter  
- Invokes the Frank pipeline  
- Invokes the Application pipeline  
- Issues the authentication cookie  
- Redirects the user  

The endpoint must not contain:

- Protocol logic  
- Business logic  
- Persistence logic  

This preserves the layering model and ensures the authentication flow is fully testable.

---

# 12. ImmutableContextBuilder Architecture

`ImmutableContextBuilder<TRequest, TContext, TResult>` is a core architectural primitive used for deterministic, multi‑stage transformations that must remain pure, strongly typed, and invariant‑checked.

It replaces the old step‑engine pattern and is now the standard mechanism for:

- Frank Auth Callback Pipeline (protocol)
- Application Auth Callback Pipeline (business)
- Identity mapping flows
- Session creation flows
- Redirect computation flows
- Any multi‑stage transformation requiring purity + determinism

## 12.1 Purpose

ImmutableContextBuilder enforces:

- **Immutability** — no mutation, no overwriting, no clearing  
- **Determinism** — each stage produces a new context  
- **Purity** — no side effects inside transformations  
- **Strong typing** — request, context, and result are explicit  
- **Governance alignment** — no cross‑layer violations  

## 12.2 Structural Pattern

A builder implements:

```
IImmutableContextBuilder<TRequest, TContext, TResult>
```

Where:

- **TRequest** — immutable input  
- **TContext** — immutable working state  
- **TResult** — immutable final output  

Each transformation:

- Accepts a context  
- Returns a new context  
- Never mutates the old one  

## 12.3 Layering Rules

- **Frank builders**  
  - May perform protocol logic  
  - May perform external HTTP calls  
  - Must not perform business logic  
  - Must not perform persistence  

- **Application builders**  
  - May perform business logic  
  - May perform identity resolution  
  - May perform session creation  
  - Must not perform HTTP  
  - Must not access hosting providers  

- **Domain**  
  - Contains no builders  

## 12.4 When to Use a Builder

Use a builder when:

- A flow has multiple sequential stages  
- Each stage depends on the previous stage  
- The flow must be deterministic  
- The flow must be pure  
- The flow must be testable  
- The flow must not use dispatcher pipelines  

Examples:

- Authentication callback  
- Payment provider callbacks  
- Webhook processing  
- Multi‑stage normalization  
- Multi‑stage validation  
- Multi‑stage redirect logic  

## 12.5 When *Not* to Use a Builder

Do **not** use a builder when:

- Implementing CQRS handlers  
- Performing domain logic  
- Performing Infrastructure logic  
- Performing HTTP in Application  
- Performing persistence in Frank  
- Implementing branching workflows  
- Implementing long‑running processes  

Builders are for **pure, deterministic, synchronous transformations**.

## 12.6 Testing Requirements

Builders must be tested at three levels:

- **Unit tests** — individual transformations  
- **Builder tests** — full end‑to‑end builder behavior  
- **Guardrail tests** — immutability, no mutation, no overwriting  

Integration tests apply only when builders participate in API flows.

---

# 13. Convention Boundaries

CampFitFurDogs maintains four convention documents: **architecture**, **code**, **docs**, and **workflow**.  
Each governs a different dimension of the system.  
To prevent cross‑domain drift, the boundaries must remain strict.

## 13.1 Architecture Conventions (this document)

Defines:

- System structure  
- Layering rules  
- Vertical slice boundaries  
- Cross‑cutting primitives  
- Hosting and startup models  
- Authentication callback architecture  
- Security architecture  
- ImmutableContextBuilder architecture  

Must not include:

- Coding style  
- Naming rules  
- Documentation rules  
- Workflow rules  
- PR process  
- Testing style  

## 13.2 Code Conventions

Defines:

- Naming rules  
- File layout  
- Handler patterns  
- CQRS conventions  
- Validation patterns  
- Error handling  
- Logging  
- Test structure  

Must not include:

- Architectural rules  
- Hosting rules  
- Documentation structure  
- Workflow process  

## 13.3 Docs Conventions

Defines:

- Documentation structure  
- Story grammar  
- Task templates  
- PR templates  
- ADR templates  
- Docs folder layout  

Must not include:

- Architectural rules  
- Coding style  
- Workflow process  

## 13.4 Workflow Conventions

Defines:

- Story → Task → PR lifecycle  
- Branching strategy  
- Commit message rules  
- Review process  
- CI/CD expectations  
- Definition of Done  
- Sprint review structure  

Must not include:

- Architectural rules  
- Coding style  
- Documentation templates  

## 13.5 Boundary Enforcement

- Architectural decisions belong in **Architecture Conventions** or **ADRs**  
- Coding style belongs in **Code Conventions**  
- Documentation templates belong in **Docs Conventions**  
- Work lifecycle rules belong in **Workflow Conventions**  

Guardrail tests and PR review checklists enforce these boundaries.

---

# Summary

The architecture ensures:

- Strict layering  
- Deterministic hosting behavior  
- Deterministic startup behavior  
- Testable, composable authentication pipelines  
- ImmutableContextBuilder as a core primitive  
- Strong security posture  
- Clear cross‑cutting boundaries  
- Frank as the backbone of shared behavior  
- Predictable CI/CD behavior  
- PR Preview safety  

All contributors must follow these conventions.
