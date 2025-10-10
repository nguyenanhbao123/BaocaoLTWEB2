# Script để chạy cả API và MVC song song

Write-Host "Starting BeverageShop - API & MVC" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green
Write-Host ""

# Start API in background
Write-Host "Starting API on http://localhost:5000 ..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\BeverageShop.API'; dotnet run --urls 'http://localhost:5000'"

# Wait a bit for API to start
Start-Sleep -Seconds 5

# Start MVC in background
Write-Host "Starting MVC on https://localhost:5001 ..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\BeverageShop.MVC'; dotnet run --urls 'https://localhost:5001'"

Write-Host ""
Write-Host "Both applications are starting..." -ForegroundColor Green
Write-Host ""
Write-Host "API (Swagger): http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "MVC (Web):     https://localhost:5001" -ForegroundColor Cyan
Write-Host "Admin Panel:   https://localhost:5001/Admin" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to stop all applications..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Kill all dotnet processes (optional - be careful in production)
# Get-Process -Name "dotnet" | Stop-Process -Force
