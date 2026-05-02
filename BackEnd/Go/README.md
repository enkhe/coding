# Go

> Pragmatic, fast, simple. Standard-library-first; small dep trees; compile to a static binary.

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| HTTP server | `net/http` + `chi` router |
| JSON | `encoding/json` (+ `[json:"..."]` tags); `goccy/go-json` for speed |
| Logger | `log/slog` (stdlib structured logging) |
| Context | `context.Context` first arg in every blocking function |
| Errors | `fmt.Errorf("doing x: %w", err)` (wrap); `errors.Is/As` to inspect |
| Concurrency | goroutines + channels; `errgroup` for fan-out |
| Tests | `testing` + `t.Run("subtest", ...)`; `httptest.NewServer` for HTTP tests |
| Lint | `golangci-lint` (curated set) |
| Modules | `go mod init`, `go.mod`, `go.sum` |
| Build static binary | `CGO_ENABLED=0 go build -ldflags='-s -w'` |

## Quick Reference (HTTP server with chi)

```go
package main

import (
    "context"
    "encoding/json"
    "log/slog"
    "net/http"
    "os"
    "os/signal"
    "syscall"
    "time"

    "github.com/go-chi/chi/v5"
    "github.com/go-chi/chi/v5/middleware"
)

type Order struct {
    ID     string  `json:"id"`
    Amount float64 `json:"amount"`
}

func main() {
    log := slog.New(slog.NewJSONHandler(os.Stdout, nil))

    r := chi.NewRouter()
    r.Use(middleware.RequestID, middleware.RealIP, middleware.Recoverer, middleware.Timeout(5*time.Second))

    r.Get("/health/live", func(w http.ResponseWriter, _ *http.Request) { w.WriteHeader(200) })

    r.Post("/orders", func(w http.ResponseWriter, req *http.Request) {
        var o Order
        if err := json.NewDecoder(req.Body).Decode(&o); err != nil {
            http.Error(w, err.Error(), 400)
            return
        }
        o.ID = "ord-" + middleware.GetReqID(req.Context())
        log.InfoContext(req.Context(), "placed", "id", o.ID)
        w.Header().Set("Content-Type", "application/json")
        w.WriteHeader(201)
        _ = json.NewEncoder(w).Encode(o)
    })

    srv := &http.Server{Addr: ":8080", Handler: r, ReadHeaderTimeout: 2 * time.Second}

    ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)
    defer stop()
    go func() {
        if err := srv.ListenAndServe(); err != nil && err != http.ErrServerClosed {
            log.Error("listen", "err", err)
        }
    }()

    <-ctx.Done()
    shutdown, cancel := context.WithTimeout(context.Background(), 5*time.Second)
    defer cancel()
    _ = srv.Shutdown(shutdown)
}
```

## Common Pitfalls

- Goroutine leaks — always pass a `context.Context` and return when it's done
- Capturing loop variable in goroutines (Go < 1.22) — pass as parameter
- `panic` from nil maps → always `make(map[...]...)`
- Forgetting `defer body.Close()` on HTTP responses
- Returning errors without wrapping → loses context

## See also

- [../CSharp](../CSharp/) (for comparisons) · [../../Tools/CLI](../../Tools/CLI/)
