# Build BepInEx plugin and install pages — delegates to dev/plugin/
& (Join-Path $PSScriptRoot "dev\plugin\build.ps1") @PSBoundParameters
