function Show-Tree {
  param(
    [string]$Path = ".",
    [string]$Indent = ""
  )

  # Folders to exclude
  $exclude = @("bin", "obj", "node_modules")

  # Get only directories, excluding unwanted ones
  $items = Get-ChildItem -LiteralPath $Path -Directory |
  Where-Object { $exclude -notcontains $_.Name } |
  Sort-Object Name

  for ($i = 0; $i -lt $items.Count; $i++) {
    $item = $items[$i]
    $isLast = ($i -eq $items.Count - 1)
    $connector = if ($isLast) { "└── " } else { "├── " }

    Write-Host "$Indent$connector$($item.Name)"

    # Recurse into subdirectories
    $newIndent = if ($isLast) { "$Indent    " } else { "$Indent│   " }
    Show-Tree -Path $item.FullName -Indent $newIndent
  }
}

Show-Tree
