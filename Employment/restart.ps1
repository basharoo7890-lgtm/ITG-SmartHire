# restart.ps1 — أوقف التطبيق القديم وأعد التشغيل
Write-Host "🔄 Stopping old processes..." -ForegroundColor Yellow
Get-NetTCPConnection -LocalPort 7051,5291 -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty OwningProcess | Sort-Object -Unique |
    ForEach-Object { Stop-Process -Id $_ -Force -ErrorAction SilentlyContinue }

Get-Process -Name "Employment" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

Start-Sleep -Seconds 1
Write-Host "🚀 Starting application..." -ForegroundColor Green
dotnet run
