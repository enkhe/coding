# Svelte 5

> Compiler-driven framework. Tiny runtime. Svelte 5 introduces **runes** — first-class reactivity primitives.

## Runes (the new model)

| Rune | Purpose |
|---|---|
| `$state` | Reactive state |
| `$derived` | Computed |
| `$effect` | Side effects |
| `$props` | Component props |
| `$bindable` | Two-way binding |

## Quick Reference

```svelte
<script lang="ts">
  let { initial = 0 } = $props<{ initial?: number }>();

  let count = $state(initial);
  let doubled = $derived(count * 2);

  $effect(() => {
    document.title = `Count: ${count}`;
  });

  function increment() { count++; }
</script>

<p>Count: {count} (doubled: {doubled})</p>
<button onclick={increment}>+1</button>

<style>
  button { padding: 0.5rem 1rem; }
</style>
```

## SvelteKit (full-stack)

- File-based routing (`src/routes/orders/+page.svelte`, `+page.server.ts`)
- Server-side data loading via `+page.server.ts` `load()`
- Form actions via `+page.server.ts` `actions = { default: ... }`
- Adapters: `adapter-node`, `adapter-vercel`, `adapter-cloudflare`

## Common Pitfalls

- Mixing pre-Svelte 5 stores (`writable()`) with runes — pick a path per project
- `$effect` running during SSR — guard with `if (typeof window !== 'undefined')` or `$effect.root`
- Reactivity surprises with class instances — use `$state.raw` if you don't want deep proxying

## See also

- [../React](../React/) · [../Vue](../Vue/) · [../StateManagement](../StateManagement/)
