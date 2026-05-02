# Angular

> Angular 20+. Standalone components, signals, control-flow blocks, `inject()`. Modern Angular feels nothing like AngularJS.

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Standalone component | `@Component({ standalone: true, imports: [...] })` |
| Signals | `count = signal(0); doubled = computed(() => count() * 2);` |
| Effect | `effect(() => console.log(count()))` |
| Control flow | `@if`, `@for (item of items; track item.id)`, `@switch` |
| DI in functions | `inject(MyService)` instead of constructor |
| HTTP | `inject(HttpClient).get<T>(...)` |
| Forms (reactive) | `FormGroup` + `FormControl` |
| Router | `provideRouter(routes)` + `routerLink` directive |

## Quick Reference

```ts
// app.routes.ts
import { Routes } from '@angular/router';
export const routes: Routes = [
  { path: 'orders', loadComponent: () => import('./orders.page').then(m => m.OrdersPage) },
];

// orders.page.ts
import { Component, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  standalone: true,
  selector: 'app-orders',
  template: `
    @if (orders()) {
      <ul>
        @for (o of orders()!; track o.id) {
          <li>{{ o.id }} — \${{ o.total | number:'1.2-2' }}</li>
        }
      </ul>
    } @else { <p>Loading…</p> }
  `,
})
export class OrdersPage {
  private http = inject(HttpClient);
  orders = toSignal(this.http.get<{ id: string; total: number }[]>('/api/orders'));
}
```

## Common Pitfalls

- Mixing legacy `NgModule` with standalone — pick one direction
- Forgetting `track` on `@for` → DOM thrash
- Subscribing without unsubscribing → memory leaks; use `toSignal` or `takeUntilDestroyed`
- Heavy change detection → use signals or `OnPush`

## Examples

- [Components](Components/) · [Routing](Routing/) · [Services](Services/)

## See also

- [../React](../React/) · [../Vue](../Vue/)
