param(
    [switch]$VerboseOutput
)

Write-Host "Validating CI dependency graph..." -ForegroundColor Cyan

# -----------------------------
# Load dependency graph
# -----------------------------
$depsPath = ".github/ci/ci-deps.json"
if (-not (Test-Path $depsPath)) {
    Write-Error "ci-deps.json not found at $depsPath"
    exit 1
}

$deps = Get-Content $depsPath | ConvertFrom-Json

# -----------------------------
# Load workflow YAML
# -----------------------------
$workflowPath = ".github/workflows/ci.yml"
if (-not (Test-Path $workflowPath)) {
    Write-Error "ci.yml not found at $workflowPath"
    exit 1
}

$workflow = Get-Content $workflowPath -Raw | ConvertFrom-Yaml

if ($null -eq $workflow.jobs) {
    Write-Error "Workflow contains no jobs."
    exit 1
}

$jobs = $workflow.jobs.Keys

# -----------------------------
# 1. Validate job existence
# -----------------------------
foreach ($zone in $deps.PSObject.Properties.Name) {
    if ($VerboseOutput) { Write-Host "Checking job existence for zone: $zone" }

    if ($zone -notin $jobs) {
        Write-Error "Workflow is missing required job: $zone"
        exit 1
    }
}

# -----------------------------
# 2. Validate dependencies match exactly
# -----------------------------
foreach ($zone in $deps.PSObject.Properties.Name) {
    $expectedDeps = $deps.$zone

    $actualDeps = $workflow.jobs.$zone.needs
    if ($null -eq $actualDeps) { $actualDeps = @() }
    if ($actualDeps -isnot [System.Collections.IEnumerable]) {
        $actualDeps = @($actualDeps)
    }

    # Missing dependencies
    $missing = $expectedDeps | Where-Object { $_ -notin $actualDeps }
    if ($missing.Count -gt 0) {
        Write-Error "Job '$zone' is missing required dependencies: $($missing -join ', ')"
        exit 1
    }

    # Extra dependencies
    $extra = $actualDeps | Where-Object { $_ -notin $expectedDeps }
    if ($extra.Count -gt 0) {
        Write-Error "Job '$zone' has extra dependencies not in ci-deps.json: $($extra -join ', ')"
        exit 1
    }
}

# -----------------------------
# 3. Validate path filters exist for determine-changes
# -----------------------------
if ($VerboseOutput) { Write-Host "Validating path filters..." }

if (-not $workflow.jobs."determine-changes".steps) {
    Write-Error "determine-changes job missing steps."
    exit 1
}

$pathsFilterStep = $workflow.jobs."determine-changes".steps |
Where-Object { $_.uses -like "*dorny/paths-filter*" }

if (-not $pathsFilterStep) {
    Write-Error "determine-changes job missing dorny/paths-filter step."
    exit 1
}

# -----------------------------
# 4. Validate nightly full run exists
# -----------------------------
if ($VerboseOutput) { Write-Host "Checking for nightly full run..." }

if (-not $workflow.on.schedule) {
    Write-Error "Workflow missing nightly schedule trigger."
    exit 1
}

$hasNightly = $workflow.on.schedule |
Where-Object { $_.cron -eq "0 9 * * *" }

if (-not $hasNightly) {
    Write-Error "Workflow missing required nightly full run at 09:00 UTC."
    exit 1
}

# -----------------------------
# 5. Validate workflow_dispatch exists
# -----------------------------
if ($VerboseOutput) { Write-Host "Checking for workflow_dispatch..." }

if (-not $workflow.on."workflow_dispatch") {
    Write-Error "Workflow missing workflow_dispatch manual trigger."
    exit 1
}

# -----------------------------
# 6. Validate determine-changes outputs are used
# -----------------------------
foreach ($zone in $deps.PSObject.Properties.Name) {
    foreach ($dep in $deps.$zone) {
        $pattern = "\{\{\s*needs\.determine-changes\.outputs\.$dep\s*\}\}"
        if ($workflow | Out-String -notmatch $pattern) {
            Write-Error "Workflow missing determine-changes output reference for dependency: $zone → $dep"
            exit 1
        }
    }
}

# -----------------------------
# 7. Validate job names match zones
# -----------------------------
foreach ($zone in $deps.PSObject.Properties.Name) {
    if ($VerboseOutput) { Write-Host "Validating job name: $zone" }

    if ($zone -notin $jobs) {
        Write-Error "Job '$zone' does not exist but is defined in ci-deps.json"
        exit 1
    }
}

# -----------------------------
# 8. Validate no extra jobs exist
# -----------------------------
$allowedJobs = @("determine-changes") + $deps.PSObject.Properties.Name

$extraJobs = $jobs | Where-Object { $_ -notin $allowedJobs }
if ($extraJobs.Count -gt 0) {
    Write-Error "Workflow contains extra jobs not defined in ci-deps.json: $($extraJobs -join ', ')"
    exit 1
}

Write-Host "CI dependency graph is valid." -ForegroundColor Green
exit 0
