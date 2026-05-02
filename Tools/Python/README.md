# Python (scripting / tooling)

> Python 3.12+ for utilities, data work, and CLI tools. For web frameworks see [`BackEnd/Python`](../../BackEnd/Python/).

## Core Concepts

- **`uv`** — fast package manager (Astral). Use it instead of pip/poetry for new projects.
- **`pathlib.Path`** — never `os.path.join`; use `Path / "..."` and `.read_text()`.
- **`tomllib`** (stdlib) for TOML; **`json`** for JSON; **`pydantic`** for validated models.
- **`typer`** for CLIs; types double as flag parsing.
- **`httpx`** for HTTP (async-capable, modern); **`requests`** still fine for sync-only.
- **`subprocess.run(..., check=True)`** — raise on non-zero exit.

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Init project | `uv init` then `uv add <pkg>` |
| Virtual env | `uv venv` then `source .venv/bin/activate` |
| Run a tool | `uv tool run ruff check .` |
| Type-check | `mypy --strict src/` |
| Format & lint | `ruff format . && ruff check .` |
| HTTP | `httpx.get(url, timeout=5).raise_for_status()` |
| Async HTTP | `async with httpx.AsyncClient() as c: ...` |
| File I/O | `Path("x").read_text()`, `.write_text("...")` |
| Env config | `os.getenv("VAR", "default")` or `pydantic-settings` |
| CLI | `typer.run(main)` |

## Quick Reference (typer CLI)

```python
from pathlib import Path
import typer
from rich import print as rprint

app = typer.Typer()

@app.command()
def ingest(
    src: Path = typer.Option(..., exists=True, readable=True),
    dst: Path = typer.Option(...),
    dry_run: bool = False,
):
    """Copy src to dst with logging."""
    if dry_run:
        rprint(f"[yellow]DRY[/yellow] would copy {src} -> {dst}")
        return
    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_bytes(src.read_bytes())
    rprint(f"[green]OK[/green] {src} -> {dst}")

if __name__ == "__main__":
    app()
```

## Common Pitfalls

- Mutable default args (`def f(x=[])`) — caught by linters; never write
- `os.system` or shell=True — command injection risk; use list arg form
- Catching bare `except:` — masks `KeyboardInterrupt`
- `print` in libraries — use `logging`
- Dependency hell from no lockfile — use `uv` / `poetry` / `pip-compile`

## Examples in this folder

- [`cli-tool.py`](cli-tool.py) — typer CLI
- [`httpx-example.py`](httpx-example.py) — async HTTP
- [`pathlib-cheatsheet.py`](pathlib-cheatsheet.py)

## See also

- [../../BackEnd/Python](../../BackEnd/Python/) · [../../AI-ML](../../AI-ML/)
