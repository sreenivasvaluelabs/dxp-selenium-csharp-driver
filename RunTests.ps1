# IHOP Selenium C# Test Suite Runner - PowerShell Version

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "IHOP Selenium C# Test Suite Runner" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host ".NET SDK Version: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: .NET SDK not found. Please install .NET 6.0 SDK or later." -ForegroundColor Red
    Read-Host "Press any key to continue..."
    exit 1
}

Write-Host ""
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
$restoreResult = dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore packages." -ForegroundColor Red
    Read-Host "Press any key to continue..."
    exit 1
}

Write-Host ""
Write-Host "Building the project..." -ForegroundColor Yellow
$buildResult = dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed." -ForegroundColor Red
    Read-Host "Press any key to continue..."
    exit 1
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Choose test category to run:" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "1. All Tests" -ForegroundColor White
Write-Host "2. UI Tests Only" -ForegroundColor White
Write-Host "3. Functional Tests Only" -ForegroundColor White
Write-Host "4. Integration Tests Only" -ForegroundColor White
Write-Host "5. Accessibility Tests Only" -ForegroundColor White
Write-Host "6. Smoke Tests Only" -ForegroundColor White
Write-Host "7. Generate Test Report" -ForegroundColor White
Write-Host "8. Exit" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Enter your choice (1-8)"

switch ($choice) {
    "1" {
        Write-Host "Running all tests..." -ForegroundColor Green
        dotnet test --logger "console;verbosity=normal"
    }
    "2" {
        Write-Host "Running UI tests..." -ForegroundColor Green
        dotnet test --filter Category=UI --logger "console;verbosity=normal"
    }
    "3" {
        Write-Host "Running Functional tests..." -ForegroundColor Green
        dotnet test --filter Category=Functional --logger "console;verbosity=normal"
    }
    "4" {
        Write-Host "Running Integration tests..." -ForegroundColor Green
        dotnet test --filter Category=Integration --logger "console;verbosity=normal"
    }
    "5" {
        Write-Host "Running Accessibility tests..." -ForegroundColor Green
        dotnet test --filter Category=Accessibility --logger "console;verbosity=normal"
    }
    "6" {
        Write-Host "Running Smoke tests..." -ForegroundColor Green
        dotnet test --filter Category=Smoke --logger "console;verbosity=normal"
    }
    "7" {
        Write-Host "Generating test report..." -ForegroundColor Green
        $reportFile = "TestResults_$(Get-Date -Format 'yyyyMMdd_HHmmss').trx"
        dotnet test --logger "trx;LogFileName=$reportFile"
        Write-Host "Test report generated: $reportFile" -ForegroundColor Green
    }
    "8" {
        Write-Host "Exiting..." -ForegroundColor Yellow
        exit 0
    }
    default {
        Write-Host "Invalid choice. Please run the script again." -ForegroundColor Red
        Read-Host "Press any key to continue..."
        exit 1
    }
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Test execution completed!" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Check the following for results:" -ForegroundColor White
Write-Host "- Console output above" -ForegroundColor Gray
Write-Host "- Log files in the 'logs' directory" -ForegroundColor Gray
Write-Host "- Screenshots in the 'Screenshots' directory (if any failures occurred)" -ForegroundColor Gray
Write-Host ""

Read-Host "Press any key to continue..."
