// ArchUnitNET — alternative fitness function library; nicer for layered/onion.
// Package: ArchUnitNET.xUnit, TngTech.ArchUnitNET
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ArchTests;

public sealed class ArchUnitTests
{
    private static readonly Architecture Architecture =
        new ArchLoader().LoadAssemblies(typeof(Domain.Order).Assembly).Build();

    private static readonly IObjectProvider<IType> DomainLayer =
        Types().That().ResideInNamespace("Domain.*", useRegularExpressions: true).As("Domain");

    private static readonly IObjectProvider<IType> ApplicationLayer =
        Types().That().ResideInNamespace("Application.*", useRegularExpressions: true).As("Application");

    private static readonly IObjectProvider<IType> InfrastructureLayer =
        Types().That().ResideInNamespace("Infrastructure.*", useRegularExpressions: true).As("Infrastructure");

    [Fact]
    public void Domain_should_not_depend_on_Application() =>
        Types().That().Are(DomainLayer)
            .Should().NotDependOnAny(ApplicationLayer)
            .Check(Architecture);

    [Fact]
    public void Domain_should_not_depend_on_Infrastructure() =>
        Types().That().Are(DomainLayer)
            .Should().NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);

    [Fact]
    public void Application_should_not_depend_on_Infrastructure() =>
        Types().That().Are(ApplicationLayer)
            .Should().NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
}
