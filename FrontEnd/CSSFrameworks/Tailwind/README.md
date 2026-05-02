# Tailwind CSS

> Utility-first CSS. Tailwind 4 dropped `tailwind.config.js` for CSS-first config (`@theme`).

## Quick Reference (v4)

```css
/* app.css */
@import "tailwindcss";

@theme {
  --color-brand: oklch(75% 0.18 230);
  --font-display: "Inter", "system-ui", sans-serif;
  --breakpoint-3xl: 1920px;
}

/* dark mode */
@media (prefers-color-scheme: dark) {
  @theme {
    --color-brand: oklch(70% 0.18 230);
  }
}
```

```html
<button class="rounded-md bg-brand px-4 py-2 text-white shadow hover:bg-brand/90 focus:outline-2">
  Place Order
</button>

<!-- Responsive + dark mode -->
<div class="grid grid-cols-1 md:grid-cols-3 gap-4 dark:bg-neutral-900">
  <article class="rounded-xl bg-white dark:bg-neutral-800 p-4 shadow">…</article>
</div>
```

## Patterns

- **Components** — extract repeated combinations into a component (React/Vue/Svelte). Don't `@apply` everywhere.
- **Plugins** — `@plugin "...";` for first-party (forms, typography, container-queries).
- **Arbitrary values** — `bg-[#1a2238]`, `top-[120%]` — escape hatch but use sparingly.

## Common Pitfalls

- `@apply` graveyards — pulls Tailwind toward... Bootstrap. Use components.
- Class-name string concatenation that's not statically analyzable — Tailwind purger drops them. Use `clsx`/`cva`.
- Forgetting `darkMode: 'class'` if you want manual toggle (v4 differs)

## See also

- [../README.md](../README.md) · [../../CSS](../../CSS/)
