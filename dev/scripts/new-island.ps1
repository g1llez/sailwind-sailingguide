<#
.SYNOPSIS
  Scaffold workspace + Assets files for a new archipelago island page.

.EXAMPLE
  .\new-island.ps1 -Archipelago alankh -Page 5 -Slug alnilem -Name "AL'NILEM"

.EXAMPLE
  .\new-island.ps1 -Archipelago alankh -Page 20 -Slug oldankhtown -Name "OLD ANKH TOWN" -Pages 3
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true)]
    [string]$Archipelago,

    [Parameter(Mandatory = $true)]
    [ValidateRange(1, 99)]
    [int]$Page,

    [Parameter(Mandatory = $true)]
    [string]$Slug,

    [Parameter(Mandatory = $true)]
    [string]$Name,

    [ValidateRange(1, 9)]
    [int]$Pages = 1,

    [switch]$Force
)

$ErrorActionPreference = "Stop"

$DevRoot = Split-Path $PSScriptRoot -Parent
$TemplatesDir = Join-Path $DevRoot "Assets\guides\archipelago\templates"
$AssetsPagesDir = Join-Path $DevRoot "Assets\guides\archipelago\pages\$Archipelago"
$islandFolder = '{0:D2}-{1}' -f $Page, $Slug
$WorkspaceIslandDir = Join-Path $DevRoot "_workspace\guide\archipelago\$Archipelago\$islandFolder"

function Assert-Template {
    param([string]$Path, [string]$Label)
    if (-not (Test-Path $Path)) {
        throw "Missing template $Label : $Path"
    }
}

function Write-TextFromTemplate {
    param(
        [string]$TemplatePath,
        [string]$DestPath,
        [hashtable]$Replacements
    )
    if ((Test-Path $DestPath) -and -not $Force) {
        throw "Already exists (use -Force): $DestPath"
    }
    $text = Get-Content -LiteralPath $TemplatePath -Raw -Encoding UTF8
    foreach ($key in $Replacements.Keys) {
        $text = $text.Replace($key, $Replacements[$key])
    }
    $parent = Split-Path $DestPath -Parent
    if (-not (Test-Path $parent)) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }
    if ($PSCmdlet.ShouldProcess($DestPath, "Write text from template")) {
        [System.IO.File]::WriteAllText($DestPath, $text, [System.Text.UTF8Encoding]::new($false))
    }
}

function Copy-SvgFromTemplate {
    param(
        [string]$TemplatePath,
        [string]$DestPath,
        [string]$DocName
    )
    if ((Test-Path $DestPath) -and -not $Force) {
        throw "Already exists (use -Force): $DestPath"
    }
    $parent = Split-Path $DestPath -Parent
    if (-not (Test-Path $parent)) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }
    if (-not $PSCmdlet.ShouldProcess($DestPath, "Copy SVG template")) {
        return
    }
    $text = Get-Content -LiteralPath $TemplatePath -Raw -Encoding UTF8
    $text = $text -replace 'sodipodi:docname="[^"]*"', "sodipodi:docname=`"$DocName`""
    $exportPath = ($DestPath -replace '\\', '/') -replace '/Assets/guides/archipelago/pages/', '/pages/'
    $exportPath = $exportPath -replace '\.svg$', '.png'
    $text = $text -replace 'inkscape:export-filename="[^"]*"', "inkscape:export-filename=`"$exportPath`""
    [System.IO.File]::WriteAllText($DestPath, $text, [System.Text.UTF8Encoding]::new($false))
}

function New-WorkspaceDir {
    param([string]$Path)
    if ($PSCmdlet.ShouldProcess($Path, "Create directory")) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Copy-OptionalBinary {
    param(
        [string]$TemplatePath,
        [string]$DestPath,
        [string]$Label
    )
    if (-not (Test-Path $TemplatePath)) {
        Write-Warning "Skip $Label - template not found: $TemplatePath"
        return
    }
    if ((Test-Path $DestPath) -and -not $Force) {
        throw "Already exists (use -Force): $DestPath"
    }
    $parent = Split-Path $DestPath -Parent
    if (-not (Test-Path $parent)) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }
    if ($PSCmdlet.ShouldProcess($DestPath, "Copy $Label")) {
        Copy-Item -LiteralPath $TemplatePath -Destination $DestPath
    }
}

# --- templates (tracked in dev/Assets) ---
$ficheTemplate = Join-Path $TemplatesDir "template_fiche_ile.txt"
$texteTemplate = Join-Path $TemplatesDir "template_texte.txt"
$mapSvgTemplate = Join-Path $TemplatesDir "template_map.svg"
$page1SvgTemplate = Join-Path $TemplatesDir "template_page1-info.svg"
$page2SvgTemplate = Join-Path $TemplatesDir "template_page2-nav.svg"
$mapXcfTemplate = Join-Path $TemplatesDir "template_map.xcf"

Assert-Template $ficheTemplate "fiche"
Assert-Template $texteTemplate "texte parchemin"
Assert-Template $mapSvgTemplate "map inkscape"
Assert-Template $page1SvgTemplate "page inkscape (info)"
if ($Pages -gt 1) {
    Assert-Template $page2SvgTemplate "page inkscape (nav)"
}

$replaceName = @{ "ISLAND NAME" = $Name }

# --- workspace island tree ---
$dirs = @(
    $WorkspaceIslandDir,
    (Join-Path $WorkspaceIslandDir "ambiance"),
    (Join-Path $WorkspaceIslandDir "maps"),
    (Join-Path $WorkspaceIslandDir "maps\ortho"),
    (Join-Path $WorkspaceIslandDir "maps\masks"),
    (Join-Path $WorkspaceIslandDir "maps\svg")
)
foreach ($dir in $dirs) {
    New-WorkspaceDir $dir
}

Write-TextFromTemplate $ficheTemplate (Join-Path $WorkspaceIslandDir "fiche.txt") $replaceName

$workspaceMapSvg = Join-Path $WorkspaceIslandDir "maps\svg\$Slug`_v0.1.svg"
Copy-SvgFromTemplate $mapSvgTemplate $workspaceMapSvg "$Slug`_v0.1.svg"

Copy-OptionalBinary $mapXcfTemplate (Join-Path $WorkspaceIslandDir "maps\map.xcf") "GIMP map.xcf"

# --- Assets pages (parchemin + inkscape masters) ---
New-WorkspaceDir $AssetsPagesDir

for ($i = 0; $i -lt $Pages; $i++) {
    $pageNum = $Page + $i
    $sheet = $i + 1

    if ($Pages -eq 1) {
        $baseName = "{0:D2}-{1}" -f $pageNum, $Slug
    }
    else {
        $baseName = "{0:D2}-{1}-{2}" -f $pageNum, $Slug, $sheet
    }

    $txtDest = Join-Path $AssetsPagesDir "$baseName.txt"
    $svgDest = Join-Path $AssetsPagesDir "$baseName.svg"
    $svgTemplate = if ($i -eq 0) { $page1SvgTemplate } else { $page2SvgTemplate }

    Write-TextFromTemplate $texteTemplate $txtDest $replaceName
    Copy-SvgFromTemplate $svgTemplate $svgDest "$baseName.svg"
}

Write-Host "Workspace: $WorkspaceIslandDir"
Write-Host "Assets:    $AssetsPagesDir"
if ($Pages -eq 1) {
    Write-Host ('Pages:     {0:D2}-{1}' -f $Page, $Slug)
}
else {
    $range = ($Page)..($Page + $Pages - 1)
    Write-Host ('Pages:     {0} - {1} sheet(s)' -f ($range -join ', '), $Pages)
}
