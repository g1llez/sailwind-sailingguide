param(
    [string]$GameDir = "C:\Program Files (x86)\Steam\steamapps\common\Sailwind"
)

$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot
$ProjectDir = Join-Path $Root "CaptureOrtho"
$ModsDir = Join-Path $GameDir "Mods"
$LocalBin = Join-Path $Root "bin"
$DllName = "CaptureOrtho.dll"
$MelonLoaderDll = Join-Path $GameDir "MelonLoader\net35\MelonLoader.dll"

if (-not (Test-Path $MelonLoaderDll)) {
    throw "MelonLoader not found at $MelonLoaderDll. Install MelonLoader on Sailwind first."
}

dotnet build (Join-Path $ProjectDir "CaptureOrtho.csproj") -c Release -p:GameDir="$GameDir"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

New-Item -ItemType Directory -Force -Path $ModsDir, $LocalBin | Out-Null

$BuiltDll = Join-Path $ProjectDir "bin\$DllName"
if (-not (Test-Path $BuiltDll)) {
    throw "Build output not found: $BuiltDll"
}

try {
    Copy-Item -Force $BuiltDll $ModsDir
    Copy-Item -Force $BuiltDll $LocalBin
} catch {
    Write-Host "DLL copy skipped (close Sailwind to update CaptureOrtho.dll): $($_.Exception.Message)"
    exit 1
}

Write-Host "Installed to $ModsDir"
Write-Host "Config: UserData/MelonPreferences.cfg (section CaptureOrtho)"
Write-Host "Keys: F8 = environnement, F9 = ortho. Screenshots via cinematruc (manual)."
