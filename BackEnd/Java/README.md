# Java

> Java 21+ LTS / 25 LTS. Records, virtual threads, sealed types, pattern matching — modern Java is much closer to C#.

## Modern features worth knowing

- **Records** — `record User(String name, String email) {}`
- **Sealed types** — `sealed interface Shape permits Circle, Square`
- **Pattern matching** — `switch (s) { case Circle c -> c.radius(); }`
- **Virtual threads** (Java 21+) — `Thread.startVirtualThread(...)`; massive concurrency
- **Text blocks** — `"""\n  multi-line\n  """`
- **`var`** local type inference

## "To Be Dangerous" Cheatsheet

| Need | Tool |
|---|---|
| HTTP server | **Spring Boot 3.x** (or **Quarkus** for native AOT) |
| Build | **Maven** or **Gradle** (Kotlin DSL preferred) |
| ORM | **Spring Data JPA** / **Hibernate** |
| HTTP client | `java.net.http.HttpClient` (stdlib) |
| Tests | **JUnit 5** + **AssertJ** + **Testcontainers** |
| Logging | **SLF4J** + **Logback** |
| OTel | `opentelemetry-spring-boot-starter` |

## Quick Reference (Spring Boot 3 controller)

```java
@RestController
@RequestMapping("/orders")
public class OrdersController {

    private final OrdersService service;

    public OrdersController(OrdersService service) { this.service = service; }

    @GetMapping("/{id}")
    public ResponseEntity<Order> get(@PathVariable UUID id) {
        return service.find(id)
            .map(ResponseEntity::ok)
            .orElseGet(() -> ResponseEntity.notFound().build());
    }

    @PostMapping
    public ResponseEntity<Order> create(@Valid @RequestBody PlaceOrder cmd) {
        var order = service.place(cmd.userId(), cmd.amount());
        return ResponseEntity.created(URI.create("/orders/" + order.id())).body(order);
    }
}

public record PlaceOrder(@NotNull UUID userId, @Positive BigDecimal amount) {}
public record Order(UUID id, UUID userId, BigDecimal amount) {}
```

## Common Pitfalls

- Stuck on Java 8/11 — modern Java has changed a lot; upgrade
- Field injection (`@Autowired`) — prefer constructor injection (testable, immutable)
- `Optional` returned for fields → use only as method return type
- Eager fetching everywhere → N+1; use `@EntityGraph` or DTO projections

## See also

- [../CSharp](../CSharp/) (comparisons)
