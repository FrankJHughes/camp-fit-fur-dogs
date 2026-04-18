# ADR-0018: Central Package Management

| Field     | Value            |
|-----------|------------------|
| Status    | Accepted         |
| Date      | 2026-04-18       |
| Deciders  | Frank Hughes     |

## Context

NuGet package versions were declared individually in each of the 11 csproj files across the solution. This caused:

- **Version drift** — FluentAssertions, Testcontainers.PostgreSql, and xunit.runner.visualstudio each had different versions in different test projects.
- **MSB3277 build warnings** — transitive dependency conflicts (EF Core Relational) produced warnings that eroded trust in CI output.
- **Onboarding friction** — contributors had to inspect multiple csproj files to understand which versions were in use, and could unknowingly introduce drift when adding packages.

## Decision

Introduce Central Package Management (CPM) via a root-level `Directory.Packages.props` file:

- All `PackageReference` entries across the solution declare the package name only; versions live exclusively in `Directory.Packages.props`.
- `ManagePackageVersionsCentrally` is enabled solution-wide.
- `CentralPackageTransitivePinningEnabled` is enabled to resolve transitive version conflicts automatically.
- Existing version drifts are normalized to the highest version at time of adoption.

## Consequences

### Positive

- Single source of truth for every NuGet dependency version.
- Transitive pinning eliminates MSB3277 and similar build warnings.
- Version upgrades are atomic — one file change, one PR, all projects updated.
- Contributors cannot accidentally introduce version drift.

### Negative

- Contributors must learn that `Version=` is no longer set in csproj files.
- Adding a new package requires editing two files (`Directory.Packages.props` + the consuming csproj).
- CPM is a relatively new SDK feature — some tooling (older VS extensions, Rider versions) may not fully support it.

### Neutral

- No runtime behavior change — only build-time dependency resolution is affected.
- CI pipeline (`make all`) is unchanged.