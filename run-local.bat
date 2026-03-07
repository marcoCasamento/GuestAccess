@echo off
REM GuestAccess Local Test Script

echo ========================================
echo GuestAccess - Local Test
echo ========================================
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Error: .NET SDK not found. Please install .NET 10 SDK first.
    exit /b 1
)

echo .NET SDK found: 
dotnet --version
echo.

REM Check if database exists
if not exist "app.db" (
    echo Creating database...
    dotnet ef database update
    echo.
)

REM Build the application
echo Building application...
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    exit /b 1
)
echo Build successful
echo.

REM Run the application
echo Starting application...
echo.
echo Application will be available at:
echo    http://localhost:5000
echo.
echo Default admin credentials:
echo    Email: admin@localhost
echo    Password: Admin123!
echo.
echo WARNING: Change these credentials in production!
echo.
echo Press Ctrl+C to stop the application
echo.
echo ========================================
echo.

dotnet run --urls "http://localhost:5000"
