// Driven (outbound, secondary) port: the core declares what it needs from the world.
// Adapters provide concrete implementations (EF, Dapper, in-memory, fake).

namespace Architecture.Hexagonal.Core;

public sealed class User
{
    public Guid Id { get; }
    public string Email { get; }
    public string Name { get; }

    public User(Guid id, string email, string name) { Id = id; Email = email; Name = name; }

    public static User Register(string email, string name)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("invalid email");
        return new User(Guid.NewGuid(), email.Trim().ToLowerInvariant(), name.Trim());
    }
}

public interface IUserRepositoryPort
{
    Task AddAsync(User user, CancellationToken ct);
    Task<User?> FindAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
}
