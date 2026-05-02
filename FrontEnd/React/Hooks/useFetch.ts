import { useEffect, useState } from 'react';

type State<T> =
  | { status: 'idle' | 'loading' }
  | { status: 'success'; data: T }
  | { status: 'error'; error: Error };

/**
 * Minimal fetch hook for demos. Prefer TanStack Query for real apps —
 * this has no caching, retries, dedup, or revalidation.
 */
export function useFetch<T>(url: string, init?: RequestInit): State<T> {
  const [state, setState] = useState<State<T>>({ status: 'loading' });

  useEffect(() => {
    const ctrl = new AbortController();
    setState({ status: 'loading' });

    fetch(url, { ...init, signal: ctrl.signal })
      .then(async (r) => {
        if (!r.ok) throw new Error(`HTTP ${r.status}`);
        return (await r.json()) as T;
      })
      .then((data) => setState({ status: 'success', data }))
      .catch((error: Error) => {
        if (error.name !== 'AbortError') setState({ status: 'error', error });
      });

    return () => ctrl.abort();
  }, [url]);

  return state;
}
