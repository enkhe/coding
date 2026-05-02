# Node.js

> Node 22+ (LTS) with native ESM, TypeScript-first.

## Subfolders

- [`Express/`](Express/README.md) — classic, ubiquitous
- [`Fastify/`](Fastify/README.md) — schema-first, performant
- [`NestJS/`](NestJS/README.md) — opinionated, DI, decorators

## Cheatsheet

| Need | Pick |
|---|---|
| Smallest, most familiar | Express |
| Best perf + JSON schema validation | Fastify |
| Enterprise structure / DI / OpenAPI | NestJS |
| Schema validation | `zod` (Express/Nest) or `typebox` (Fastify) |
| ORM | Prisma or Drizzle |
| Logging | `pino` |
| Testing | `vitest` + `supertest` (or Fastify inject) |

## Modern Node idioms

```ts
// ESM + top-level await
import { readFile } from 'node:fs/promises';
const cfg = JSON.parse(await readFile('./cfg.json', 'utf8'));

// Native test runner
import { test } from 'node:test';
import assert from 'node:assert/strict';
test('adds', () => assert.equal(1 + 1, 2));

// AbortController for cancellation
const ac = new AbortController();
setTimeout(() => ac.abort(), 5000);
await fetch(url, { signal: ac.signal });
```

## package.json basics

```json
{
  "type": "module",
  "engines": { "node": ">=22" },
  "scripts": {
    "dev": "tsx watch src/server.ts",
    "build": "tsc",
    "start": "node dist/server.js",
    "test": "node --test"
  }
}
```

## See also

- [`../README.md`](../README.md) — backend index
