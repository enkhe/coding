"""
Prompt caching with the Anthropic SDK.

Static prefix (system + few-shots + reference docs) is cached by the provider.
Subsequent calls with the same prefix pay much less per token and lower latency.

Cache TTL: ~5 minutes (default ephemeral). Vary only the user message at the end.

Requires: pip install anthropic
"""
from anthropic import Anthropic

client = Anthropic()

SYSTEM_PROMPT = """\
You are a senior .NET architect helping a developer write production-grade code.
Cite the .NET 10 / C# 14 version explicitly. Prefer modern idioms (records, primary
constructors, Span<T>, async/await with cancellation tokens).
"""

# Imagine this is a 50-page reference doc you don't want to re-tokenize each call.
LARGE_REFERENCE = open("./dotnet-2026-roadmap.md").read()

def ask(question: str) -> str:
    response = client.messages.create(
        model="claude-opus-4-7",
        max_tokens=1024,
        system=[
            {"type": "text", "text": SYSTEM_PROMPT},
            {
                "type": "text",
                "text": LARGE_REFERENCE,
                "cache_control": {"type": "ephemeral"},   # <-- cache this big chunk
            },
        ],
        messages=[{"role": "user", "content": question}],
    )
    return response.content[0].text


if __name__ == "__main__":
    print(ask("What's new in C# 14 for source generators?"))
    print(ask("Show me a Polly v8 retry pipeline."))   # cache hit on the system prefix
