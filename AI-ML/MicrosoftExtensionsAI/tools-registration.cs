// Multiple AIFunction tool registration with descriptions, async, and DI access.

using System.ComponentModel;
using Microsoft.Extensions.AI;

public sealed class CrmTools(HttpClient http)
{
    [Description("Look up customer by id. Returns name and tier.")]
    public async Task<string> GetCustomerAsync(
        [Description("Customer GUID")] string id,
        CancellationToken ct = default)
    {
        var resp = await http.GetStringAsync($"/customers/{id}", ct);
        return resp;
    }

    [Description("Create a support ticket and return its number.")]
    public async Task<int> CreateTicketAsync(
        [Description("Customer GUID")] string customerId,
        [Description("Short summary, < 200 chars")] string summary,
        CancellationToken ct = default)
    {
        var r = await http.PostAsJsonAsync("/tickets", new { customerId, summary }, ct);
        return (await r.Content.ReadFromJsonAsync<TicketDto>(cancellationToken: ct))!.Number;
    }
    private record TicketDto(int Number);
}

// Build tool list from instance methods
static IList<AITool> BuildTools(CrmTools crm) =>
[
    AIFunctionFactory.Create(crm.GetCustomerAsync),
    AIFunctionFactory.Create(crm.CreateTicketAsync),
];

// Usage in an endpoint
public static class Demo
{
    public static async Task RunAsync(IChatClient chat, CrmTools crm)
    {
        var response = await chat.GetResponseAsync(
            "Open a ticket for customer 1234 about login failure.",
            new ChatOptions { Tools = BuildTools(crm), Temperature = 0f });
        Console.WriteLine(response.Text);
    }
}
