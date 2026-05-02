#!/usr/bin/env pwsh
# Safe PowerShell script template.
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string]$InputPath,
    [Parameter(Mandatory)] [string]$OutputPath,
    [switch]$Verbose
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$ProgressPreference   = 'SilentlyContinue'

if ($Verbose) { $VerbosePreference = 'Continue' }

function Log { param($Msg) Write-Verbose ("{0:s}Z {1}" -f [DateTime]::UtcNow, $Msg) }

if (-not (Test-Path $InputPath)) { throw "input not found: $InputPath" }

$tmp = New-Item -ItemType Directory -Path ([IO.Path]::GetTempPath() + [Guid]::NewGuid())
try {
    New-Item -ItemType Directory -Path (Split-Path -Parent $OutputPath) -Force | Out-Null
    Copy-Item $InputPath $OutputPath -Force
    Log "OK: $InputPath -> $OutputPath"
}
finally {
    Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
}
