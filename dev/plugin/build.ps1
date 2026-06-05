param(
    [string]$GameDir = "C:\Program Files (x86)\Steam\steamapps\common\Sailwind"
)

$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot
$RepoRoot = (Resolve-Path (Join-Path $Root "..\..")).Path
$ProjectDir = Join-Path $Root "SailingGuide"
$PluginDir = Join-Path $GameDir "BepInEx\plugins\SailingGuide"
$LocalBin = Join-Path $Root "bin"
$PagesDst = Join-Path $PluginDir "pages"
$DllName = "SailingGuide.dll"
$ContentPages = Join-Path $RepoRoot "pages"

dotnet build (Join-Path $ProjectDir "SailingGuide.csproj") -c Release -p:GameDir="$GameDir"

New-Item -ItemType Directory -Force -Path $PluginDir, $LocalBin, $PagesDst | Out-Null

$BuiltDll = Join-Path $ProjectDir "bin\$DllName"
try {
    Copy-Item -Force $BuiltDll $PluginDir
    Copy-Item -Force $BuiltDll $LocalBin
} catch {
    Write-Host "DLL copy skipped (close Sailwind to update SailingGuide.dll): $($_.Exception.Message)"
}

if (Test-Path $ContentPages) {
    Get-ChildItem $ContentPages -Directory -ErrorAction SilentlyContinue | ForEach-Object {
        $langDst = Join-Path $PagesDst $_.Name
        New-Item -ItemType Directory -Force -Path $langDst | Out-Null
        Copy-Item -Path (Join-Path $_.FullName "*") -Destination $langDst -Recurse -Force
    }
    Copy-Item -Force (Join-Path $ContentPages "*.png") $PagesDst -ErrorAction SilentlyContinue
    Get-ChildItem $PagesDst -File -Filter "Al'Ankh_*.png" -ErrorAction SilentlyContinue | Remove-Item -Force
    Write-Host "Copied pages from: $ContentPages (en/, fr/, …)"
} else {
    Write-Host "No content pages folder: $ContentPages"
}

$HackCfg = Join-Path $GameDir "BepInEx\config\gillez.sailwindhack.cfg"
if (Test-Path $HackCfg) {
    $cfg = Get-Content $HackCfg -Raw
    if ($cfg -match 'ReplaceTutorialScroll\s*=\s*true') {
        Write-Host "Tip: set gillez.sailwindhack.cfg [CustomScroll] ReplaceTutorialScroll = false (use SailingGuide instead)."
    }
}

Write-Host "Installed to $PluginDir"
Write-Host "Pages: $PagesDst"
Write-Host "Config: BepInEx\config\gillez.sailingguide.cfg"
