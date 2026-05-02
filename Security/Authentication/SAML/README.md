# SAML 2.0

> Legacy XML-based federation. Integrate when forced (gov, education, big enterprise SSO). Prefer OIDC for new work.

## Core Concepts

- **Service Provider (SP)** = your app.
- **Identity Provider (IdP)** = ADFS, PingFederate, Shibboleth, Entra ID (yes, it speaks SAML too).
- **Assertion** = signed XML statement: who the user is, when, for whom.
- **Bindings**: HTTP-Redirect (request), HTTP-POST (response), Artifact (rare).
- **Profiles**: Web Browser SSO is the common one.
- **Trust** is established via metadata exchange (XML doc with certificates and endpoints).

## Flow (SP-initiated)

```
User -> SP /login -> redirect to IdP /SSO?SAMLRequest=...
IdP authenticates user
IdP -> POST SAMLResponse to SP /acs (Assertion Consumer Service)
SP validates signature, conditions, audience -> sets session
```

## "To Be Dangerous" Cheatsheet

- Always validate the **assertion signature** AND the **response signature** if present.
- Validate `Audience` (your SP entity ID), `NotBefore`/`NotOnOrAfter`, `Recipient`, `InResponseTo`.
- Use **certificate pinning** via metadata; do not trust system root for IdP signing.
- XML parsers must disable **DTD/XXE** and **external entities**.
- Reject `xmlns` smuggling; canonicalize before verifying signatures (XMLDSig).
- Prefer libraries: **Sustainsys.Saml2** or **ITfoxtec.Identity.Saml2** for ASP.NET Core.

## Sample Assertion (truncated)

See [SampleAssertion.xml](./SampleAssertion.xml).

## Quick Reference

| Element | Purpose |
| --- | --- |
| `<Issuer>` | Who issued the assertion (IdP) |
| `<Subject><NameID>` | User identifier |
| `<Conditions>` | Validity window + audience |
| `<AuthnStatement>` | When + how user authenticated |
| `<AttributeStatement>` | User claims |
| `<ds:Signature>` | XMLDSig over the assertion |

## Common Pitfalls

- Trusting the **assertion** but not validating its signature.
- XML signature wrapping attacks - move signed element via XPath; library bugs cause this.
- Logging full SAMLResponse - PII + tokens.
- Hardcoding IdP cert and not rotating with metadata.
- Treating SAML logout (SLO) as reliable; many IdPs implement it poorly.

## Examples in this folder

- [SampleAssertion.xml](./SampleAssertion.xml) - canonical assertion structure
- [Sustainsys.Saml2 wiring snippet](#aspnet-core-wiring-with-sustainsyssaml2)

## ASP.NET Core wiring with Sustainsys.Saml2

```csharp
// dotnet add package Sustainsys.Saml2.AspNetCore2
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "saml2";
    })
    .AddCookie()
    .AddSaml2(options =>
    {
        options.SPOptions.EntityId = new EntityId("https://app.example.com/saml2");
        options.IdentityProviders.Add(new IdentityProvider(
            new EntityId("https://idp.example.com"),
            options.SPOptions)
        {
            MetadataLocation = "https://idp.example.com/metadata",
            LoadMetadata = true
        });
    });
```

## See also

- [OpenIdConnect](../OpenIdConnect/README.md) (preferred for new work)
- [SAML 2.0 Core](https://docs.oasis-open.org/security/saml/v2.0/saml-core-2.0-os.pdf)
- [Sustainsys.Saml2](https://github.com/Sustainsys/Saml2)
