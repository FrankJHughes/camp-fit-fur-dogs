param(
  [string]$Solution = "CampFitFurDogs.slnx"
)

# Ensure we are at repo root
if (-not (Test-Path $Solution)) {
  Write-Error "Solution file '$Solution' not found at repo root."
  exit 1
}

# Create src projects
$srcProjects = @(
  "Frank.Core.Application"
)

foreach ($proj in $srcProjects) {
  $projPath = "src/$proj"
  $csproj = "$projPath/$proj.csproj"

  if (-not (Test-Path $projPath)) {
    New-Item -ItemType Directory -Path $projPath | Out-Null
  }

  dotnet new classlib -n $proj -o $projPath --framework net10.0
  dotnet sln $Solution add $csproj
}

# Create test projects
$testProjects = @(
  "Frank.Core.Application.Tests"
)

foreach ($proj in $testProjects) {
  $projPath = "tests/$proj"
  $csproj = "$projPath/$proj.csproj"

  if (-not (Test-Path $projPath)) {
    New-Item -ItemType Directory -Path $projPath | Out-Null
  }

  dotnet new xunit -n $proj -o $projPath --framework net10.0
  dotnet sln $Solution add $csproj
}
