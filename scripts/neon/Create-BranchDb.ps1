param(
    [Parameter(Mandatory = $true)]
    [string]$BranchName
)

if (-not $env:NEON_API_KEY -or -not $env:NEON_PROJECT_ID) {
    Write-Error "NEON_API_KEY and NEON_PROJECT_ID must be set in the environment."
    exit 1
}

$body = @{
    branch = @{
        name = $BranchName
    }
} | ConvertTo-Json

$response = Invoke-RestMethod `
    -Method Post `
    -Uri "https://console.neon.tech/api/v2/projects/$($env:NEON_PROJECT_ID)/branches" `
    -Headers @{ Authorization = "Bearer $($env:NEON_API_KEY)" } `
    -ContentType "application/json" `
    -Body $body

$response | ConvertTo-Json -Depth 5
