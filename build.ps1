# Script di build per AetherFM
# Esegui questo script per compilare e copiare il plugin nella cartella di sviluppo

param(
    [string]$Configuration = "Debug",
    [switch]$Clean,
    [switch]$Deploy
)

Write-Host "=== Build Script per AetherFM ===" -ForegroundColor Green

# Percorso della cartella di sviluppo Dalamud
$DalamudDevPath = "$env:APPDATA\XIVLauncher\addon\Hooks\dev\Plugins\AetherFM"

# Pulizia se richiesto
if ($Clean) {
    Write-Host "Pulizia dei file di build..." -ForegroundColor Yellow
    dotnet clean
    if (Test-Path $DalamudDevPath) {
        Remove-Item $DalamudDevPath -Recurse -Force
        Write-Host "Cartella di sviluppo pulita." -ForegroundColor Green
    }
}

# Build del progetto
Write-Host "Compilazione del progetto..." -ForegroundColor Yellow
$buildResult = dotnet build --configuration $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Host "Errore durante la compilazione!" -ForegroundColor Red
    exit 1
}

Write-Host "Compilazione completata con successo!" -ForegroundColor Green

# Deploy se richiesto
if ($Deploy) {
    Write-Host "Deploy nella cartella di sviluppo..." -ForegroundColor Yellow
    
    # Crea la cartella se non esiste
    if (!(Test-Path $DalamudDevPath)) {
        New-Item -ItemType Directory -Path $DalamudDevPath -Force
    }
    
    # Copia i file necessari
    $possiblePaths = @("bin\$Configuration\net7.0", "bin\$Configuration")
    $filesToCopy = @("AetherFM.dll", "AetherFM.pdb", "AetherFM.json")
    
    foreach ($file in $filesToCopy) {
        $found = $false
        foreach ($sourcePath in $possiblePaths) {
            $sourceFile = Join-Path $sourcePath $file
            if (Test-Path $sourceFile) {
                $destFile = Join-Path $DalamudDevPath $file
                Copy-Item $sourceFile $destFile -Force
                Write-Host "Copiato: $file da $sourcePath" -ForegroundColor Cyan
                $found = $true
                break
            }
        }
        if (-not $found) {
            Write-Host "File non trovato: $file" -ForegroundColor Yellow
        }
    }
    
    Write-Host "Deploy completato! Il plugin è pronto per il test." -ForegroundColor Green
    Write-Host "Riavvia XIVLauncher per caricare le modifiche." -ForegroundColor Cyan
}

Write-Host "Script completato!" -ForegroundColor Green 