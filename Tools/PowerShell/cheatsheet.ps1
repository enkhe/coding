# PowerShell idioms cheatsheet (PowerShell 7+).
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Pipeline of objects (vs strings)
Get-Process | Where-Object CPU -GT 100 | Select-Object Name, CPU | Format-Table

# Splatting — keep parameter blocks tidy
$params = @{
    Uri    = 'https://api.example.com/orders'
    Method = 'POST'
    Body   = @{ userId = 'u1'; amount = 9.99 } | ConvertTo-Json
    Headers = @{ Authorization = "Bearer $token" }
    ContentType = 'application/json'
}
$response = Invoke-RestMethod @params

# JSON
$obj = '{"a":1,"b":[2,3]}' | ConvertFrom-Json
$obj.b[0] = 99
$obj | ConvertTo-Json -Depth 10

# Hashtables
$cfg = @{ Env = 'prod'; Region = 'eastus' }
$cfg['Env']
$cfg.Keys | Sort-Object

# String formatting
"{0:N2} ({1:P1})" -f 1234.5, 0.075   # 1,234.50 (7.5%)

# File I/O
$lines = Get-Content -Path .\big.log -ReadCount 1000   # batched read
Get-ChildItem . -Recurse -File -Include '*.cs','*.csproj'

# Try/Catch
try {
    Invoke-RestMethod 'https://example.invalid'
}
catch [System.Net.Http.HttpRequestException] {
    Write-Warning "HTTP failed: $($_.Exception.Message)"
}
finally {
    Write-Host 'cleanup'
}

# Parallel ForEach (PowerShell 7+)
1..10 | ForEach-Object -Parallel { Start-Sleep -Milliseconds (Get-Random -Max 200); $_ } -ThrottleLimit 5

# Regex
'orders-2026-04-12' -match '^(?<kind>\w+)-(?<date>\d{4}-\d{2}-\d{2})$'
$Matches.date     # 2026-04-12
