// Pulumi for .NET — declarative infra in C#.
// Packages: Pulumi, Pulumi.AzureNative
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

return await Deployment.RunAsync(() =>
{
    var config = new Config();
    var env = config.Require("env");
    var tags = config.RequireObject<Dictionary<string, string>>("tags");

    var rg = new ResourceGroup($"rg-orders-{env}", new ResourceGroupArgs
    {
        Tags = tags,
    });

    var sa = new StorageAccount($"storders{env}", new StorageAccountArgs
    {
        ResourceGroupName = rg.Name,
        Sku = new SkuArgs { Name = SkuName.Standard_LRS },
        Kind = Kind.StorageV2,
        AccessTier = AccessTier.Hot,
        MinimumTlsVersion = MinimumTlsVersion.TLS1_2,
        AllowBlobPublicAccess = false,
        PublicNetworkAccess = "Disabled",
        Tags = tags,
    });

    return new Dictionary<string, object?>
    {
        ["resourceGroup"] = rg.Name,
        ["storageEndpoint"] = sa.PrimaryEndpoints.Apply(e => e.Blob),
    };
});
