// Secondary (driven) adapter: implements IUserRepositoryPort with EF Core.
// Swappable for InMemoryUserAdapter, DapperUserAdapter, etc.

using Architecture.Hexagonal.Core;
using Microsoft.EntityFrameworkCore;

namespace Architecture.Hexagonal.EfAdapter;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> o) : DbContext(o)
{
    public DbSet<User> Users => Set<User>();
}

public sealed class EfUserAdapter(UsersDbContext db) : IUserRepositoryPort
{
    public async Task AddAsync(User user, CancellationToken ct)
    {
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task<User?> FindAsync(Guid id, CancellationToken ct) =>
        db.Users.SingleOrDefaultAsync(u => u.Id == id, ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct) =>
        db.Users.AnyAsync(u => u.Email == email, ct);
}
