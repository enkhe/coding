# Docker

> Multi-stage builds, distroless / chiseled base images, AOT, scanning.

## Core Concepts

- **Layers** — every `RUN`/`COPY` is a layer; cache reuses identical layers.
- **Multi-stage** — separate `build` stage from `runtime` stage; final image carries only artifacts.
- **Base images** — `mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled` for slim attack surface.
- **Distroless / chiseled** — no shell, no package manager. Smaller, harder to exploit.
- **`.dockerignore`** — exclude `bin/`, `obj/`, `.git`, secrets.
- **Image signing + SBOM** — see [`Security/SupplyChain`](../../Security/SupplyChain/).

## "To Be Dangerous" Cheatsheet

| Need | Pattern |
|---|---|
| Cache restore | `COPY *.csproj .` then `RUN dotnet restore` before `COPY . .` |
| Multi-arch build | `docker buildx build --platform linux/amd64,linux/arm64` |
| Smallest .NET runtime | `mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled-extra` (AOT) |
| Smallest ASP.NET | `mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled` |
| Non-root | `USER app` (chiseled images include `app` user) |
| Healthcheck | `HEALTHCHECK --interval=30s CMD curl -f http://localhost:8080/health/live` |

## Quick Reference

```dockerfile
# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.sln ./
COPY src/Orders.Api/*.csproj src/Orders.Api/
RUN dotnet restore src/Orders.Api/Orders.Api.csproj --locked-mode

COPY src/Orders.Api/. src/Orders.Api/
RUN dotnet publish src/Orders.Api/Orders.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
USER app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Orders.Api.dll"]
```

## Common Pitfalls

- `COPY . .` before `dotnet restore` → cache miss on every code change
- `latest` tag → non-reproducible builds
- Running as root in container → mount escapes are easier
- Forgetting `.dockerignore` → `bin/`/`obj/` shipped in image

## Examples in this folder

- [`Dockerfile`](Dockerfile) — multi-stage ASP.NET Core 10
- [`Dockerfile.aot`](Dockerfile.aot) — Native AOT
- [`.dockerignore`](.dockerignore)

## See also

- [../Kubernetes](../Kubernetes/) · [../../Security/SupplyChain](../../Security/SupplyChain/) · [../../BackEnd/CSharp/NativeAOT](../../BackEnd/CSharp/NativeAOT/)
