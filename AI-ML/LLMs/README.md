# LLMs

> Large Language Models — families, sampling, function calling, structured output, streaming.

## Core Concepts

- **Model families** — Anthropic Claude (4.x: Haiku/Sonnet/Opus), OpenAI GPT (4o, 4.1, 5), Google Gemini (1.5/2.x), Meta Llama (3.x/4), Mistral (Large, Mixtral), open weights (Qwen, DeepSeek).
- **Prompt vs completion** — chat models take a list of messages (`system`, `user`, `assistant`, `tool`); base/completion models take a single string.
- **System message** — sets identity, constraints, format. Stable across turns. Cache-friendly.
- **Token limits** — context window (input) vs max output tokens. Modern frontier models: 200K-2M context. Always count tokens before sending.
- **Sampling** — `temperature` controls randomness; `top_p` is nucleus sampling. Use one, not both. `temperature=0` for deterministic extraction.
- **Streaming** — server-sent events (SSE) yielding deltas. Required for chat UX above ~500ms latency.
- **Function/tool calling** — model emits a structured request (`name`, `arguments`); your code executes; result fed back as a `tool` message.
- **Structured output** — JSON Schema constrained decoding (`response_format`, `output_schema`). Eliminates parser fragility.

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Pick model | Tier by capability/cost/latency: Haiku/4o-mini for cheap classify; Sonnet/4o for general; Opus/5 for hard reasoning | Right-size per call, not per app |
| Deterministic extraction | `temperature=0`, JSON schema, low `max_tokens` | Batch ETL, classification |
| Creative generation | `temperature=0.9-1.2`, no schema | Marketing copy, brainstorm |
| Long context | Use prompt cache, put stable prefix first | RAG with docs, codebase Q&A |
| Token count | `tiktoken` (OpenAI), Anthropic count-tokens API | Budgeting, truncation |
| Streaming | SSE, accumulate `delta.content` | Chat UI, agent traces |
| Tool calling | Pass `tools` array; loop until no `tool_use` | Agents, retrievers |
| Structured output | `response_format: { type: "json_schema", schema }` | Anything machine-parsed |

## Quick Reference

### C# — Anthropic via Microsoft.Extensions.AI

```csharp
using Microsoft.Extensions.AI;

IChatClient client = new AnthropicChatClient("claude-sonnet-4-7", apiKey)
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var response = await client.GetResponseAsync(
    [new ChatMessage(ChatRole.System, "You are a precise extractor."),
     new ChatMessage(ChatRole.User, "Extract entities: 'Acme bought Foo for $1B.'")],
    new ChatOptions { Temperature = 0f, MaxOutputTokens = 256 });
```

### Python — Anthropic SDK with prompt cache

```python
from anthropic import Anthropic
client = Anthropic()

resp = client.messages.create(
    model="claude-sonnet-4-7",
    max_tokens=1024,
    system=[{"type": "text", "text": LONG_SYSTEM,
             "cache_control": {"type": "ephemeral"}}],
    messages=[{"role": "user", "content": "Summarize."}],
)
```

### Python — OpenAI structured output

```python
from openai import OpenAI
client = OpenAI()

resp = client.chat.completions.create(
    model="gpt-5",
    messages=[{"role": "user", "content": "Parse: John, 30, NYC"}],
    response_format={"type": "json_schema", "json_schema": {
        "name": "Person",
        "schema": {"type": "object", "properties": {
            "name": {"type": "string"}, "age": {"type": "integer"},
            "city": {"type": "string"}}, "required": ["name","age","city"]}}},
    temperature=0,
)
```

## Common Pitfalls

- Setting both `temperature` and `top_p`. Pick one.
- Treating `max_tokens` as a request size — it caps output only.
- Not handling truncation when output hits `max_tokens`.
- Forgetting to feed tool results back as a `tool` message — model loops or hallucinates.
- Mixing system instructions inside user messages — wastes cache and confuses scope.
- Counting characters instead of tokens for budgeting.

## Examples in this folder

- [`anthropic-chat.cs`](anthropic-chat.cs) — Claude chat with system + user, streaming
- [`openai-structured.py`](openai-structured.py) — JSON schema constrained output
- [`function-calling.cs`](function-calling.cs) — Tool definition + invocation loop
- [`token-counting.py`](token-counting.py) — Token budgeting before send

## See also

- [PromptEngineering](../PromptEngineering/) — how to write the system message
- [MicrosoftExtensionsAI](../MicrosoftExtensionsAI/) — provider-agnostic abstraction
- [Agents](../Agents/) — multi-turn tool-calling loops
