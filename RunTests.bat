@echo off
echo ===============================================
echo IHOP Selenium C# Test Suite Runner
echo ===============================================
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK not found. Please install .NET 6.0 SDK or later.
    pause
    exit /b 1
)

echo .NET SDK Version:
dotnet --version
echo.

echo Restoring NuGet packages...
dotnet restore
if errorlevel 1 (
    echo ERROR: Failed to restore packages.
    pause
    exit /b 1
)

echo.
echo Building the project...
dotnet build
if errorlevel 1 (
    echo ERROR: Build failed.
    pause
    exit /b 1
)

echo.
echo ===============================================
echo Choose test category to run:
echo ===============================================
echo 1. All Tests
echo 2. UI Tests Only
echo 3. Functional Tests Only
echo 4. Integration Tests Only
echo 5. Accessibility Tests Only
echo 6. Smoke Tests Only
echo 7. Exit
echo.

set /p choice="Enter your choice (1-7): "

if "%choice%"=="1" (
    echo Running all tests...
    dotnet test --logger "console;verbosity=normal"
) else if "%choice%"=="2" (
    echo Running UI tests...
    dotnet test --filter Category=UI --logger "console;verbosity=normal"
) else if "%choice%"=="3" (
    echo Running Functional tests...
    dotnet test --filter Category=Functional --logger "console;verbosity=normal"
) else if "%choice%"=="4" (
    echo Running Integration tests...
    dotnet test --filter Category=Integration --logger "console;verbosity=normal"
) else if "%choice%"=="5" (
    echo Running Accessibility tests...
    dotnet test --filter Category=Accessibility --logger "console;verbosity=normal"
) else if "%choice%"=="6" (
    echo Running Smoke tests...
    dotnet test --filter Category=Smoke --logger "console;verbosity=normal"
) else if "%choice%"=="7" (
    echo Exiting...
    exit /b 0
) else (
    echo Invalid choice. Please run the script again.
    pause
    exit /b 1
)

echo.
echo ===============================================
echo Test execution completed!
echo ===============================================
echo.
echo Check the following for results:
echo - Console output above
echo - Log files in the 'logs' directory
echo - Screenshots in the 'Screenshots' directory (if any failures occurred)
echo.

pause
