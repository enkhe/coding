using FluentValidation;

namespace VerticalSlice.Features.CreateOrder;

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(100_000m);
        RuleFor(x => x.Tier).NotEmpty().Must(t => t is "standard" or "premium");
    }
}
