# Accessibility (a11y)

> WCAG 2.2 baseline. Test with **keyboard only**, **screen reader**, and **automated** (axe-core).

## "To Be Dangerous" Cheatsheet

| Concern | Fix |
|---|---|
| Semantic HTML | Use `<button>`, `<a>`, `<nav>`, `<main>` properly |
| Labels | `<label for="email">` or wrap; `aria-label` only when no visible text |
| Focus | Visible focus ring (`:focus-visible`); never `outline: none` without replacement |
| Keyboard | Every interactive element reachable; tab order logical; `Esc` to close modals |
| Contrast | 4.5:1 for body, 3:1 for large text (use a checker) |
| Reduced motion | `@media (prefers-reduced-motion: reduce)` |
| Headings | One `<h1>`; never skip levels |
| Live regions | `aria-live="polite"` for status; `aria-live="assertive"` for alerts |
| Forms | Group with `<fieldset>` + `<legend>`; show errors next to fields |
| Images | `alt` for content; `alt=""` for decorative |
| Skip link | `<a href="#main" class="skip-link">Skip to main</a>` |
| Color | Don't rely on color alone (icons + text) |

## Test patterns

```ts
// Playwright + axe
import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

test('home page has no automatically detectable a11y issues', async ({ page }) => {
  await page.goto('/');
  const results = await new AxeBuilder({ page })
    .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa', 'wcag22aa'])
    .analyze();
  expect(results.violations).toEqual([]);
});
```

## Manual smoke test

1. Tab through the page from the address bar — every interactive element should be reachable in a sensible order.
2. Activate everything with `Enter`/`Space`.
3. `Esc` should close modals/menus.
4. Open VoiceOver (Mac) / NVDA (Windows) — landmarks and headings sensible?
5. Zoom to 200% — anything cut off?

## Common Pitfalls

- `<div onclick>` instead of `<button>` — not keyboard-accessible
- Custom dropdown without `aria-expanded` / `aria-controls`
- Modal that doesn't trap focus and doesn't return focus on close
- `aria-label` overriding visible text → screen reader hears the label, not the text

## See also

- [../HTML](../HTML/) · [../Testing](../Testing/) · [../../Testing/EndToEnd](../../Testing/EndToEnd/)
