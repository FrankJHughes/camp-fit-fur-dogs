dotnet user-secrets remove "ConnectionStrings:DefaultConnection" --project $PSScriptRoot/../../src/CampFitFurDogs.Api
Write-Host "Secret Database mode OFF (local Postgres active)"
