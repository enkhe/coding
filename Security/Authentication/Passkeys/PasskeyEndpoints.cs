// WebAuthn endpoints — register and authenticate.
// Library shown: Fido2.NetCore (a maintained fork). Adjust to your library of choice.
using Fido2NetLib;
using Fido2NetLib.Objects;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFido2(o =>
{
    o.ServerDomain = "example.com";
    o.ServerName = "Example";
    o.Origins = new HashSet<string> { "https://example.com" };
    o.TimestampDriftTolerance = 300_000;
});

builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();
app.UseSession();

app.MapPost("/passkeys/register/begin", async (RegisterStartRequest req, IFido2 fido2, IUserStore users, HttpContext http) =>
{
    var user = await users.FindOrCreateAsync(req.Email);
    var existing = await users.GetCredentialsAsync(user.Id);

    var options = fido2.RequestNewCredential(
        new Fido2User { Name = user.Email, Id = user.Id.ToByteArray(), DisplayName = user.Email },
        existing.Select(c => c.Descriptor).ToList(),
        new AuthenticatorSelection { ResidentKey = ResidentKeyRequirement.Preferred, UserVerification = UserVerificationRequirement.Required },
        AttestationConveyancePreference.None);

    http.Session.SetString("attestationOptions", options.ToJson());
    return Results.Ok(options);
});

app.MapPost("/passkeys/register/finish", async (AuthenticatorAttestationRawResponse raw, IFido2 fido2, IUserStore users, HttpContext http, CancellationToken ct) =>
{
    var json = http.Session.GetString("attestationOptions") ?? throw new InvalidOperationException();
    var options = CredentialCreateOptions.FromJson(json);

    var result = await fido2.MakeNewCredentialAsync(raw, options,
        async (args, _) => !await users.IsCredentialUsedAsync(args.CredentialId, ct), cancellationToken: ct);

    if (result.Result is null) return Results.BadRequest(result.ErrorMessage);

    await users.AddCredentialAsync(new StoredCredential
    {
        CredentialId = result.Result.CredentialId,
        PublicKey = result.Result.PublicKey,
        UserHandle = result.Result.User.Id,
        SignCount = result.Result.Counter,
    }, ct);

    return Results.Ok();
});

app.MapPost("/passkeys/login/begin", (LoginStartRequest req, IFido2 fido2, HttpContext http) =>
{
    var options = fido2.GetAssertionOptions(
        allowedCredentials: [],
        userVerification: UserVerificationRequirement.Required);

    http.Session.SetString("assertionOptions", options.ToJson());
    return Results.Ok(options);
});

app.MapPost("/passkeys/login/finish", async (AuthenticatorAssertionRawResponse raw, IFido2 fido2, IUserStore users, HttpContext http, CancellationToken ct) =>
{
    var json = http.Session.GetString("assertionOptions") ?? throw new InvalidOperationException();
    var options = AssertionOptions.FromJson(json);

    var stored = await users.GetCredentialAsync(raw.Id, ct);
    if (stored is null) return Results.Unauthorized();

    var result = await fido2.MakeAssertionAsync(raw, options, stored.PublicKey, stored.SignCount,
        (args, _) => Task.FromResult(stored.UserHandle.SequenceEqual(args.UserHandle)),
        cancellationToken: ct);

    await users.UpdateSignCountAsync(stored.CredentialId, result.Counter, ct);
    return Results.Ok();
});

app.Run();

public sealed record RegisterStartRequest(string Email);
public sealed record LoginStartRequest(string Email);

public sealed record StoredCredential
{
    public required byte[] CredentialId { get; init; }
    public required byte[] PublicKey { get; init; }
    public required byte[] UserHandle { get; init; }
    public PublicKeyCredentialDescriptor Descriptor =>
        new(PublicKeyCredentialType.PublicKey, CredentialId, [AuthenticatorTransport.Internal, AuthenticatorTransport.Hybrid]);
    public required uint SignCount { get; init; }
}

public interface IUserStore
{
    Task<UserRecord> FindOrCreateAsync(string email);
    Task<IReadOnlyList<StoredCredential>> GetCredentialsAsync(Guid userId);
    Task<bool> IsCredentialUsedAsync(byte[] credentialId, CancellationToken ct);
    Task AddCredentialAsync(StoredCredential c, CancellationToken ct);
    Task<StoredCredential?> GetCredentialAsync(byte[] credentialId, CancellationToken ct);
    Task UpdateSignCountAsync(byte[] credentialId, uint signCount, CancellationToken ct);
}

public sealed record UserRecord(Guid Id, string Email);

internal sealed class InMemoryUserStore : IUserStore
{
    public Task<UserRecord> FindOrCreateAsync(string email) =>
        Task.FromResult(new UserRecord(Guid.NewGuid(), email));
    public Task<IReadOnlyList<StoredCredential>> GetCredentialsAsync(Guid _) =>
        Task.FromResult<IReadOnlyList<StoredCredential>>([]);
    public Task<bool> IsCredentialUsedAsync(byte[] _, CancellationToken __) => Task.FromResult(false);
    public Task AddCredentialAsync(StoredCredential _, CancellationToken __) => Task.CompletedTask;
    public Task<StoredCredential?> GetCredentialAsync(byte[] _, CancellationToken __) => Task.FromResult<StoredCredential?>(null);
    public Task UpdateSignCountAsync(byte[] _, uint __, CancellationToken ___) => Task.CompletedTask;
}
