param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
)

$ErrorActionPreference = "Stop"

$docsDir = Join-Path $RepoRoot "docs"
$inventoryPath = Join-Path $docsDir "NAMESPACE-INVENTORY.md"
$sourceMapPath = Join-Path $docsDir "SOURCE-MAP.md"
$provenancePath = Join-Path $docsDir "PROVENANCE.md"
$projectPath = Join-Path $RepoRoot "src\LeXtudio.RichText\LeXtudio.RichText.csproj"

if (!(Test-Path $inventoryPath)) { throw "Missing $inventoryPath" }
if (!(Test-Path $sourceMapPath)) { throw "Missing $sourceMapPath" }
if (!(Test-Path $provenancePath)) { throw "Missing $provenancePath" }
if (!(Test-Path $projectPath)) { throw "Missing $projectPath" }

$inventoryFiles = Get-Content $inventoryPath |
    Where-Object { $_.StartsWith('- `') -and $_.EndsWith('.cs`') } |
    ForEach-Object { $_.Substring(3, $_.Length - 4) }

$sourceMap = Get-Content $sourceMapPath -Raw
$provenance = Get-Content $provenancePath -Raw
$project = Get-Content $projectPath -Raw

$commitHash = (git -C (Join-Path $RepoRoot "..\wpf") rev-parse HEAD).Trim()

$sourceMapRows = New-Object System.Collections.Generic.List[string]
$provenanceRows = New-Object System.Collections.Generic.List[string]
$projectRows = New-Object System.Collections.Generic.List[string]

foreach ($file in $inventoryFiles) {
    $typeName = [System.IO.Path]::GetFileNameWithoutExtension($file)

    $hasSourceMap = $sourceMap.Contains("| ``$typeName`` |") -or $sourceMap.Contains("| `$typeName` |") -or $sourceMap.Contains("| $typeName |")
    if (-not $hasSourceMap) {
        $sourceMapRows.Add("| ``$typeName`` | _TBD_ | ``$file`` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |")
    }

    $hasProvenance = $provenance.Contains("| ``$typeName`` |") -or $provenance.Contains("| `$typeName` |") -or $provenance.Contains("| $typeName |")
    if (-not $hasProvenance) {
        $provenanceRows.Add("| ``$typeName`` | _TBD_ | ``System/Windows/Documents/$file`` | ``$commitHash`` | Planned | Generated placeholder entry. |")
    }

    $link = "UpstreamWpf\Documents\$file"
    if ($project -notmatch [regex]::Escape($link)) {
        $projectRows.Add("    <None Include=`"..\..\wpf\src\Microsoft.DotNet.Wpf\src\PresentationFramework\System\Windows\Documents\$file`"")
        $projectRows.Add("          Link=`"$link`" />")
    }
}

if ($sourceMapRows.Count -gt 0) {
    Add-Content -Path $sourceMapPath -Value ""
    Add-Content -Path $sourceMapPath -Value "## Backlog (Auto-generated)"
    $sourceMapRows | Add-Content -Path $sourceMapPath
}

if ($provenanceRows.Count -gt 0) {
    Add-Content -Path $provenancePath -Value ""
    Add-Content -Path $provenancePath -Value "## Backlog (Auto-generated)"
    $provenanceRows | Add-Content -Path $provenancePath
}

if ($projectRows.Count -gt 0) {
    $insertion = "  <ItemGroup>`r`n" + (($projectRows -join "`r`n") + "`r`n") + "  </ItemGroup>`r`n"
    $project = [regex]::Replace($project, "</Project>\s*$", "$insertion</Project>")
    Set-Content -Path $projectPath -Value $project -NoNewline
}

Write-Host "Bootstrap complete."
Write-Host "Added SOURCE-MAP rows: $($sourceMapRows.Count)"
Write-Host "Added PROVENANCE rows: $($provenanceRows.Count)"
Write-Host "Added csproj links: $([int]($projectRows.Count / 2))"
