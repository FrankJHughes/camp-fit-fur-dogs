# US-053 — Feature Slice Scaffold Script

## Intent

As a developer, I want a PowerShell script that generates the full file structure for a new feature slice so that I start every slice with the correct folders, stubs, and a failing test at each layer — enforcing TDD from the first keystroke.

## Value

Every vertical slice shipped so far follows an identical structure across Domain, Application, Infrastructure, and API layers, each with a corresponding test project. Manually creating these files is tedious, error-prone, and tempts developers to skip the red step. A scaffold script encodes the team's conventions into an executable template, eliminating boilerplate setup and guaranteeing that every new slice begins with `Assert.Fail("RED — implement me")` tests at every layer.

## Acceptance Criteria

- `scripts/New-FeatureSlice.ps1` exists and is executable from the repo root
- Script accepts `-Name` (e.g., "UpdateCustomer") and `-Type` ("Command" or "Query") parameters
- For a Command slice, the script generates:
  - `src/CampFitFurDogs.Application/Commands/{Name}/` — command and handler stubs
  - `src/CampFitFurDogs.Infrastructure/` — repository method stub (if new aggregate)
  - `src/CampFitFurDogs.Api/Endpoints/` — endpoint stub
  - `tests/CampFitFurDogs.Domain.Tests/` — failing test
  - `tests/CampFitFurDogs.Application.Tests/Commands/{Name}/` — failing test
  - `tests/CampFitFurDogs.Infrastructure.Tests/` — failing test
  - `tests/CampFitFurDogs.Api.Tests/` — failing test
- For a Query slice, the script generates the equivalent query-side structure
- Every generated test file contains a `[Fact]` with `Assert.Fail("RED — implement me")`
- Running `dotnet build` succeeds after scaffold (stubs compile)
- Running `dotnet test` shows the new tests failing (red step confirmed)
- Script is idempotent — refuses to overwrite existing files
- Developer guide (US-052) references the script as the recommended starting point for new slices

## Dependencies

- US-051 (ADR-0011: CQRS) — the script implements the patterns documented in the ADR
- US-052 (Developer Guide: Feature Slice Walkthrough) — the guide references this script

## Emotional Guarantees

- EG-01 No surprises — the generated structure matches the documented architecture exactly
- EG-04 Always know where you stand — every slice starts at red with a clear path to green
