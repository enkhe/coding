# Vite

> Lightning-fast dev server (esbuild) + Rollup-based prod build. Default for new React/Vue/Svelte SPAs.

## Core Concepts

- **Native ESM dev** — no bundling at dev time; HMR is near-instant
- **`vite.config.ts`** — plugins, aliases, proxy
- **Env vars** — `VITE_*` prefix is exposed to the client; others stay server-only
- **Path aliases** — match `tsconfig.json` `paths` with `resolve.alias`

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Create app | `npm create vite@latest my-app -- --template react-ts` | new project |
| Dev server | `npm run dev` | port 5173 by default |
| Build | `npm run build` (outputs `dist/`) | CI |
| API proxy | `server.proxy` in config | avoid CORS in dev |
| Env files | `.env`, `.env.development`, `.env.production` | per-mode config |
| Bundle analyze | `rollup-plugin-visualizer` | size budgets |

## Quick Reference

```ts
// vite.config.ts
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'node:path';

export default defineConfig({
  plugins: [react()],
  resolve: { alias: { '@': path.resolve(__dirname, 'src') } },
  server: {
    port: 5173,
    proxy: { '/api': 'http://localhost:5000' },
  },
});
```

## Common Pitfalls

- Forgetting `VITE_` prefix — env var is `undefined` in browser
- Importing Node-only modules into client code — fails at build
- Misaligned `tsconfig` paths and Vite aliases — dev works, build breaks
- Large dependencies optimized eagerly — set `optimizeDeps.exclude`

## Examples in this folder

- [`vite.config.ts`](vite.config.ts) — plugins, alias, proxy
- [`main.tsx`](main.tsx) — entry
- [`App.tsx`](App.tsx) — minimal component
- [`env.d.ts`](env.d.ts) — typed `import.meta.env`

## See also

- [../README.md](../README.md) — React overview
- [../../Performance](../../Performance/README.md) — bundle analysis
