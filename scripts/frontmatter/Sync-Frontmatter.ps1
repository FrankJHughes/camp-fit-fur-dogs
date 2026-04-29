<#
.SYNOPSIS
    Reads product/catalog.csv and updates story file frontmatter to match.

.DESCRIPTION
    For each row in the catalog, finds the corresponding story file and
    updates its YAML frontmatter with the catalog values. New fields are
    added; existing fields are updated. The story body is preserved.

    Syncable fields: milestone, status, urgency, importance, covey_quadrant,
    emotional_guarantees, legal_guarantees.

    Run from the repo root after editing the catalog during grooming.

.EXAMPLE
    .\scripts\Sync-Frontmatter.ps1
    .\scripts\Sync-Frontmatter.ps1 -WhatIf
#>

param(
    [string]$CatalogPath = "product/catalog.csv",
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"
$storyRoot = "product/stories"

# Fields the catalog is allowed to push into frontmatter
$scalarFields = @("milestone","status","urgency","importance","covey_quadrant")
$listFields   = @("emotional_guarantees","legal_guarantees")

function Find-StoryFile {
    param([string]$Id)
    $pattern = "$Id-*"
    $found = Get-ChildItem -Path $storyRoot -Filter $pattern -Recurse -ErrorAction SilentlyContinue |
        Select-Object -First 1
    return $found
}

function Update-Frontmatter {
    param(
        [string]$FilePath,
        [hashtable]$Updates
    )

    $content = Get-Content $FilePath -Raw -Encoding utf8
    if ($content -notmatch '(?s)^(---\r?\n)(.+?)(\r?\n---\r?\n)(.*)$') {
        Write-Warning "  Could not parse frontmatter in $FilePath"
        return
    }
    $fmOpen  = $Matches[1]
    $fmBody  = $Matches[2]
    $fmClose = $Matches[3]
    $body    = $Matches[4]

    $newlines = $fmBody -split "`n" | ForEach-Object { $_.TrimEnd() }
    $applied  = @{}

    # --- Update existing scalar fields ---
    $updatedLines = @()
    $skipList = $false
    foreach ($line in $newlines) {
        # Detect if we're in a list block that should be replaced
        if ($skipList) {
            if ($line -match '^\s+-\s') { continue }
            $skipList = $false
        }

        $matched = $false
        # Scalar field replacement
        foreach ($sf in $scalarFields) {
            if ($line -match "^${sf}:\s") {
                if ($Updates.ContainsKey($sf) -and $Updates[$sf] -ne "") {
                    $updatedLines += "${sf}: $($Updates[$sf])"
                    $applied[$sf] = $true
                    $matched = $true
                } else {
                    $updatedLines += $line
                    $matched = $true
                }
                break
            }
        }
        if ($matched) { continue }

        # List field replacement
        foreach ($lf in $listFields) {
            if ($line -match "^${lf}:\s*$") {
                if ($Updates.ContainsKey($lf) -and $Updates[$lf] -ne "") {
                    $items = $Updates[$lf] -split ";"
                    $updatedLines += "${lf}:"
                    foreach ($item in $items) {
                        $updatedLines += "  - $($item.Trim())"
                    }
                    $applied[$lf] = $true
                    $skipList = $true
                    $matched = $true
                } else {
                    $updatedLines += $line
                    $matched = $true
                    $skipList = $true
                }
                break
            }
        }
        if ($matched) { continue }

        $updatedLines += $line
    }

    # --- Append new fields not already in frontmatter ---
    foreach ($sf in $scalarFields) {
        if (-not $applied.ContainsKey($sf) -and $Updates.ContainsKey($sf) -and $Updates[$sf] -ne "") {
            $updatedLines += "${sf}: $($Updates[$sf])"
        }
    }
    foreach ($lf in $listFields) {
        if (-not $applied.ContainsKey($lf) -and $Updates.ContainsKey($lf) -and $Updates[$lf] -ne "") {
            $items = $Updates[$lf] -split ";"
            $updatedLines += "${lf}:"
            foreach ($item in $items) {
                $updatedLines += "  - $($item.Trim())"
            }
        }
    }

    $newFm = $updatedLines -join "`n"
    $newContent = "${fmOpen}${newFm}${fmClose}${body}"

    if ($WhatIf) {
        Write-Host "  [WhatIf] Would update $FilePath" -ForegroundColor Yellow
    } else {
        Set-Content -Path $FilePath -Value $newContent -Encoding utf8NoBOM -NoNewline
    }
}

# --- Main ---
$catalog = Import-Csv -Path $CatalogPath -Encoding utf8

$updated = 0
$skipped = 0

foreach ($row in $catalog) {
    $id = $row.id
    if ([string]::IsNullOrWhiteSpace($id)) { continue }

    # Find the story file
    $file = $null
    if ($row.file_path -and (Test-Path $row.file_path)) {
        $file = Get-Item $row.file_path
    } else {
        $file = Find-StoryFile -Id $id
    }

    if ($null -eq $file) {
        Write-Warning "  $id — file not found, skipping"
        $skipped++
        continue
    }

    # Build updates hashtable from catalog row
    $updates = @{}
    foreach ($sf in $scalarFields) {
        $val = $row.$sf
        if (-not [string]::IsNullOrWhiteSpace($val)) {
            $updates[$sf] = $val
        }
    }
    foreach ($lf in $listFields) {
        $val = $row.$lf
        if (-not [string]::IsNullOrWhiteSpace($val)) {
            $updates[$lf] = $val
        }
    }

    if ($updates.Count -eq 0) {
        $skipped++
        continue
    }

    Write-Host "  $id → $($file.FullName -replace [regex]::Escape((Get-Location).Path + '\'),'')" -ForegroundColor Cyan
    Update-Frontmatter -FilePath $file.FullName -Updates $updates
    $updated++
}

Write-Host ""
if ($WhatIf) {
    Write-Host "Dry run complete. $updated files would be updated, $skipped skipped." -ForegroundColor Yellow
} else {
    Write-Host "Sync complete. $updated files updated, $skipped skipped." -ForegroundColor Green
    Write-Host "Run 'git diff' to review changes before committing." -ForegroundColor Cyan
}
param(
    [switch]$CheckOnly
)

# Existing logic computes $expectedFrontmatter and $actualFrontmatter

if ($CheckOnly) {
    if ($expectedFrontmatter -eq $actualFrontmatter) {
        "OK"
        exit 0
    } else {
        "DIFF"
        exit 1
    }
}

# Existing write-mode behavior continues here
