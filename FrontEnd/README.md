# FrontEnd

> Cross-framework frontend cheatsheet aligned with the .NET 2026 senior/architect roadmap.

## Core Concepts

- **Component model** — declarative UI as a function of state; tree of reusable units
- **State** — local (component) / shared (store) / server (cache) / URL (router)
- **Styling** — utility-first (Tailwind), CSS-in-JS, CSS Modules, vanilla CSS with custom properties
- **Build** — Vite / esbuild / SWC for bundling; tree-shaking + code splitting
- **Rendering** — CSR, SSR, SSG, ISR, RSC (React Server Components), streaming
- **Performance** — Core Web Vitals (LCP, INP, CLS), hydration cost, virtualization
- **Accessibility** — semantic HTML first, ARIA only when needed, keyboard nav, contrast

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| .NET full-stack | Blazor Server / WASM / Auto | C# everywhere, real-time UI |
| SPA, big ecosystem | React 19 + Vite or Next.js | most jobs, broadest hire pool |
| Opinionated MVVM | Angular 20 (signals, standalone) | large enterprise teams |
| Approachable SFC | Vue 3.5 + Pinia | smaller teams, fast onboarding |
| Compile-time reactivity | Svelte 5 (runes) + SvelteKit | smallest bundles, lean apps |
| Static + SSR | Next.js, Nuxt, SvelteKit, Astro | content sites, e-commerce |
| Server state | TanStack Query / SWR | any client app talking to APIs |
| Client store | Zustand / Pinia / Redux Toolkit | shared cross-component state |
| E2E tests | Playwright | release gating |
| Component tests | Vitest + Testing Library | day-to-day TDD |

## Quick Reference

```ts
// modern frontend stack (React example)
import { useState } from 'react';
export function Counter() {
  const [n, setN] = useState(0);
  return <button onClick={() => setN(n + 1)}>{n}</button>;
}
```

## Common Pitfalls

- Reaching for a framework when plain HTML/CSS would do
- Over-using `useEffect` / lifecycle hooks instead of derived state
- Shipping huge JS bundles without code-splitting or RSC
- Ignoring a11y until the audit fails
- Mixing client and server state in one store

## Subdomains

### Frameworks
- [Blazor](Blazor/README.md) — .NET in the browser & on the server
- [React](React/README.md) — function components + hooks (v19)
- [Angular](Angular/README.md) — standalone + signals (v20+)
- [Vue](Vue/README.md) — Composition API (v3.5+)
- [Svelte](Svelte/README.md) — runes + SvelteKit (v5)

### Languages & Markup
- [JavaScript](JavaScript/README.md) — modern ES2024
- [TypeScript](TypeScript/README.md) — types, generics, narrowing
- [HTML](HTML/README.md) — semantic + modern elements
- [CSS](CSS/README.md) — modern layout & features

### Styling
- [CSSFrameworks/Tailwind](CSSFrameworks/Tailwind/README.md)
- [CSSFrameworks/Bootstrap](CSSFrameworks/Bootstrap/README.md)
- [CSSFrameworks/SCSS](CSSFrameworks/SCSS/README.md)

### Cross-cutting
- [StateManagement](StateManagement/README.md) — stores, signals, server state
- [Performance](Performance/README.md) — Core Web Vitals, virtualization
- [Testing](Testing/README.md) — Vitest, Playwright, MSW
- [Accessibility](Accessibility/README.md) — ARIA, keyboard, contrast

### Legacy (maintenance only)
- [WebForms](WebForms/README.md)
- [jQuery](jQuery/README.md)

## See also

- [Roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [Testing/EndToEnd](../Testing/EndToEnd) — Playwright details
