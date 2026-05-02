# iOS

> Native iOS development with Swift 6, SwiftUI, iOS 18 SDK. UIKit only when legacy demands it.

## Core Concepts

- **SwiftUI is the default** for new code. Declarative UI; views are value types described by `body`.
- **State property wrappers**:
  - `@State` — view-local mutable state.
  - `@Binding` — read/write reference to state owned elsewhere.
  - `@Observable` (Swift 5.9+ macro) — observable model class; replaces `ObservableObject`/`@Published`.
  - `@Environment` — read injected environment values (locale, color scheme, dismiss).
- **Navigation** with `NavigationStack` + `NavigationLink(value:)` + `.navigationDestination(for:)`.
- **Concurrency**: `async`/`await`, `Task`, `actor`, `MainActor` for UI updates.
- **Strict concurrency**: Swift 6 enforces data-race safety; mark UI types `@MainActor`.
- **Persistence**: SwiftData (preferred), Core Data (legacy), `UserDefaults`, `Keychain`.
- **UIKit interop**: `UIViewRepresentable` / `UIViewControllerRepresentable` to embed legacy views.

## "To Be Dangerous" Cheatsheet

```swift
@Observable
final class WeatherStore {
    var temperature: Double = 0
    func refresh() async throws {
        let url = URL(string: "https://api.example.com/weather")!
        let (data, _) = try await URLSession.shared.data(from: url)
        temperature = try JSONDecoder().decode(Reading.self, from: data).temp
    }
}

struct WeatherView: View {
    @State private var store = WeatherStore()
    var body: some View {
        VStack {
            Text("\(store.temperature, specifier: "%.1f")C")
            Button("Refresh") { Task { try? await store.refresh() } }
        }
    }
}
```

Project anatomy:

```
MyApp/
  MyAppApp.swift          // @main, App scene
  ContentView.swift       // root view
  Assets.xcassets/        // images, colors
  Info.plist              // permissions, configuration
  Preview Content/        // #Preview snapshots
```

## Quick Reference

| Need                  | API                                                   |
| --------------------- | ----------------------------------------------------- |
| Lists                 | `List`, `ForEach`, `LazyVStack`                       |
| Forms                 | `Form { Section { TextField(..) } }`                  |
| Navigation            | `NavigationStack` + `navigationDestination(for:)`     |
| Sheets / popovers     | `.sheet(isPresented:)`, `.popover`, `.alert`          |
| Persistence           | `@Model` + `ModelContainer` (SwiftData)               |
| HTTP                  | `URLSession.shared.data(for:)` with `async`           |
| Tasks                 | `Task { await ... }`, `.task { ... }` modifier        |
| Previews              | `#Preview { WeatherView() }`                          |

## Common Pitfalls

- Mutating UI state off `@MainActor` — Swift 6 will reject at compile time, fix don't suppress.
- Holding strong refs in `Task`s -> retain cycles. Prefer structured concurrency via `.task` view modifier.
- Force-unwrapping optionals in production. Use `guard let` / `if let`.
- Using `ObservableObject` for new code — switch to `@Observable`.
- Skipping iOS Privacy Manifest (`PrivacyInfo.xcprivacy`) for SDKs that need declarations.

## Examples in this folder

- [ContentView.swift](./ContentView.swift) - Root view with a `NavigationStack`.
- [WeatherView.swift](./WeatherView.swift) - Async fetch with `@Observable` store + `.task`.

## See also

- [Mobile/](../README.md)
- Apple SwiftUI tutorials: <https://developer.apple.com/tutorials/swiftui>
- Swift 6 migration: <https://www.swift.org/migration/>
