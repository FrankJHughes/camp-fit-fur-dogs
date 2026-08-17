function Show-Tree {
  param(
    [string]$Path = ".",
    [string]$Indent = ""
  )

  # Exclude bin and obj folders
  $items = Get-ChildItem -LiteralPath $Path |
  Where-Object { $_.Name -notin @("bin", "obj") } |
  Sort-Object Name

  for ($i = 0; $i -lt $items.Count; $i++) {
    $item = $items[$i]
    $isLast = ($i -eq $items.Count - 1)
    $connector = if ($isLast) { "└── " } else { "├── " }

    Write-Host "$Indent$connector$($item.Name)"

    if ($item.PSIsContainer) {
      $newIndent = if ($isLast) { "$Indent    " } else { "$Indent│   " }
      Show-Tree -Path $item.FullName -Indent $newIndent
    }
  }
}

Show-Tree
