# SOAP → REST (or gRPC)

> SOAP is enterprise-stable but heavy. REST + OpenAPI is the default for new external APIs; gRPC for internal high-throughput.

## Mapping

| SOAP | REST equivalent |
|---|---|
| WSDL | OpenAPI 3.1 |
| XML envelope | JSON body |
| `<Action>` SOAP header | HTTP method + path |
| `<Fault>` | `ProblemDetails` (RFC 7807) + status code |
| WS-Security UsernameToken / X.509 | OAuth2/OIDC + JWT, mTLS |
| WS-Trust / SAML 1.1 | OIDC + JWT |
| WS-ReliableMessaging | Message bus (Service Bus / Kafka) + idempotency keys |
| WS-Transactions | Saga (orchestration / choreography) |

## Migration approach

1. **Catalog** every SOAP operation; mark each as command / query / event.
2. **Define REST shape** — resource-oriented; `POST /orders`, `GET /orders/{id}`, etc.
3. **Stand up REST** alongside SOAP; **gateway** routes legacy clients to SOAP, new clients to REST.
4. **Deprecate SOAP** per operation as clients migrate. Sunset by date.

## "To Be Dangerous" Cheatsheet

| Need | Pattern |
|---|---|
| Generate OpenAPI from EF/MVC | `Microsoft.AspNetCore.OpenApi` (.NET 10) |
| Strong types from OpenAPI | NSwag, Kiota, OpenAPI Generator |
| Side-by-side hosting | YARP routes `/soap/*` to legacy, `/api/*` to ASP.NET Core |
| Errors | `Results.Problem(...)` with consistent error shape |
| Versioning | `/v1/orders`, `Accept: application/vnd.contoso.v2+json`, or `?api-version=2026-01` |

## Side-by-side example

**SOAP (legacy):**
```xml
<soap:Envelope>
  <soap:Body>
    <PlaceOrder>
      <UserId>u-123</UserId>
      <Amount>9.99</Amount>
    </PlaceOrder>
  </soap:Body>
</soap:Envelope>
```

**REST (modern):**
```http
POST /api/orders
Content-Type: application/json
Authorization: Bearer <token>
Idempotency-Key: 4d1f...

{ "userId": "u-123", "amount": 9.99 }
```
Response:
```http
HTTP/1.1 201 Created
Location: /api/orders/8f3a...
Content-Type: application/json

{ "id": "8f3a...", "amount": 9.99 }
```

## Common Pitfalls

- 1:1 endpoint mapping → ugly URLs / chatty clients. Redesign for resources.
- Forgetting idempotency for retries (POST without Idempotency-Key) → duplicates.
- SOAP fault → REST 200 with error body. Use proper status codes.
- Stripping WS-Security with no replacement → unauthenticated APIs.

## Examples in this folder

- [`order.wsdl-snippet.xml`](order.wsdl-snippet.xml) — legacy contract
- [`OrdersEndpoints.cs`](OrdersEndpoints.cs) — modern Minimal API equivalent
- [`openapi.yaml`](openapi.yaml) — generated contract

## See also

- [../WcfToAspNetCore](../WcfToAspNetCore/) · [../../BackEnd/CSharp/MinimalApi](../../BackEnd/CSharp/MinimalApi/)
