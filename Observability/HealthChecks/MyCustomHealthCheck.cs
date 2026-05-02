// Custom IHealthCheck — implement when no off-the-shelf check fits.
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Orders.Api.Health;

public sealed class DiskSpaceHealthCheck(IConfiguration cfg) : IHealthCheck
{
    private readonly long _minFreeBytes = cfg.GetValue<long?>("Health:Disk:MinFreeBytes") ?? 1_073_741_824; // 1 GiB

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var drive = new DriveInfo(Path.GetPathRoot(AppContext.BaseDirectory)!);
        var data = new Dictionary<string, object>
        {
            ["drive"] = drive.Name,
            ["available_bytes"] = drive.AvailableFreeSpace,
            ["min_required_bytes"] = _minFreeBytes,
        };

        return Task.FromResult(drive.AvailableFreeSpace >= _minFreeBytes
            ? HealthCheckResult.Healthy("disk OK", data)
            : HealthCheckResult.Degraded($"low disk: {drive.AvailableFreeSpace:N0} bytes free", data: data));
    }
}
