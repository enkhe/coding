import { useState } from 'react';

export function App() {
  const [count, setCount] = useState(0);
  const apiBase = import.meta.env.VITE_API_BASE ?? '/api';

  return (
    <main>
      <h1>Vite + React + TS</h1>
      <p>API base: <code>{apiBase}</code></p>
      <button onClick={() => setCount((c) => c + 1)}>count is {count}</button>
    </main>
  );
}
