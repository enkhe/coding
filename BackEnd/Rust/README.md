# Rust

> Memory-safe, high performance, no GC. Zero-cost abstractions, fearless concurrency.

## "To Be Dangerous" Cheatsheet

| Need | Crate |
|---|---|
| HTTP server | **`axum`** + `tokio` |
| HTTP client | **`reqwest`** |
| Async runtime | **`tokio`** |
| JSON | **`serde` + `serde_json`** |
| ORM | **`sqlx`** (compile-time-checked SQL) or `diesel` |
| Errors | **`thiserror`** (libs) + **`anyhow`** (apps) |
| Logging | **`tracing`** + `tracing-subscriber` |
| OTel | `tracing-opentelemetry` + `opentelemetry-otlp` |
| Config | `figment` / `config` |
| Tests | built-in `cargo test` |

## Quick Reference (axum)

```rust
// Cargo.toml: tokio = { version = "1", features = ["full"] }
//             axum = "0.7"
//             tower = "0.5"
//             tracing = "0.1"
//             tracing-subscriber = { version = "0.3", features = ["env-filter", "json"] }
//             serde = { version = "1", features = ["derive"] }

use axum::{
    routing::{get, post},
    Json, Router,
    extract::State,
    http::StatusCode,
};
use serde::{Deserialize, Serialize};
use std::sync::Arc;
use tokio::net::TcpListener;
use tracing::{info, instrument};

#[derive(Clone)]
struct AppState {
    // shared deps (db pool, http client) go here
}

#[derive(Deserialize)] struct PlaceOrder { user_id: String, amount: f64 }
#[derive(Serialize)]   struct Order      { id: String,      amount: f64 }

#[instrument]
async fn place_order(
    State(_state): State<Arc<AppState>>,
    Json(cmd): Json<PlaceOrder>,
) -> (StatusCode, Json<Order>) {
    info!(user_id = %cmd.user_id, amount = cmd.amount, "place order");
    let id = format!("ord-{}", uuid::Uuid::new_v4());
    (StatusCode::CREATED, Json(Order { id, amount: cmd.amount }))
}

#[tokio::main]
async fn main() -> anyhow::Result<()> {
    tracing_subscriber::fmt().json().with_env_filter("info").init();

    let state = Arc::new(AppState {});
    let app = Router::new()
        .route("/health/live", get(|| async { "ok" }))
        .route("/orders", post(place_order))
        .with_state(state);

    let listener = TcpListener::bind("0.0.0.0:8080").await?;
    info!("listening :8080");
    axum::serve(listener, app).await?;
    Ok(())
}
```

## Common Pitfalls

- Borrow checker → fight less, design more (clone for now, optimize later)
- `unwrap()` in production → use `?` and proper error types
- Spawning blocking work on the async runtime → use `tokio::task::spawn_blocking`
- Long-held `Mutex` across `await` → deadlock (use `tokio::sync::Mutex`)

## See also

- [../Go](../Go/) (comparisons) · [../../Tools/CLI](../../Tools/CLI/)
