# Angular Services

> `@Injectable({ providedIn: 'root' })` for tree-shakeable singletons. Inject HttpClient via `inject()`.

```ts
import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class OrdersService {
  private http = inject(HttpClient);

  // Internal state as a signal
  private readonly _orders = signal<Order[]>([]);
  readonly orders = this._orders.asReadonly();

  async load(): Promise<void> {
    const list = await this.http.get<Order[]>('/api/orders').toPromise();
    this._orders.set(list ?? []);
  }
}

export interface Order { id: string; total: number; }
```

## HTTP interceptor

```ts
import { HttpInterceptorFn } from '@angular/common/http';
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');
  return next(req.clone({ setHeaders: token ? { Authorization: `Bearer ${token}` } : {} }));
};

// in app.config.ts:
provideHttpClient(withInterceptors([authInterceptor]))
```

See [../README.md](../README.md).
