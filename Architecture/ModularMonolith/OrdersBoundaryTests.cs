// Fitness function — fail the build when boundary rules are violated.
// Package: NetArchTest.Rules
using NetArchTest.Rules;
using Xunit;

namespace Modules.Orders.Tests;

public sealed class OrdersBoundaryTests
{
    [Fact]
    public void Orders_should_not_depend_on_other_modules_internals()
    {
        var result = Types.InAssembly(typeof(OrdersModule).Assembly)
            .That().ResideInNamespaceStartingWith("Modules.Orders")
            .Should().NotHaveDependencyOnAny(
                "Modules.Billing.Application",
                "Modules.Billing.Infrastructure",
                "Modules.Notifications.Application",
                "Modules.Notifications.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Boundary violation: " + string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Domain_should_not_depend_on_Infrastructure()
    {
        var result = Types.InAssembly(typeof(OrdersModule).Assembly)
            .That().ResideInNamespaceStartingWith("Modules.Orders.Domain")
            .Should().NotHaveDependencyOn("Modules.Orders.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Domain reaches into Infrastructure: " + string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Public_contract_types_must_be_in_Contracts_namespace()
    {
        var result = Types.InAssembly(typeof(OrdersModule).Assembly)
            .That().ArePublic()
            .And().ResideInNamespaceStartingWith("Modules.Orders")
            .Should().ResideInNamespaceStartingWith("Modules.Orders.Contracts")
            .Or().HaveNameEndingWith("Module")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Non-contract public types: " + string.Join(", ", result.FailingTypeNames ?? []));
    }
}
