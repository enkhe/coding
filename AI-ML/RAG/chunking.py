"""Markdown header-aware chunking with overlap. Uses tiktoken for token-accurate sizing.

pip install tiktoken
"""
import re
import tiktoken

ENC = tiktoken.encoding_for_model("text-embedding-3-small")
HEADER = re.compile(r"^(#{1,3})\s+(.+)$", re.M)


def split_by_headers(md: str) -> list[tuple[str, str]]:
    """Yield (heading_path, body) tuples honoring H1/H2/H3 hierarchy."""
    sections, last_end, stack = [], 0, []
    for m in HEADER.finditer(md):
        if last_end:
            sections.append(("/".join(stack), md[last_end:m.start()].strip()))
        level = len(m.group(1))
        stack = stack[:level - 1] + [m.group(2).strip()]
        last_end = m.end()
    sections.append(("/".join(stack), md[last_end:].strip()))
    return [s for s in sections if s[1]]


def chunk(text: str, target: int = 512, overlap: int = 64) -> list[str]:
    ids = ENC.encode(text)
    if len(ids) <= target:
        return [text]
    out, step = [], target - overlap
    for i in range(0, len(ids), step):
        out.append(ENC.decode(ids[i:i + target]))
        if i + target >= len(ids):
            break
    return out


def chunk_markdown(md: str, target: int = 512, overlap: int = 64) -> list[dict]:
    chunks = []
    for path, body in split_by_headers(md):
        for piece in chunk(body, target, overlap):
            chunks.append({"path": path, "text": f"# {path}\n\n{piece}"})
    return chunks


if __name__ == "__main__":
    sample = """# Intro\nMongolia is large.\n\n## Geography\nThe Gobi spans the south. ...\n## Economy\nMining is dominant."""
    for c in chunk_markdown(sample, target=64, overlap=8):
        print(c["path"], "->", len(ENC.encode(c["text"])), "tokens")
