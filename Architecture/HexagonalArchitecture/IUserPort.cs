// Driving (inbound, primary) port: the application's use-case API.
// Owned by the core; adapters call it.

namespace Architecture.Hexagonal.Core;

public sealed record UserDto(Guid Id, string Email, string Name);

public interface IUserPort
{
    Task<UserDto> RegisterAsync(string email, string name, CancellationToken ct);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct);
}
