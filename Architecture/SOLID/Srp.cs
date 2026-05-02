// Single Responsibility Principle
// One reason to change per type.

namespace Architecture.SOLID.Srp;

// BAD: persistence + email + formatting all in one class.
public sealed class UserService_Bad
{
    public void Register(string email, string name)
    {
        // persistence
        File.AppendAllText("users.csv", $"{email},{name}\n");
        // notification
        // SmtpClient.Send(...)
        // formatting
        Console.WriteLine($"Welcome {name}");
    }
}

// GOOD: each collaborator owns its axis of change.
public sealed record User(string Email, string Name);

public interface IUserRepository { Task AddAsync(User user, CancellationToken ct); }
public interface IWelcomeMailer { Task SendAsync(User user, CancellationToken ct); }

public sealed class RegisterUser(IUserRepository repo, IWelcomeMailer mailer)
{
    public async Task HandleAsync(string email, string name, CancellationToken ct)
    {
        var user = new User(email, name);
        await repo.AddAsync(user, ct);
        await mailer.SendAsync(user, ct);
    }
}
