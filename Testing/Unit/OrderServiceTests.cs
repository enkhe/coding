using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Xunit;

namespace Testing.Unit;

public sealed record Order(Guid Id, decimal Total, bool Paid);

public interface IOrderRepository
{
    Task<Order?> GetAsync(Guid id, CancellationToken ct);
    Task SaveAsync(Order order, CancellationToken ct);
}

public sealed class OrderService(IOrderRepository repo, TimeProvider time)
{
    public async Task<Order> MarkPaidAsync(Guid id, CancellationToken ct)
    {
        var order = await repo.GetAsync(id, ct)
            ?? throw new InvalidOperationException($"Order {id} not found");

        if (order.Paid) return order;

        var paid = order with { Paid = true };
        await repo.SaveAsync(paid, ct);
        _ = time.GetUtcNow(); // illustrate TimeProvider usage
        return paid;
    }
}

public sealed class OrderServiceTests
{
    private readonly IOrderRepository _repo = Substitute.For<IOrderRepository>();
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

    [Fact]
    public async Task MarkPaidAsync_Should_PersistPaidOrder_When_OrderUnpaid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = new Order(id, 100m, Paid: false);
        _repo.GetAsync(id, Arg.Any<CancellationToken>()).Returns(order);
        var sut = new OrderService(_repo, _time);

        // Act
        var result = await sut.MarkPaidAsync(id, CancellationToken.None);

        // Assert
        result.Paid.Should().BeTrue();
        await _repo.Received(1).SaveAsync(
            Arg.Is<Order>(o => o.Id == id && o.Paid),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkPaidAsync_Should_BeIdempotent_When_OrderAlreadyPaid()
    {
        var id = Guid.NewGuid();
        _repo.GetAsync(id, Arg.Any<CancellationToken>())
            .Returns(new Order(id, 100m, Paid: true));
        var sut = new OrderService(_repo, _time);

        var result = await sut.MarkPaidAsync(id, CancellationToken.None);

        result.Paid.Should().BeTrue();
        await _repo.DidNotReceive().SaveAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(9_999.99)]
    public async Task MarkPaidAsync_Should_PreserveTotal(decimal total)
    {
        var id = Guid.NewGuid();
        _repo.GetAsync(id, Arg.Any<CancellationToken>())
            .Returns(new Order(id, total, false));
        var sut = new OrderService(_repo, _time);

        var result = await sut.MarkPaidAsync(id, CancellationToken.None);

        result.Total.Should().Be(total);
    }

    [Fact]
    public async Task MarkPaidAsync_Should_Throw_When_OrderMissing()
    {
        _repo.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);
        var sut = new OrderService(_repo, _time);

        var act = async () => await sut.MarkPaidAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Order * not found");
    }
}
