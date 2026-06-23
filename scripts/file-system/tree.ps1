function Show-Tree {
  param(
    [string]$Path = ".",
    [string]$Indent = ""
  )

  $items = Get-ChildItem -LiteralPath $Path | Sort-Object Name

  for ($i = 0; $i -lt $items.Count; $i++) {
    $item = $items[$i]
    $isLast = ($i -eq $items.Count - 1)
    $connector = if ($isLast) { "└── " } else { "├── " }

    # Only output the name, not the full path
    Write-Host "$Indent$connector$($item.Name)"

    if ($item.PSIsContainer) {
      $newIndent = if ($isLast) { "$Indent    " } else { "$Indent│   " }
      Show-Tree -Path $item.FullName -Indent $newIndent
    }
  }
}

Show-Tree
