# HTTP Status Codes — when to use what

## 2xx Success

| Code | Use |
|---|---|
| `200 OK` | Successful read, generic success |
| `201 Created` | Resource created; **set `Location` header** |
| `202 Accepted` | Async — work queued, not done |
| `204 No Content` | Success but nothing to return (DELETE, PUT-as-update) |

## 3xx Redirect

| Code | Use |
|---|---|
| `301 Moved Permanently` | URL changed, cache forever |
| `302 Found` | Temp redirect (mostly avoid; use 303/307) |
| `303 See Other` | After POST, redirect GET (PRG pattern) |
| `304 Not Modified` | Conditional GET — `If-None-Match` matched |
| `307 Temporary Redirect` | Temp, **preserve method** |
| `308 Permanent Redirect` | Permanent, **preserve method** |

## 4xx Client error

| Code | Use |
|---|---|
| `400 Bad Request` | Invalid syntax / failed validation |
| `401 Unauthorized` | Missing or invalid auth (the name lies — it means *unauthenticated*) |
| `403 Forbidden` | Authenticated but not allowed |
| `404 Not Found` | Resource doesn't exist (or you don't want to reveal it does) |
| `405 Method Not Allowed` | Method not supported on this resource — must include `Allow` header |
| `406 Not Acceptable` | Can't satisfy `Accept` header |
| `408 Request Timeout` | Client took too long |
| `409 Conflict` | Resource state collision (concurrent updates, duplicate create) |
| `410 Gone` | Was here, gone forever |
| `412 Precondition Failed` | `If-Match`/`If-Unmodified-Since` failed |
| `415 Unsupported Media Type` | Body Content-Type the API doesn't support |
| `422 Unprocessable Entity` | Syntax OK but semantics fail (validation) — common for APIs |
| `425 Too Early` | Request replayed (anti-replay) |
| `428 Precondition Required` | Server demands `If-Match` for safety |
| `429 Too Many Requests` | Rate limited — include `Retry-After` |
| `451 Unavailable For Legal Reasons` | Censored / takedown |

## 5xx Server error

| Code | Use |
|---|---|
| `500 Internal Server Error` | Unhandled exception. Log it. |
| `501 Not Implemented` | Method not implemented (rare; usually 405) |
| `502 Bad Gateway` | Upstream returned bad response |
| `503 Service Unavailable` | Maintenance / overloaded; include `Retry-After` |
| `504 Gateway Timeout` | Upstream didn't respond in time |

## ProblemDetails (RFC 7807) — return body for 4xx/5xx

```json
{
  "type": "https://example.com/errors/insufficient-funds",
  "title": "Insufficient funds",
  "status": 422,
  "detail": "Account balance is below the requested amount.",
  "instance": "/orders/8f3a-...",
  "errors": { "amount": ["must be <= balance"] }
}
```

`Content-Type: application/problem+json`.

## Idempotency for POST

```http
POST /orders
Idempotency-Key: 4d1f...
```

Server: same key + same body → same response (cache for ~24h). Different key OR different body → process normally.
