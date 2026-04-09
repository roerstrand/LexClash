# =============================================
#  Ordspelet - LAN-start
#  Kör detta skript för att starta spelet
#  och låta andra på samma WiFi ansluta
# =============================================

# Hitta datorns WiFi-IP automatiskt
$ip = (Get-NetIPAddress -AddressFamily IPv4 |
       Where-Object { $_.PrefixOrigin -eq "Dhcp" -or $_.PrefixOrigin -eq "Manual" } |
       Where-Object { $_.IPAddress -notlike "169.*" } |
       Select-Object -First 1).IPAddress

if (-not $ip) {
    Write-Host "Kunde inte hitta nätverks-IP. Kontrollera att du är ansluten till WiFi." -ForegroundColor Red
    exit 1
}

$apiPort = 5260
$uiPort  = 5235
$apiUrl  = "http://$($ip):$apiPort"
$uiUrl   = "http://$($ip):$uiPort"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  Startar Ordspelet..." -ForegroundColor Cyan
Write-Host "  Din IP: $ip" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Sätt miljövariabler så API och UI vet om LAN-IP
$env:LAN_IP      = $ip
$env:API_PORT    = $apiPort
$env:UI_PORT     = $uiPort
$env:API_BASE_URL = $apiUrl

# Starta API i ett separat fönster
$apiDir = Join-Path $PSScriptRoot "OrdSpel.API"
Start-Process powershell -ArgumentList "-NoExit", "-Command",
    "cd '$apiDir'; Write-Host 'API startar...' -ForegroundColor Green; dotnet run --launch-profile LAN" `
    -WindowStyle Normal

Write-Host "API startar i separat fönster..." -ForegroundColor Green
Start-Sleep -Seconds 3

# Starta UI i ett separat fönster
$uiDir = Join-Path $PSScriptRoot "OrdSpel"
Start-Process powershell -ArgumentList "-NoExit", "-Command",
    "cd '$uiDir'; Write-Host 'UI startar...' -ForegroundColor Green; dotnet run --launch-profile LAN" `
    -WindowStyle Normal

Write-Host "UI startar i separat fönster..." -ForegroundColor Green
Write-Host "Vantar 6 sekunder pa att allt startar..." -ForegroundColor Gray
Start-Sleep -Seconds 6

# Öppna browsern automatiskt
Start-Process "http://localhost:$uiPort"

# Visa länken i en popup så den är lätt att kopiera
Add-Type -AssemblyName Microsoft.VisualBasic
[Microsoft.VisualBasic.Interaction]::MsgBox(
    "Ordspelet koer!`n`nDen har datorn:`nhttp://localhost:$uiPort`n`nAnnan dator pa samma WiFi:`n$uiUrl`n`nSkicka den nedre lanken till kompisen!`nStang INTE terminalfonstren.",
    [Microsoft.VisualBasic.MsgBoxStyle]::OkOnly,
    "Ordspelet - LAN"
) | Out-Null

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  Ordspelet koer!" -ForegroundColor Green
Write-Host "  Stang INTE detta fonster!" -ForegroundColor Red
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

# Håll fönstret öppet för alltid tills användaren stänger det manuellt
while ($true) { Start-Sleep -Seconds 60 }
