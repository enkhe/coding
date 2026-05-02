// Few-shot prompting — exemplars in the conversation prime format and behavior.
using Microsoft.Extensions.AI;

namespace Prompts;

public sealed class FewShotExtractor(IChatClient chat)
{
    private static readonly List<ChatMessage> _exemplars =
    [
        new(ChatRole.System,
            "Extract product, quantity, and price from order text. Reply JSON: { product, quantity, price }."),

        new(ChatRole.User, "Two coffees at $4.50 each."),
        new(ChatRole.Assistant, """{ "product": "coffee", "quantity": 2, "price": 4.50 }"""),

        new(ChatRole.User, "1 large pizza, $18"),
        new(ChatRole.Assistant, """{ "product": "pizza", "quantity": 1, "price": 18.00 }"""),

        new(ChatRole.User, "Three sodas for twelve dollars"),
        new(ChatRole.Assistant, """{ "product": "soda", "quantity": 3, "price": 4.00 }"""),
    ];

    public async Task<string> ExtractAsync(string text, CancellationToken ct = default)
    {
        var messages = new List<ChatMessage>(_exemplars) { new(ChatRole.User, text) };
        var response = await chat.CompleteAsync(messages,
            new ChatOptions
            {
                Temperature = 0,
                ResponseFormat = ChatResponseFormat.Json,
            }, ct);
        return response.Message.Text ?? "{}";
    }
}
