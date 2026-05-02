# End-to-End Tests

> Drive the deployed system through the user-facing surface (browser/API) - the smallest, slowest, most valuable layer.

## Core Concepts

- E2E covers user journeys: "as a user I can sign up, place an order, get a receipt".
- Keep the count tiny - E2E is expensive, slow, and flaky compared to integration tests.
- Run against ephemeral environments (Docker Compose, k8s preview, Testcontainers stack).
- Page Object Model isolates locators from test logic.

## "To Be Dangerous" Cheatsheet

```bash
# Install Playwright for .NET
dotnet add package Microsoft.Playwright
pwsh bin/Debug/net10.0/playwright.ps1 install
```

```csharp
await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
var page = await browser.NewPageAsync();
await page.GotoAsync("https://app.local/login");
await page.GetByLabel("Email").FillAsync("alice@example.com");
await page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync();
await Expect(page.GetByText("Welcome")).ToBeVisibleAsync();
```

| Tool                  | Use                                                |
|-----------------------|----------------------------------------------------|
| Playwright .NET       | Browser automation, auto-wait, tracing             |
| Playwright (Node)     | UI test devs prefer; same engine                   |
| Cypress               | JS-only, no .NET runner; not recommended for .NET  |
| Selenium              | Legacy; avoid for new work                         |

## Quick Reference

- Locators: `page.GetByRole`, `GetByLabel`, `GetByPlaceholder`, `GetByText` (accessibility-first).
- Auto-wait: Playwright waits for actionable state - no `Thread.Sleep`.
- Tracing: `await context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });`
- Storage state: persist auth via `context.StorageStateAsync()` to skip login per test.

## Common Pitfalls

- Brittle CSS selectors (`div > span:nth-child(3)`) - use roles/text.
- Sharing browser state across tests causing order coupling.
- Running against shared staging - use ephemeral envs to avoid noisy-neighbor flakes.
- No retries on CI - Playwright supports `retries: 2` for the truly flaky 1%.

## Examples in this folder

- [LoginTests.cs](./LoginTests.cs) - Playwright .NET, page object pattern
- [playwright.config.ts](./playwright.config.ts) - JS variant config

## See also

- [../Integration/README.md](../Integration/README.md)
- [../Performance/README.md](../Performance/README.md)
