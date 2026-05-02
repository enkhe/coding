# Frontend Testing

> Vitest / Jest + Testing Library + Playwright + MSW.

## Core Concepts

- **Test the user-visible behavior**, not implementation details (Testing Library philosophy).
- **Vitest** = the modern Jest; faster, ESM-native, Vite-aligned.
- **Testing Library** — `@testing-library/{react, vue, svelte}` — accessible queries (`getByRole`, `getByLabelText`).
- **Playwright** — E2E + component tests; cross-browser.
- **MSW** (Mock Service Worker) — intercepts `fetch`/XHR for unit + E2E.

## Tooling map

| Goal | Pick |
|---|---|
| Unit | Vitest (or Jest) |
| Component | Testing Library + Vitest, or Playwright Component Tests |
| E2E | Playwright |
| Visual regression | Playwright `toHaveScreenshot()`, Chromatic |
| Network mocks | MSW |
| Accessibility | `axe-core` via `@axe-core/playwright` |

## Quick Reference (Vitest + RTL)

```tsx
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect } from 'vitest';
import { Counter } from './Counter';

describe('Counter', () => {
  it('increments on click', async () => {
    render(<Counter />);
    await userEvent.click(screen.getByRole('button', { name: /increment/i }));
    expect(screen.getByText(/count: 1/i)).toBeInTheDocument();
  });
});
```

## Quick Reference (Playwright)

```ts
import { test, expect } from '@playwright/test';

test('login and place order', async ({ page }) => {
  await page.goto('/');
  await page.getByLabel('Email').fill('me@example.com');
  await page.getByLabel('Password').fill('correct horse');
  await page.getByRole('button', { name: 'Sign in' }).click();
  await expect(page.getByRole('heading', { name: /dashboard/i })).toBeVisible();
});
```

## Common Pitfalls

- Snapshot tests over fragile DOM → flaky. Snapshot small, semantic outputs only.
- Querying by CSS selectors / test ids → couples tests to implementation. Prefer roles + labels.
- E2E hitting real prod APIs → slow + flaky. Use staging or contract tests.
- Forgetting to await user events → false greens.

## Examples in this folder

- [`vitest-rtl.test.tsx`](vitest-rtl.test.tsx) — Vitest + Testing Library
- [`playwright.spec.ts`](playwright.spec.ts) — E2E
- [`msw-handlers.ts`](msw-handlers.ts) — network mocks

## See also

- [../../Testing/EndToEnd](../../Testing/EndToEnd/) · [../../Testing/Unit](../../Testing/Unit/) · [../Accessibility](../Accessibility/)
