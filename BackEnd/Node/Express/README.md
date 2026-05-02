# Express

> The classic Node HTTP framework. Simple middleware pipeline. Pair with **zod** for validation and **pino** for logs.

## Quick Reference (TypeScript)

```ts
import express, { type ErrorRequestHandler } from 'express';
import pino from 'pino';
import pinoHttp from 'pino-http';
import { z } from 'zod';

const log = pino();
const app = express();
app.use(express.json({ limit: '1mb' }));
app.use(pinoHttp({ logger: log }));

const PlaceOrder = z.object({ userId: z.string().uuid(), amount: z.number().positive() });

app.post('/orders', (req, res, next) => {
  const parsed = PlaceOrder.safeParse(req.body);
  if (!parsed.success) return res.status(400).json({ errors: parsed.error.flatten() });
  const order = { id: crypto.randomUUID(), ...parsed.data };
  res.status(201).json(order);
});

app.get('/health/live', (_req, res) => res.json({ ok: true }));

const errorHandler: ErrorRequestHandler = (err, req, res, _next) => {
  req.log.error({ err }, 'unhandled');
  res.status(500).json({ type: 'about:blank', title: 'Internal Server Error', status: 500 });
};
app.use(errorHandler);

app.listen(8080, () => log.info('listening :8080'));
```

## Common Pitfalls

- Forgetting `next(err)` in async handlers → unhandled rejection. Use `express-async-errors` or async wrappers.
- `app.use(cors())` wide-open in prod
- Mounting middleware AFTER the route → middleware doesn't run
- No request size limit → memory DoS

## See also

- [../Fastify](../Fastify/) — more performance, better defaults
- [../NestJS](../NestJS/) — more structure
