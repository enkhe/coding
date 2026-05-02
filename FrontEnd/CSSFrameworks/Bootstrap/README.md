# Bootstrap 5+

> Component-class CSS. Fast for back-office / admin panels. Customize via Sass variables.

## Quick Reference

```html
<button class="btn btn-primary">Place</button>

<div class="container">
  <div class="row g-3">
    <div class="col-12 col-md-6 col-lg-4">
      <div class="card">
        <div class="card-body">
          <h5 class="card-title">Order #123</h5>
          <p class="card-text">$9.99</p>
        </div>
      </div>
    </div>
  </div>
</div>

<!-- Utilities -->
<div class="d-flex justify-content-between align-items-center p-3 bg-light rounded">
  <span class="fw-semibold">Total</span>
  <span class="text-primary fs-4">$42.00</span>
</div>
```

## Customize via Sass

```scss
// custom.scss
$primary: #6750a4;
$border-radius: 0.75rem;
$enable-shadows: true;

@import "bootstrap/scss/bootstrap";
```

## Common Pitfalls

- Loading the full kit when you use 10% of it — tree-shake with imports
- jQuery dependency anachronism — Bootstrap 5 dropped jQuery; don't bring it back
- Pixel-perfect overrides → write a real component instead

## See also

- [../README.md](../README.md) · [../Tailwind](../Tailwind/)
