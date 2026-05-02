// Zustand — minimal global state in React.
// npm i zustand
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface CartItem { id: string; name: string; qty: number; price: number }

interface CartState {
  items: CartItem[];
  add: (item: Omit<CartItem, 'qty'>) => void;
  remove: (id: string) => void;
  clear: () => void;
  total: () => number;
}

export const useCart = create<CartState>()(
  persist(
    (set, get) => ({
      items: [],
      add: (item) =>
        set((s) => {
          const existing = s.items.find((i) => i.id === item.id);
          if (existing) {
            return { items: s.items.map((i) => (i.id === item.id ? { ...i, qty: i.qty + 1 } : i)) };
          }
          return { items: [...s.items, { ...item, qty: 1 }] };
        }),
      remove: (id) => set((s) => ({ items: s.items.filter((i) => i.id !== id) })),
      clear: () => set({ items: [] }),
      total: () => get().items.reduce((sum, i) => sum + i.price * i.qty, 0),
    }),
    { name: 'cart' } // localStorage key
  )
);

// Selector pattern avoids re-renders for unrelated state changes.
export const useCartCount = () => useCart((s) => s.items.length);
export const useCartTotal = () => useCart((s) => s.total());
