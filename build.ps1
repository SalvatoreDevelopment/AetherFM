# Build script for AetherFM
# Usage:
#   .\build.ps1 -deploy-local     # Deploys to devPlugins for local testing
#   .\build.ps1 -switch-to-repo   # Removes devPlugins/AetherFM to test repo version
#   .\build.ps1 -zip              # Creates the release ZIP
#
# Only one version (local or repo) should be active at a time!

param(
    [string]$Configuration = "Debug",
    [switch]$Clean,
    [switch]$DeployLocal,
    [switch]$SwitchToRepo,
    [switch]$Zip
)

Write-Host "=== Build Script for AetherFM ===" -ForegroundColor Green

# Correct path for Dalamud dev folder
$DalamudDevPath = "$env:APPDATA\XIVLauncher\devPlugins\AetherFM"

# Clean if requested
if ($Clean) {
    Write-Host "Cleaning build files..." -ForegroundColor Yellow
    dotnet clean
    if (Test-Path $DalamudDevPath) {
        Remove-Item $DalamudDevPath -Recurse -Force
        Write-Host "Development folder cleaned." -ForegroundColor Green
    }
}

# Deploy local version for testing
if ($DeployLocal) {
    Write-Host "Deploying local build to devPlugins..." -ForegroundColor Yellow
    if (!(Test-Path $DalamudDevPath)) {
        New-Item -ItemType Directory -Path $DalamudDevPath -Force
    }
    $possiblePaths = @("bin\$Configuration\net9.0", "bin\$Configuration")
    $filesToCopy = @(
        "AetherFM.dll", "AetherFM.pdb", "AetherFM.json",
        "NAudio.dll", "NAudio.Core.dll", "NAudio.WinMM.dll", "NAudio.Wasapi.dll", "NAudio.Midi.dll", "NAudio.Asio.dll"
    )
    foreach ($file in $filesToCopy) {
        $found = $false
        foreach ($sourcePath in $possiblePaths) {
            $sourceFile = Join-Path $sourcePath $file
            if (Test-Path $sourceFile) {
                $destFile = Join-Path $DalamudDevPath $file
                try {
                    Copy-Item $sourceFile $destFile -Force
                    Write-Host "Copied: $file from $sourcePath" -ForegroundColor Cyan
                    $found = $true
                    break
                } catch {
                    $errMsg = $_.Exception.Message
                    Write-Host ("Error copying {0}: {1}" -f $file, $errMsg) -ForegroundColor Red
                }
            }
        }
        if (-not $found) {
            Write-Host "File not found: $file" -ForegroundColor Yellow
        }
    }
    $naudioDlls = Get-ChildItem -Path "bin\$Configuration\net9.0" -Filter "NAudio*.dll" -File
    foreach ($dll in $naudioDlls) {
        $destFile = Join-Path $DalamudDevPath $dll.Name
        try {
            Copy-Item $dll.FullName $destFile -Force
            Write-Host "Force copied: $($dll.Name)" -ForegroundColor Cyan
        } catch {
            $errMsg = $_.Exception.Message
            Write-Host ("Error force copying {0}: {1}" -f $dll.Name, $errMsg) -ForegroundColor Red
        }
    }
    Write-Host "Local deploy completed! Restart XIVLauncher to test local version." -ForegroundColor Green
    exit 0
}

# Switch to repo version (remove devPlugins/AetherFM)
if ($SwitchToRepo) {
    if (Test-Path $DalamudDevPath) {
        Remove-Item $DalamudDevPath -Recurse -Force
        Write-Host "devPlugins/AetherFM removed. Now you can test the repository version!" -ForegroundColor Green
    } else {
        Write-Host "devPlugins/AetherFM not found. Already ready for repo version." -ForegroundColor Yellow
    }
    exit 0
}

# Build the project
Write-Host "Building the project..." -ForegroundColor Yellow
$buildResult = dotnet build --configuration $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build error!" -ForegroundColor Red
    exit 1
}

Write-Host "Build completed successfully!" -ForegroundColor Green

