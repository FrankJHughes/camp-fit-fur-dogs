# Hosting Modules

Hosting modules provide environment‑specific configuration for the Camp Fit Fur Dogs platform.  
They live in the **CampFitFurDogs.Api** assembly but are **executed by the Host layer** during startup.

This separation ensures the API remains pure and host‑agnostic, while the Host layer adapts the application to its runtime environment.

---

## Purpose

Hosting modules allow the platform to:

- detect hosting environments  
- apply configuration overrides  
- integrate with CI/CD artifacts  
- adapt behavior for PR previews  
- modify settings before DI registration  

They are executed by the Host layer through:

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

---

## Render PR Preview Hosting Module

The primary hosting module is:

```
HostingModules/RenderPrPreviewHostingModule.cs
```

It activates when Render PR Preview environment variables are present:

- `IS_PULL_REQUEST`
- `RENDER_GIT_REPO_SLUG`
- `RENDER_SERVICE_NAME`
- `GITHUB_PAT`

### Responsibilities

- detect PR preview environment  
- extract PR number from service name  
- fetch GitHub Actions artifacts:
  - `db-conn.txt`
  - `frontend-url.txt`
- apply configuration overrides:
  - `ConnectionStrings:DefaultConnection`
  - `Frontend:BaseUrl`

This enables dynamic configuration per pull request.

---

## Supporting Components

### GitHubArtifactClient

Retrieves files from GitHub Actions artifacts.

### RenderPrParser

Parses Render service names to extract PR numbers.

### Interfaces

- `IGitHubArtifactClient`
- `IRenderPrParser`

These abstractions make hosting modules testable and composable.

---

## Execution Flow

1. Host builds `WebApplicationBuilder`  
2. Host constructs hosting modules  
3. Host executes each module  
4. Modules produce configuration overrides  
5. Host merges overrides into the builder configuration  
6. Platform services are registered using the updated configuration  

This ensures environment‑specific behavior is applied **before** DI registration.

---

## Why Hosting Modules Live in the API Assembly

Hosting modules depend on:

- API configuration conventions  
- API environment variables  
- API‑specific hosting behavior

But they must be executed by the Host layer.

This split provides:

- API‑defined hosting logic  
- Host‑controlled execution  
- clean separation of concerns  
- reusable hosting modules across hosts (e.g., test harness)

---

## Summary

Hosting modules provide dynamic, environment‑specific configuration for the platform.  
They are defined in the API assembly but executed by the Host layer, ensuring:

- API purity  
- host‑controlled startup  
- predictable environment adaptation  
- clean vertical‑slice architecture  
- seamless CI/CD integration  
