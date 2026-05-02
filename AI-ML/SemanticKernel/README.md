# Semantic Kernel

> Microsoft's higher-level orchestration framework for LLMs — kernel, plugins, planners, agents, memory.

## Core Concepts

- **`Kernel`** — DI container for AI services + plugins. Built via `Kernel.CreateBuilder()`.
- **Plugin** — group of `KernelFunction`s. Native (C# methods) or prompt-based (template files).
- **`KernelFunction`** — the unit of invocation. Discoverable, schema-described, callable by the model.
- **Function calling** — automatic tool selection: enable `FunctionChoiceBehavior.Auto()` and SK loops until done.
- **Prompt templates** — handlebars or SK syntax `{{$var}}`, `{{plugin.fn $arg}}`. Loaded from `.prompty` or string.
- **Planners** — deprecated in favor of native function calling, but `Handlebars` and `Stepwise` planners still exist for explicit plans.
- **Memory** — `VectorStore` abstraction (Azure AI Search, pgvector, Qdrant, in-mem). Stores text + embedding + metadata.
- **Agents** — `ChatCompletionAgent`, `OpenAIAssistantAgent`, multi-agent group chat (`AgentGroupChat`).

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Build kernel | `Kernel.CreateBuilder().AddOpenAIChatCompletion(model, key).Build()` | Always |
| Add plugin from class | `kernel.Plugins.AddFromType<MyPlugin>("name")` | Native C# tools |
| Add prompt plugin | `kernel.ImportPluginFromPromptDirectory("path")` | Prompt-as-code |
| Auto tool calling | `new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }` | Agent-style |
| Run a function | `await kernel.InvokeAsync("plugin", "fn", args)` | Direct call |
| Chat agent | `new ChatCompletionAgent { Kernel = k, Instructions = "..." }` | Multi-turn |
| Vector memory | `IVectorStore` + `VectorStoreCollection<TKey,TRecord>` | RAG |

## Quick Reference

### Build kernel + invoke

```csharp
using Microsoft.SemanticKernel;

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-5", apiKey)
    .Build();

kernel.Plugins.AddFromType<TimePlugin>("Time");

var result = await kernel.InvokePromptAsync(
    "What time is it? Use the Time plugin.",
    new(new OpenAIPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    }));

Console.WriteLine(result);
```

### Native plugin

```csharp
using System.ComponentModel;
using Microsoft.SemanticKernel;

public sealed class TimePlugin
{
    [KernelFunction("now"), Description("Get UTC time as ISO 8601.")]
    public string Now() => DateTime.UtcNow.ToString("o");
}
```

### Prompt template (handlebars)

```handlebars
<message role="system">You translate English to {{$language}}.</message>
<message role="user">{{$input}}</message>
```

### Chat agent

```csharp
var agent = new ChatCompletionAgent
{
    Name = "Reviewer",
    Instructions = "You review code for SOLID violations. Be specific.",
    Kernel = kernel,
    Arguments = new(new OpenAIPromptExecutionSettings
        { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }),
};

await foreach (var item in agent.InvokeAsync("Review this method: ..."))
    Console.WriteLine(item.Message.Content);
```

## Common Pitfalls

- Mixing `IChatClient` (Microsoft.Extensions.AI) with SK without bridging — pick one entrypoint per app.
- Forgetting `FunctionChoiceBehavior.Auto()` — model returns text describing the tool instead of calling it.
- Storing secrets in prompt templates — use `KernelArguments`, not template literals.
- Old `IKernelMemory` patterns are deprecated; use `Microsoft.Extensions.VectorData` abstractions.
- Planners are heavyweight; native function calling is usually better.

## Examples in this folder

- [`kernel-basic.cs`](kernel-basic.cs) — Build kernel, register plugin, invoke
- [`prompt-template.cs`](prompt-template.cs) — Handlebars template + variables
- [`chat-agent.cs`](chat-agent.cs) — `ChatCompletionAgent` with tools
- [`vector-memory.cs`](vector-memory.cs) — In-memory vector store + RAG retrieval

## See also

- [MicrosoftExtensionsAI](../MicrosoftExtensionsAI/) — lower-level alternative
- [Agents](../Agents/) — agent loop patterns
- [RAG](../RAG/) — full retrieval pipeline
