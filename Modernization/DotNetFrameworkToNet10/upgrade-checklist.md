# Upgrade Checklist — .NET Framework → .NET 10

## Pre-flight

- [ ] Inventory every project, its TFM, third-party dependencies
- [ ] Run `upgrade-assistant analyze` on the solution
- [ ] Identify packages without a .NET 10-compatible version → find replacements before starting
- [ ] Establish a green CI baseline before changing anything

## SDK-style csproj

- [ ] `try-convert -p <project>.csproj` for each project
- [ ] Remove `packages.config` references; let SDK manage `PackageReference`
- [ ] Move `AssemblyInfo` content into csproj properties

## Libraries

- [ ] Multi-target where consumers haven't migrated: `<TargetFrameworks>net10.0;net48</TargetFrameworks>`
- [ ] Replace `WebClient` with `HttpClient` via `IHttpClientFactory`
- [ ] Replace `ConfigurationManager` with `IConfiguration`
- [ ] Replace `BinaryFormatter` payloads — migrate stored data **before** the cutover
- [ ] Replace `System.Drawing.Common` for non-Windows with `SkiaSharp` / `ImageSharp`

## ASP.NET → ASP.NET Core

- [ ] Plan strangler routes via YARP
- [ ] Map global filters / handlers / modules to middleware
- [ ] Auth: replace `[Authorize]` MVC filter, OWIN OIDC; modernize to `Microsoft.Identity.Web` + Entra
- [ ] Anti-forgery, CORS, error pages — opt in explicitly
- [ ] Session state: avoid; use distributed cache or tokens

## EF6 → EF Core 10

- [ ] Re-evaluate model — change tracking, lazy loading defaults differ
- [ ] Remove EDMX → use code-first
- [ ] Migrations: do not import; create a baseline migration matching prod

## WCF

- [ ] Plan REST/gRPC replacement
- [ ] Or run `CoreWCF` for time-bounded compatibility
- [ ] Modernize auth: WS-Security → JWT/OIDC

## Validation

- [ ] Architecture tests in CI (NetArchTest) for layer rules
- [ ] Performance regression suite (BenchmarkDotNet) on hot paths
- [ ] Load test (k6 / NBomber) on the cutover route
- [ ] Telemetry parity with legacy (errors, latency, throughput)

## Cutover

- [ ] Dual-running with YARP
- [ ] Per-route cutover with quick rollback
- [ ] Customer-visible error budgets defined
- [ ] On-call ready, runbook in place
