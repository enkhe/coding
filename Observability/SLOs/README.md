# SLOs — Service Level Objectives

> The discipline of choosing what to measure, what target to hit, and what to do when you miss.

## Core Concepts

- **SLI (Indicator)** — a measurement (e.g., success rate, p99 latency).
- **SLO (Objective)** — target for an SLI over a window (e.g., 99.9% success over 30 days).
- **SLA (Agreement)** — a contract; usually weaker than the internal SLO so you have headroom.
- **Error budget** = `1 - SLO`. If budget remaining → ship features. If budget burned → freeze, fix.
- **Burn-rate alerts** — multi-window (fast burn = page; slow burn = ticket). Avoids alert fatigue from single-threshold rules.
- **RED method** — Rate, Errors, Duration. Default for request-driven services.
- **USE method** — Utilization, Saturation, Errors. Default for resources (CPU, memory, queues).

## "To Be Dangerous" Cheatsheet

| Service type | SLI candidates |
|---|---|
| HTTP API | success rate, p95/p99 latency |
| Async worker | message age (oldest unacked), DLQ rate, processing latency |
| Stream processor | end-to-end lag, throughput |
| Database | query p99, replication lag, connection saturation |
| Frontend (RUM) | INP (Interaction to Next Paint), LCP, CLS |

| SLO target | Error budget per 30d |
|---|---|
| 99% | 7h 12m |
| 99.5% | 3h 36m |
| 99.9% | 43m 12s |
| 99.95% | 21m 36s |
| 99.99% | 4m 19s |

## Quick Reference (Sloth-style YAML)

```yaml
version: prometheus/v1
service: orders-api
labels:
  team: platform
slos:
  - name: requests-availability
    objective: 99.9
    description: "99.9% of requests return non-5xx in 30 days."
    sli:
      events:
        error_query: sum(rate(http_server_request_duration_seconds_count{job="orders-api",http_response_status_code=~"5.."}[{{.window}}]))
        total_query: sum(rate(http_server_request_duration_seconds_count{job="orders-api"}[{{.window}}]))
    alerting:
      page_alert: { labels: { severity: page } }
      ticket_alert: { labels: { severity: ticket } }
```

## Common Pitfalls

- Choosing SLOs without a customer perspective ("CPU under 70%" is not an SLO).
- 99.99% SLOs without the engineering investment to support them — pick what you can defend.
- Single-threshold alerts → false alarms or missed slow burns. Use multi-window burn-rate.
- No runbook tied to the alert — the page is useless.
- Dashboards as performance art — every panel must answer "what do I do if this changes?"

## Examples in this folder

- [`slo-definitions.yaml`](slo-definitions.yaml) — Sloth-format SLO + burn-rate alerts
- [`runbook-error-budget-burn.md`](runbook-error-budget-burn.md) — what to do when budget burns

## See also

- [../OpenTelemetry](../OpenTelemetry/) · [../../Docs/Runbooks](../../Docs/Runbooks/)
