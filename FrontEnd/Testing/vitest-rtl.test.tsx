// Vitest + React Testing Library — typical unit test.
// npm i -D vitest @testing-library/react @testing-library/user-event @testing-library/jest-dom jsdom
/// <reference types="@testing-library/jest-dom/vitest" />
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect } from 'vitest';

function Counter() {
  const [n, setN] = (globalThis as any).React.useState(0);
  return (
    <>
      <p>Count: {n}</p>
      <button onClick={() => setN((x: number) => x + 1)}>Increment</button>
    </>
  );
}

describe('Counter', () => {
  it('starts at 0', () => {
    render(<Counter />);
    expect(screen.getByText('Count: 0')).toBeInTheDocument();
  });

  it('increments on click', async () => {
    render(<Counter />);
    await userEvent.click(screen.getByRole('button', { name: /increment/i }));
    expect(screen.getByText('Count: 1')).toBeInTheDocument();
  });
});
