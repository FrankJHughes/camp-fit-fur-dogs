# Workflow Conventions

## TDD Discipline

Work follows a red–green–refactor cycle:

1. Start with a failing test (red).
2. Make the test pass with the simplest possible change (green).
3. Refactor while keeping tests green (refactor).

This discipline applies across all layers: Api, Application, Domain, Infrastructure, and Frontend.  
New behavior is always driven by tests, not by ad‑hoc manual verification.

## Story Grammar

Stories use this grammar:

**As a `<role>`, I must or should be able to `<verb>` so that `<value>`.**

Stories must clearly express:

- The role (who)
- The capability (what)
- The business value (why)

Technical details belong in tasks, not in the story sentence.

## Debugging Discipline

Bug reports are treated as debugging sessions, not feature requests.

- Restate the problem clearly.
- Identify the smallest reproducible surface (failing test, endpoint, method).
- Prefer reasoning from test failures or debugger output instead of guessing.
- Avoid architectural changes until the problem is well understood.
- Minimize blast radius with local, reversible changes.
- Be explicit when inference is required; request missing signals instead of assuming them.

This discipline keeps debugging stable, predictable, and low‑churn.

## Universal Patch Rule

When a patch is required, regenerate and return the **entire file** with the patch already applied.  
This applies to all file types, including code and documentation.

- No partial edits  
- No diffs  
- No search‑and‑replace instructions  

Full file regeneration:

- Prevents corruption  
- Eliminates ambiguity  
- Aligns with script‑first and debugging discipline conventions  

## Script‑First and Fencing Rules

Scripts are the primary way to create or update files.

- Use PowerShell as the primary shell.
- Scripts must be copy‑pasteable.
- Generated files must use `utf8NoBOM` encoding.

Fencing and quoting rules:

- Avoid nested triple‑backtick fences in Markdown.
- When inner fences are required, use outer fences with four backticks.
- PowerShell here‑strings for PR bodies must be single‑quoted.
- Avoid literal double‑quote characters inside here‑strings.
- Avoid the sequences `@'` and `'@` inside here‑strings.
- Avoid backticks inside here‑strings.
- Avoid nested here‑strings.
- When documentation requires showing tricky syntax, describe it in words instead of using literal characters.

When generating files via scripts:

- Write content using single‑quoted here‑strings.
- Avoid fenced code blocks inside here‑strings.
- Prefer script‑driven, idempotent file generation.

## Makefile Conventions

Make targets are stack‑scoped:

- `backend-*`
- `frontend-*`
- `infra-*`

Bare targets are aggregates:

- `restore`
- `build`
- `test`
- `clean`
- `dev`

Each bare target orchestrates the corresponding stack‑scoped targets.

## Changelog Conventions

The changelog:

- Uses an `[Unreleased]` section for in‑progress work.
- References **issue numbers**, not PR numbers.
- Includes only user‑facing or architectural changes.

Internal refactors without user impact generally do not need entries.

## Tooling Conventions

- Primary shell: PowerShell.
- Scripts must be copy‑pasteable without manual fixes.
- Encoding for generated files: `utf8NoBOM`.

Tooling and automation must respect the fencing and script‑first rules.

## Identity Resolution Workflow

Endpoints never accept identity from the request body.  
Identity is resolved via `ICurrentUserService`.

- Application defines the abstraction.
- Infrastructure provides a concrete implementation.
- Api tests provide a test implementation.

This keeps identity resolution consistent, testable, and decoupled from transport details.

---

# API Deployment Workflow (US‑140)

The Camp Fit Fur Dogs API is deployed on Render using a Dockerized .NET 10 container.  
Deployment is automated and triggered by changes to the `main` branch.

## Deployment Model

- Render Web Service  
- Service name: **`campfitfurdogsapi`**  
- Dockerfile located at `src/CampFitFurDogs.Api/Dockerfile`  
- Health check path: `/health`  
- HTTPS termination handled by Render  
- Environment variables injected at runtime  

## CI/CD Behavior

- Every push to `main` triggers an automatic deploy on Render.
- No GitHub Actions workflow is required for deployment.
- Build and runtime logs are available in the Render dashboard.

## Secrets & Configuration

All secrets and connection strings are stored in Render’s Environment tab:

- `ConnectionStrings__DefaultConnection`
- `ASPNETCORE_ENVIRONMENT=Production`
- `Frontend__BaseUrl=<frontend-host-url>`

No secrets are committed to source control.

---

# PR Preview Workflow (Neon + Render)

Pull requests targeting `main` create a fully isolated preview environment consisting of:

- an **ephemeral Neon database branch**, and  
- a **Render PR Preview instance** of the API service (`campfitfurdogsapi`).  

This environment is rebuilt automatically on every commit to the PR branch.

## Preview Database Lifecycle

- A Neon branch is created using the naming convention:  
  `preview/pr-<number>-<branch>`
- A connection string is generated for the preview branch.
- The GitHub Actions workflow exports this value as `PREVIEW_DB_CONNECTION_STRING`.
- EF Core migrations are applied to the preview branch before the API preview is exercised.
- The Neon branch expires automatically after two weeks or is deleted when the PR closes.

## Render PR Preview Lifecycle

The API service is Git‑backed and has PR Previews enabled.

- Each commit to the PR triggers a new Render PR Preview build.
- Render evaluates `render.yaml` and injects `PREVIEW_DB_CONNECTION_STRING` via `previewValue`.
- The preview instance is deployed at a deterministic URL:  
  `https://campfitfurdogsapi-pr-<number>.onrender.com`
- The workflow waits for `/health` to return 200 before running integration tests.

## Integration Tests Against the Preview

API integration tests run against the live preview instance:

- The workflow computes the expected preview URL.
- It polls the `/health` endpoint until the service is ready.
- Tests run only after the preview instance is healthy.

## Cleanup

When the PR closes:

- The Neon preview branch is deleted.
- Render automatically deletes the PR Preview instance.

This workflow ensures each PR has a fully isolated, production‑like environment with its own database, migrations, and API instance.
