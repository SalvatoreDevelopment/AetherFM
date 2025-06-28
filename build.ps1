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

# Funzione per creare lo zip del plugin
function Create-PluginZip {
    Write-Host "Creazione ZIP per AetherFM..." -ForegroundColor Green

    # Percorso della DLL compilata
    $dllPath = "bin\Release\net9.0\AetherFM.dll"
    $jsonPath = "AetherFM.json"

    # Verifica che i file esistano
    if (-not (Test-Path $dllPath)) {
        Write-Host "ERRORE: File DLL non trovato in $dllPath" -ForegroundColor Red
        Write-Host "Assicurati di aver compilato il progetto con: dotnet build -c Release" -ForegroundColor Yellow
        exit 1
    }

    if (-not (Test-Path $jsonPath)) {
        Write-Host "ERRORE: File JSON non trovato in $jsonPath" -ForegroundColor Red
        exit 1
    }

    # Nome del file ZIP
    $zipName = "AetherFM.zip"

    # Rimuovi il file ZIP esistente se presente
    if (Test-Path $zipName) {
        Remove-Item $zipName -Force
        Write-Host "File ZIP esistente rimosso." -ForegroundColor Yellow
    }

    try {
        # Crea il file ZIP
        Compress-Archive -Path $dllPath, $jsonPath -DestinationPath $zipName -Force
        
        # Verifica che il file ZIP sia stato creato
        if (Test-Path $zipName) {
            $zipSize = (Get-Item $zipName).Length
            Write-Host "ZIP creato con successo: $zipName" -ForegroundColor Green
            Write-Host "Dimensione: $($zipSize) bytes" -ForegroundColor Green
            Write-Host "File inclusi:" -ForegroundColor Cyan
            Write-Host "  - $dllPath" -ForegroundColor White
            Write-Host "  - $jsonPath" -ForegroundColor White
        } else {
            Write-Host "ERRORE: Il file ZIP non è stato creato" -ForegroundColor Red
            exit 1
        }
    } catch {
        Write-Host "ERRORE durante la creazione dello ZIP: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# Esegui la funzione se richiesto da argomento
if ($args -contains 'zip') {
    Create-PluginZip
}

# Esegui sempre la funzione per debug
Create-PluginZip

Write-Host "Script completato!" -ForegroundColor Green 