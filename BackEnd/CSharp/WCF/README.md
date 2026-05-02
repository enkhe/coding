# WCF (legacy reference)

> Windows Communication Foundation. **For maintaining legacy services only.** New work: REST + OpenAPI or gRPC. See [`Modernization/WcfToAspNetCore`](../../../Modernization/WcfToAspNetCore/).

## Concepts (just enough to maintain it)

- **`[ServiceContract]` interface** — the public surface
- **`[OperationContract]`** — exposed methods
- **Bindings** — `BasicHttpBinding`, `WSHttpBinding`, `NetTcpBinding`, `NetNamedPipeBinding`
- **Behaviors** — service behaviors, endpoint behaviors (security, throttling)
- **Endpoint** = address + binding + contract (ABC)

## Skeleton

```csharp
[ServiceContract]
public interface IOrders
{
    [OperationContract]
    Guid PlaceOrder(Guid userId, decimal amount);
}

public class OrdersService : IOrders
{
    public Guid PlaceOrder(Guid userId, decimal amount) => Guid.NewGuid();
}

// app.config
//   <system.serviceModel>
//     <services>
//       <service name="Orders.OrdersService">
//         <endpoint address="" binding="basicHttpBinding" contract="Orders.IOrders" />
//       </service>
//     </services>
//   </system.serviceModel>
```

## Hosting on .NET 10 — CoreWCF

[`CoreWCF`](https://github.com/CoreWCF/CoreWCF) re-implements most WCF surface for ASP.NET Core. Use during a migration window.

## Common Pitfalls (in legacy)

- `wsHttpBinding` + WS-Security in modern infrastructure — fragile; plan migration to JWT/OIDC
- Forgetting `MaxReceivedMessageSize` defaults — 64KB silent truncation on payloads
- Throttling defaults too low for prod traffic
- Binding mismatch between client and service — opaque errors

## See also

- [../../../Modernization/WcfToAspNetCore](../../../Modernization/WcfToAspNetCore/) · [../../../Modernization/SoapToRest](../../../Modernization/SoapToRest/)
