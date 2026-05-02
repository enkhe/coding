// Application layer: use cases + ports. References Domain only.

using MyApp.Domain.Users;

namespace MyApp.Application.Users.CreateUser;

public sealed record CreateUserCommand(string Email, string Name);
public sealed record CreateUserResult(Guid UserId);

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}

public interface IClock { DateTimeOffset UtcNow { get; } }

public sealed class CreateUserHandler(IUserRepository repo, IClock clock)
{
    public async Task<CreateUserResult> HandleAsync(CreateUserCommand cmd, CancellationToken ct)
    {
        if (await repo.ExistsByEmailAsync(cmd.Email, ct))
            throw new InvalidOperationException("Email already in use");

        var user = User.Register(cmd.Email, cmd.Name, clock.UtcNow);
        await repo.AddAsync(user, ct);
        return new CreateUserResult(user.Id.Value);
    }
}
