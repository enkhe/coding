# Web API (controllers)

> Controller-based ASP.NET Core. Pick when you need attribute filters, conventions, or are migrating from older codebases. Otherwise prefer Minimal APIs.

## Quick Reference

```csharp
[ApiController]
[Route("orders")]
[Authorize]
[Produces("application/json")]
public sealed class OrdersController(AppDb db) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType<Order>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var order = await db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [ProducesResponseType<Order>(201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> Create([FromBody] CreateOrder cmd, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var order = new Order(Guid.NewGuid(), cmd.UserId, cmd.Amount);
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }
}
```

## When to choose

| Choose Web API when… | Choose Minimal API when… |
|---|---|
| Heavy use of action filters / model binders | Greenfield, smaller APIs |
| Conventional routes (CRUD-heavy) | Vertical slices |
| Migrating from MVC | Want least ceremony |
| Need attribute-based versioning | Use `MapGroup` for organization |

## Common Pitfalls

- Returning entities directly → API coupled to schema; use DTOs
- Forgetting `[ApiController]` → no automatic 400 on model validation
- Manually validating `ModelState` everywhere → `[ApiController]` does it for you
- Mixing controllers and Minimal APIs without convention — pick one per service

## See also

- [../MinimalApi](../MinimalApi/) · [../AspNetCore](../AspNetCore/)
