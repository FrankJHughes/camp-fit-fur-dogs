# Add .NET Solution Skeleton

## Intent
Scaffold the foundational .NET solution structure with an API project, a test project, and a health-check endpoint to prove the build chain works end-to-end.

## Value
Gives every developer a runnable solution on day one and provides CI with a compilable target.

## Acceptance Criteria
- [x] CampFitFurDogs.sln exists at repo root
- [x] src/CampFitFurDogs.Api project targets .NET 8 and exposes /health
- [x] tests/CampFitFurDogs.Api.Tests project references the API project
- [x] dotnet build and dotnet test succeed locally
- [x] Health-check endpoint returns 200 OK

## Emotional Guarantees: N/A
