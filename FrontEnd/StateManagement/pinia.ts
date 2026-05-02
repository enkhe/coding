// Pinia — Vue 3 store. Setup-style stores read like composables.
// npm i pinia
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export interface CartItem { id: string; name: string; qty: number; price: number }

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([]);

  const total = computed(() => items.value.reduce((s, i) => s + i.price * i.qty, 0));
  const count = computed(() => items.value.length);

  function add(item: Omit<CartItem, 'qty'>) {
    const existing = items.value.find((i) => i.id === item.id);
    if (existing) existing.qty += 1;
    else items.value.push({ ...item, qty: 1 });
  }
  function clear() { items.value = []; }

  return { items, total, count, add, clear };
});
