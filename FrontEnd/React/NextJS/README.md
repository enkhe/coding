# Next.js

> React framework with file-based routing, RSC, Server Actions, edge/runtime split, ISR, image optimization.

## Core Concepts

- **App Router** (`app/`) — RSC by default; opt into client with `"use client"`
- **Layouts** — `layout.tsx` wraps nested routes; persistent across navigations
- **Server Components** — run on server only; can be async and read DB directly
- **Client Components** — `"use client"`; have hooks, browser APIs
- **Route Handlers** — `app/api/.../route.ts` for REST endpoints
- **Server Actions** — `"use server"` async functions; call from forms or buttons
- **Metadata** — `export const metadata` or `generateMetadata()` for SEO
- **Caching** — request memoization, data cache, full-route cache, router cache

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Page | `app/about/page.tsx` | static or RSC route |
| Layout | `app/layout.tsx` | shared shell |
| Loading UI | `app/.../loading.tsx` | streaming Suspense fallback |
| Error UI | `app/.../error.tsx` (`"use client"`) | error boundary |
| Not found | `app/.../not-found.tsx` | 404 |
| API | `app/api/foo/route.ts` exporting `GET`, `POST` | REST endpoints |
| Server Action | `async function action() { 'use server'; ... }` | mutations from forms |
| SEO | `generateMetadata({ params })` | dynamic meta |
| ISR | `export const revalidate = 60` | revalidate every N sec |
| Dynamic | `export const dynamic = 'force-dynamic'` | opt-out of caching |

## Quick Reference

```tsx
// app/posts/[id]/page.tsx — async RSC
export default async function Post({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const post = await db.post.findUnique({ where: { id } });
  return <article><h1>{post?.title}</h1></article>;
}
```

## Common Pitfalls

- Putting hooks in a server component — must be `"use client"`
- Reading cookies/headers in a static page — forces dynamic rendering
- Forgetting `revalidatePath` after a Server Action mutation
- Mixing `fetch` cache options unintentionally — read the cache table

## Examples in this folder

- [`page.tsx`](page.tsx) — RSC with async data
- [`layout.tsx`](layout.tsx) — root layout with metadata
- [`route.ts`](route.ts) — API route handler
- [`actions.ts`](actions.ts) — Server Action with revalidation

## See also

- [../README.md](../README.md) — React overview
- [../../Performance](../../Performance/README.md) — caching, streaming
