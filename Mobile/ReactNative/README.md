# React Native

> React Native with Expo SDK 52+, TypeScript, the new architecture (Fabric + TurboModules), and Hermes.

## Core Concepts

- **Expo (managed)** vs **bare** workflow:
  - Expo: hosted builds (EAS), config plugins, OTA updates, prebuilt native modules. Default for greenfield.
  - Bare: native projects exposed; pick when you need exotic native code or custom signing flows.
- **JSX components** + **hooks** (`useState`, `useEffect`, `useMemo`, `useCallback`).
- **Navigation**: `@react-navigation/native` with `native-stack` (uses platform navigators).
- **Styling**: inline `StyleSheet.create({...})`, or `nativewind` for Tailwind classes.
- **Native modules**: bridge Swift/Kotlin with `expo-modules-core` or TurboModules. Or use a config plugin.
- **State**: React state -> Zustand / Redux Toolkit / Jotai for app-scope; React Query / TanStack Query for server state.

## "To Be Dangerous" Cheatsheet

```bash
# new app
npx create-expo-app my-app -t default
cd my-app
npm run ios       # iOS simulator (macOS only)
npm run android   # Android emulator
npm run web       # web preview

# add deps
npx expo install expo-router @react-navigation/native
npm i @tanstack/react-query zustand zod

# native build (cloud)
eas build --platform ios
eas build --platform android
eas update --branch production
```

Component pattern:

```tsx
import { View, Text, Pressable } from 'react-native';
import { useState } from 'react';

export function Counter() {
  const [n, setN] = useState(0);
  return (
    <View>
      <Text>Count: {n}</Text>
      <Pressable onPress={() => setN(n + 1)}><Text>+</Text></Pressable>
    </View>
  );
}
```

## Quick Reference

| Need               | Lib / API                                            |
| ------------------ | ---------------------------------------------------- |
| Routing            | `expo-router` (file-based) or `@react-navigation`    |
| HTTP + cache       | `@tanstack/react-query` + `fetch`                    |
| Forms              | `react-hook-form` + `zod`                            |
| Storage            | `expo-secure-store`, `@react-native-async-storage`   |
| Lists              | `FlashList` (Shopify) > `FlatList` for perf          |
| Images             | `expo-image`                                         |
| Animations         | `react-native-reanimated` v3                         |
| Push               | `expo-notifications`                                 |

## Common Pitfalls

- Mixing JS bridge work on the UI thread — use `InteractionManager.runAfterInteractions`.
- Keys missing in lists -> bad reconciliation; always pass stable `keyExtractor`.
- Using `console.log` in tight loops on release — disable in production.
- Forgetting to run `npx expo install` (it pins to Expo-compatible versions) instead of plain `npm i`.
- Native module changes without `eas build` rebuild.

## Examples in this folder

- [App.tsx](./App.tsx) - Expo entry with React Query + Navigation provider.
- [HomeScreen.tsx](./HomeScreen.tsx) - Screen using hooks, FlatList, and a typed fetch.

## See also

- [Mobile/](../README.md)
- Expo: <https://docs.expo.dev>
- React Navigation: <https://reactnavigation.org>
