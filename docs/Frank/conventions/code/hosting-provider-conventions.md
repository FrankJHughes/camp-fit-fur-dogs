# Hosting Provider Conventions (Frank)

Frank provides a hosting abstraction layer that allows products to run in
different environments (local, preview, production) without environment‑specific
code.

Hosting providers are discovered and executed by `HostingEngine`.

---

## Purpose

Hosting providers configure:

- environment‑specific settings  
- preview environment behavior  
- external service URLs  
- database connection strings  
- feature flags  

Providers must remain **pure configuration transformers** and must not perform
business logic.

---

## Provider Rules

Hosting providers must:

- use injected abstractions only  
- never read environment variables directly  
- never perform HTTP calls directly  
- never parse JSON or ZIP files directly  
- never write configuration directly  
- be fully testable  
- be auto‑registered via Frank’s DI engine  

Providers must not:

- depend on product code  
- depend on product configuration formats  
- mutate global state  
- perform I/O outside HttpClient abstractions  

---

## Provider Selection

- Providers are evaluated in order.  
- The first provider whose `IsActive()` returns `true` is selected.  
- Guardrails ensure only one provider is active.  

---

## Render Hosting Provider

Render is the canonical provider for PR previews.

It uses:

- `IEnvironment`  
- `IRenderPrParser`  
- `IGitHubArtifactClient`  
- `IRenderConfigurationWriter`  

Required environment variables:

- `IS_PULL_REQUEST`  
- `RENDER_GIT_REPO_SLUG`  
- `RENDER_SERVICE_NAME`  
- `GITHUB_PAT`  

Required artifacts:

- `pr-{n}-db/db-conn.txt`  
- `pr-{n}-frontend/frontend-url.txt`  

