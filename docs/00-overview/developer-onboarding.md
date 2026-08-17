# Developer Onboarding

A good starting point is the source tree itself. The project is intentionally organized so that product behavior and reusable platform concerns can be understood from the folder structure.

## Recommended sequence

1. Read the repository-level docs and root `README.md` files.
2. Review the overview material under `docs/00-overview`.
3. Read the product docs under `docs/01-campfitfurdogs`.
4. Study the platform responsibilities under `docs/02-frank-core` and `docs/03-frank-identity`.
5. Run the solution build and automated tests before making behavioral changes.

## Typical workflow

- find the feature or subsystem being changed
- trace the request from API endpoint to application handler
- inspect infrastructure only after understanding the operation contract
- validate behavior with the narrowest relevant test surface

## Good habits

- keep domain logic free of HTTP and EF Core concerns
- prefer abstractions and interfaces where the platform already defines them
- route changes through the relevant vertical slice instead of scattering logic across layers
