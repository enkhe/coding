# Microsoft.Extensions.AI

> Provider-agnostic AI abstraction for .NET — `IChatClient`, `IEmbeddingGenerator`, middleware pipeline.

## Core Concepts

- **`IChatClient`** — single interface across OpenAI, Azure OpenAI, Anthropic, Ollama, Gemini, AWS Bedrock. Swap providers via DI, not code.
- **`IEmbeddingGenerator<TInput, TEmbedding>`** — same idea for embeddings (`IEmbeddingGenerator<string, Embedding<float>>`).
- **`ChatMessage`** — role (`System`, `User`, `Assistant`, `Tool`) + `Contents` (multi-modal: text, image, function call, function result).
- **`ChatOptions`** — `Temperature`, `MaxOutputTokens`, `Tools`, `ResponseFormat`, `Seed`, `StopSequences`, `AdditionalProperties` (provider-specific).
- **Function tools** — register with `AIFunctionFactory.Create(Delegate)`. Reflection-driven, descriptions from `[Description]`.
- **Middleware pipeline** — `IChatClient` decorators: `UseFunctionInvocation`, `UseLogging`, `UseOpenTelemetry`, `UseDistributedCache`, custom.
- **Pipeline pattern** — builder composes layers; outermost wraps innermost. Same shape as ASP.NET Core middleware.

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Register chat client | `services.AddChatClient(b => b.UseFunctionInvocation().UseLogging().Use(innerClient))` | Always, in DI |
| Switch provider | Replace inner client; pipeline unchanged | Multi-cloud, fallback |
| Auto-invoke tools | `.UseFunctionInvocation()` middleware | Agents, retrievers |
| Distributed cache | `.UseDistributedCache(cache)` on stable prompts | Repeat queries, idempotent |
| Tracing | `.UseOpenTelemetry(sourceName: "my-app")` | Always in prod |
| Embeddings | `IEmbeddingGenerator<string,Embedding<float>>` + `GenerateAsync(IEnumerable<string>)` | RAG ingest, search |
| Structured output | `ChatOptions.ResponseFormat = ChatResponseFormat.Json` | Anything machine-parsed |
| Streaming | `client.GetStreamingResponseAsync(...)` returns `IAsyncEnumerable<ChatResponseUpdate>` | UX |

## Quick Reference

### Register in ASP.NET Core

```csharp
using Microsoft.Extensions.AI;
using OpenAI;

var openAi = new OpenAIClient(builder.Configuration["OpenAI:ApiKey"]);

builder.Services.AddChatClient(b => b
        .UseLogging()
        .UseOpenTelemetry(sourceName: "myapp.ai")
        .UseFunctionInvocation()
        .Use(openAi.AsChatClient("gpt-5")));

builder.Services.AddEmbeddingGenerator(b => b
        .Use(openAi.AsEmbeddingGenerator("text-embedding-3-small")));
```

### Inject and use

```csharp
public class ChatEndpoint(IChatClient chat)
{
    public async Task<string> Ask(string prompt) =>
        (await chat.GetResponseAsync(prompt,
            new ChatOptions { Temperature = 0.2f })).Text;
}
```

### Custom middleware

```csharp
public sealed class RedactPiiClient(IChatClient inner) : DelegatingChatClient(inner)
{
    public override Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages, ChatOptions? options = null,
        CancellationToken ct = default)
    {
        var redacted = messages.Select(m =>
            new ChatMessage(m.Role, Redact(m.Text ?? "")));
        return base.GetResponseAsync(redacted, options, ct);
    }
    static string Redact(string s) => /* regex SSN, email, etc */ s;
}
```

## Common Pitfalls

- Forgetting `.UseFunctionInvocation()` — the model returns tool requests that never execute.
- Wrapping order matters — `UseLogging` outermost sees redacted text only if redact is inside.
- Using provider SDK types directly bypasses the pipeline (no logging/tracing).
- `IEmbeddingGenerator` returns `GeneratedEmbeddings<T>`; index by position, do not assume order on parallel calls.
- `ChatOptions` `Seed` is best-effort — do not rely on it for strict determinism.

## Examples in this folder

- [`chat-client-di.cs`](chat-client-di.cs) — ASP.NET Core registration with full pipeline
- [`embeddings.cs`](embeddings.cs) — Generate embeddings for a batch
- [`custom-middleware.cs`](custom-middleware.cs) — `DelegatingChatClient` redacts PII
- [`tools-registration.cs`](tools-registration.cs) — Multiple tool registration

## See also

- [LLMs](../LLMs/) — model families
- [SemanticKernel](../SemanticKernel/) — higher-level alternative
- [Agents](../Agents/) — building loops on top
