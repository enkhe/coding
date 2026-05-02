"""Token counting before send. Budget = context_window - max_output - safety_margin.

pip install tiktoken anthropic
"""
import tiktoken
from anthropic import Anthropic

# OpenAI / GPT-family
enc = tiktoken.encoding_for_model("gpt-4o")
text = "The quick brown fox jumps over the lazy dog."
tokens = enc.encode(text)
print(f"OpenAI tokens: {len(tokens)}")

# Anthropic count-tokens API (authoritative for Claude)
client = Anthropic()
result = client.messages.count_tokens(
    model="claude-sonnet-4-7",
    system="You are concise.",
    messages=[{"role": "user", "content": text}],
)
print(f"Anthropic tokens: {result.input_tokens}")


def truncate_to_budget(text: str, max_tokens: int, model: str = "gpt-4o") -> str:
    enc = tiktoken.encoding_for_model(model)
    ids = enc.encode(text)
    if len(ids) <= max_tokens:
        return text
    return enc.decode(ids[:max_tokens])


# Always leave headroom for the response.
CONTEXT_WINDOW = 200_000
MAX_OUTPUT = 4_096
SAFETY = 1_000
input_budget = CONTEXT_WINDOW - MAX_OUTPUT - SAFETY
print(f"Input token budget: {input_budget}")
