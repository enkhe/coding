# Security

> Security domain index for the .NET 2026 senior/architect roadmap. Cheatsheet-first, code where it matters.

## Core Concepts

Security is layered: identity, access, data, code, supply chain, and operations. Modern .NET defaults to **Zero Trust**: never assume network = trust, verify every call, give least privilege, assume breach.

- **AuthN** = who are you (token-bearing, federated, often OIDC).
- **AuthZ** = what may you do (policy + claims, not roles-only).
- **Crypto** = mechanics; you consume primitives, never invent them.
- **Data Protection** = at-rest and in-transit; ASP.NET Core Data Protection API for app-level secrets in flight.
- **Secrets** = never in source; vault + managed identity.
- **OWASP / Threat Modeling** = lenses for what can go wrong.
- **Supply Chain** = SBOM, signed artifacts, pinned deps.

## "To Be Dangerous" Cheatsheet

- Default to **OIDC + Authorization Code + PKCE**; never implicit, never resource-owner-password.
- **JWT**: validate `iss`, `aud`, `exp`, `nbf`, signature, key id; prefer **RS256/ES256**; small clock-skew.
- **Passwords**: Argon2id or PBKDF2 (>= 600k iters SHA256); never MD5/SHA1; constant-time compare.
- **Crypto**: AES-256-GCM for symmetric; RSA-OAEP / ECDSA P-256 for asymmetric. Don't roll your own.
- **Secrets**: `dotnet user-secrets` in dev, Azure Key Vault + Managed Identity in prod.
- **HTTP**: HTTPS only, HSTS preload, secure + same-site cookies, anti-forgery on cookie auth.
- **OWASP Top 10** mapped: parameterized queries, output encoding, AuthZ checks per resource, no verbose errors.
- **Zero Trust**: mTLS or token between services; conditional access at the edge.
- **Supply chain**: SBOM (CycloneDX) per release, sign artifacts (cosign/Sigstore), aim for SLSA L3.
- **Threat-model** every bounded context with **STRIDE**.

## Quick Reference

| Need | Reach for |
| --- | --- |
| Web app login (Microsoft) | Microsoft.Identity.Web + Entra ID |
| External customer login | Entra External ID (CIAM) or Auth0/Okta |
| Token validation in API | `Microsoft.AspNetCore.Authentication.JwtBearer` |
| Policy authorization | `AddAuthorization` + custom `AuthorizationHandler` |
| Symmetric encrypt at app | AES-GCM via `System.Security.Cryptography.AesGcm` |
| Encrypt small payloads w/ rotation | ASP.NET Core Data Protection |
| Password hashing | `PasswordHasher<TUser>` or `Konscious.Security.Cryptography.Argon2id` |
| Secrets in prod | Azure Key Vault + Managed Identity |
| Supply chain | `dotnet-CycloneDX`, `cosign`, GitHub Dependabot |

## Common Pitfalls

- HS256 JWTs across orgs (shared symmetric secret = federated leak).
- Roles-only AuthZ; modern systems need **policy + resource-based** checks.
- Storing connection strings in `appsettings.json` and committing them.
- Disabling cert validation "to make it work."
- Treating internal network as trusted.
- No SBOM, no signature verification of containers/binaries.

## Examples in this folder

- [Authentication/](./Authentication/README.md) - OIDC, OAuth2, JWT, SAML, Passkeys, Entra, DualAuth
- [Authorization/](./Authorization/README.md) - RBAC/ABAC/ReBAC, ASP.NET Core policies
- [Cryptography/](./Cryptography/README.md) - hashing, AES-GCM, RSA/ECDSA, KDFs
- [DataProtection/](./DataProtection/README.md) - ASP.NET Core Data Protection API
- [SecretsManagement/](./SecretsManagement/README.md) - vaults + managed identity
- [OWASP/](./OWASP/README.md) - Top 10 with .NET fixes
- [ThreatModeling/](./ThreatModeling/README.md) - STRIDE, DREAD, sample model
- [SupplyChain/](./SupplyChain/README.md) - SBOM, Sigstore, SLSA
- [ZeroTrust/](./ZeroTrust/README.md) - principles + .NET application

## See also

- [.NET 2026 senior/architect roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [OWASP Top 10 (2021)](https://owasp.org/Top10/)
- [Microsoft Security best practices](https://learn.microsoft.com/azure/security/fundamentals/best-practices-and-patterns)
- [NIST SP 800-63 Digital Identity Guidelines](https://pages.nist.gov/800-63-3/)
