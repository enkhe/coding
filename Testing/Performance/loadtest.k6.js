import http from 'k6/http';
import { check, sleep } from 'k6';
import { Trend } from 'k6/metrics';

const orderLatency = new Trend('order_latency_ms', true);

export const options = {
  scenarios: {
    steady_load: {
      executor: 'ramping-arrival-rate',
      startRate: 10,
      timeUnit: '1s',
      preAllocatedVUs: 50,
      maxVUs: 200,
      stages: [
        { target: 50, duration: '30s' },   // ramp to 50 RPS
        { target: 50, duration: '2m' },    // hold
        { target: 0, duration: '15s' },    // ramp down
      ],
    },
  },
  thresholds: {
    http_req_failed: ['rate<0.01'],         // < 1% errors
    http_req_duration: ['p(95)<300', 'p(99)<800'],
    order_latency_ms: ['p(99)<800'],
  },
};

const BASE = __ENV.BASE_URL || 'https://localhost:5001';

export default function () {
  const res = http.get(`${BASE}/orders`, {
    headers: { Accept: 'application/json' },
    tags: { name: 'GET /orders' },
  });

  orderLatency.add(res.timings.duration);

  check(res, {
    'status is 200': (r) => r.status === 200,
    'body is json':  (r) => (r.headers['Content-Type'] || '').includes('application/json'),
  });

  sleep(0.1);
}
