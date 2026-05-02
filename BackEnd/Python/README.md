# Python

> Modern Python 3.12+ web frameworks. Type-first, async-first.

## Subfolders

- [`FastAPI/`](FastAPI/README.md) — modern async API; Pydantic v2; OpenAPI; OAuth2
- [`Django/`](Django/README.md) — full-stack ORM/admin; DRF for APIs
- [`Flask/`](Flask/README.md) — micro-framework; blueprints

## Cheatsheet

| Need | Pick |
|---|---|
| New JSON API, async-first | FastAPI |
| Full app with admin/ORM/templates | Django |
| Tiny service, sync, simple | Flask |
| Type-checked everywhere | FastAPI + Pydantic v2 + `mypy --strict` |
| Background work | `arq`, `celery`, or FastAPI `BackgroundTasks` |
| Long-poll / SSE / WS | FastAPI / Starlette |

## Idioms (Python 3.12+)

```python
# PEP 695 generic syntax
def first[T](xs: list[T]) -> T | None:
    return xs[0] if xs else None

# Pattern matching
match event:
    case {"type": "click", "x": int(x), "y": int(y)}:
        ...
    case _:
        ...

# Async context manager
async with httpx.AsyncClient() as client:
    r = await client.get(url)
```

## Quality stack

- `ruff` (lint + format), `mypy` or `pyright` (types), `pytest`, `pytest-asyncio`, `httpx`
- Dependency manager: `uv` (fast) or `poetry`
- Settings: `pydantic-settings` (env-driven, validated)

## See also

- [`../README.md`](../README.md) — backend index
