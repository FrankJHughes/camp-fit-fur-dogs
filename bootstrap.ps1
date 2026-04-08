#Requires -Version 5.1
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$StartTime = Get-Date

function Write-Header {
    Write-Host ""
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "  Camp Fit Fur Dogs - Local Bootstrap" -ForegroundColor Cyan
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Phase($Message) {
    Write-Host ""
    Write-Host "> $Message" -ForegroundColor Yellow
    Write-Host ""
}

function Assert-Command($Name, $InstallHint) {
    $cmd = Get-Command $Name -ErrorAction SilentlyContinue
    if (-not $cmd) {
        Write-Host "  x $Name is not installed." -ForegroundColor Red
        Write-Host "    $InstallHint"
        exit 1
    }
    $ver = & $Name --version 2>&1 | Select-Object -First 1
    Write-Host "  + $Name -- $ver" -ForegroundColor Green
}

Write-Header

# -- Phase 1: Validate Prerequisites ------------------------------------------
Write-Phase "Phase 1/4 -- Validating prerequisites"

Assert-Command "docker" "Install Docker Desktop: https://www.docker.com/products/docker-desktop/"
Assert-Command "dotnet" "Install .NET SDK: https://dot.net/download"

$null = docker info 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "  x Docker daemon is not running." -ForegroundColor Red
    Write-Host "    Start Docker Desktop and try again."
    exit 1
}
Write-Host "  + Docker daemon is running" -ForegroundColor Green

# -- Phase 2: Start Infrastructure --------------------------------------------
Write-Phase "Phase 2/4 -- Starting infrastructure"

docker compose up -d --wait
if ($LASTEXITCODE -ne 0) {
    Write-Host "  x Infrastructure failed to start." -ForegroundColor Red
    exit 1
}
Write-Host "  + Infrastructure services are healthy" -ForegroundColor Green

# -- Phase 3: Build & Test ----------------------------------------------------
Write-Phase "Phase 3/4 -- Building and testing"

dotnet restore
if ($LASTEXITCODE -ne 0) { Write-Host "  x Restore failed." -ForegroundColor Red; exit 1 }

dotnet build --no-restore --configuration Release
if ($LASTEXITCODE -ne 0) { Write-Host "  x Build failed." -ForegroundColor Red; exit 1 }

dotnet test --no-build --configuration Release --verbosity normal
if ($LASTEXITCODE -ne 0) { Write-Host "  x Tests failed." -ForegroundColor Red; exit 1 }

# -- Phase 4: Readiness Report ------------------------------------------------
$Elapsed = [math]::Round(((Get-Date) - $StartTime).TotalSeconds)

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Readiness Report" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Services"
Write-Host "  PostgreSQL          :5432    +" -ForegroundColor Green
Write-Host "  Redis               :6379    +" -ForegroundColor Green
Write-Host "  RabbitMQ            :5672    +" -ForegroundColor Green
Write-Host "  RabbitMQ Management :15672   +" -ForegroundColor Green
Write-Host ""
Write-Host "  Pipeline"
Write-Host "  Infrastructure      PASS" -ForegroundColor Green
Write-Host "  Build               PASS" -ForegroundColor Green
Write-Host "  Tests               PASS" -ForegroundColor Green
Write-Host ""
Write-Host "  Elapsed             ${Elapsed}s"
Write-Host ""
Write-Host "  Ready to develop!" -ForegroundColor Green
Write-Host ""