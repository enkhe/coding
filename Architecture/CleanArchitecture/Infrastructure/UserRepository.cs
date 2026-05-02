// Infrastructure layer: implements Application ports using concrete tech (EF Core here).
// References Application + Domain. Application does NOT reference Infrastructure.

using Microsoft.EntityFrameworkCore;
using MyApp.Application.Users.CreateUser;
using MyApp.Domain.Users;

namespace MyApp.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        var u = mb.Entity<User>();
        u.HasKey(x => x.Id);
        u.Property(x => x.Id).HasConversion(id => id.Value, v => new UserId(v));
        u.Property(x => x.Email).HasMaxLength(256).IsRequired();
        u.Property(x => x.Name).HasMaxLength(128).IsRequired();
    }
}

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct) =>
        db.Users.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
    }
}
