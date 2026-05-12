<#
.SYNOPSIS
    Safely syncs frontmatter fields from catalog.csv into story files.

.DESCRIPTION
    - Preserves story bodies
    - Normalizes list fields (no duplicates)
    - Updates scalar fields cleanly
    - Creates frontmatter if missing
    - Never touches id/title/file_path
    - Allows empty overwrites
#>

param(
  [string]$CatalogPath = "product/catalog.csv",
  [string]$StoryRoot = "product/stories",
  [switch]$WhatIf
)

$ErrorActionPreference = "Stop"

# -------------------------------
# Backup the entire stories folder before modifying files
# -------------------------------
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$storiesRoot = (Resolve-Path $StoryRoot).Path
$backupRoot = Join-Path (Split-Path $storiesRoot -Parent) "stories.backup-$timestamp"

Write-Host "Backing up stories folder to: $backupRoot" -ForegroundColor Yellow
Copy-Item -Path $storiesRoot -Destination $backupRoot -Recurse -Force

# Fields allowed to sync
$scalarFields = @(
  "milestone",
  "status",
  "urgency",
  "importance",
  "covey_quadrant",
  "domain",
  "vertical_slice",
  "epic"
)

$listFields = @(
  "emotional_guarantees",
  "legal_guarantees",
  "dependencies"
)

function Find-StoryFile {
  param([string]$Id)
  Get-ChildItem -Path $StoryRoot -Filter "$Id-*" -Recurse -ErrorAction SilentlyContinue |
  Select-Object -First 1
}

function Parse-Frontmatter {
  param([string]$Content)

  if ($Content -notmatch '(?s)^(---\r?\n)(.*?)(\r?\n---\r?\n)(.*)$') {
    return $null
  }

  return @{
    Open  = $Matches[1]
    Body  = $Matches[2]
    Close = $Matches[3]
    Rest  = $Matches[4]
  }
}

function Normalize-ListField {
  param(
    [string]$FieldName,
    [string]$CatalogValue
  )

  if ([string]::IsNullOrWhiteSpace($CatalogValue)) {
    return @("${FieldName}:")  # empty list root
  }

  $items = $CatalogValue -split ";" | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }

  $lines = @("${FieldName}:")
  foreach ($item in $items) {
    $lines += "  - $item"
  }

  return $lines
}

function Update-Frontmatter {
  param(
    [string]$FilePath,
    [hashtable]$Updates
  )

  $raw = Get-Content $FilePath -Raw -Encoding utf8
  $parsed = Parse-Frontmatter -Content $raw

  # CASE 1 — No frontmatter: create it
  if ($null -eq $parsed) {
    Write-Warning "  No frontmatter found — creating it."

    $fm = @("---")

    foreach ($sf in $scalarFields) {
      $fm += "${sf}: $($Updates[$sf])"
    }

    foreach ($lf in $listFields) {
      $fm += Normalize-ListField -FieldName $lf -CatalogValue $Updates[$lf]
    }

    $fm += "---"

    $newContent = ($fm -join "`n") + "`n" + $raw

    if ($WhatIf) {
      Write-Host "  [WhatIf] Would create frontmatter" -ForegroundColor Yellow
    }
    else {
      Set-Content -Path $FilePath -Value $newContent -Encoding utf8NoBOM -NoNewline
    }

    return
  }

  # CASE 2 — Update existing frontmatter
  $lines = $parsed.Body -split "`n"
  $updated = @{}
  $output = @()
  $skipList = $false

  foreach ($line in $lines) {

    # Skip list items after list root
    if ($skipList) {
      if ($line -match '^\s+-\s') { continue }
      $skipList = $false
    }

    $matched = $false

    # Scalar fields
    foreach ($sf in $scalarFields) {
      if ($line -match "^${sf}:\s*") {
        $output += "${sf}: $($Updates[$sf])"
        $updated[$sf] = $true
        $matched = $true
        break
      }
    }
    if ($matched) { continue }

    # List fields (normalize ANY lf: line, even scalar)
    foreach ($lf in $listFields) {
      if ($line -match "^${lf}:\s*(.*)$") {
        $output += Normalize-ListField -FieldName $lf -CatalogValue $Updates[$lf]
        $updated[$lf] = $true
        $skipList = $true
        $matched = $true
        break
      }
    }
    if ($matched) { continue }

    # Keep untouched lines
    $output += $line
  }

  # Append missing scalar fields
  foreach ($sf in $scalarFields) {
    if (-not $updated.ContainsKey($sf)) {
      $output += "${sf}: $($Updates[$sf])"
    }
  }

  # Append missing list fields
  foreach ($lf in $listFields) {
    if (-not $updated.ContainsKey($lf)) {
      $output += Normalize-ListField -FieldName $lf -CatalogValue $Updates[$lf]
    }
  }

  $newFm = $output -join "`n"
  $newContent = $parsed.Open + $newFm + $parsed.Close + $parsed.Rest

  if ($WhatIf) {
    Write-Host "  [WhatIf] Would update frontmatter" -ForegroundColor Yellow
  }
  else {
    Set-Content -Path $FilePath -Value $newContent -Encoding utf8NoBOM -NoNewline
  }
}

# MAIN
$catalog = Import-Csv -Path $CatalogPath -Encoding utf8

foreach ($row in $catalog) {

  $id = $row.id
  if ([string]::IsNullOrWhiteSpace($id)) { continue }

  $file = Find-StoryFile -Id $id
  if ($null -eq $file) {
    Write-Warning "  $id — story file not found"
    continue
  }

  $updates = @{}
  foreach ($sf in $scalarFields) { $updates[$sf] = $row.$sf }
  foreach ($lf in $listFields) { $updates[$lf] = $row.$lf }

  Write-Host "  $id → $($file.FullName)" -ForegroundColor Cyan
  Update-Frontmatter -FilePath $file.FullName -Updates $updates
}

Write-Host "`nSync complete." -ForegroundColor Green
