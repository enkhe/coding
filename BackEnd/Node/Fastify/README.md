# Fastify

> Faster Express alternative. Schema-validated routes (typebox/json schema), plugin system, hooks, async-first.

## Quick Reference

```ts
import Fastify from 'fastify';
import { Type } from '@sinclair/typebox';
import type { Static } from '@sinclair/typebox';

const app = Fastify({ logger: true });

const PlaceOrder = Type.Object({
  userId: Type.String({ format: 'uuid' }),
  amount: Type.Number({ minimum: 0.01 }),
});
type PlaceOrder = Static<typeof PlaceOrder>;

app.post<{ Body: PlaceOrder }>('/orders', { schema: { body: PlaceOrder } }, async (req, reply) => {
  reply.code(201).send({ id: crypto.randomUUID(), ...req.body });
});

app.get('/health/live', async () => ({ ok: true }));

app.listen({ port: 8080, host: '0.0.0.0' });
```

## Why Fastify over Express

- Schema validation built-in (no middleware)
- Faster — by 2-3× in raw benchmarks
- Plugin encapsulation (no leaky middleware order issues)
- Better TypeScript story

## Common Pitfalls

- Mixing async and `reply.send()` — pick one
- Schema mismatch between TypeBox/JSON Schema and the runtime payload → silent strip
- Plugin scoping surprises — read the encapsulation docs once

## See also

- [../Express](../Express/) · [../NestJS](../NestJS/)
