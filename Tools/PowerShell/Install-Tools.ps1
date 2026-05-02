# Install developer tools across Windows / macOS / Linux idiomatically.
[CmdletBinding()]
param([switch]$Force)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Install-Tool {
    param([string]$Name, [string]$Linux, [string]$Mac, [string]$Windows)
    $ok = $false
    if ($IsLinux   -and $Linux)   { sudo apt-get install -y $Linux;            $ok = $true }
    elseif ($IsMacOS -and $Mac)   { brew install $Mac;                          $ok = $true }
    elseif ($IsWindows -and $Windows) { winget install --id $Windows --silent;  $ok = $true }

    if (-not $ok) { Write-Warning "No installer mapping for $Name on this OS." }
}

Install-Tool -Name jq -Linux jq -Mac jq -Windows 'jqlang.jq'
Install-Tool -Name ripgrep -Linux ripgrep -Mac ripgrep -Windows 'BurntSushi.ripgrep.MSVC'
Install-Tool -Name fzf -Linux fzf -Mac fzf -Windows 'junegunn.fzf'
Install-Tool -Name bat -Linux bat -Mac bat -Windows 'sharkdp.bat'
Install-Tool -Name azure-cli -Linux azure-cli -Mac azure-cli -Windows 'Microsoft.AzureCLI'

# Cross-platform global dotnet tools.
dotnet tool update -g CycloneDX
dotnet tool update -g upgrade-assistant
dotnet tool update -g dotnet-counters
dotnet tool update -g dotnet-trace
