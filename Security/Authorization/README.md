# Authorization (AuthZ)

> What an authenticated user is allowed to do. RBAC, ABAC, ReBAC, policy-based.

## Models

| Model | Idea | Example |
|---|---|---|
| **RBAC** (role-based) | User → Role → Permission | "admins can delete" |
| **ABAC** (attribute-based) | Decide by attributes (user, resource, env) | "owner of doc can edit; same-org can read" |
| **ReBAC** (relationship-based) | Decide by graph relationships | "can_view if user → member-of → group → has-access → doc" |
| **Policy-based** | Code-driven `IAuthorizationRequirement` | ASP.NET Core's native model |
| **OPA / Cedar** | External policy engine | Decoupled policy as code |

## ASP.NET Core 10 policy authorization

```csharp
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("orders:read",  p => p.RequireScope("Orders.Read"));
    o.AddPolicy("orders:write", p => p.RequireScope("Orders.Write"));
    o.AddPolicy("orders:owner", p => p.AddRequirements(new ResourceOwnerRequirement()));
});
builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerHandler>();

// Usage
app.MapDelete("/orders/{id:guid}", (Guid id) => Results.NoContent())
   .RequireAuthorization("orders:owner");
```

```csharp
public sealed class ResourceOwnerRequirement : IAuthorizationRequirement;

public sealed class ResourceOwnerHandler(IOrderRepository repo)
    : AuthorizationHandler<ResourceOwnerRequirement, Order>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext ctx, ResourceOwnerRequirement req, Order order)
    {
        var userId = Guid.Parse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (order.UserId == userId) ctx.Succeed(req);
    }
}
```

## OPA / Rego sketch

```rego
package orders.allow

default allow = false

allow {
  input.action == "read"
  input.subject.id == input.resource.owner_id
}
allow {
  input.action == "delete"
  "OrdersAdmin" in input.subject.roles
}
```

## Common Pitfalls

- Role explosion — too many roles, too few principles → migrate to ABAC/ReBAC
- AuthZ checks scattered across controllers + services → centralize via policies/handlers
- Trusting client-supplied scopes — always validate against issuer
- Forgetting **authorization boundaries** in queries (multi-tenant data) — use row-level security or always include tenant filter

## See also

- [Authentication](../Authentication/) · [../OWASP](../OWASP/) · [../ZeroTrust](../ZeroTrust/)
