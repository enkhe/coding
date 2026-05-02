# React

> Function components + hooks; v19 adds `use`, Actions, Server Components, the React Compiler.

## Core Concepts

- **Function components** — JSX returns a tree; props in, UI out
- **Hooks** — `useState`, `useEffect`, `useMemo`, `useCallback`, `useReducer`, `useRef`, `useContext`, `use`
- **State colocation** — keep state as close to where it's used as possible
- **Memoization** — `React.memo`, `useMemo`, `useCallback`; auto-handled by React Compiler
- **Suspense** — declarative loading boundaries; works with `use()` and lazy
- **Server Components (RSC)** — render on server, send serialized output; zero JS for static parts
- **Actions** — `<form action={async fn}>` + `useActionState` / `useFormStatus`

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Local state | `useState` | per-component |
| Derived state | compute inline (don't store) | "select all", filtered list |
| Async data | `use(promise)` + `<Suspense>` | RSC or Suspense-aware libs |
| Side effects | `useEffect` (sparingly) | subscriptions, sync to non-React |
| Form submission | server `action` + `useActionState` | RSC apps |
| Global state | Zustand / Context + reducer | cross-page UI state |
| Server cache | TanStack Query | REST/GraphQL data |

## Quick Reference

```tsx
import { useState } from 'react';

export function Greeting({ name }: { name: string }) {
  const [count, setCount] = useState(0);
  return (
    <div>
      <h1>Hello, {name}</h1>
      <button onClick={() => setCount(c => c + 1)}>Clicks: {count}</button>
    </div>
  );
}
```

## Common Pitfalls

- `useEffect` for derived state (store + recompute) — just compute inline
- Stale closures — capture vars in deps array or use refs
- Missing `key` on lists or using array index as key for sortable items
- Mutating state directly (`state.items.push(x)`) — return new arrays/objects
- Heavy work in render — wrap in `useMemo`

## Subfolders

- [Hooks](Hooks/README.md) — built-in + custom hooks
- [Vite](Vite/README.md) — fast dev/build for React + TS
- [NextJS](NextJS/README.md) — App Router, RSC, Server Actions

## See also

- [TypeScript](../TypeScript/README.md) — typing components & hooks
- [StateManagement](../StateManagement/README.md) — Zustand, Redux Toolkit
- [Testing](../Testing/README.md) — React Testing Library
