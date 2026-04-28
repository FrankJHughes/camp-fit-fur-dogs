param()

Write-Host "Checking story frontmatter consistency..."

# Generate expected frontmatter in memory
$result = pwsh scripts/frontmatter/Sync-FrontMatter.ps1 -CheckOnly

if ($result -ne "OK") {
    Write-Error "Frontmatter is out of sync. Run Sync-Frontmatter.ps1 locally."
    exit 1
}

Write-Host "Frontmatter is consistent."
