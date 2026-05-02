# Cryptography

> **Don't roll your own.** Use library primitives correctly.

## Pick the right tool

| Need | Algorithm |
|---|---|
| Password hashing | **Argon2id** (preferred), **PBKDF2** (legacy compat), **bcrypt** |
| Generic hash (integrity) | **SHA-256** / SHA-512 / **BLAKE3** |
| HMAC (keyed integrity) | HMAC-SHA-256 |
| Symmetric encryption | **AES-256-GCM** (auth + encrypt) |
| Asymmetric encryption | **RSA-OAEP** (small payloads) or hybrid (RSA-OAEP + AES-GCM) |
| Digital signatures | **Ed25519** or **ECDSA P-256** |
| Key derivation | **HKDF**, **Argon2id**, **PBKDF2** |
| Random | `RandomNumberGenerator.Fill(bytes)` (CSPRNG) — never `Random` |

## Never do

- MD5 / SHA-1 / DES / 3DES for security
- Roll your own AES mode (CBC without HMAC)
- Compare hashes/macs with `==` — use **constant-time** compare (`CryptographicOperations.FixedTimeEquals`)
- Reuse nonces with AES-GCM — catastrophic
- Store passwords with anything but a slow KDF

## Quick Reference (.NET)

```csharp
// Password hashing — use ASP.NET Core Identity's PasswordHasher (PBKDF2 by default)
// or Konscious.Security.Cryptography for Argon2id.
using Microsoft.AspNetCore.Identity;
var hasher = new PasswordHasher<object>();
var hash = hasher.HashPassword(null!, plaintext);
var verified = hasher.VerifyHashedPassword(null!, hash, plaintext);

// AES-GCM (authenticated encryption)
using var aes = new AesGcm(key, tagSizeInBytes: 16);
Span<byte> nonce = stackalloc byte[12];
RandomNumberGenerator.Fill(nonce);
var cipher = new byte[plaintext.Length];
var tag = new byte[16];
aes.Encrypt(nonce, plaintext, cipher, tag, associatedData: null);
// On decrypt:
aes.Decrypt(nonce, cipher, tag, plaintextOut);

// Constant-time compare
CryptographicOperations.FixedTimeEquals(a, b);
```

## Key management

- Store keys in **Key Vault** / KMS / HSM — never in app config
- Rotate on a schedule (90-180 days for symmetric)
- For ASP.NET Core data protection, see [`DataProtection`](../DataProtection/)

## Common Pitfalls

- Using `string` for keys → mutable, not zeroed; prefer `byte[]` from `RandomNumberGenerator`
- Forgetting nonce uniqueness in GCM
- "Encrypt-then-MAC" with custom MAC — just use AES-GCM
- Same key for encryption + signing — separate per purpose

## See also

- [../DataProtection](../DataProtection/) · [../SecretsManagement](../SecretsManagement/) · [../Authentication/JWT](../Authentication/JWT/)
