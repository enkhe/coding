# Vue 3.5+

> Composition API + `<script setup>` + Pinia for state. Less ceremony than Angular, more structure than React.

## Quick Reference (single-file component)

```vue
<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';

const count = ref(0);
const doubled = computed(() => count.value * 2);

watch(count, (n) => console.log('count =', n));
onMounted(() => console.log('mounted'));

function increment() { count.value++; }
</script>

<template>
  <p>Count: {{ count }} (doubled: {{ doubled }})</p>
  <button @click="increment">+1</button>
</template>

<style scoped>
button { padding: 0.5rem 1rem; }
</style>
```

## "To Be Dangerous"

| Need | API |
|---|---|
| State | `ref(0)`, `reactive({})` |
| Computed | `computed(() => ...)` |
| Watch | `watch(source, cb)` |
| Composables | `useFoo()` functions returning refs |
| Router | `vue-router` (`<RouterLink>`, `useRoute`, `useRouter`) |
| State store | **Pinia** |
| HTTP | `fetch` or **`@tanstack/vue-query`** |

## Common Pitfalls

- Forgetting `.value` on refs in script
- Mutating reactive arrays without triggering reactivity (modern Vue handles this; older code didn't)
- Mixing Options API and Composition API in one file
- Heavy non-shallow reactives — use `shallowRef` for big read-mostly trees

## See also

- [../React](../React/) · [../Svelte](../Svelte/) · [../StateManagement](../StateManagement/)
