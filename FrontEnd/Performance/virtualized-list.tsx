// Virtualized list — only render visible rows. Critical for 1000+ row tables.
// npm i @tanstack/react-virtual
import { useRef } from 'react';
import { useVirtualizer } from '@tanstack/react-virtual';

export function OrdersList({ orders }: { orders: { id: string; total: number }[] }) {
  const parentRef = useRef<HTMLDivElement>(null);

  const virtualizer = useVirtualizer({
    count: orders.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => 48,
    overscan: 8,
  });

  return (
    <div
      ref={parentRef}
      style={{ height: 600, overflow: 'auto', contain: 'strict' }}
    >
      <div style={{ height: virtualizer.getTotalSize(), position: 'relative' }}>
        {virtualizer.getVirtualItems().map((vi) => {
          const order = orders[vi.index];
          return (
            <div
              key={order.id}
              style={{
                position: 'absolute',
                top: 0, left: 0, width: '100%',
                height: vi.size,
                transform: `translateY(${vi.start}px)`,
              }}
            >
              <a href={`/orders/${order.id}`}>{order.id} — ${order.total.toFixed(2)}</a>
            </div>
          );
        })}
      </div>
    </div>
  );
}
