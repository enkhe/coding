# Azure Functions

> Event-driven serverless compute, isolated worker model on .NET 10.

## Core Concepts

- **Triggers**: HTTP, Timer (NCRONTAB), Queue Storage, Service Bus, Blob, Event Grid, Event Hub, Cosmos Change Feed, Kafka.
- **Bindings**: input/output to Storage, Service Bus, Blob, Cosmos, Table, etc.
- **Hosting plans**: Consumption (scale-to-zero), Flex Consumption (always-warm option, VNet), Premium (always-on, VNet, larger SKUs), App Service, Container Apps.
- **Isolated worker** (.NET 10): function host runs in a separate process; full DI, middleware, and modern .NET features.
- **Durable Functions**: orchestrator + activities + entities for stateful workflows (fan-out/fan-in, monitor, human approval).

## "To Be Dangerous" Cheatsheet

- Use **Flex Consumption** for prod HTTP/event apps that need VNet + always-warm-able instances.
- Trigger throughput tuning: `host.json` `extensions.serviceBus.maxConcurrentCalls`, `prefetchCount`.
- Always set **`FUNCTIONS_WORKER_RUNTIME=dotnet-isolated`** + targeted `.NET 10`.
- Identity-based connections: `ServiceBus__fullyQualifiedNamespace`, `Storage__accountName` -> uses MI.
- For long workflows, use **Durable Functions** orchestrator (don't sleep in HTTP triggers).
- Avoid **fan-out without throttling** -> downstream meltdown.

## Quick Reference

- NCRONTAB: `0 */5 * * * *` = every 5 minutes (6 fields, includes seconds).
- Cold start mitigation: Premium / Flex always-ready, AOT-friendly small assemblies.
- `host.json` controls per-extension config; `local.settings.json` for local dev only.

## Common Pitfalls

- Mixing in-process and isolated -> in-process is deprecated for new work.
- Storing connection strings in app settings instead of identity-based connections.
- Long-running HTTP triggers (>230s) -> use Durable.
- Deploying to Consumption then complaining about cold start.

## Examples in this folder

- [HttpTriggerFunction.cs](./HttpTriggerFunction.cs)
- [host.json](./host.json)

## See also

- [Service Bus](../ServiceBus/README.md)
- [Storage](../Storage/README.md)
- [Container Apps](../ContainerApps/README.md)
