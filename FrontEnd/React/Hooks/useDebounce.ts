import { useEffect, useState } from 'react';

/**
 * Returns `value` after `delay` ms of stability — useful for search inputs.
 *
 *   const debounced = useDebounce(query, 300);
 */
export function useDebounce<T>(value: T, delay = 300): T {
  const [debounced, setDebounced] = useState(value);

  useEffect(() => {
    const id = setTimeout(() => setDebounced(value), delay);
    return () => clearTimeout(id);
  }, [value, delay]);

  return debounced;
}
