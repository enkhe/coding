# Web Forms → Blazor

> ASP.NET Web Forms is in extended-only servicing. Blazor is the .NET 2026 stateful UI default.

## Mental model shift

| Web Forms | Blazor |
|---|---|
| `Page_Load`, `IsPostBack` | Component lifecycle (`OnInitialized`, `OnParametersSet`) |
| ViewState | Component state (`@code` block) |
| Postback / `__doPostBack` | SignalR messages (Server) or local state (WASM) |
| `<asp:GridView>` | `<QuickGrid>` or custom components |
| Master pages | Layout components (`@inherits LayoutComponentBase`) |
| User controls (`*.ascx`) | Components (`*.razor`) |
| `Eval()` data binding | `@bind` two-way; `@bind:event="oninput"` |
| Code-behind | `@code` block (or partial class `*.razor.cs`) |

## Strangler approach

1. Stand up a Blazor app (Server or Auto) alongside.
2. Put YARP in front. New routes → Blazor; existing `*.aspx` → Web Forms.
3. Migrate page-by-page. Auth is the hard part — see [DualAuth](../../Security/Authentication/DualAuth/).

## Side-by-side

**Web Forms (`Default.aspx.cs`):**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack) { Items.DataSource = Repo.Get(); Items.DataBind(); }
}
protected void Add_Click(object sender, EventArgs e) { Repo.Add(NameTextBox.Text); }
```

**Blazor (`Default.razor`):**
```razor
@page "/"
@inject IRepo Repo

<QuickGrid Items="@items" />
<input @bind="name" /><button @onclick="Add">Add</button>

@code {
    IQueryable<Item> items = default!;
    string name = "";
    protected override void OnInitialized() => items = Repo.Query();
    void Add() { Repo.Add(name); items = Repo.Query(); name = ""; }
}
```

## Common Pitfalls

- Trying to map `IsPostBack` 1:1 → stop thinking in postbacks; rethink the page
- ViewState bridging → don't; redesign page state
- Mixing render modes accidentally — know your Server / WASM / Auto choice
- Forms-auth cookies → shift to OIDC during migration

## See also

- [../../FrontEnd/Blazor](../../FrontEnd/Blazor/) · [../../Security/Authentication/DualAuth](../../Security/Authentication/DualAuth/)
