# Selective CI Testing

This repository uses a selective, dependency-aware CI pipeline to reduce build
times and ensure only impacted components are tested.

## Dependency Model

- Backend changes impact the frontend.
- Frontend changes do not impact the backend.
- Governance changes (stories, catalog, scripts) only require governance checks.
- Infrastructure changes only require compose validation.

## Path Mapping

| Area        | Paths                          | CI Actions                     |
|-------------|--------------------------------|--------------------------------|
| Backend     | \src/**\, \	ests/**\       | \make backend-test\ + frontend |
| Frontend    | \rontend/**\                | \make frontend-test\, lint, build |
| Governance  | \product/stories/**\, \scripts/**\, \catalog.csv\ | Catalog + frontmatter |
| Infra       | \docker-compose.yml\, \infrastructure/**\ | Compose validation |

## Workflow Behavior

1. CI detects changed paths.
2. CI determines which areas are impacted.
3. CI runs only the required Make targets.
4. Backend changes automatically trigger frontend tests.

This keeps CI fast, predictable, and aligned with the repository architecture.
