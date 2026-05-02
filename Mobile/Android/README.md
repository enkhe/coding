# Android

> Native Android with Kotlin 2, Jetpack Compose 1.7+, Hilt DI, Coroutines/Flow.

## Core Concepts

- **Composables** are functions annotated `@Composable`. UI is described, not constructed.
- **State**: `remember { mutableStateOf(...) }` for local; `StateFlow`/`SharedFlow` from `ViewModel` for screen-scope.
- **State hoisting**: lift state up, pass `value` + `onValueChange` down. Keeps composables stateless and testable.
- **Navigation**: `androidx.navigation:navigation-compose` with typed destinations (Kotlin Serialization).
- **DI**: Hilt (`@HiltAndroidApp`, `@AndroidEntryPoint`, `@HiltViewModel`, `@Inject`).
- **Async**: `viewModelScope.launch { ... }`, `Flow` operators, `collectAsStateWithLifecycle()` in UI.
- **Side effects**: `LaunchedEffect`, `DisposableEffect`, `rememberCoroutineScope`.
- **Theming**: Material 3 (`MaterialTheme`, dynamic color from Android 12+).

## "To Be Dangerous" Cheatsheet

```kotlin
@Composable
fun Counter(initial: Int = 0) {
    var count by remember { mutableStateOf(initial) }
    Column {
        Text("Count: $count")
        Button(onClick = { count++ }) { Text("+") }
    }
}
```

ViewModel + Flow:

```kotlin
@HiltViewModel
class WeatherViewModel @Inject constructor(
    private val repo: WeatherRepository
) : ViewModel() {
    val state: StateFlow<UiState> = repo.observe()
        .map { UiState.Loaded(it) }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5_000), UiState.Loading)
}
```

Project layout:

```
app/
  src/main/java/com/acme/app/
    MainActivity.kt
    di/                 // Hilt modules
    feature/weather/    // screen + viewmodel + repo
    ui/theme/           // Material 3 theme
  src/main/AndroidManifest.xml
build.gradle.kts (app + module)
```

## Quick Reference

| Need                  | API                                                       |
| --------------------- | --------------------------------------------------------- |
| Local state           | `var x by remember { mutableStateOf(...) }`               |
| Lifecycle-aware flow  | `flow.collectAsStateWithLifecycle()`                      |
| Lists                 | `LazyColumn { items(list) { ... } }`                      |
| Navigation            | `NavHost(navController) { composable<Route> { ... } }`    |
| HTTP                  | Retrofit + OkHttp + kotlinx.serialization                 |
| Persistence           | Room (SQL), DataStore (KV)                                |
| DI                    | Hilt (`@Inject`, `@Provides`, `@HiltViewModel`)           |
| Background            | WorkManager (deferrable), Foreground Services (ongoing)   |

## Common Pitfalls

- Triggering side effects in composition body — wrap in `LaunchedEffect(key)`.
- Reading `Flow` without `collectAsStateWithLifecycle` -> leaks/jank.
- Holding Activity/Context references in ViewModel -> memory leaks.
- Skipping `@Stable`/`@Immutable` on data classes -> unnecessary recomposition.
- Forgetting `INTERNET`, runtime permissions, or POST_NOTIFICATIONS (Android 13+) in manifest.

## Examples in this folder

- [MainActivity.kt](./MainActivity.kt) - Single-activity app with Compose + Navigation.
- [WeatherScreen.kt](./WeatherScreen.kt) - Stateful screen consuming a `StateFlow` from a Hilt ViewModel.

## See also

- [Mobile/](../README.md)
- Compose docs: <https://developer.android.com/jetpack/compose>
- Hilt: <https://dagger.dev/hilt/>
