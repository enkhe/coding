// MSW handlers — drop-in network mocks for unit + dev.
// npm i -D msw
import { http, HttpResponse } from 'msw';

export const handlers = [
  http.get('/api/orders', () =>
    HttpResponse.json([{ id: 'o-1', total: 9.99 }])
  ),

  http.post('/api/orders', async ({ request }) => {
    const body = (await request.json()) as { amount: number };
    return HttpResponse.json({ id: crypto.randomUUID(), total: body.amount }, { status: 201 });
  }),

  http.get('/api/me', ({ request }) => {
    const auth = request.headers.get('Authorization');
    if (!auth) return new HttpResponse(null, { status: 401 });
    return HttpResponse.json({ id: 'u-1', email: 'me@example.com' });
  }),
];

// Test setup (vitest):
// import { setupServer } from 'msw/node';
// import { handlers } from './msw-handlers';
// export const server = setupServer(...handlers);
// beforeAll(() => server.listen());
// afterEach(() => server.resetHandlers());
// afterAll(() => server.close());
