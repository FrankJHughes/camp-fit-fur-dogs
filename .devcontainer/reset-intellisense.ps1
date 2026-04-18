@'
Resetting IntelliSense inside dev container...
'@

# Kill stale language servers
Get-Process dotnet -ErrorAction SilentlyContinue | Where-Object {
  $_.Path -like "*omnisharp*" -or $_.Path -like "*roslyn*"
} | Stop-Process -Force

# Clear caches
Remove-Item -Recurse -Force /root/.omnisharp -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force /root/.cache/omnisharp -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force /root/.vscode-server/data/User/workspaceStorage -ErrorAction SilentlyContinue

# Rebuild
dotnet clean
dotnet build

Write-Host "IntelliSense reset complete."
