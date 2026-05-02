<script lang="ts">
  // Svelte 5 — runes are first-class language features for reactivity.
  // No more $: stores; use $state, $derived, $effect.

  let count = $state(0);
  let doubled = $derived(count * 2);

  $effect(() => {
    document.title = `Count: ${count}`;
  });

  // Cross-component shared state — export a function returning runes from a .svelte.ts module.
  // (See cart.svelte.ts pattern.)
</script>

<button onclick={() => count++}>+1</button>
<p>Count: {count}, doubled: {doubled}</p>
