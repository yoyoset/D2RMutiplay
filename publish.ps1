$ErrorActionPreference = "Stop"
$ProgressPreference = 'SilentlyContinue'

$version = "v0.5.5"
$outputDir = "ReleaseOutput"

# 1. Clean
if (Test-Path $outputDir) {
    Remove-Item $outputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $outputDir | Out-Null

Write-Host "Cleaning done."

# 2. Build Green (Folder-based Self-Contained)
Write-Host "Building Green version..."
$greenDir = "$outputDir/D2RMultiplay_${version}_Green"
dotnet publish src/D2RMultiplay.UI/D2RMultiplay.UI.csproj -c Release -r win-x64 --self-contained true --no-restore -o $greenDir /p:PublishSingleFile=false
dotnet build-server shutdown
Start-Sleep -Seconds 5
Compress-Archive -Path "$greenDir/*" -DestinationPath "$outputDir/D2RMultiplay_${version}_Green.zip"

# 3. Build Portable (Single-File Self-Contained)
Write-Host "Building Portable version..."
$portableDir = "$outputDir/D2RMultiplay_${version}_Portable_x64"
dotnet publish src/D2RMultiplay.UI/D2RMultiplay.UI.csproj -c Release -r win-x64 --self-contained true --no-restore -o $portableDir /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
dotnet build-server shutdown
Start-Sleep -Seconds 5
Compress-Archive -Path "$portableDir/*" -DestinationPath "$outputDir/D2RMultiplay_${version}_Portable.zip"

# 4. Build Dependent (Framework-Dependent)
Write-Host "Building Dependent version..."
$dependentDir = "$outputDir/D2RMultiplay_${version}_Dependent"
dotnet publish src/D2RMultiplay.UI/D2RMultiplay.UI.csproj -c Release -r win-x64 --self-contained false --no-restore -o $dependentDir
dotnet build-server shutdown
Start-Sleep -Seconds 5
Compress-Archive -Path "$dependentDir/*" -DestinationPath "$outputDir/D2RMultiplay_${version}.zip"

Write-Host "Packaging complete. Check $outputDir for zip files."