# Create ZIP for release if requested
function Create-PluginZip {
    Write-Host "Creating ZIP for AetherFM..." -ForegroundColor Green

    # Paths
    $dllPathNet = "bin\Release\net9.0\AetherFM.dll"
    $dllPathRoot = "bin\Release\AetherFM.dll"
    $pdbPathNet = "bin\Release\net9.0\AetherFM.pdb"
    $pdbPathRoot = "bin\Release\AetherFM.pdb"
    $jsonPath = "AetherFM.json"
    $naudioPathsNet = @(
        "bin\Release\net9.0\NAudio.dll",
        "bin\Release\net9.0\NAudio.Core.dll",
        "bin\Release\net9.0\NAudio.WinMM.dll",
        "bin\Release\net9.0\NAudio.Wasapi.dll",
        "bin\Release\net9.0\NAudio.Midi.dll",
        "bin\Release\net9.0\NAudio.Asio.dll"
    )
    $naudioPathsRoot = @(
        "bin\Release\NAudio.dll",
        "bin\Release\NAudio.Core.dll",
        "bin\Release\NAudio.WinMM.dll",
        "bin\Release\NAudio.Wasapi.dll",
        "bin\Release\NAudio.Midi.dll",
        "bin\Release\NAudio.Asio.dll"
    )
    $binDirNet = "bin\Release\net9.0"
    $binDirRoot = "bin\Release"

    # Copy JSON file to output folder
    if (Test-Path $jsonPath) {
        Copy-Item $jsonPath -Destination $binDirNet -Force -ErrorAction SilentlyContinue
        Copy-Item $jsonPath -Destination $binDirRoot -Force -ErrorAction SilentlyContinue
        Write-Host "AetherFM.json copied to $binDirNet and $binDirRoot" -ForegroundColor Cyan
    } else {
        Write-Host "ERROR: AetherFM.json file not found!" -ForegroundColor Red
    }

    # Check that files exist (prefer net9.0, fallback to root)
    $dllPath = if (Test-Path $dllPathNet) { $dllPathNet } elseif (Test-Path $dllPathRoot) { $dllPathRoot } else { $null }
    $pdbPath = if (Test-Path $pdbPathNet) { $pdbPathNet } elseif (Test-Path $pdbPathRoot) { $pdbPathRoot } else { $null }
    $naudioPaths = @()
    foreach ($p in $naudioPathsNet) { if (Test-Path $p) { $naudioPaths += $p } }
    foreach ($p in $naudioPathsRoot) { if (Test-Path $p) { $naudioPaths += $p } }

    if (-not $dllPath) {
        Write-Host "ERROR: DLL file not found in $dllPathNet or $dllPathRoot" -ForegroundColor Red
        Write-Host "Make sure you have built the project with: dotnet build -c Release" -ForegroundColor Yellow
        exit 1
    }

    if (-not (Test-Path $jsonPath)) {
        Write-Host "ERROR: JSON file not found in $jsonPath" -ForegroundColor Red
        exit 1
    }

    # ZIP file name
    $zipName = "AetherFM.zip"

    # Remove existing ZIP file if present
    if (Test-Path $zipName) {
        Remove-Item $zipName -Force
        Write-Host "Existing ZIP file removed." -ForegroundColor Yellow
    }

    $filesToZip = @($dllPath, $pdbPath, $jsonPath) + $naudioPaths | Where-Object { $_ -ne $null }

    try {
        # Create the ZIP file
        Compress-Archive -Path $filesToZip -DestinationPath $zipName -Force
        
        # Check that the ZIP file was created
        if (Test-Path $zipName) {
            $zipSize = (Get-Item $zipName).Length
            Write-Host "ZIP created successfully: $zipName" -ForegroundColor Green
            Write-Host "Size: $($zipSize) bytes" -ForegroundColor Green
            Write-Host "Included files:" -ForegroundColor Cyan
            foreach ($f in $filesToZip) { Write-Host "  - $f" -ForegroundColor White }
        } else {
            Write-Host "ERROR: ZIP file was not created" -ForegroundColor Red
            exit 1
        }
    } catch {
        $errMsg = $_.Exception.Message
        Write-Host ("ERROR creating ZIP: {0}" -f $errMsg) -ForegroundColor Red
        exit 1
    }
}

# Run the function if requested by argument
if ($Zip) {
    Create-PluginZip
    exit 0
}

# Always run the function for debug
Create-PluginZip

Write-Host "Script completed!" -ForegroundColor Green 