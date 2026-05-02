// Chain-of-thought + structured extraction.
// Ask for hidden reasoning, then a final answer; throw away the reasoning to keep responses compact.
using Microsoft.Extensions.AI;

namespace Prompts;

public sealed record InvoiceExtraction(
    string Vendor,
    decimal Total,
    string Currency,
    DateOnly InvoiceDate,
    string[] Items);

public sealed class CotExtractor(IChatClient chat)
{
    public async Task<InvoiceExtraction?> ExtractAsync(string invoiceText, CancellationToken ct = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System,
                """
                You extract invoice fields from raw text.
                Think step-by-step about what each field is, then output ONLY JSON matching:
                { "vendor": str, "total": number, "currency": str, "invoiceDate": "YYYY-MM-DD", "items": [str] }.
                If a field is missing, return null. Use the JSON output only — no narration.
                """),
            new(ChatRole.User, invoiceText),
        };

        var response = await chat.CompleteAsync<InvoiceExtraction>(
            messages,
            new ChatOptions { Temperature = 0 },
            cancellationToken: ct);

        return response.Result;
    }
}
