# Authentication

> Who are you? Federated identity, tokens, and the modern AuthN landscape.

## Core Concepts

- **AuthN** verifies identity; **AuthZ** decides access. Don't conflate.
- **OAuth 2.0** is a delegation/authorization framework (access tokens). It is not authentication.
- **OpenID Connect (OIDC)** sits on OAuth 2.0 and adds an **ID token** (JWT) so you can authenticate.
- **SAML 2.0** is the legacy enterprise federation protocol (XML, browser POST). Use only when forced.
- **JWT** is a token format. Bearer JWT access tokens are common but not mandatory (opaque tokens exist).
- **Passkeys / FIDO2 / WebAuthn**: phishing-resistant public-key auth. Replaces passwords for the user-facing leg.
- **Federation**: trust between an Identity Provider (IdP) and a Service Provider/Relying Party (SP/RP).

## "To Be Dangerous" Cheatsheet

- New web apps -> **OIDC Authorization Code + PKCE** (works for SPAs, mobile, server-rendered).
- M2M / daemon -> **OAuth2 Client Credentials**.
- API receives **access token**; validates `iss`, `aud`, `exp`, `nbf`, `sig`, `kid`.
- ID token is for the **client**, never sent to APIs.
- Refresh tokens: rotated, server-stored or sender-constrained (DPoP / mTLS).
- Don't use **implicit** or **resource owner password** grants in 2026.
- Prefer asymmetric (RS256/ES256) signing; clients fetch keys from JWKS endpoint.

## Decision Tree

```
Need user login?
  -> Customer (B2C/CIAM)?      Entra External ID, Auth0, Okta CIAM
  -> Workforce (employees)?    Entra ID, Okta, Ping
  -> Stuck with old enterprise IdP that only speaks SAML?  SAML SP
  -> Greenfield mobile/SPA?    OIDC + PKCE
Need M2M?
  -> Client credentials grant
Need passwordless?
  -> Passkeys (FIDO2/WebAuthn)
Mid-migration from legacy ASP.NET?
  -> DualAuth pattern (legacy IdP + OIDC bridged)
```

## Quick Reference

| Flow | Use case | Tokens |
| --- | --- | --- |
| Auth Code + PKCE | Web/SPA/Mobile user login | ID + Access + Refresh |
| Client Credentials | Service-to-service | Access |
| Device Code | TVs, CLI tools | ID + Access (+ Refresh) |
| Refresh Token | Renew without re-login | Access (+ rotated Refresh) |
| Hybrid (OIDC) | Server-side apps wanting code + id_token | ID + Code |

## Common Pitfalls

- Sending **ID tokens** to APIs (use access tokens).
- Validating only signature, skipping `aud`/`iss` -> token confusion attacks.
- Storing refresh tokens in `localStorage` (XSS-exfiltratable). Prefer http-only cookie or BFF pattern.
- Using `HS256` across organizations (shared secret).
- Skipping PKCE because "we are confidential client" - do it anyway.

## Examples in this folder

- [OpenIdConnect/](./OpenIdConnect/README.md) - OIDC and ASP.NET Core wiring
- [OAuth2/](./OAuth2/README.md) - grants and client credentials sample
- [JWT/](./JWT/README.md) - structure and validation
- [SAML/](./SAML/README.md) - legacy integration notes
- [Passkeys/](./Passkeys/README.md) - FIDO2 / WebAuthn
- [Entra/](./Entra/README.md) - Microsoft Entra ID + External ID
- [DualAuth/](./DualAuth/README.md) - hybrid migration pattern

## See also

- [Authorization](../Authorization/README.md)
- [JWT debugger](https://jwt.io)
- [OAuth 2.1 draft](https://datatracker.ietf.org/doc/draft-ietf-oauth-v2-1/)
- [OpenID Connect Core 1.0](https://openid.net/specs/openid-connect-core-1_0.html)
