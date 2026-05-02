# ASP.NET Web Forms (legacy)

> Legacy reference. Web Forms is in extended-only servicing on .NET Framework. Use this folder to support migrations; new work goes to [Blazor](../Blazor/).

## Concepts (just enough to maintain)

- **Page lifecycle** — `Page_PreInit` → `Page_Init` → `LoadViewState` → `Page_Load` → events → `PreRender` → `SaveViewState` → `Render` → `Unload`
- **`IsPostBack`** — distinguish first request from postback
- **ViewState** — server-state round-tripped through hidden form fields (binary, encrypted)
- **Server controls** — `<asp:Button>`, `<asp:GridView>`, `<asp:Repeater>`, `<asp:UpdatePanel>` (partial postback via UpdatePanel + ScriptManager)
- **Master pages** — `<%@ Master %>` + `<asp:ContentPlaceHolder>` + content pages with `<asp:Content>`
- **User controls** — `*.ascx`
- **Code-behind** — partial class linked via `Inherits=` directive

## Skeleton (Default.aspx + .cs)

```aspx
<%@ Page Language="C#" Inherits="Orders.Default" CodeBehind="Default.aspx.cs" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="ph" runat="server">
  <asp:GridView ID="OrdersGrid" runat="server" AutoGenerateColumns="false">
    <Columns>
      <asp:BoundField DataField="Id" />
      <asp:BoundField DataField="Amount" DataFormatString="{0:C}" />
    </Columns>
  </asp:GridView>
  <asp:Button ID="AddBtn" runat="server" Text="Add" OnClick="AddBtn_Click" />
</asp:Content>
```

```csharp
public partial class Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) { OrdersGrid.DataSource = Repo.Get(); OrdersGrid.DataBind(); }
    }
    protected void AddBtn_Click(object sender, EventArgs e) { Repo.Add(); }
}
```

## Migration

See [`Modernization/WebFormsToBlazor`](../../Modernization/WebFormsToBlazor/).

## Common Pitfalls (during maintenance)

- ViewState bloat → disable on read-only controls
- Async events without `Page.RegisterAsyncTask` → swallowed exceptions
- `Response.Redirect` in async paths → `endResponse: false` to avoid `ThreadAbortException`
- Mixing UpdatePanel (partial postback) with non-AJAX controls → strange race conditions

## See also

- [../Blazor](../Blazor/) · [../../Modernization/WebFormsToBlazor](../../Modernization/WebFormsToBlazor/)
