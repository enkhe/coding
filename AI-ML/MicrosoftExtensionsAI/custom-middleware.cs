// Custom middleware via DelegatingChatClient — redact PII before forwarding upstream.

using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;

public sealed partial class RedactPiiClient(IChatClient inner) : DelegatingChatClient(inner)
{
    [GeneratedRegex(@"\b\d{3}-\d{2}-\d{4}\b")]
    private static partial Regex Ssn();

    [GeneratedRegex(@"[\w\.-]+@[\w\.-]+\.\w+")]
    private static partial Regex Email();

    public override Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => base.GetResponseAsync(messages.Select(Redact), options, cancellationToken);

    public override IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => base.GetStreamingResponseAsync(messages.Select(Redact), options, cancellationToken);

    static ChatMessage Redact(ChatMessage m)
    {
        var text = m.Text ?? "";
        text = Ssn().Replace(text, "[SSN]");
        text = Email().Replace(text, "[EMAIL]");
        return new ChatMessage(m.Role, text);
    }
}

// Builder extension
public static class PiiPipelineExtensions
{
    public static ChatClientBuilder UsePiiRedaction(this ChatClientBuilder b)
        => b.Use((inner, _) => new RedactPiiClient(inner));
}

// Usage:
// services.AddChatClient(b => b
//     .UsePiiRedaction()      // outermost: incoming text redacted before logging
//     .UseLogging()
//     .Use(innerClient));
