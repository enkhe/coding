"""OpenAI structured output via JSON schema.

pip install openai pydantic
"""
import os
from openai import OpenAI
from pydantic import BaseModel, Field

client = OpenAI(api_key=os.environ["OPENAI_API_KEY"])


class Invoice(BaseModel):
    vendor: str
    total_usd: float = Field(ge=0)
    line_items: list[str]


# Pydantic-driven structured output (SDK builds schema for you).
resp = client.beta.chat.completions.parse(
    model="gpt-5",
    messages=[
        {"role": "system", "content": "Extract invoices verbatim. No invented fields."},
        {"role": "user", "content": "Vendor: Acme. Total: $42.50. Items: pens, paper."},
    ],
    response_format=Invoice,
    temperature=0,
)

invoice: Invoice = resp.choices[0].message.parsed
print(invoice.model_dump_json(indent=2))
