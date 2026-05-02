# Agents

> An LLM in a loop with tools. Plan → call tool → observe → reflect → continue.

## Mental model

```
loop:
  prompt the LLM (system + history + last observation)
  parse the response
    if a tool call → execute, append result to history
    if a final answer → exit
  guard: max turns, max cost, timeout
```

## Building blocks

- **Tool** — typed function the LLM can call (`get_order(id)`, `send_email(...)`).
- **Tool schema** — JSON Schema generated from your function signature.
- **Memory** — short-term (chat history), long-term (vector store).
- **Planner** — explicit plan-first agent (slower, more controllable) vs ReAct (interleaved).

## Patterns

| Pattern | When |
|---|---|
| **ReAct** | General-purpose; cheap; loose control |
| **Plan-and-Execute** | Complex multi-step; you want to inspect plan before running |
| **Code-as-action** | Agent writes Python; sandboxed exec — very expressive |
| **Multi-agent** | Specialist agents (researcher / coder / critic) |
| **Tree-of-Thought** | Search across reasoning branches; expensive |

## Quick Reference (Microsoft.Extensions.AI tool calling)

```csharp
public sealed class OrdersTools(IOrdersService svc)
{
    [Description("Get an order by id.")]
    public async Task<OrderDto?> GetOrder(Guid orderId, CancellationToken ct) =>
        await svc.GetAsync(orderId, ct);

    [Description("Refund an order. Use only after explicit user confirmation.")]
    public async Task<bool> Refund(Guid orderId, decimal amount, CancellationToken ct) =>
        await svc.RefundAsync(orderId, amount, ct);
}

// Wire to the chat client
var chat = client.AsBuilder()
    .UseFunctionInvocation()                  // auto-execute tool calls
    .UseLogging()
    .Build();

var tools = AIFunctionFactory.Create(orderTools);

await foreach (var update in chat.GetStreamingResponseAsync(messages,
    new ChatOptions { Tools = [..tools] }))
{
    Console.Write(update.Text);
}
```

## Guardrails (do not skip)

- **Confirm before destructive tools** (refund, send email, delete)
- **Max turns / max cost / max wall-clock** budgets
- **Tool input validation** — never trust the LLM's args
- **PII redaction** in logged tool inputs
- **Sandbox** code-execution agents (containers, no network)

## Common Pitfalls

- Letting the agent loop forever — bound it
- Tools that aren't idempotent → retries cause duplicates; use idempotency keys
- "I'll review the plan later" — review BEFORE the agent acts on it
- Trusting the LLM to summarize state without a structured store

## See also

- [../SemanticKernel](../SemanticKernel/) · [../MicrosoftExtensionsAI](../MicrosoftExtensionsAI/) · [../PromptEngineering](../PromptEngineering/) · [../Evaluation](../Evaluation/)
