# Package Managers

> One screen to remember them all.

## Map

| Manager | Lang | Project file | Lock | Install command |
|---|---|---|---|---|
| **NuGet** | .NET | `*.csproj` (`PackageReference`) | `packages.lock.json` | `dotnet add package X` |
| **npm** | Node | `package.json` | `package-lock.json` | `npm install X` |
| **pnpm** | Node | `package.json` | `pnpm-lock.yaml` | `pnpm add X` |
| **yarn** | Node | `package.json` | `yarn.lock` | `yarn add X` |
| **pip** | Python | `pyproject.toml` / `requirements.txt` | (none/manual) | `pip install X` |
| **uv** | Python (modern) | `pyproject.toml` | `uv.lock` | `uv add X` |
| **poetry** | Python | `pyproject.toml` | `poetry.lock` | `poetry add X` |
| **cargo** | Rust | `Cargo.toml` | `Cargo.lock` | `cargo add X` |
| **go mod** | Go | `go.mod` | `go.sum` | `go get X` |
| **brew** | macOS / Linux apps | n/a | `Brewfile` (bundle) | `brew install X` |
| **choco** | Windows apps | n/a | n/a | `choco install X` |
| **winget** | Windows apps | n/a | n/a | `winget install X` |
| **apt** | Debian/Ubuntu | n/a | n/a | `apt install X` |

## Lockfile discipline

| Lock | Commit? | CI flag |
|---|---|---|
| `packages.lock.json` | yes | `dotnet restore --locked-mode` |
| `package-lock.json` | yes | `npm ci` |
| `pnpm-lock.yaml` | yes | `pnpm install --frozen-lockfile` |
| `yarn.lock` | yes | `yarn install --immutable` |
| `uv.lock` | yes | `uv sync --frozen` |
| `Cargo.lock` | apps yes / libs no | `cargo build --locked` |
| `go.sum` | yes | `go build -mod=readonly` |

## Central versioning (.NET)

`Directory.Packages.props` at the repo root:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageVersion Include="Polly.Core" Version="8.5.0" />
    <PackageVersion Include="Serilog.AspNetCore" Version="9.0.0" />
  </ItemGroup>
</Project>
```

In each csproj: `<PackageReference Include="Microsoft.EntityFrameworkCore" />` (no Version).

## Common Pitfalls

- `npm install` (mutates lock) in CI → use `npm ci`
- Two managers competing in one repo (npm + yarn) → pick one
- No source mapping in NuGet → dependency confusion attacks (see [`Security/SupplyChain`](../../Security/SupplyChain/))
- Letting `~` ranges drift → reproducible builds are not "build twice and pray"

## See also

- [../../Security/SupplyChain](../../Security/SupplyChain/)
