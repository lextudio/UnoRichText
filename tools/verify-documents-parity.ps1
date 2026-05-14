param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path,
    [ValidateSet("core", "full")]
    [string]$Mode = "core",
    [string[]]$RequireCompiled = @()
)

$ErrorActionPreference = "Stop"

$sourceMapPath = Join-Path $RepoRoot "docs/SOURCE-MAP.md"
$provenancePath = Join-Path $RepoRoot "docs/PROVENANCE.md"
$projectPath = Join-Path $RepoRoot "src/LeXtudio.RichText/LeXtudio.RichText.csproj"
$wpfRoot = Resolve-Path (Join-Path $RepoRoot "..\wpf\src\Microsoft.DotNet.Wpf\src\PresentationFramework\System\Windows\Documents")

if (!(Test-Path $sourceMapPath)) { throw "Missing SOURCE-MAP.md at $sourceMapPath" }
if (!(Test-Path $provenancePath)) { throw "Missing PROVENANCE.md at $provenancePath" }
if (!(Test-Path $projectPath)) { throw "Missing project file at $projectPath" }

$sourceMap = Get-Content $sourceMapPath -Raw
$provenance = Get-Content $provenancePath -Raw
$project = Get-Content $projectPath -Raw

$allWpfFiles = Get-ChildItem -Path $wpfRoot -File | Select-Object -ExpandProperty Name
$errors = New-Object System.Collections.Generic.List[string]

if ($Mode -eq "full") {
    $wpfFiles = $allWpfFiles
}
else {
    # Core mode validates only files currently tracked in SOURCE-MAP.
    $wpfFiles = @()
    foreach ($f in $allWpfFiles) {
        if ($sourceMap -match [regex]::Escape($f)) {
            $wpfFiles += $f
        }
    }
}

foreach ($file in $wpfFiles) {
    if ($sourceMap -notmatch [regex]::Escape($file)) {
        $errors.Add("SOURCE-MAP missing WPF file entry: $file")
    }

    $linkedPattern = "UpstreamWpf\\Documents\\$([regex]::Escape($file))"
    $compiledPattern = "System\.Windows\.Documents\\$([regex]::Escape($file))"
    if ($project -notmatch $linkedPattern -and $project -notmatch $compiledPattern) {
        $errors.Add("Project link missing for WPF file: $file")
    }
}

foreach ($file in $RequireCompiled) {
    $compilePattern = "<Compile\s+Include=.*System\\Windows\\Documents\\$([regex]::Escape($file))"
    if ($project -notmatch $compilePattern) {
        $errors.Add("Project compile link missing for WPF file: $file")
    }
}

foreach ($file in $wpfFiles) {
    if ($provenance -notmatch [regex]::Escape("System/Windows/Documents/$file")) {
        $errors.Add("PROVENANCE missing entry for upstream file: $file")
    }
}

if ($errors.Count -gt 0) {
    Write-Host "Parity verification failed:" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host " - $_" -ForegroundColor Red }
    exit 1
}

Write-Host "Parity verification passed." -ForegroundColor Green
Write-Host "Mode: $Mode"
Write-Host "Checked files: $($wpfFiles.Count)"
