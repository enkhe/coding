# Flutter

> Flutter 3.27+ — Dart-based cross-platform UI toolkit, single codebase for iOS, Android, Web, desktop.

## Core Concepts

- **Everything is a Widget**: `StatelessWidget` for pure render, `StatefulWidget` when local mutable state is needed.
- **Composition over inheritance**: build complex UIs by nesting small widgets.
- **Render pipeline**: framework -> Skia / Impeller, sub-frame rebuilds via `setState` or reactive providers.
- **State management**: `setState` (local), Provider (small apps), **Riverpod 2** (recommended), Bloc (event-driven).
- **Navigation**: imperative `Navigator.push`, declarative `go_router` (URL-driven, recommended).
- **Async**: `Future`/`async`/`await` for one-shot, `Stream` for sequences, `FutureBuilder`/`StreamBuilder` to wire to UI.
- **Packages**: from <https://pub.dev>, declared in `pubspec.yaml`.

## "To Be Dangerous" Cheatsheet

```bash
flutter create my_app
cd my_app && flutter run               # picks attached device
flutter run -d chrome                  # web
flutter pub add riverpod go_router dio
flutter test
flutter build apk --release
flutter build ipa --release
```

Key widgets:

```
Container, Padding, Center, Column, Row, Stack, Expanded, Flexible
Text, Image, Icon
ListView.builder, GridView.builder
Scaffold, AppBar, Drawer, BottomNavigationBar
TextField, ElevatedButton, GestureDetector
```

Riverpod minimal pattern:

```dart
final counterProvider = StateProvider<int>((ref) => 0);

class MyButton extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final count = ref.watch(counterProvider);
    return ElevatedButton(
      onPressed: () => ref.read(counterProvider.notifier).state++,
      child: Text('Count: $count'),
    );
  }
}
```

## Quick Reference

| Need                | API                                                 |
| ------------------- | --------------------------------------------------- |
| Local state         | `StatefulWidget` + `setState`                       |
| App-wide state      | `Riverpod` providers / `Bloc`                       |
| Navigation          | `go_router` (declarative URL routing)               |
| HTTP                | `dio` or `http` package                             |
| Persistence         | `shared_preferences`, `drift`, `sqflite`, `isar`    |
| Async UI            | `FutureBuilder` / `StreamBuilder`                   |
| Theming             | `ThemeData`, `Theme.of(context)`                    |
| Forms               | `Form` + `TextFormField` + `GlobalKey<FormState>`   |

## Common Pitfalls

- Calling `setState` after `dispose()` — guard with `if (!mounted) return;`.
- Rebuilding huge subtrees on every state change — split widgets, use `const` constructors.
- Blocking the UI thread with synchronous heavy work — use `compute()` / isolates.
- Forgetting `flutter pub get` after editing `pubspec.yaml`.
- Mixing imperative `Navigator` with `go_router` — pick one.

## Examples in this folder

- [main.dart](./main.dart) - Material 3 app entry point with `ProviderScope`.
- [counter_app.dart](./counter_app.dart) - Riverpod counter screen showing reactive state + async fetch.

## See also

- [Mobile/](../README.md)
- Flutter docs: <https://docs.flutter.dev>
- Riverpod: <https://riverpod.dev>
