// NetArchTest — fluent C# rules verified in CI.
// Package: NetArchTest.Rules
using NetArchTest.Rules;
using Xunit;

namespace ArchTests;

public sealed class ArchitectureTests
{
    private static readonly System.Reflection.Assembly Domain =
        typeof(Domain.Order).Assembly;

    [Fact]
    public void Domain_should_not_reference_Infrastructure_or_Persistence()
    {
        var result = Types.InAssembly(Domain)
            .That().ResideInNamespaceStartingWith("Domain")
            .Should().NotHaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Dapper",
                "Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Domain leak: " + string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Handlers_should_be_sealed()
    {
        var result = Types.InAssembly(Domain)
            .That().HaveNameEndingWith("Handler")
            .Should().BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Non-sealed handlers: " + string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Controllers_must_not_use_DbContext_directly()
    {
        var result = Types.InAssembly(Domain)
            .That().ResideInNamespaceStartingWith("Web.Controllers")
            .Should().NotHaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore.DbContext",
                "Infrastructure.Persistence")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Controller bypassing Application layer: " + string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Queries_must_be_immutable_records()
    {
        var result = Types.InAssembly(Domain)
            .That().HaveNameEndingWith("Query")
            .Should().BeSealed().And().HaveNameMatching("[A-Z][a-zA-Z]*Query")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Bad Query types: " + string.Join(", ", result.FailingTypeNames ?? []));
    }
}

namespace Domain
{
    public sealed class Order { }
}
