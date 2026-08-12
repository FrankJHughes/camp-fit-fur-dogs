# HostingModules

The **HostingModules** folder contains all hosting‑environment adaptation logic for
the Camp Fit Fur Dogs API.  
These modules allow the application to dynamically adjust configuration based on
the environment in which it is running — especially Render PR Preview deployments.

This folder implements a small, composable hosting pipeline built on
`IHostingModule`, enabling environment‑specific overrides without polluting
startup code.

---

## Files

```
HostingModules/
│
├── RenderPrPreviewHostingModule.cs
├── GitHubArtifactClient.cs
├── IGitHubArtifactClient.cs
├── RenderPrParser.cs
└── IRenderPrParser.cs
```

---

## Module Overview

### RenderPrPreviewHostingModule.cs

The primary hosting module for Render PR Preview environments.

**Responsibilities:**

- Detects whether the app is running inside a Render PR Preview deployment  
- Extracts the PR number from the Render service name  
- Fetches PR‑specific configuration from GitHub Actions artifacts:
  - `db-conn.txt` → database connection string  
  - `frontend-url.txt` → frontend base URL  
- Produces configuration overrides for:
  - `ConnectionStrings:DefaultConnection`
  - `Frontend:BaseUrl`

This module activates only when all required environment variables are present:

- `IS_PULL_REQUEST`
- `RENDER_GIT_REPO_SLUG`
- `RENDER_SERVICE_NAME`
- `GITHUB_PAT`

---

## Supporting Components

### GitHubArtifactClient.cs

A GitHub REST API client used to retrieve files from GitHub Actions artifacts.

**Responsibilities:**

- Query GitHub for artifacts matching a PR‑specific name  
- Download the artifact ZIP archive  
- Extract a specific file from the archive  
- Return the file contents as a string  

Used by `RenderPrPreviewHostingModule` to fetch environment‑specific configuration.

---

### IGitHubArtifactClient.cs

Interface abstraction for retrieving files from GitHub Actions artifacts.

**Responsibilities:**

- Defines `GetArtifactFileAsync`  
- Allows mocking and test injection  
- Decouples hosting logic from HTTP/GitHub implementation details  

---

### RenderPrParser.cs

Parses Render service names to extract pull‑request numbers.

**Example:**

```
campfitfurdogs-api-pr-123 → "123"
```

**Responsibilities:**

- Split service name on hyphens  
- Validate expected structure  
- Return the trailing PR number  

Used by `RenderPrPreviewHostingModule` to determine which artifacts to fetch.

---

### IRenderPrParser.cs

Interface abstraction for parsing Render service names.

**Responsibilities:**

- Defines `TryParse`  
- Enables testability and alternative parsing strategies  

---

## How Hosting Modules Integrate

Hosting modules are composed and executed through the `HostingEngine` in:

```
Helpers/Hosting.cs
```

The hosting pipeline:

1. Constructs hosting modules  
2. Determines which modules are active  
3. Applies environment‑specific configuration overrides  
4. Merges overrides into the application’s configuration  

This keeps startup clean and environment logic isolated.

---

## Design Principles

Hosting modules follow these principles:

- **Environment isolation** — hosting logic is kept separate from startup  
- **Composability** — multiple hosting modules can be combined  
- **Testability** — all modules use abstractions (`IEnvironmentVariables`, `IRenderPrParser`, `IGitHubArtifactClient`)  
- **Predictability** — each module declares activation rules and override behavior  
- **CI/CD integration** — modules can consume artifacts produced during GitHub Actions workflows  

---

## Summary

The HostingModules folder defines the complete hosting‑environment adaptation layer
for the Camp Fit Fur Dogs API:

- 5 focused hosting components  
- Full support for Render PR Preview deployments  
- GitHub artifact integration  
- Clean separation from startup and domain logic  
- Extensible hosting pipeline via `IHostingModule`

This structure ensures the API behaves consistently and predictably across all
deployment environments.

