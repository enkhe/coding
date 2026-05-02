# Demos

> Small, deliberate full-stack demos. One feature, end-to-end, well-named.

## Naming convention

`YYYY-MM-NN-<topic>/` — date prefix keeps them ordered, NN is a 2-digit counter for the month.

## Demo ideas (start here)

- **`2026-05-01-rag-with-citations`** — Aspire + Blazor + Postgres + pgvector + Microsoft.Extensions.AI; user asks question, gets answer + cited sources.
- **`2026-05-02-realtime-orders`** — Blazor Server + SignalR; multiple tabs see new orders pop in.
- **`2026-05-03-multi-tenant-saas`** — row-level security in Postgres + Entra External ID + tenant claim.
- **`2026-05-04-strangler-yarp`** — legacy MVC + new Minimal API behind YARP, gradual cutover.
- **`2026-05-05-passkeys-only-login`** — passwordless login with FIDO2.
- **`2026-05-06-aot-edge-cli`** — Native AOT CLI with Spectre.Console.

## What every demo needs

- README explaining the why and architecture
- Runnable in one command (`docker compose up`, `dotnet aspire run`, or `npm dev`)
- One test for the happy path
- Screenshot of the working UI (if applicable)

## See also

- [../Sandbox](../Sandbox/) — earlier-stage experiments
- [../Portfolio](../Portfolio/) — production-grade flagship work
