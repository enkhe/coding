# Frontend Performance

> Core Web Vitals + bundle hygiene + smart hydration.

## Core Concepts (Web Vitals 2026)

| Metric | What | Good |
|---|---|---|
| **LCP** | Largest Contentful Paint | < 2.5s |
| **INP** | Interaction to Next Paint (replaced FID) | < 200ms |
| **CLS** | Cumulative Layout Shift | < 0.1 |
| **TTFB** | Time to First Byte | < 800ms |
| **FCP** | First Contentful Paint | < 1.8s |

## "To Be Dangerous" Cheatsheet

| Lever | What |
|---|---|
| Code splitting | Route-level `lazy()` / dynamic `import()` |
| Image optimization | `<img loading="lazy" decoding="async" srcset="...">`; `next/image` etc. |
| Font loading | `font-display: swap`; preload critical fonts; subsetting |
| Prefetch / preload | `<link rel="preload" as="...">` for critical, `prefetch` for next-route |
| Virtualization | TanStack Virtual / react-window for long lists |
| Memoization | `React.memo`, `useMemo`, `useCallback`; Vue `computed`; Solid signals |
| Streaming SSR / RSC | React 19 server components + suspense |
| HTTP/3 + Brotli | gateway-level; usually free win |
| Tree-shaking | ESM only; avoid `import * as X` |
| Bundle analysis | `vite-plugin-inspect`, `webpack-bundle-analyzer`, `rollup-plugin-visualizer` |

## Quick Reference

```tsx
// Route-level code split
const OrdersPage = lazy(() => import('./pages/OrdersPage'));

<Suspense fallback={<Spinner/>}>
  <Routes>
    <Route path="/orders" element={<OrdersPage/>} />
  </Routes>
</Suspense>
```

```html
<!-- Critical above-the-fold image -->
<img src="hero.avif"
     srcset="hero-480.avif 480w, hero-960.avif 960w, hero-1920.avif 1920w"
     sizes="(max-width: 600px) 480px, 960px"
     fetchpriority="high"
     decoding="async"
     width="1920" height="1080"
     alt="…">
```

## Common Pitfalls

- Hydration cost > rendering cost — measure before adopting SSR
- "All routes prefetched" — clogs the network for what won't be used
- `useEffect` running on every render → infinite loops or wasted work
- Hydration mismatches from server time / locale / random IDs
- Polling instead of SSE/WebSocket → battery drain on mobile

## Examples in this folder

- [`code-split.tsx`](code-split.tsx) — route-level lazy + Suspense
- [`virtualized-list.tsx`](virtualized-list.tsx) — TanStack Virtual
- [`measure.ts`](measure.ts) — `web-vitals` reporter

## See also

- [../React](../React/) · [../Accessibility](../Accessibility/) · [../Testing](../Testing/)
