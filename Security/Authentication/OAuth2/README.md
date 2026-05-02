# OAuth 2.0 / 2.1

> Delegated authorization framework. Issues access tokens to clients on behalf of resource owners.

## Core Concepts

- **Roles**: Resource Owner (user), Client, Authorization Server, Resource Server.
- **Token types**: `access_token` (mandatory), `refresh_token` (optional). OIDC adds `id_token`.
- **Scopes**: capability strings (`orders:read`); the AS issues tokens scoped to what was approved.
- **Audience (`aud`)**: which API the token is for. Validate it.
- **Confidential vs public client**: confidential = has secret (server-side); public = cannot keep a secret (SPA, mobile).
- OAuth **2.1** consolidates best-current-practice: PKCE required, implicit and ROPG removed.

## Grant Types

| Grant | When | Notes |
| --- | --- | --- |
| Authorization Code + PKCE | User-facing apps (web, SPA, mobile) | Default choice |
| Client Credentials | M2M / daemon | No user; client_id/secret or cert |
| Device Code | Smart TV, CLI | User authenticates on second device |
| Refresh Token | Renew access | Rotate; bind to client |
| ~~Implicit~~ | Deprecated | Tokens in URL fragment - leak risk |
| ~~Resource Owner Password~~ | Deprecated | App handles password directly |

## "To Be Dangerous" Cheatsheet

- Use **Client Credentials** for backend-to-backend; one identity per service.
- Use **least-privilege scopes**; prefer audience-bound scopes (`api://orders/orders.read`).
- **Cache** access tokens until ~5 min before expiry; don't request a new token per call.
- For high-risk APIs, pin tokens to client via **mTLS** or **DPoP**.
- Never put secrets or tokens in URLs or logs.

## Quick Reference

Client credentials request:

```
POST /oauth2/v2.0/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=<id>
&client_secret=<secret>
&scope=api://orders/.default
```

Response:

```json
{ "token_type": "Bearer", "expires_in": 3600, "access_token": "eyJ..." }
```

## Common Pitfalls

- Treating OAuth as authentication. ID tokens (OIDC) authenticate; access tokens authorize.
- Storing client secrets in mobile/SPA apps - use PKCE + public client + a backend if you need secrets.
- Forgetting to revalidate tokens after key rotation.
- Mixing audiences: never reuse a token issued for service A to call service B.

## Examples in this folder

- [ClientCredentialsExample.cs](./ClientCredentialsExample.cs) - fetch a token + call a protected API
- [TokenCache.cs](./TokenCache.cs) - simple in-memory token cache

## See also

- [OpenIdConnect](../OpenIdConnect/README.md)
- [JWT](../JWT/README.md)
- [OAuth 2.1 draft](https://datatracker.ietf.org/doc/draft-ietf-oauth-v2-1/)
- [OAuth 2.0 BCP (RFC 9700)](https://datatracker.ietf.org/doc/html/rfc9700)
