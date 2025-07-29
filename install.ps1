param(
    [switch]$Force
)

Write-Host "Unity Project Initializer - Windows Installation Script" -ForegroundColor Green
Write-Host "=======================================================" -ForegroundColor Green

$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $isAdmin) {
    Write-Host "Warning: Running without Administrator privileges. Installation may fail." -ForegroundColor Yellow
}

try {
    $dotnetVersion = & dotnet --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet command failed"
    }
    Write-Host "Found .NET version: $dotnetVersion" -ForegroundColor Cyan
} catch {
    Write-Host "Error: .NET is not installed or not in PATH. Please install .NET 9.0+" -ForegroundColor Red
    exit 1
}

$cliProjectDir = "cll\UnityInitializer.Cli"
if (-not (Test-Path $cliProjectDir)) {
    Write-Host "Error: CLI project directory not found: $cliProjectDir" -ForegroundColor Red
    exit 1
}

Write-Host "Building Unity Initializer CLI in Release mode..." -ForegroundColor Yellow
Push-Location $cliProjectDir

try {
    & dotnet clean --configuration Release | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet clean failed"
    }

    & dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed"
    }

    $releaseDir = "bin\Release\net9.0"
    if (-not (Test-Path $releaseDir)) {
        throw "Release build failed - directory not found: $releaseDir"
    }

    if (-not (Test-Path "$releaseDir\unity-init.exe")) {
        throw "unity-init.exe not found in $releaseDir"
    }

    Write-Host "Build completed successfully!" -ForegroundColor Green

    $installDir = "$env:ProgramFiles\UnityInit"
    Write-Host "Creating installation directory: $installDir" -ForegroundColor Yellow
    
    if (Test-Path $installDir) {
        if ($Force) {
            Remove-Item $installDir -Recurse -Force
        } else {
            Write-Host "Installation directory already exists. Use -Force to overwrite." -ForegroundColor Yellow
        }
    }
    
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null

    Write-Host "Copying files to $installDir..." -ForegroundColor Yellow
    Copy-Item "$releaseDir\*" $installDir -Recurse -Force

    $currentPath = [Environment]::GetEnvironmentVariable("PATH", "Machine")
    if ($currentPath -notlike "*$installDir*") {
        Write-Host "Adding to system PATH..." -ForegroundColor Yellow
        $newPath = "$currentPath;$installDir"
        [Environment]::SetEnvironmentVariable("PATH", $newPath, "Machine")
        
        $env:PATH = "$env:PATH;$installDir"
        Write-Host "Added $installDir to system PATH" -ForegroundColor Green
    } else {
        Write-Host "Installation directory already in PATH" -ForegroundColor Cyan
    }

    Write-Host "Verifying installation..." -ForegroundColor Yellow
    
    $env:PATH = [Environment]::GetEnvironmentVariable("PATH", "Machine") + ";" + [Environment]::GetEnvironmentVariable("PATH", "User")
    
    Start-Sleep -Seconds 2
    
    try {
        $testResult = & unity-init --help 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Installation successful!" -ForegroundColor Green
            Write-Host "You can now use 'unity-init' from anywhere on your system." -ForegroundColor Green
            Write-Host ""
            Write-Host "Usage examples:" -ForegroundColor Cyan
            Write-Host "  unity-init -l                 # List available Unity versions" -ForegroundColor White
            Write-Host "  unity-init -v 6000.1.12      # Initialize Unity project" -ForegroundColor White
            Write-Host ""
            Write-Host "Note: You may need to restart your command prompt/PowerShell for PATH changes to take effect." -ForegroundColor Yellow
        } else {
            throw "unity-init command returned error code: $LASTEXITCODE"
        }
    } catch {
        Write-Host "❌ Installation completed but verification failed." -ForegroundColor Red
        Write-Host "Please restart your command prompt and try 'unity-init --help'" -ForegroundColor Yellow
        Write-Host "Error: $_" -ForegroundColor Red
    }

} catch {
    Write-Host "❌ Installation failed: $_" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}