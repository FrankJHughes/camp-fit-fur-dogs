<#
.SYNOPSIS
    Generates catalog.csv from story frontmatter, backing up the existing file first.

.DESCRIPTION
    - Scans product/stories recursively
    - Extracts frontmatter fields
    - Normalizes list fields
    - Populates canonical catalog schema
    - Backs up existing catalog.csv before overwriting it
    - Dependencies come from story frontmatter (source of truth)
#>

param(
  [string]$StoryRoot = "product/stories",
  [string]$OutputPath = "product/catalog.csv"
)

$ErrorActionPreference = "Stop"

# -------------------------------
# 1. Backup existing catalog.csv before overwriting
# -------------------------------
if (Test-Path $OutputPath) {
  $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
  $backupPath = (Join-Path (Split-Path $OutputPath -Parent) "catalog.backup-$timestamp.csv")
  Copy-Item -Path $OutputPath -Destination $backupPath -Force
  Write-Host "Existing catalog backed up to: $backupPath" -ForegroundColor Yellow
}

# Canonical schema (Option A)
$schema = @(
  "id",
  "title",
  "epic",
  "milestone",
  "status",
  "domain",
  "urgency",
  "importance",
  "covey_quadrant",
  "vertical_slice",
  "dependencies",
  "emotional_guarantees",
  "legal_guarantees",
  "file_path"
)

# List fields
$listFields = @(
  "dependencies",
  "emotional_guarantees",
  "legal_guarantees"
)

function Parse-Frontmatter {
  param([string]$Content)

  if ($Content -notmatch '(?s)^---\r?\n(.*?)\r?\n---\r?\n') {
    return $null
  }

  return ($Matches[1] -split "`n")
}

function Extract-Scalar {
  param([string[]]$Lines, [string]$Field)

  foreach ($line in $Lines) {
    if ($line -match "^${Field}:\s*(.*)$") {
      $value = $Matches[1].Trim()

      # Strip surrounding double quotes
      if ($value -match '^"(.*)"$') {
        return $Matches[1]
      }

      # Strip surrounding single quotes
      if ($value -match "^'(.*)'$") {
        return $Matches[1]
      }

      return $value
    }
  }

  return ""
}

function Extract-List {
  param([string[]]$Lines, [string]$Field)

  $items = @()
  $inBlock = $false

  foreach ($line in $Lines) {

    if ($line -match "^${Field}:\s*$") {
      $inBlock = $true
      continue
    }

    if ($inBlock) {
      if ($line -match '^\s+-\s*(.+)$') {
        $items += $Matches[1].Trim()
      }
      elseif ($line -match '^\S') {
        break
      }
    }
  }

  return ($items -join ";")
}

# Collect rows
$rows = @()

$files = Get-ChildItem -Path $StoryRoot -Recurse -Filter "US-*.md"

foreach ($file in $files) {

  $raw = Get-Content $file.FullName -Raw -Encoding utf8
  $fm = Parse-Frontmatter -Content $raw

  if ($null -eq $fm) {
    Write-Warning "No frontmatter in $($file.FullName) — skipping"
    continue
  }

  $row = [ordered]@{}

  # Populate scalar fields
  foreach ($field in $schema) {
    if ($listFields -contains $field) { continue }
    if ($field -eq "file_path") { continue }

    $row[$field] = Extract-Scalar -Lines $fm -Field $field
  }

  # Populate list fields
  foreach ($lf in $listFields) {
    $row[$lf] = Extract-List -Lines $fm -Field $lf
  }

  # Add file_path
  $row["file_path"] = $file.FullName.Replace("\", "/")

  $rows += New-Object PSObject -Property $row
}

# Write CSV
$rows |
Select-Object $schema |
Export-Csv -Path $OutputPath -NoTypeInformation -Encoding utf8

Write-Host ""
Write-Host "Catalog generation complete." -ForegroundColor Green
Write-Host "Updated catalog written to $OutputPath" -ForegroundColor Cyan
