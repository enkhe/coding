# Passkeys (FIDO2 / WebAuthn)

> Phishing-resistant, passwordless authentication built on public-key crypto. The 2026 baseline for CIAM.

## Core Concepts

- **Authenticator** — device that holds the private key (Windows Hello, Touch ID, security key, phone)
- **Relying Party (RP)** — your service (`rp.id` = your origin's eTLD+1, e.g., `example.com`)
- **Ceremony** — Registration (create credential) and Authentication (verify with existing credential)
- **Attestation** — optional proof of authenticator type; usually `none` for consumer apps
- **Resident credential / "passkey"** — credential lives on the authenticator and syncs (e.g., iCloud Keychain, Google Password Manager)
- **Platform vs Cross-platform** — built-in (Windows Hello) vs roaming (USB key, phone via QR)

## "To Be Dangerous" Cheatsheet

| Step | What you call |
|---|---|
| Begin registration | server: build `PublicKeyCredentialCreationOptions` |
| Browser registers | `navigator.credentials.create({ publicKey })` |
| Finish registration | server: validate attestation, store `credentialId` + public key + `userHandle` |
| Begin auth | server: build `PublicKeyCredentialRequestOptions` (allow-list of credential ids) |
| Browser authenticates | `navigator.credentials.get({ publicKey })` |
| Finish auth | server: verify signature against stored public key, check counter, issue session |

## Quick Reference (.NET — Fido2 NetCore)

```csharp
// Package: Fido2.NetCore  (or modern equivalent)
builder.Services.AddFido2(o =>
{
    o.ServerDomain = "example.com";
    o.ServerName = "Example";
    o.Origins = new HashSet<string> { "https://example.com" };
});
```

```csharp
app.MapPost("/passkeys/register/begin", async (RegisterRequest req, IFido2 fido2, IUserStore users) =>
{
    var user = await users.FindOrCreateAsync(req.Email);
    var existing = await users.GetCredentialsAsync(user.Id);

    var options = fido2.RequestNewCredential(
        new Fido2User { Name = user.Email, Id = user.Id.ToByteArray(), DisplayName = user.Email },
        existing.Select(c => c.Descriptor).ToList(),
        AuthenticatorSelection.Default,
        AttestationConveyancePreference.None);

    return Results.Ok(options);
});
```

## Common Pitfalls

- Not pinning `rp.id` to your origin → cross-origin credential sharing or rejection
- Using `attestation: "direct"` for consumer apps → friction without value
- Not persisting `signCount` / counter → cannot detect cloned authenticators
- Allowing the same `credentialId` across users → account takeover vector
- Leaking `userHandle` (it's a stable ID) — keep it opaque to clients

## Examples in this folder

- [`PasskeyEndpoints.cs`](PasskeyEndpoints.cs) — register/login endpoints
- [`passkey-client.ts`](passkey-client.ts) — browser side

## See also

- [../OpenIdConnect](../OpenIdConnect/) — when you also need federated identity
- [../Entra](../Entra/) — Entra External ID supports passkeys natively
