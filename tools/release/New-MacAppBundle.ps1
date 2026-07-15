[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$PublishedDirectory,

    [Parameter(Mandatory)]
    [string]$BundleOutputDirectory,

    [Parameter(Mandatory)]
    [string]$Version,

    [string]$IconPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force

if (-not $IsMacOS) {
    throw "The .app bundle must be created on macOS to preserve permissions and metadata."
}

$repositoryRoot = Get-RepositoryRoot
if ($Version -cne (Get-AbituriaVersion -RepositoryRoot $repositoryRoot)) {
    throw "Wersja pakietu macOS nie odpowiada Directory.Build.props."
}

$publishedDirectory = (Resolve-Path -LiteralPath $PublishedDirectory).Path
$publishedExecutable = Join-Path $publishedDirectory "Abituria"
if (-not (Test-Path -LiteralPath $publishedExecutable -PathType Leaf)) {
    throw "Publikacja macOS nie zawiera pliku wykonywalnego Abituria."
}

if (-not $IconPath) {
    $IconPath = Join-Path $repositoryRoot "img/icon.ico"
}

if (-not (Test-Path -LiteralPath $IconPath -PathType Leaf)) {
    throw "Source icon not found: $IconPath"
}

$appDirectory = Join-Path $BundleOutputDirectory "Abituria.app"
$contentsDirectory = Join-Path $appDirectory "Contents"
$macOsDirectory = Join-Path $contentsDirectory "MacOS"
$resourcesDirectory = Join-Path $contentsDirectory "Resources"
Reset-Directory -Path $appDirectory
New-Item -ItemType Directory -Path $macOsDirectory, $resourcesDirectory -Force | Out-Null

Copy-Item -Path (Join-Path $publishedDirectory "*") -Destination $macOsDirectory -Recurse -Force
Invoke-ExternalCommand -FilePath "chmod" -ArgumentList @("+x", (Join-Path $macOsDirectory "Abituria"))

$iconWorkDirectory = Join-Path ([IO.Path]::GetTempPath()) ("abituria-icon-" + [guid]::NewGuid().ToString("N"))
try {
    New-Item -ItemType Directory -Path $iconWorkDirectory -Force | Out-Null
    $sourcePng = Join-Path $iconWorkDirectory "source.png"
    Invoke-ExternalCommand -FilePath "sips" -ArgumentList @("-s", "format", "png", $IconPath, "--out", $sourcePng)

    $iconSetDirectory = Join-Path $iconWorkDirectory "Abituria.iconset"
    New-Item -ItemType Directory -Path $iconSetDirectory -Force | Out-Null
    $iconVariants = @(
        @{ Name = "icon_16x16.png"; Size = 16 },
        @{ Name = "icon_16x16@2x.png"; Size = 32 },
        @{ Name = "icon_32x32.png"; Size = 32 },
        @{ Name = "icon_32x32@2x.png"; Size = 64 },
        @{ Name = "icon_128x128.png"; Size = 128 },
        @{ Name = "icon_128x128@2x.png"; Size = 256 },
        @{ Name = "icon_256x256.png"; Size = 256 },
        @{ Name = "icon_256x256@2x.png"; Size = 512 },
        @{ Name = "icon_512x512.png"; Size = 512 },
        @{ Name = "icon_512x512@2x.png"; Size = 1024 }
    )

    foreach ($variant in $iconVariants) {
        $destination = Join-Path $iconSetDirectory $variant.Name
        Copy-Item -LiteralPath $sourcePng -Destination $destination
        Invoke-ExternalCommand -FilePath "sips" -ArgumentList @("-z", $variant.Size, $variant.Size, $destination)
    }

    Invoke-ExternalCommand -FilePath "iconutil" -ArgumentList @(
        "-c", "icns",
        $iconSetDirectory,
        "-o", (Join-Path $resourcesDirectory "Abituria.icns")
    )
}
finally {
    if (Test-Path -LiteralPath $iconWorkDirectory) {
        Remove-TemporaryDirectory -Path $iconWorkDirectory
    }
}

$coreVersion = $Version.Split("-", 2)[0]
$prereleaseNumber = 1
if ($Version -match '-[^.]+\.(\d+)$') {
    $prereleaseNumber = [int]$Matches[1]
}
$coreVersionParts = $coreVersion.Split(".")
$bundleBuildComponent = ([int]$coreVersionParts[2] * 1000) + $prereleaseNumber
$bundleVersion = "$($coreVersionParts[0]).$($coreVersionParts[1]).$bundleBuildComponent"

$infoPlist = @"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "https://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleDevelopmentRegion</key>
  <string>pl</string>
  <key>CFBundleDisplayName</key>
  <string>Abituria</string>
  <key>CFBundleExecutable</key>
  <string>Abituria</string>
  <key>CFBundleIconFile</key>
  <string>Abituria.icns</string>
  <key>CFBundleIdentifier</key>
  <string>io.github.haribo841.abituria</string>
  <key>CFBundleInfoDictionaryVersion</key>
  <string>6.0</string>
  <key>CFBundleName</key>
  <string>Abituria</string>
  <key>CFBundlePackageType</key>
  <string>APPL</string>
  <key>CFBundleShortVersionString</key>
  <string>$coreVersion</string>
  <key>CFBundleVersion</key>
  <string>$bundleVersion</string>
  <key>LSMinimumSystemVersion</key>
  <string>15.0</string>
  <key>NSHighResolutionCapable</key>
  <true/>
  <key>AbituriaReleaseVersion</key>
  <string>$Version</string>
</dict>
</plist>
"@

$utf8WithoutBom = [Text.UTF8Encoding]::new($false)
[IO.File]::WriteAllText((Join-Path $contentsDirectory "Info.plist"), $infoPlist, $utf8WithoutBom)
Invoke-ExternalCommand -FilePath "plutil" -ArgumentList @("-lint", (Join-Path $contentsDirectory "Info.plist"))

Write-Host "Utworzono pakiet macOS: $appDirectory"
$appDirectory
