// Claude streaming chat via Anthropic .NET SDK.
// Package: Anthropic.SDK (community) or Microsoft.Extensions.AI.Anthropic.
//
// dotnet add package Anthropic.SDK
// dotnet add package Microsoft.Extensions.AI

using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

var client = new AnthropicClient(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY"));

var parameters = new MessageParameters
{
    Model = AnthropicModels.Claude4Sonnet,
    MaxTokens = 1024,
    Temperature = 0.7m,
    System = new List<SystemMessage>
    {
        new("You are a senior .NET architect. Be concise.",
            new CacheControl { Type = CacheControlType.ephemeral })
    },
    Messages = new List<Message>
    {
        new(RoleType.User, "Explain async streams in 3 bullets.")
    },
    Stream = true,
};

await foreach (var ev in client.Messages.StreamClaudeMessageAsync(parameters))
{
    if (ev.Delta?.Text is { } text)
        Console.Write(text);
}
Console.WriteLine();
