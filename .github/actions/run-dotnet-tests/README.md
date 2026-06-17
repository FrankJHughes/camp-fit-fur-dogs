# Run .NET Tests

A reusable GitHub Action that builds and tests one or more .NET test projects  
using a consistent, deterministic configuration. This action centralizes the  
logic used across CI workflows to ensure all test projects are built cleanly and  
executed with the same settings.

---

## What this action does

1. Installs the .NET SDK using `actions/setup-dotnet`, honoring `global.json` when present.
2. Restores dependencies (solution‑wide or per‑project fallback).
3. Removes all `bin/` and `obj/` directories to guarantee a clean build.
4. Builds each test project in **Debug** configuration using absolute paths rooted at `$GITHUB_WORKSPACE`.
5. Runs tests for each project using:
   - `--framework net10.0`
   - `--no-build`
   - `--no-restore`
   - detailed console logging.

Each project is grouped in the GitHub Actions log for easier navigation.

---

## Inputs

### `projects` (required)

A **newline‑separated list** of test project paths, relative to the repository root.

Example:

```text
tests/Frank.Tests
tests/CampFitFurDogs.Api.Tests
tests/CampFitFurDogs.Infrastructure.Tests
```

---

## Usage

```yaml
- uses: ./.github/actions/run-dotnet-tests
  with:
    projects: |
      tests/Frank.Tests
      tests/CampFitFurDogs.Api.Tests
      tests/CampFitFurDogs.Infrastructure.Tests
```

---

## Notes

- This action uses `$GITHUB_WORKSPACE` to ensure project paths resolve correctly  
  even when composite actions run from inside `.github/actions/...`.
- If a `global.json` file is present at the repository root, the SDK version  
  specified there will be installed automatically.
- All test projects must target **net10.0** or the framework specified in the action.
- Build and test steps are intentionally separated to provide clearer logs and  
  easier debugging.

---

## Example (full job)

```yaml
jobs:
  backend-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Restore dependencies
        run: dotnet restore

      - uses: ./.github/actions/run-dotnet-tests
        with:
          projects: |
            tests/CampFitFurDogs.Api.Tests
            tests/CampFitFurDogs.Application.Tests
            tests/CampFitFurDogs.Domain.Tests
            tests/CampFitFurDogs.Infrastructure.Tests
```

---

## License

This action is part of the Camp Fit Fur Dogs repository and follows the  
repository’s licensing terms.
