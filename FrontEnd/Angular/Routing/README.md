# Angular Routing

> `provideRouter` + lazy-loaded standalone components.

```ts
import { Routes } from '@angular/router';
import { authGuard } from './auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'orders', pathMatch: 'full' },
  {
    path: 'orders',
    canActivate: [authGuard],
    loadComponent: () => import('./orders.page').then(m => m.OrdersPage),
  },
  {
    path: 'orders/:id',
    loadComponent: () => import('./order-details.page').then(m => m.OrderDetailsPage),
    resolve: {
      order: (route) => inject(OrdersService).get(route.paramMap.get('id')!)
    }
  },
];

// auth.guard.ts
import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
export const authGuard: CanActivateFn = () => {
  return inject(AuthService).isAuthenticated() || inject(Router).createUrlTree(['/login']);
};
```

See [../README.md](../README.md).
