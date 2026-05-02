# CSS

> Modern CSS — flex, grid, custom properties, container queries, `:has()`, `@layer`. Almost no JS needed for layout.

## "To Be Dangerous" Cheatsheet

```css
/* Custom properties */
:root {
  --space-1: 0.25rem;
  --space-2: 0.5rem;
  --space-4: 1rem;
  --color-fg: #111;
  --color-bg: #fff;
}
@media (prefers-color-scheme: dark) {
  :root { --color-fg: #f5f5f5; --color-bg: #0b0d10; }
}

/* Layers — explicit cascade priority */
@layer reset, base, components, utilities;

/* Logical properties (RTL-friendly) */
.card { padding-block: var(--space-4); padding-inline: var(--space-2); }

/* Container queries — adapt to parent, not viewport */
.card-list { container-type: inline-size; }
@container (min-width: 600px) {
  .card { display: grid; grid-template-columns: 1fr 2fr; }
}

/* :has() — parent selector */
.field:has(input:invalid) { color: red; }

/* Subgrid */
.outer { display: grid; grid-template-columns: 1fr 2fr 1fr; }
.inner { display: grid; grid-template-columns: subgrid; }

/* aspect-ratio, accent-color, scroll-margin-top */
.hero { aspect-ratio: 16/9; }
input[type="checkbox"] { accent-color: var(--brand); }
section { scroll-margin-top: 4rem; }   /* offset for sticky header in fragment-jump */
```

## Layout cheats

```css
/* Center any block */
.center { display: grid; place-items: center; min-height: 100vh; }

/* Holy grail (sidebar + main) */
.app { display: grid; grid-template-columns: 240px 1fr; min-height: 100vh; }

/* Auto-fit responsive cards */
.cards { display: grid; gap: 1rem;
         grid-template-columns: repeat(auto-fit, minmax(min(100%, 320px), 1fr)); }
```

## Common Pitfalls

- `!important` everywhere → use `@layer` or refactor specificity
- Z-index races — establish a small z-index scale (`--z-modal: 100`, `--z-toast: 1000`)
- Hardcoded breakpoints in components → use container queries when component sizes itself
- CSS-in-JS for the whole app → first-load cost; use `@layer` + tokens

## See also

- [../HTML](../HTML/) · [../CSSFrameworks](../CSSFrameworks/) · [../Accessibility](../Accessibility/)
