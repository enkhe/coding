# Flask

> Minimal Python micro-framework. Great for small services and learning. For new larger services, prefer FastAPI.

## Quick Reference

```python
from flask import Flask, request, jsonify, abort
from uuid import uuid4

app = Flask(__name__)

@app.get("/health/live")
def live(): return {"ok": True}

@app.post("/orders")
def place_order():
    body = request.get_json(silent=True) or {}
    if not body.get("user_id") or not isinstance(body.get("amount"), (int, float)):
        abort(400, description="invalid body")
    return jsonify({"id": str(uuid4()), **body}), 201

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=8080)
```

## Blueprints (organize larger apps)

```python
from flask import Blueprint
orders_bp = Blueprint("orders", __name__, url_prefix="/orders")

@orders_bp.get("/<order_id>")
def get_order(order_id): return {"id": order_id}

app.register_blueprint(orders_bp)
```

## "To Be Dangerous"

| Need | Pattern |
|---|---|
| Async | Flask supports `async def` since 2.0; or use **Quart** for full async |
| Background | RQ / Celery |
| ORM | SQLAlchemy (with Flask-SQLAlchemy) |
| Validation | `marshmallow` or `pydantic` |
| Production WSGI | `gunicorn -w 4 app:app` |

## Common Pitfalls

- Using `app.run()` in production → use gunicorn / uwsgi
- `debug=True` in prod → arbitrary code execution via debugger
- No request size limit → memory DoS

## See also

- [../FastAPI](../FastAPI/) · [../Django](../Django/)
