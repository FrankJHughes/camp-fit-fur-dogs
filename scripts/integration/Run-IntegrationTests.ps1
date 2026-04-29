param(
    [Parameter(Mandatory = $true)]
    [string]$ConnectionString
)

Write-Host "Running integration tests against provided connection string..."

# Set temporary environment variable
$env:ConnectionStrings__DefaultConnection = $ConnectionString

try {
    dotnet test tests/CampFitFurDogs.IntegrationTests
}
finally {
    # Always clean up, even if tests fail
    Remove-Item Env:\ConnectionStrings__DefaultConnection -ErrorAction SilentlyContinue
    Write-Host "Environment variable cleared."
}
