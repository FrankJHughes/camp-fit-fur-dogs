# Invoke-WithPrettyLogging.ps1
param(
    [Parameter(Mandatory)]
    [string]$ScriptPath,

    [Parameter(Mandatory)]
    [string]$LogPath
)

# Normalize ScriptPath ONLY (caller controls LogPath)
if (-not (Split-Path $ScriptPath -IsAbsolute)) {
    $ScriptPath = Join-Path $PSScriptRoot $ScriptPath
}

# Overwrite log file
if (Test-Path $LogPath) {
    Remove-Item $LogPath -Force
}

# Redirect ALL streams into the pipeline:
#   1 = Output
#   2 = Error
#   3 = Warning
#   4 = Verbose
#   5 = Debug
#   6 = Information (Write-Host)
#
# Each is tagged with a label so you know what produced it.

& $ScriptPath *>&1 |
    ForEach-Object {
        $stream = $_.GetType().Name

        switch ($stream) {
            "ErrorRecord"      { $label = "ERROR" }
            "WarningRecord"    { $label = "WARNING" }
            "VerboseRecord"    { $label = "VERBOSE" }
            "DebugRecord"      { $label = "DEBUG" }
            "InformationRecord"{ $label = "INFO" }
            default            { $label = "OUTPUT" }
        }

        $line = "[{0}] [{1}] {2}" -f (Get-Date -Format "HH:mm:ss"), $label, $_

        Write-Host $line
        Add-Content -Path $LogPath -Value $line
    }
