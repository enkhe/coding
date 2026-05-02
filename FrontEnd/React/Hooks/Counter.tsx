import { useReducer, useCallback } from 'react';

type State = { count: number };
type Action = { type: 'inc' } | { type: 'dec' } | { type: 'set'; value: number };

function reducer(state: State, action: Action): State {
  switch (action.type) {
    case 'inc': return { count: state.count + 1 };
    case 'dec': return { count: state.count - 1 };
    case 'set': return { count: action.value };
  }
}

export function Counter() {
  const [state, dispatch] = useReducer(reducer, { count: 0 });

  // Stable callback identity — useful when passing to memoized children
  const reset = useCallback(() => dispatch({ type: 'set', value: 0 }), []);

  return (
    <div>
      <button onClick={() => dispatch({ type: 'dec' })}>-</button>
      <span>{state.count}</span>
      <button onClick={() => dispatch({ type: 'inc' })}>+</button>
      <button onClick={reset}>reset</button>
    </div>
  );
}
