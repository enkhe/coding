# Prompt Engineering

> The discipline of writing prompts that elicit the behavior you want, reliably.

## Core Concepts

- **System prompt** — sets persona, format, constraints. Stable across requests.
- **User prompt** — the request itself.
- **Few-shot** — include examples of input → desired output. Signals format, tone, edge cases.
- **Chain-of-thought** (CoT) — ask for reasoning steps before the answer. Works for math/logic/planning.
- **Output format constraints** — JSON schema, regex, "answer in 2 sentences." Less ambiguity = more reliability.
- **Prompt caching** — provider feature to cache the static prefix of long prompts. Reduces cost/latency.
- **Guardrails** — content filters, jailbreak detection, output validation, refusal patterns.

## "To Be Dangerous" Cheatsheet

| Goal | Pattern |
|---|---|
| Strict format | "Reply with JSON matching this schema: …" + structured outputs |
| Reliable extraction | Few-shot with 2-3 examples |
| Reasoning task | "Think step-by-step before answering." |
| Reduce hallucination | "If unsure, say 'I don't know.' Cite from the provided context only." |
| Multi-turn agent | System prompt with tools list + invocation pattern |
| Cost reduction | Prompt cache the system + few-shots; vary only the user message |

## Quick Reference (Microsoft.Extensions.AI)

```csharp
var messages = new List<ChatMessage>
{
    new(ChatRole.System,
        """
        You are an order-classification assistant. Classify each input into one of:
        REFUND, COMPLAINT, INQUIRY, OTHER.
        Reply with JSON: {"category": "...", "confidence": 0.0-1.0}.
        If you are unsure, return "OTHER".
        """),
    new(ChatRole.User, "Example: 'I want my money back' → {\"category\":\"REFUND\",\"confidence\":0.95}"),
    new(ChatRole.User, "Example: 'this product broke' → {\"category\":\"COMPLAINT\",\"confidence\":0.9}"),
    new(ChatRole.User, customerMessage),
};

var response = await chat.CompleteAsync(messages,
    new ChatOptions { ResponseFormat = ChatResponseFormat.Json });
```

## Output schema (structured outputs)

```csharp
var options = new ChatOptions
{
    ResponseFormat = ChatResponseFormatJson.Strict<Classification>(),
};

public sealed record Classification(string Category, double Confidence);
```

## Anti-jailbreak

- Treat user input as **data, not instructions** in your system prompt.
- Refuse to override the system prompt: "Ignore prior instructions" → reject.
- Sanitize tool inputs from the model — no shell, no SQL, no file paths from the LLM unless validated.

## Common Pitfalls

- "Just tell the model to be careful" → reliability theater. Use schemas + validators.
- Mega-prompts that mix N tasks → split into chained calls.
- Few-shot with leakage (examples too close to test data).
- Forgetting prompt-cache invalidation when the static prefix changes.
- Assuming temperature=0 = deterministic — same model can still vary across versions.

## Examples in this folder

- [`structured-output.cs`](structured-output.cs) — typed responses with validation
- [`few-shot.cs`](few-shot.cs) — classification with exemplars
- [`cot-extract.cs`](cot-extract.cs) — chain-of-thought + structured extract
- [`anthropic-cache.py`](anthropic-cache.py) — prompt caching with Anthropic SDK

## See also

- [../LLMs](../LLMs/) · [../MicrosoftExtensionsAI](../MicrosoftExtensionsAI/) · [../Evaluation](../Evaluation/)
