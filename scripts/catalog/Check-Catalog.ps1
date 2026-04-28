param()

Write-Host "Checking catalog consistency..."

# Load committed catalog
$catalogPath = "catalog.csv"
if (-not (Test-Path $catalogPath)) {
    Write-Error "catalog.csv not found"
    exit 1
}

$committed = Get-Content $catalogPath -Raw

# Generate expected catalog in memory
$generated = pwsh scripts/catalog/Generate-Catalog.ps1 -OutputString

if ($generated -ne $committed) {
    Write-Error "Catalog is out of sync. Run Generate-Catalog.ps1 locally."
    exit 1
}

Write-Host "Catalog is consistent."
