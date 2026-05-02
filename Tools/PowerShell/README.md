# PowerShell 7+

> Cross-platform, object-oriented shell. Pipelines pass *objects*, not strings.

## Core Concepts

- **Verb-Noun** cmdlet naming: `Get-Process`, `Set-Item`
- **Pipeline of objects** — `|` flows real .NET objects with properties
- **Splatting** — pass parameters as a hashtable: `Invoke-RestMethod @params`
- **Advanced functions** — full param validation, pipeline support, common parameters
- **`PSCustomObject`** for typed-ish records
- **Modules** — `*.psm1` + `*.psd1` (manifest)
- **`Set-StrictMode -Version Latest`** + `$ErrorActionPreference = 'Stop'` at top of any real script

## "To Be Dangerous" Cheatsheet

| Need | Cmdlet |
|---|---|
| HTTP | `Invoke-RestMethod` (auto-parses JSON) |
| JSON ↔ object | `ConvertTo-Json` / `ConvertFrom-Json` |
| Filter | `Where-Object { $_.Status -eq 'Running' }` (or `?`) |
| Project | `Select-Object Name, Status` |
| Iterate | `ForEach-Object { ... }` (or `%`) |
| Group | `Group-Object Status` |
| Aggregate | `Measure-Object Length -Sum -Average` |
| Files | `Get-ChildItem -Recurse -File -Filter '*.cs'` |
| Read text | `Get-Content -Raw file.txt` |
| Run process | `Start-Process` / `& cmd args` |
| Errors | `try { } catch { } finally { }`; `$Error[0]` |

## Quick Reference (advanced function)

```powershell
function Get-LogSummary {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [string]$Path,

        [ValidateSet('Info','Warn','Error')]
        [string]$Level = 'Error'
    )
    begin { $matches = 0 }
    process {
        $matches += (Select-String -Path $Path -Pattern "\[$Level\]" -CaseSensitive).Count
    }
    end { [PSCustomObject]@{ Level = $Level; Count = $matches } }
}

Get-ChildItem ./logs -Filter *.log | Get-LogSummary -Level Error
```

## Common Pitfalls

- Forgetting `$ErrorActionPreference = 'Stop'` — silent failures
- Treating output like strings — it's objects; `Select-Object -ExpandProperty Name`
- Implicit casting (`[int]$null` → 0) — use `Set-StrictMode`
- Aliases (`%`, `?`) in committed scripts → use full names for readability
- `Out-Null` in tight loops — use `[void]` or `$null = ...`

## Examples in this folder

- [`safe-template.ps1`](safe-template.ps1)
- [`Install-Tools.ps1`](Install-Tools.ps1)
- [`Get-LogSummary.ps1`](Get-LogSummary.ps1)
- [`cheatsheet.ps1`](cheatsheet.ps1)

## See also

- [../Bash](../Bash/) · [../CLI](../CLI/) · [../../Cloud/Azure](../../Cloud/Azure/)
