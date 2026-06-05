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
- **Frank** — cross‑cutting primitives, DI auto‑registration engine, endpoint discovery, hosting provider abstractions, environment abstraction, security headers, test seams

## 1.1 Layering Rules

These rules are enforced by guardrail tests:

- Domain depends on **nothing** outside Domain  
- Application depends only on **Domain** and **Frank abstractions**  
- Infrastructure depends on **Application**, **Domain**, and **Frank abstractions**  
- Api depends on **Application**, **Domain**, and **Frank**  
- Frank depends on **nothing in the product code**  

Frank is the **only allowed cross‑layer dependency**.

---

# 2. Frank (Cross‑Cutting Layer)

Frank provides the system’s cross‑cutting primitives and infrastructure.  
It is not a general DI container — it registers **core services** and **attributed interfaces**, nothing more.

## 2.1 Responsibilities

Frank provides:

- **DI auto‑registration engine**  
  - Registers services derived from interfaces marked with `[AutoRegister]`  
  - Registers core CQRS services (CommandDispatcher, QueryDispatcher, DomainEventDispatcher)

- **Endpoint discovery**  
  - Scans assemblies for `IEndpoint` implementations  
  - Registers them automatically

- **Validator scanning**  
  - Discovers and registers FluentValidation validators

- **Security headers**  
  - `AddSecurityHeaders()` extension  
  - `SecurityHeadersMiddleware` with hardened OWASP defaults  
  - Required for all environments, including PR Previews

- **Hosting provider abstractions**  
  - `IHostingProvider`  
  - `IEnvironment`  
  - `IRenderPrParser`  
  - `IGitHubArtifactClient`  
  - `IRenderConfigurationWriter`  
  - Deterministic, testable hosting provider pipeline

- **Environment abstraction**  
  - `IEnvironment` for safe, testable environment variable access

- **Test seams**  
  - Environment seam  
  - Hosting provider seam  
  - Artifact client seam  
  - PR parser seam  
  - Configuration writer seam

Frank does **not** register slice‑specific services or own all DI.

---

# 3. Hosting Model

The system supports pluggable hosting providers.  
Hosting providers configure environment‑specific behavior at startup.

## 3.1 Provider Selection

- Providers are evaluated **in order**  
- The **first provider whose `IsActive()` returns true** wins  
- All others are skipped  
- This rule is enforced by guardrails

## 3.2 Provider Requirements

All hosting providers must:

- Use **injected abstractions only**  
- Never read environment variables directly  
- Never perform HTTP calls directly  
- Never parse JSON or ZIP files directly  
- Never write configuration directly  
- Be fully testable without touching real infrastructure

## 3.3 Render Hosting Provider

Render is the canonical hosting provider for PR Previews.

It uses:

- `IEnvironment`  
- `IRenderPrParser`  
- `IGitHubArtifactClient`  
- `IRenderConfigurationWriter`

### Required environment variables:

- `IS_PULL_REQUEST`  
- `RENDER_GIT_REPO_SLUG`  
- `RENDER_SERVICE_NAME`  
- `GITHUB_PAT`

### Required artifacts:

- `pr-{n}-db/db-conn.txt`  
- `pr-{n}-frontend/frontend-url.txt`

### Behavior:

- Extract PR number from `RENDER_SERVICE_NAME`  
- Load DB connection string from GitHub artifacts  
- Load frontend base URL from GitHub artifacts  
- Apply configuration via `IRenderConfigurationWriter`  
- Throw on missing or invalid data

---

# 4. PR Preview Architecture

PR Previews run on Render using Git‑backed deployments.

## 4.1 Lifecycle

1. Label removed  
2. Teardown probe (`/health` → 404 ×3)  
3. Database provisioning  
4. Migrations  
5. Infrastructure tests  
6. Label added  
7. Readiness probe (`/api/dogs` → 200/400/401 ×3)  
8. API tests

## 4.2 Requirements

- Code must tolerate empty databases  
- Migrations must be idempotent  
- No environment‑specific branching  
- No hardcoded URLs or secrets  
- Security headers must be present  
- Hosting provider must be deterministic and testable

---

# 5. Api Architecture

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

# 6. Application Architecture

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

# 7. Domain Architecture

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

# 8. Infrastructure Architecture

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

# 9. Security Architecture

Security headers are mandatory.

Api must apply:

- `AddSecurityHeaders()`  
- `SecurityHeadersMiddleware`

Headers include:

- X‑Content‑Type‑Options  
- X‑Frame‑Options  
- X‑XSS‑Protection  
- Referrer‑Policy  
- Permissions‑Policy  
- COOP / COEP / CORP  
- CSP (strict baseline)

Guardrails enforce presence.

---

# Summary

The architecture ensures:

- Strict layering  
- Deterministic hosting behavior  
- Testable, composable providers  
- Strong security posture  
- Clear cross‑cutting boundaries  
- Frank as the backbone of shared behavior  
- PR Preview safety  
- Predictable CI/CD behavior

All contributors must follow these conventions.
