// Domain layer: no framework, no IO, no DI references.
// Encodes invariants and emits domain events.

namespace MyApp.Domain.Users;

public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
}

public sealed class User
{
    public UserId Id { get; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    private User(UserId id, string email, string name, DateTimeOffset createdAt)
    {
        Id = id; Email = email; Name = name; CreatedAt = createdAt;
    }

    public static User Register(string email, string name, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Invalid email");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name required");
        return new User(UserId.New(), email.Trim().ToLowerInvariant(), name.Trim(), now);
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Name required");
        Name = newName.Trim();
    }
}
