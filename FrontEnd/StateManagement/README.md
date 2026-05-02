# State Management

> Pick by team familiarity, framework, and the **shape** of the state.

## Core Concepts — kinds of state

| Kind | Lives where | Example |
|---|---|---|
| Local UI | component-level | toggle, modal open |
| Form | form lib (RHF, Formik) | inputs + validation |
| Cross-component | global store | user, theme |
| Server | server cache (TanStack Query, SWR) | API responses |
| URL | router / search params | filters, page |
| Persistent | localStorage / IndexedDB | drafts |
| Real-time | SignalR / WebSocket | live data |

## Library Map

| Need | Pick |
|---|---|
| React global, simple | **Zustand**, **Jotai** |
| React global, structured | **Redux Toolkit** (still excellent) |
| React server cache | **TanStack Query** (or SWR) |
| Vue 3 global | **Pinia** |
| Angular global | **NgRx** or **Signals** + services |
| Svelte 5 | **runes** (`$state`, `$derived`, `$effect`) |
| Solid / fine-grained | signals built-in |
| Form state | **react-hook-form** + zod |

## Quick Reference (Zustand)

```ts
import { create } from 'zustand';

interface CartState {
  items: { id: string; qty: number }[];
  add: (id: string) => void;
  clear: () => void;
}

export const useCart = create<CartState>((set) => ({
  items: [],
  add: (id) => set((s) => ({ items: [...s.items, { id, qty: 1 }] })),
  clear: () => set({ items: [] }),
}));
```

## Quick Reference (TanStack Query)

```ts
const { data, isPending, error } = useQuery({
  queryKey: ['orders', userId],
  queryFn: ({ signal }) =>
    fetch(`/api/orders?userId=${userId}`, { signal }).then((r) => r.json()),
  staleTime: 30_000,
});
```

## Common Pitfalls

- Putting server data in a global store → cache invalidation hell. Use a server-cache lib.
- "Everything in Redux" → boilerplate explosion. Use Redux Toolkit + slices.
- Forgetting to abort fetches on unmount → memory leaks (`AbortController`).
- Persisting too much to localStorage → 5MB cap + sync writes block UI.

## Examples in this folder

- [`zustand.tsx`](zustand.tsx) — minimal store
- [`tanstack-query.tsx`](tanstack-query.tsx) — server cache
- [`pinia.ts`](pinia.ts) — Vue 3
- [`svelte5-runes.svelte`](svelte5-runes.svelte) — Svelte 5

## See also

- [../React](../React/) · [../Vue](../Vue/) · [../Svelte](../Svelte/) · [../Angular](../Angular/)
