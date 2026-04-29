param(
    [Parameter(Mandatory = $true)]
    [string]$BranchId
)

if (-not $env:NEON_API_KEY -or -not $env:NEON_PROJECT_ID) {
    Write-Error "NEON_API_KEY and NEON_PROJECT_ID must be set in the environment."
    exit 1
}

Invoke-RestMethod `
    -Method Delete `
    -Uri "https://console.neon.tech/api/v2/projects/$($env:NEON_PROJECT_ID)/branches/$BranchId" `
    -Headers @{ Authorization = "Bearer $($env:NEON_API_KEY)" }

Write-Host "Deleted Neon branch $BranchId"
