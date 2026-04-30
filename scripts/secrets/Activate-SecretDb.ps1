param(
    [Parameter(Mandatory = $true)]
    [string]$ConnectionString
)

Write-Host "Setting Secret Database connection string via dotnet user-secrets..."
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$ConnectionString" --project src/CampFitFurDogs.Api
Write-Host "Secret Database mode ON"
