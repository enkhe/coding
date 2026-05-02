# BlazorServer

> Components run on the server; UI events stream over a SignalR circuit.

## Core Concepts

- **Circuit** — one SignalR connection per active tab; scoped DI lives here
- **`OwningComponentBase`** — gives a component its own DI scope (use for `DbContext`)
- **State** — server-side; survives reloads only via persistent storage
- **Latency** — every event is a round-trip; design accordingly

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Scoped DbContext | inherit `OwningComponentBase<TService>` | EF Core in components |
| Background updates | `InvokeAsync(StateHasChanged)` | timers, hubs, channels |
| Detect disconnect | implement `CircuitHandler` | log session ends |
| Throttle UI input | debounce with `PeriodicTimer` | search-as-you-type |
| Prerender + interactive | `@rendermode InteractiveServer` (default `prerender:true`) | SEO + fast TTFB |

## Quick Reference

```razor
@inherits OwningComponentBase<MyDbContext>

@code {
    protected override async Task OnInitializedAsync()
    {
        var items = await Service.Items.ToListAsync();
    }
}
```

## Common Pitfalls

- Storing huge state on the server — multiplies by user count
- Long-running CPU work blocks the circuit — offload to a background service
- Capturing `DbContext` from singleton scope — use `OwningComponentBase`
- Forgetting `InvokeAsync` when updating UI from a non-UI thread

## Examples in this folder

- [`Counter.razor`](Counter.razor) — minimal interactive component
- [`WeatherList.razor`](WeatherList.razor) — owning scope + DbContext pattern
- [`Program.cs`](Program.cs) — server registration

## See also

- [../README.md](../README.md) — render modes overview
- [../../Performance](../../Performance/README.md) — circuit cost
