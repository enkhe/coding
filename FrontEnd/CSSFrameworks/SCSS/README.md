# Sass / SCSS

> Preprocessor: nesting, variables, mixins, modules. Modern CSS has narrowed the gap; still useful for big design systems and Bootstrap customization.

## "To Be Dangerous" Cheatsheet

```scss
// modules — replaces @import
@use "tokens" as *;
@use "sass:math";
@use "sass:color";

// variables (or use CSS custom properties for runtime themes)
$radius: 0.5rem;
$gap: 1rem;

// mixins
@mixin focus-ring($color: $brand) {
  outline: 2px solid color.adjust($color, $alpha: -0.4);
  outline-offset: 2px;
}

// functions
@function spacing($n) { @return math.div($n, 4) * 1rem; }

.btn {
  border-radius: $radius;
  padding: spacing(2) spacing(4);

  &:focus-visible {
    @include focus-ring;
  }

  &.is-primary { background: $brand; }
}
```

## Modern CSS replacements

| Sass feature | Modern CSS equivalent |
|---|---|
| `$variable` | `var(--variable)` (runtime) |
| `&__element` | CSS nesting (in-flight; available in current browsers) |
| `@import` | `@use` (Sass) or native `@import url(...)` |
| math | `calc()`, `min()`, `max()`, `clamp()` |
| color manip | `color-mix()`, `oklch()` (CSS Color 4) |

## When still useful

- Customizing Bootstrap (whose source IS SCSS)
- Existing large design systems mid-migration
- Build-time math/loops where you don't want runtime cost

## See also

- [../README.md](../README.md) · [../../CSS](../../CSS/)
