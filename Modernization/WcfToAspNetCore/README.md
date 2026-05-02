# WCF → ASP.NET Core

> WCF is not the future. Choose the modern target by access pattern: REST for external, gRPC for internal high-throughput.

## Decision

| Old WCF binding / use | Modern target |
|---|---|
| `basicHttpBinding` (HTTP/SOAP) | REST + OpenAPI 3.1 |
| `wsHttpBinding` (security/transactions) | OIDC + JWT + saga (no WS-AT) |
| `netTcpBinding` (binary, internal) | **gRPC** |
| `netNamedPipeBinding` (in-machine) | gRPC over UDS, or IPC via `NamedPipeServerStream` |
| Duplex callbacks | SignalR or gRPC bi-directional |
| Reliable messaging | Service Bus / Kafka + outbox/inbox |

## Compatibility bridge: CoreWCF

[CoreWCF](https://github.com/CoreWCF/CoreWCF) re-implements most WCF surface on top of ASP.NET Core. Use it to keep legacy clients alive while you redesign.

## Side-by-side

**WCF service:**
```csharp
[ServiceContract]
public interface IOrders
{
    [OperationContract] Guid PlaceOrder(Guid userId, decimal amount);
}

public class OrdersService : IOrders { /* ... */ }
```

**ASP.NET Core gRPC equivalent (`orders.proto`):**
```proto
syntax = "proto3";
service Orders {
  rpc PlaceOrder(PlaceOrderRequest) returns (PlaceOrderResponse);
}
message PlaceOrderRequest { string user_id = 1; double amount = 2; }
message PlaceOrderResponse { string order_id = 1; }
```

```csharp
public sealed class OrdersService : Orders.OrdersBase
{
    public override Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest req, ServerCallContext ctx)
        => Task.FromResult(new PlaceOrderResponse { OrderId = Guid.NewGuid().ToString() });
}
```

## "To Be Dangerous" Cheatsheet

| Need | Replacement |
|---|---|
| WSDL → OpenAPI | `Microsoft.AspNetCore.OpenApi`, NSwag |
| WS-Security → JWT | `Microsoft.Identity.Web` + Entra |
| Faults → ProblemDetails | `Results.Problem(...)` |
| MEX endpoint | drop |
| Duplex callbacks | SignalR (`Hub`/`HubConnection`) |

## Common Pitfalls

- 1:1 operation port to REST endpoint → ugly URLs. Redesign as resources.
- WS-Security binding to a custom `IPasswordValidator` → migrate users to OIDC, not the validator.
- Streaming over WCF → gRPC server-streaming or chunked HTTP.
- Client SDK regeneration churn — keep WCF endpoint alive via CoreWCF until clients move.

## See also

- [../SoapToRest](../SoapToRest/) · [../../BackEnd/CSharp/MinimalApi](../../BackEnd/CSharp/MinimalApi/) · [../DotNetFrameworkToNet10](../DotNetFrameworkToNet10/)
