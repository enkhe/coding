// Application core: implements driving ports using only driven ports.
// Pure logic, no IO, fully testable with fakes for the driven ports.

namespace Architecture.Hexagonal.Core;

public sealed class UserService(IUserRepositoryPort repo) : IUserPort
{
    public async Task<UserDto> RegisterAsync(string email, string name, CancellationToken ct)
    {
        if (await repo.ExistsByEmailAsync(email, ct))
            throw new InvalidOperationException("email already in use");

        var user = User.Register(email, name);
        await repo.AddAsync(user, ct);
        return new UserDto(user.Id, user.Email, user.Name);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await repo.FindAsync(id, ct);
        return user is null ? null : new UserDto(user.Id, user.Email, user.Name);
    }
}
