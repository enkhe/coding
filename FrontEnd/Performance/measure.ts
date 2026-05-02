// Report Web Vitals to your analytics / OTel backend.
// npm i web-vitals
import { onLCP, onINP, onCLS, onTTFB, onFCP, type Metric } from 'web-vitals/attribution';

function send(metric: Metric) {
  const body = JSON.stringify({
    name: metric.name,
    value: metric.value,
    delta: metric.delta,
    id: metric.id,
    rating: metric.rating,
    attribution: metric.attribution,
    page: location.pathname,
  });

  // navigator.sendBeacon survives page unload; falls back to fetch.
  if (navigator.sendBeacon) navigator.sendBeacon('/_metrics/web-vitals', body);
  else fetch('/_metrics/web-vitals', { body, method: 'POST', keepalive: true });
}

onLCP(send);
onINP(send);
onCLS(send);
onTTFB(send);
onFCP(send);
