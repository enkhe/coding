# Supply Chain Security

> SBOM, signing, and provenance — defenses against compromised dependencies and build pipelines.

## Core Concepts

- **SBOM** (Software Bill of Materials) — inventory of every package shipped. Format: **CycloneDX** or **SPDX**.
- **Sigstore / cosign** — keyless signing of artifacts (containers, SBOMs) using OIDC identities.
- **SLSA** (Supply-chain Levels for Software Artifacts) — maturity framework. SLSA L3 = signed, hermetic, isolated builds.
- **Provenance** — verifiable record of *who built what, where, from which source*.
- **Reproducible builds** — same source + same toolchain → same bytes. Detects build tampering.
- **Pinned dependencies** — lock files (NuGet `packages.lock.json`, npm `package-lock.json`).
- **Restricted feeds** — internal NuGet/npm feeds with allow-list; block public feed pulls in CI.
- **NuGet package source mapping** — explicit mapping of package prefix → source.

## "To Be Dangerous" Cheatsheet

| Need | Tool / API |
|---|---|
| Generate SBOM (.NET) | `dotnet CycloneDX MyApp.csproj -o ./sbom` |
| Generate SBOM (npm) | `npx @cyclonedx/cyclonedx-npm --output-file sbom.json` |
| Sign image keylessly | `cosign sign --yes <registry>/<image>:<tag>` |
| Verify signature | `cosign verify --certificate-identity-regexp ... --certificate-oidc-issuer-regexp ... <image>` |
| Sign SBOM | `cosign attest --predicate sbom.json --type cyclonedx <image>` |
| Reproducible build | `<Deterministic>true</Deterministic>` in csproj |
| Lock files | `dotnet restore --use-lock-file` |
| GitHub OIDC to cloud | `id-token: write` in workflow + cloud-side trust policy |

## Quick Reference (GitHub Actions snippet)

```yaml
permissions:
  id-token: write    # for OIDC (cosign keyless, cloud auth)
  contents: read
  packages: write
  attestations: write

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '10.0.x' }

      - run: dotnet restore --locked-mode
      - run: dotnet build --no-restore -c Release
      - run: dotnet test --no-build -c Release

      - name: Generate CycloneDX SBOM
        run: |
          dotnet tool install --global CycloneDX
          dotnet CycloneDX src/Orders.Api/Orders.Api.csproj -o ./artifacts -j

      - name: Build & push image
        id: push
        uses: docker/build-push-action@v6
        with: { push: true, tags: 'ghcr.io/contoso/orders:${{ github.sha }}' }

      - uses: sigstore/cosign-installer@v3
      - name: Sign image
        run: cosign sign --yes ghcr.io/contoso/orders@${{ steps.push.outputs.digest }}

      - name: Attest SBOM
        run: |
          cosign attest --yes --predicate ./artifacts/bom.json \
            --type cyclonedx ghcr.io/contoso/orders@${{ steps.push.outputs.digest }}
```

## NuGet package source mapping

```xml
<!-- nuget.config -->
<configuration>
  <packageSources>
    <add key="contoso-internal" value="https://contoso.pkgs.visualstudio.com/_packaging/internal/nuget/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="contoso-internal">
      <package pattern="Contoso.*" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
```

## Common Pitfalls

- SBOM generated but never attested or verified at consume time.
- Deps not pinned → "yesterday's build is gone."
- Public feeds + private feeds with the same package name → dependency confusion attacks.
- Long-lived publish secrets → use OIDC.
- Signing only the image, not the SBOM → can't trust the inventory.

## Examples in this folder

- [`generate-sbom.sh`](generate-sbom.sh) — local SBOM gen
- [`cosign-sign.yaml`](cosign-sign.yaml) — sign + attest SBOM in GitHub Actions
- [`nuget.config`](nuget.config) — source mapping

## See also

- [../ThreatModeling](../ThreatModeling/) · [../../DevOps/GitHubActions](../../DevOps/GitHubActions/)
