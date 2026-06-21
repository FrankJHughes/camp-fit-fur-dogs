# PowerShell tree-style output
# Requires PowerShell 7+

Get-ChildItem -Recurse -Directory `
| Where-Object {
  $_.FullName -notmatch 'bin' -and
  $_.FullName -notmatch 'obj' -and
  $_.FullName -notmatch 'node_modules' -and
  $_.FullName -notmatch 'dist' -and
  $_.FullName -notmatch 'build'
} `
| ForEach-Object {
  $_.FullName.Replace((Get-Location).Path, '')
}
