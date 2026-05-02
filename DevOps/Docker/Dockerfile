# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore (cache-friendly).
COPY *.sln ./
COPY src/Orders.Api/*.csproj src/Orders.Api/
COPY Directory.Packages.props .
RUN dotnet restore src/Orders.Api/Orders.Api.csproj --locked-mode

# Build & publish.
COPY src/Orders.Api/. src/Orders.Api/
RUN dotnet publish src/Orders.Api/Orders.Api.csproj \
    -c Release -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
USER app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_PRINT_TELEMETRY_MESSAGE=false \
    DOTNET_NOLOGO=true
ENTRYPOINT ["dotnet", "Orders.Api.dll"]
