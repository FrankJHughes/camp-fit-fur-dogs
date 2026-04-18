# CI Baseline — Build and Test

## Intent
Create a GitHub Actions workflow that builds the solution and runs tests on every push and pull request, providing fast feedback on code health.

## Value
Catches regressions before merge, establishes the quality gate that all other CI enhancements build upon.

## Acceptance Criteria
- [x] .github/workflows/ci.yml triggers on push and pull_request to main
- [x] Workflow restores, builds, and tests the .NET solution
- [x] Build status badge is embedded in README.md
- [x] Workflow completes in under 3 minutes

## Emotional Guarantees: N/A
