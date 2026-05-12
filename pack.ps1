Param(
    [string]$OutDir = ".\dist",
    [string]$Configuration = "Release",
    [string[]]$Projects = @(
        "src\LeXtudio.RichText\LeXtudio.RichText.csproj"
    )
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$RepoRoot = $PSScriptRoot
$BuildRoot = Join-Path $RepoRoot ".build_out"
$PackageStaging = Join-Path $RepoRoot ".pkg_staging"

function Find-MSBuild {
    $programFilesX86 = [Environment]::GetEnvironmentVariable("ProgramFiles(x86)")
    $vswhere = if ($programFilesX86) {
        Join-Path $programFilesX86 "Microsoft Visual Studio\Installer\vswhere.exe"
    }

    if ($vswhere -and (Test-Path $vswhere)) {
        $installPath = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath 2>$null
        if ($installPath) {
            $candidate = Join-Path $installPath "MSBuild\Current\Bin\MSBuild.exe"
            if (Test-Path $candidate) {
                return (Resolve-Path $candidate).Path
            }
        }
    }

    $command = Get-Command msbuild -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Path
    }

    throw "MSBuild was not found. Install Visual Studio/MSBuild to pack the project."
}

function Resolve-ProjectPath([string]$Project) {
    if (Test-Path $Project) {
        return (Resolve-Path $Project).Path
    }

    $candidate = Join-Path $RepoRoot $Project
    if (Test-Path $candidate) {
        return (Resolve-Path $candidate).Path
    }

    throw "Project file not found: $Project"
}

function Reset-Directory([string]$Path) {
    if (Test-Path $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }

    New-Item -ItemType Directory -Path $Path -Force | Out-Null
}

function Invoke-PackProject([string]$MSBuild, [string]$ProjectPath) {
    $projectName = [IO.Path]::GetFileNameWithoutExtension($ProjectPath)
    $projectOutput = Join-Path $BuildRoot $projectName
    Reset-Directory $projectOutput

    $arguments = @(
        $ProjectPath,
        "/restore",
        "/t:Pack",
        "/p:Configuration=$Configuration",
        "/p:BaseOutputPath=$projectOutput\",
        "/p:PackageOutputPath=$PackageStaging",
        "/v:minimal",
        "/nologo"
    )

    Write-Host ""
    Write-Host "Packing $ProjectPath"
    Write-Host "  Output:   $projectOutput"
    Write-Host "  Packages: $PackageStaging"

    $previousOutDir = [Environment]::GetEnvironmentVariable("OutDir", "Process")
    [Environment]::SetEnvironmentVariable("OutDir", $null, "Process")
    try {
        & $MSBuild @arguments
        if ($LASTEXITCODE -ne 0) {
            throw "MSBuild pack failed for '$ProjectPath' with exit code $LASTEXITCODE."
        }
    } finally {
        [Environment]::SetEnvironmentVariable("OutDir", $previousOutDir, "Process")
    }
}

function Copy-PackagesToOutput([string]$Source, [string]$Destination) {
    $packages = @(
        Get-ChildItem -Path $Source -File |
            Where-Object { $_.Extension -eq ".nupkg" -or $_.Extension -eq ".snupkg" } |
            Sort-Object Name
    )

    if ($packages.Count -eq 0) {
        throw "No .nupkg or .snupkg files were produced in '$Source'."
    }

    Write-Host ""
    Write-Host "Copying packages to $Destination"
    foreach ($package in $packages) {
        Copy-Item -LiteralPath $package.FullName -Destination $Destination -Force
        Write-Host "  $($package.Name)"
    }

    $nonPackages = @(
        Get-ChildItem -Path $Destination -File |
            Where-Object { $_.Extension -ne ".nupkg" -and $_.Extension -ne ".snupkg" }
    )
    if ($nonPackages.Count -gt 0) {
        throw "Non-package files found in output directory: $($nonPackages.FullName -join ', ')"
    }
}

try {
    $msbuild = Find-MSBuild
    $outputPath = if ([IO.Path]::IsPathRooted($OutDir)) { $OutDir } else { Join-Path (Get-Location) $OutDir }
    $outputPath = [IO.Path]::GetFullPath($outputPath)

    Write-Host "MSBuild: $msbuild"
    Write-Host "Configuration: $Configuration"
    Write-Host "Output directory: $outputPath"

    Reset-Directory $BuildRoot
    Reset-Directory $PackageStaging
    Reset-Directory $outputPath

    foreach ($project in $Projects) {
        Invoke-PackProject $msbuild (Resolve-ProjectPath $project)
    }

    Copy-PackagesToOutput $PackageStaging $outputPath

    Write-Host ""
    Write-Host "Packing complete." -ForegroundColor Green
    Get-ChildItem -Path $outputPath -File |
        Sort-Object Name |
        ForEach-Object { Write-Host "  $($_.Name)" -ForegroundColor Green }
} finally {
    if (Test-Path $BuildRoot) {
        Remove-Item -LiteralPath $BuildRoot -Recurse -Force -ErrorAction SilentlyContinue
    }
    if (Test-Path $PackageStaging) {
        Remove-Item -LiteralPath $PackageStaging -Recurse -Force -ErrorAction SilentlyContinue
    }
}
