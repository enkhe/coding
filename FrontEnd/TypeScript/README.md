# TypeScript

> JavaScript with types. Use **strict** mode. Prefer types over interfaces unless you need declaration merging.

## "To Be Dangerous" Cheatsheet

```ts
// Generics
function pick<T, K extends keyof T>(obj: T, ...keys: K[]): Pick<T, K> {
  return keys.reduce((acc, k) => ({ ...acc, [k]: obj[k] }), {} as Pick<T, K>);
}

// Discriminated unions
type Result<T> =
  | { kind: 'ok'; value: T }
  | { kind: 'err'; error: string };

function unwrap<T>(r: Result<T>): T {
  if (r.kind === 'ok') return r.value;     // narrowed
  throw new Error(r.error);
}

// satisfies — preserves narrow types while ensuring conformance
const config = {
  env: 'prod',
  timeoutMs: 5000,
} satisfies { env: 'prod' | 'staging' | 'dev'; timeoutMs: number };

// Mapped + conditional
type ReadonlyDeep<T> = { readonly [K in keyof T]: T[K] extends object ? ReadonlyDeep<T[K]> : T[K] };
type Awaited2<T> = T extends Promise<infer U> ? Awaited2<U> : T;

// Branded types (nominal typing)
type UserId = string & { readonly __brand: 'UserId' };
const asUserId = (s: string) => s as UserId;

// const assertions
const ROLES = ['admin', 'user', 'viewer'] as const;
type Role = typeof ROLES[number];   // 'admin' | 'user' | 'viewer'
```

## tsconfig.json (strict baseline)

```jsonc
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "ESNext",
    "moduleResolution": "Bundler",
    "strict": true,
    "noUncheckedIndexedAccess": true,
    "noImplicitOverride": true,
    "exactOptionalPropertyTypes": true,
    "isolatedModules": true,
    "verbatimModuleSyntax": true,
    "skipLibCheck": true,
    "resolveJsonModule": true
  }
}
```

## Common Pitfalls

- `any` everywhere → defeats the point. Use `unknown` and narrow.
- Type assertions (`as Foo`) hiding bugs the compiler caught
- Forgetting `strict: true` (or selectively turning checks off)
- Misunderstanding structural typing — duck typing through and through

## See also

- [../JavaScript](../JavaScript/) · [../React](../React/) · [../Vue](../Vue/)
