# OpenID Connect (OIDC)

> Authentication layer on top of OAuth 2.0. Adds an ID token (JWT) and standard claims.

## Core Concepts

- **ID Token** (JWT) - identifies the user (`sub`, `name`, `email`, `iat`, `auth_time`, `nonce`).
- **Access Token** - calls APIs; OIDC borrows OAuth2 here.
- **Refresh Token** - renews access without re-prompt.
- **Discovery document** - `https://issuer/.well-known/openid-configuration` exposes endpoints + JWKS URI.
- **JWKS** - JSON Web Key Set; rotating public keys for signature verification.
- **Scopes** - `openid` (required), `profile`, `email`, custom scopes for APIs.
- **Claims** - assertions inside the ID token; `claims` request parameter for fine-grained.
- **PKCE** (Proof Key for Code Exchange) - mandatory in OAuth 2.1; mitigates code interception.

## Authorization Code + PKCE Flow

```
Browser/SPA       Authorization Server         Resource API
  |                       |                         |
  | 1. /authorize?code_challenge=S256(...)          |
  |---------------------->|                         |
  |       2. login + consent                        |
  |<----------------------|                         |
  |   redirect ?code=...                            |
  | 3. /token  code+code_verifier                   |
  |---------------------->|                         |
  |   id_token + access_token + refresh_token       |
  |<----------------------|                         |
  | 4. GET /api  Bearer access_token                |
  |------------------------------------------------>|
  |                       | 5. validate via JWKS    |
```

## "To Be Dangerous" Cheatsheet

- Always **PKCE**, even for confidential clients.
- Validate `nonce` from ID token == nonce you sent.
- Use `state` to prevent CSRF on the redirect.
- Use **back-channel logout** for SSO sign-out.
- Use **Microsoft.Identity.Web** for ME-ID; raw `AddOpenIdConnect` for generic IdPs.
- Cache OIDC metadata; respect cache headers, refresh on key rotation.

## Quick Reference

| Endpoint | Purpose |
| --- | --- |
| `/.well-known/openid-configuration` | Discovery |
| `/authorize` | User login + consent |
| `/token` | Code -> tokens |
| `/userinfo` | Extra claims |
| `/jwks` | Public keys |
| `/end_session` | Logout |

## Common Pitfalls

- Forgetting to map inbound claims (`MapInboundClaims = false` is preferred for clarity in .NET 10).
- Trusting `email` as identity - use `sub`.
- Not validating `aud` on the ID token (must equal client_id).
- Letting access tokens leak into URLs/logs.

## Examples in this folder

- [Program.cs](./Program.cs) - ASP.NET Core 10 wiring `AddOpenIdConnect()` + claims transformation
- [ClaimsTransformer.cs](./ClaimsTransformer.cs) - enrich ClaimsPrincipal post-auth

## See also

- [OAuth2](../OAuth2/README.md)
- [JWT](../JWT/README.md)
- [Entra](../Entra/README.md)
- [OIDC spec](https://openid.net/specs/openid-connect-core-1_0.html)
