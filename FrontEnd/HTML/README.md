# HTML

> Modern HTML5+. Semantic elements, native form validation, lazy loading, modern primitives like `<dialog>`.

## Semantic structure

```html
<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="...">
    <title>Orders — Contoso</title>
    <link rel="canonical" href="https://contoso.com/orders">
    <link rel="preload" href="/fonts/inter.woff2" as="font" type="font/woff2" crossorigin>
  </head>
  <body>
    <header>
      <nav aria-label="Primary"><!-- ... --></nav>
    </header>
    <main>
      <article>
        <h1>Orders</h1>
        <section aria-labelledby="recent">
          <h2 id="recent">Recent</h2>
          <!-- ... -->
        </section>
      </article>
    </main>
    <footer><!-- ... --></footer>
  </body>
</html>
```

## "To Be Dangerous" Cheatsheet

| Feature | Use |
|---|---|
| `<dialog>` | Native modal — `dialog.showModal()` / `.close()` |
| `<details>` / `<summary>` | Built-in accordion |
| `<picture>` + `srcset` + `sizes` | Responsive images, art-direction |
| `<input type="...">` | `email`, `url`, `tel`, `date`, `color`, `search` — built-in validation + UI |
| Form validation | `required`, `pattern="\d+"`, `minlength`, `maxlength` |
| Lazy load | `loading="lazy"` on `<img>` / `<iframe>` |
| Decoding hint | `decoding="async"` |
| `loading="eager"` + `fetchpriority="high"` | Above-the-fold image |
| `<input list="ids">` + `<datalist>` | Autocomplete |
| `<dialog>` inside fieldset | Native confirm dialogs |

## Accessibility-related

- Always set `lang` on `<html>`
- Headings in order — never skip levels
- `<label>` for every form control (or `aria-label`)
- Landmarks: `<header>`, `<main>`, `<nav>`, `<aside>`, `<footer>`
- Skip-to-content link for keyboard users

## Common Pitfalls

- Heading levels jumping (h1 → h3) — breaks screen-reader navigation
- `<button>` styled as link or `<a>` styled as button — pick the right element
- `<table>` for layout — use grid/flex
- Missing `alt` on informational images (or non-empty `alt` on decorative ones)
- Building a custom dropdown when `<select>` would have worked

## See also

- [../CSS](../CSS/) · [../Accessibility](../Accessibility/)
