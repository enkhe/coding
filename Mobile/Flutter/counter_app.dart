// Counter + async fetch demo using Riverpod 2.
import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

// Synchronous, mutable state.
final counterProvider = StateProvider<int>((ref) => 0);

// Async, cached "remote" greeting that refetches when count changes.
final greetingProvider = FutureProvider<String>((ref) async {
  final count = ref.watch(counterProvider);
  await Future.delayed(const Duration(milliseconds: 300));
  return 'Hello, you tapped $count time${count == 1 ? '' : 's'}.';
});

class CounterScreen extends ConsumerWidget {
  const CounterScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final count = ref.watch(counterProvider);
    final greetingAsync = ref.watch(greetingProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Riverpod Counter')),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text('Count: $count', style: Theme.of(context).textTheme.headlineMedium),
            const SizedBox(height: 16),
            greetingAsync.when(
              data: (text) => Text(text),
              loading: () => const CircularProgressIndicator(),
              error: (e, _) => Text('Error: $e'),
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => ref.read(counterProvider.notifier).state++,
        child: const Icon(Icons.add),
      ),
    );
  }
}
