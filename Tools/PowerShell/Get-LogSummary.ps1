# Advanced function — pipelines, parameter validation, custom objects.
function Get-LogSummary {
    <#
    .SYNOPSIS
    Summarize log files by level.

    .EXAMPLE
    Get-ChildItem ./logs -Filter *.log | Get-LogSummary -Level Error
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('FullName')]
        [string]$Path,

        [ValidateSet('Trace','Debug','Information','Warning','Error','Critical')]
        [string]$Level = 'Error'
    )
    begin {
        $results = [System.Collections.Generic.List[object]]::new()
    }
    process {
        $count = (Select-String -Path $Path -Pattern "\[$Level\]" -CaseSensitive).Count
        $results.Add([PSCustomObject]@{ Path = $Path; Level = $Level; Count = $count })
    }
    end {
        $results
        $totals = $results | Measure-Object Count -Sum
        Write-Verbose "Total $Level: $($totals.Sum)"
    }
}
