// TanStack Query — server state cache for React.
// npm i @tanstack/react-query
import {
  QueryClient,
  QueryClientProvider,
  useQuery,
  useMutation,
  useQueryClient,
} from '@tanstack/react-query';

const client = new QueryClient({
  defaultOptions: { queries: { staleTime: 30_000, refetchOnWindowFocus: false } },
});

export function App() {
  return (
    <QueryClientProvider client={client}>
      <Orders userId="u-123" />
    </QueryClientProvider>
  );
}

function Orders({ userId }: { userId: string }) {
  const { data, isPending, error } = useQuery({
    queryKey: ['orders', userId],
    queryFn: async ({ signal }) => {
      const r = await fetch(`/api/orders?userId=${userId}`, { signal });
      if (!r.ok) throw new Error(r.statusText);
      return (await r.json()) as { id: string; total: number }[];
    },
  });

  const queryClient = useQueryClient();
  const placeOrder = useMutation({
    mutationFn: (body: { amount: number }) =>
      fetch('/api/orders', { method: 'POST', body: JSON.stringify(body) }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['orders', userId] }),
  });

  if (isPending) return <p>Loading…</p>;
  if (error) return <p>Error: {String(error)}</p>;

  return (
    <div>
      <button onClick={() => placeOrder.mutate({ amount: 9.99 })} disabled={placeOrder.isPending}>
        Place order
      </button>
      <ul>{data!.map((o) => <li key={o.id}>{o.total}</li>)}</ul>
    </div>
  );
}
