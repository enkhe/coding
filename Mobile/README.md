# Mobile

> Cross-platform and native mobile development for the .NET 2026 senior/architect roadmap.

## Core Concepts

- **Cross-platform UI**: one codebase, multiple stores. Trade UI fidelity for velocity.
- **Native UI**: SwiftUI / Jetpack Compose. Best UX, two codebases, two skill sets.
- **Shared business logic**: keep domain/persistence/networking in a portable layer (.NET Standard, Kotlin Multiplatform, Dart, TS).
- **Platform integration**: camera, BLE, push, biometrics, background tasks — every framework has a different escape hatch.
- **App store realities**: review cycles (1-3 days Apple, hours Google), entitlements, privacy manifests, signing, OTA limits.

## "To Be Dangerous" Cheatsheet

Decision tree:

```
Need pixel-perfect platform UX or heavy native APIs? -> Native (SwiftUI + Compose)
Already a .NET/C# shop, want one team?               -> .NET MAUI (or MAUI Blazor Hybrid)
Web team, JS/TS skillset, need fast iteration?       -> React Native (Expo)
Want best cross-platform UI fidelity + perf?         -> Flutter
Just an internal/B2B app, web is fine?               -> PWA / MAUI Blazor Hybrid
```

| Framework      | Language     | UI Engine            | Hot reload | Native feel | Bundle size |
| -------------- | ------------ | -------------------- | ---------- | ----------- | ----------- |
| MAUI 10        | C# / XAML    | Native controls      | Yes        | Good        | Medium      |
| Flutter 3.27+  | Dart         | Skia/Impeller canvas | Excellent  | Excellent   | Medium      |
| React Native   | TS/JS        | Native bridge/JSI    | Excellent  | Good        | Small-Med   |
| iOS native     | Swift 6      | SwiftUI/UIKit        | Previews   | Perfect     | Small       |
| Android native | Kotlin 2     | Jetpack Compose      | Yes        | Perfect     | Small       |

## Quick Reference

- Keep logic in plain libraries, push UI to the leaves. Test logic without simulators.
- CI/CD: Fastlane, GitHub Actions runners with macOS for iOS, MAUI workloads for .NET.
- Distribution: TestFlight (iOS), Play Internal Testing, Firebase App Distribution, App Center (sunset path).
- Telemetry: App Insights, Sentry, Firebase Crashlytics.
- Privacy: iOS PrivacyInfo.xcprivacy, Android Data Safety form, GDPR consent flows.

## Common Pitfalls

- Choosing framework on developer preference instead of product UX needs.
- Ignoring background-execution and battery rules — they differ wildly per OS.
- Skipping accessibility (Dynamic Type, TalkBack/VoiceOver) until late.
- Treating mobile like web: assuming always-online, big screens, fast CPUs.
- Forgetting app store rejection vectors (private APIs, missing privacy manifests, unclear IAP).

## Examples in this folder

- [MAUI/](./MAUI/README.md) - .NET MAUI 10, XAML/MVVM, Blazor Hybrid.
- [Flutter/](./Flutter/README.md) - Flutter 3.27+, Dart, Riverpod.
- [ReactNative/](./ReactNative/README.md) - Expo SDK 52+, TypeScript.
- [iOS/](./iOS/README.md) - SwiftUI, Swift 6, iOS 18.
- [Android/](./Android/README.md) - Kotlin 2, Jetpack Compose 1.7+.

## See also

- [.NET 2026 Senior/Architect Roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [FrontEnd/](../FrontEnd/README.md) - shared UI patterns.
- [Architecture/](../Architecture/README.md) - clean architecture for shared logic.
