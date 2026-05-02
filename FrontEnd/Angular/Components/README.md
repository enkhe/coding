# Angular Components

> Standalone components, signals-driven.

```ts
import { Component, computed, signal } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-counter',
  template: `
    <p>Count: {{ count() }} (doubled: {{ doubled() }})</p>
    <button (click)="increment()">+1</button>
  `,
})
export class Counter {
  readonly count = signal(0);
  readonly doubled = computed(() => this.count() * 2);
  increment() { this.count.update(n => n + 1); }
}
```

See [../README.md](../README.md) for the broader Angular cheatsheet.
