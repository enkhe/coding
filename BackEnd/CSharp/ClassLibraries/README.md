# Class Libraries

> Reusable, packaged code. Multi-target until your consumers move; ship NuGet packages with discipline.

## "To Be Dangerous" Cheatsheet

| Need | Property |
|---|---|
| Multi-target | `<TargetFrameworks>net10.0;net8.0;netstandard2.0</TargetFrameworks>` |
| Test internal | `<InternalsVisibleTo Include="MyLib.Tests" />` (csproj item) |
| Treat warnings as errors | `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` |
| Deterministic build | `<Deterministic>true</Deterministic>` |
| Source link | `<PublishRepositoryUrl>true</PublishRepositoryUrl>` + `<EmbedUntrackedSources>true</EmbedUntrackedSources>` |
| NuGet metadata | `<PackageId>`, `<Version>`, `<Description>`, `<Authors>` |
| Sign | `<DelaySign>` + strong-name key, or NuGet package signing |

## Sample csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net10.0;net8.0;netstandard2.0</TargetFrameworks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Deterministic>true</Deterministic>
    <PackageId>Contoso.Orders.Sdk</PackageId>
    <Version>1.0.0</Version>
    <Authors>Contoso</Authors>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>
  <ItemGroup>
    <InternalsVisibleTo Include="Contoso.Orders.Sdk.Tests" />
  </ItemGroup>
</Project>
```

## Versioning

- **SemVer**: `major.minor.patch`. Breaking → bump major.
- **Public API analyzers** (`Microsoft.CodeAnalysis.PublicApiAnalyzers`) — track Shipped/Unshipped public surfaces.

## Common Pitfalls

- Adding net10.0-only API to a multi-target lib without conditional compilation → breaks net8/netstandard targets
- Leaking `internal` types in public signatures
- Bumping minor while making breaking changes — semver lies anger consumers
- Forgetting `<IncludeSymbols>` → no debugging in NuGet consumers

## See also

- [../../../Security/SupplyChain](../../../Security/SupplyChain/) · [../../../Tools/PackageManagers](../../../Tools/PackageManagers/)
