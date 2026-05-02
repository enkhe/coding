# JavaScript

> Modern ES2024+. Type-fluency optional via JSDoc/TypeScript. Knowing the language is more important than knowing any framework.

## "To Be Dangerous" Cheatsheet

```js
// Destructuring + spread
const { name, age = 18, ...rest } = user;
const merged = { ...defaults, ...overrides };
const [first, ...others] = list;

// Optional chaining + nullish coalescing
const city = user?.address?.city ?? 'Unknown';

// async/await + Promise.all
const [orders, profile] = await Promise.all([
  fetch('/api/orders').then(r => r.json()),
  fetch('/api/profile').then(r => r.json()),
]);

// AbortController for cancellable fetch
const ac = new AbortController();
fetch(url, { signal: ac.signal }).catch(e => { if (e.name !== 'AbortError') throw e; });
ac.abort();

// Top-level await (ESM only)
const data = await fetchData();

// Iteration
for (const v of arr) { /* ... */ }
for (const [k, v] of Object.entries(obj)) { /* ... */ }
for await (const chunk of asyncIterable) { /* ... */ }

// Modules
import { defaults, type Settings } from './settings.js';   // .js extension required
export { Foo, Bar };
export default Foo;

// Classes
class Order {
  #id;                                // private field
  static #counter = 0;
  constructor(amount) { this.#id = ++Order.#counter; this.amount = amount; }
  get id() { return this.#id; }
}

// Maps & Sets
const cache = new Map();
const seen = new Set();
const weak = new WeakMap();           // GC-friendly keys

// Structured clone (deep copy without JSON-ing)
const copy = structuredClone(original);

// Intl
new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(1234.56);
new Intl.DateTimeFormat('en-US', { dateStyle: 'medium' }).format(new Date());
```

## Common Pitfalls

- `==` vs `===` — always `===` unless you specifically want coercion
- `this` in callbacks — arrow functions or `.bind`
- Mutating arrays you're iterating — produce new arrays via map/filter/reduce
- Floating point `0.1 + 0.2 !== 0.3` — use `Math.fround` or BigInt for money
- Mixing CJS and ESM in one project — pick one

## See also

- [../TypeScript](../TypeScript/) · [../React](../React/) · [../Performance](../Performance/)
