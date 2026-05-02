# React Hooks

> Built-in primitives for state, effects, refs, and context — plus custom hooks for reuse.

## Core Concepts

- Hooks must be called at the **top level** of a component or another hook
- Order matters — never call inside `if` / loops
- A custom hook is just a function that calls other hooks (must start with `use`)

## "To Be Dangerous" Cheatsheet

| Hook | Use for | Note |
|---|---|---|
| `useState` | local mutable state | functional updater for closures |
| `useReducer` | complex transitions | dispatch + pure reducer |
| `useEffect` | sync to external systems | rare with RSC; cleanup function |
| `useLayoutEffect` | DOM measurement | runs sync after paint commit |
| `useMemo` | cache expensive compute | rarely needed with React Compiler |
| `useCallback` | stable fn identity | for memoized children |
| `useRef` | mutable box, DOM refs | doesn't trigger re-render |
| `useContext` | inject from `<Provider>` | combine with reducer for store |
| `use` (v19) | unwrap promise/context | inside RSC or Suspense |
| `useTransition` | low-priority updates | avoid blocking input |
| `useDeferredValue` | debounced render | typeahead, search |
| `useId` | stable a11y ids | SSR-safe |
| `useSyncExternalStore` | external state lib | Zustand/Redux integration |

## Quick Reference

```tsx
const [n, setN] = useState(0);
const dbl = useMemo(() => n * 2, [n]);
const onClick = useCallback(() => setN(x => x + 1), []);
const ref = useRef<HTMLInputElement>(null);

useEffect(() => {
  const id = setInterval(() => setN(x => x + 1), 1000);
  return () => clearInterval(id);
}, []);
```

## Common Pitfalls

- Calling hooks conditionally — breaks the rule of stable order
- Missing dependencies in `useEffect` — stale data
- Putting non-serializable values in deps (object literals, fresh fns each render)
- Using `useState` for derived data — compute inline

## Examples in this folder

- [`useDebounce.ts`](useDebounce.ts) — debounced value
- [`useFetch.ts`](useFetch.ts) — minimal data fetcher (prefer TanStack Query in real apps)
- [`useLocalStorage.ts`](useLocalStorage.ts) — persisted state with `useSyncExternalStore`
- [`Counter.tsx`](Counter.tsx) — `useReducer` + `useCallback`

## See also

- [../README.md](../README.md) — React overview
- [../../StateManagement](../../StateManagement/README.md)
