# FastAPI

> Async Python web framework. Pydantic v2 schemas, automatic OpenAPI, dependency injection.

## Quick Reference

```python
from fastapi import FastAPI, Depends, HTTPException
from pydantic import BaseModel, Field, EmailStr
from contextlib import asynccontextmanager
from uuid import UUID, uuid4

@asynccontextmanager
async def lifespan(app: FastAPI):
    # startup: connect pools, etc.
    yield
    # shutdown: close pools

app = FastAPI(lifespan=lifespan, title="Orders API", version="2026-01")

class PlaceOrder(BaseModel):
    user_id: UUID
    amount: float = Field(gt=0)

class Order(PlaceOrder):
    id: UUID

class CurrentUser(BaseModel):
    id: UUID
    email: EmailStr

async def get_current_user() -> CurrentUser:
    # extract from Authorization header / verify JWT
    return CurrentUser(id=uuid4(), email="me@example.com")

@app.get("/health/live")
async def live(): return {"ok": True}

@app.post("/orders", status_code=201)
async def place_order(cmd: PlaceOrder, user: CurrentUser = Depends(get_current_user)) -> Order:
    return Order(id=uuid4(), **cmd.model_dump())

@app.get("/orders/{order_id}")
async def get_order(order_id: UUID) -> Order:
    raise HTTPException(404, detail="not found")
```

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Path / query / body params | Type-annotate the function parameter |
| Validation | Pydantic v2 — `Field(gt=0)` etc. |
| Auth | `Depends(get_current_user)`; `OAuth2PasswordBearer` for OAuth2 |
| Background tasks | `BackgroundTasks` parameter |
| Async DB | `asyncpg` / SQLAlchemy 2.0 async |
| Docs | `/docs` (Swagger UI) and `/redoc` automatic |
| Run | `uvicorn app:app --host 0.0.0.0 --port 8080 --workers 4` |

## Common Pitfalls

- Sync I/O in async handlers (e.g., `requests.get`) — blocks the event loop
- Forgetting return-type annotation → no schema in OpenAPI
- Mixing Pydantic v1 syntax with v2
- DB session lifetime → use a `Depends` that yields and closes

## See also

- [../Django](../Django/) · [../Flask](../Flask/) · [../../CSharp/MinimalApi](../../CSharp/MinimalApi/)
