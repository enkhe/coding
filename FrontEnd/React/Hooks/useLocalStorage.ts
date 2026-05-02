import { useCallback, useSyncExternalStore } from 'react';

/**
 * Persisted state via localStorage, SSR-safe and tab-synchronized
 * (listens to the `storage` event).
 *
 *   const [theme, setTheme] = useLocalStorage('theme', 'light');
 */
export function useLocalStorage<T>(
  key: string,
  initial: T,
): [T, (next: T | ((prev: T) => T)) => void] {
  const subscribe = (cb: () => void) => {
    window.addEventListener('storage', cb);
    return () => window.removeEventListener('storage', cb);
  };

  const getSnapshot = () => {
    const raw = localStorage.getItem(key);
    return raw ?? JSON.stringify(initial);
  };

  const getServerSnapshot = () => JSON.stringify(initial);

  const raw = useSyncExternalStore(subscribe, getSnapshot, getServerSnapshot);
  const value = JSON.parse(raw) as T;

  const setValue = useCallback(
    (next: T | ((prev: T) => T)) => {
      const resolved =
        typeof next === 'function' ? (next as (p: T) => T)(value) : next;
      localStorage.setItem(key, JSON.stringify(resolved));
      // Notify same-tab subscribers (storage event only fires across tabs)
      window.dispatchEvent(new StorageEvent('storage', { key }));
    },
    [key, value],
  );

  return [value, setValue];
}
