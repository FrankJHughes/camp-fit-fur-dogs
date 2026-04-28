<#
.SYNOPSIS
    Reads all story files under product/stories/ and generates product/catalog.csv.

.DESCRIPTION
    Parses YAML frontmatter from each story markdown file and outputs a CSV
    catalog suitable for grooming and sprint planning. Run from the repo root.

.EXAMPLE
    .\scripts\Generate-Catalog.ps1
#>

$ErrorActionPreference = "Stop"
$storyRoot = "product/stories"
$outputPath = "product/catalog.csv"

$fields = @(
    "id","title","epic","milestone","status","domain",
    "urgency","importance","covey_quadrant","vertical_slice",
    "emotional_guarantees","legal_guarantees","file_path"
)

# List fields that are stored as YAML arrays in frontmatter
$listFields = @("emotional_guarantees","legal_guarantees","dependencies")

function Parse-Frontmatter {
    param([string]$Content, [string]$FilePath)

    if ($Content -notmatch '(?s)^---\r?\n(.+?)\r?\n---') { return $null }
    $fm = $Matches[1]
    $result = @{}
    $result["file_path"] = $FilePath -replace "\\","/"

    $currentList = $null
    $currentItems = @()

    foreach ($rawLine in ($fm -split "`n")) {
        $line = $rawLine.TrimEnd()

        # Continuation of a list
        if ($null -ne $currentList -and $line -match '^\s+-\s+(.+)') {
            $currentItems += $Matches[1].Trim().Trim('"')
            continue
        }
        # End of list — flush
        if ($null -ne $currentList) {
            $result[$currentList] = $currentItems -join ";"
            $currentList = $null
            $currentItems = @()
        }
        # Key with empty value (start of list)
        if ($line -match '^(\w[\w_]*):\s*$') {
            $key = $Matches[1]
            $currentList = $key
            $currentItems = @()
            continue
        }
        # Key: value (scalar)
        if ($line -match '^(\w[\w_]*):\s*(.+)$') {
            $key = $Matches[1]
            $val = $Matches[2].Trim().Trim('"').Trim("'")
            $result[$key] = $val
        }
    }
    # Flush trailing list
    if ($null -ne $currentList) {
        $result[$currentList] = $currentItems -join ";"
        $currentList = $null
    }

    return $result
}

$rows = @()
Get-ChildItem -Path $storyRoot -Filter "*.md" -Recurse |
    Where-Object { $_.Name -ne "STORY-TEMPLATE.md" } |
    ForEach-Object {
        $content = Get-Content $_.FullName -Raw -Encoding utf8
        $relativePath = $_.FullName.Substring((Get-Location).Path.Length + 1)
        $parsed = Parse-Frontmatter -Content $content -FilePath $relativePath
        if ($null -ne $parsed -and $parsed.ContainsKey("id")) {
            $obj = [ordered]@{}
            foreach ($f in $fields) {
                $obj[$f] = if ($parsed.ContainsKey($f)) { $parsed[$f] } else { "" }
            }
            $rows += [PSCustomObject]$obj
        }
    }

$rows |
    Sort-Object { [int]($_.id -replace '\D','') } |
    Export-Csv -Path $outputPath -NoTypeInformation -Encoding utf8NoBOM

$count = $rows.Count
Write-Host "Generated $outputPath with $count stories." -ForegroundColor Green
