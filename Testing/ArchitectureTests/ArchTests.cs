using NetArchTest.Rules;
using Xunit;

namespace ArchTests;

public sealed class CommonArchTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(Domain.Order).Assembly;

    [Fact]
    public void Handlers_should_be_sealed_and_in_Application_namespace()
    {
        var result = Types.InAssembly(Domain)
            .That().HaveNameEndingWith("Handler")
            .Should().BeSealed()
            .And().ResideInNamespaceStartingWith("Application")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(",", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Public_DTOs_must_be_records()
    {
        var result = Types.InAssembly(Domain)
            .That().HaveNameEndingWith("Dto")
            .Should().BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Repositories_must_be_internal()
    {
        var result = Types.InAssembly(Domain)
            .That().HaveNameEndingWith("Repository")
            .Should().NotBePublic()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}

namespace Domain { public sealed class Order { } }
