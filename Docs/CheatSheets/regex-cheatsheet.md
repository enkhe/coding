# Regex Cheatsheet

## Atoms

| Pattern | Matches |
|---|---|
| `.` | any char (no newline) |
| `\d` | digit `[0-9]` |
| `\D` | non-digit |
| `\w` | word char `[A-Za-z0-9_]` |
| `\W` | non-word |
| `\s` | whitespace |
| `\S` | non-whitespace |
| `\b` | word boundary |
| `^` / `$` | start / end of line (or string) |

## Quantifiers (greedy by default; add `?` for lazy)

| Pattern | Meaning |
|---|---|
| `*` | 0 or more |
| `+` | 1 or more |
| `?` | 0 or 1 |
| `{n}` | exactly n |
| `{n,}` | n or more |
| `{n,m}` | n to m |
| `*?` `+?` `??` `{n,m}?` | lazy |

## Groups & captures

```
(abc)          # capture
(?:abc)        # non-capture
(?P<name>abc)  # named (Python style)
(?<name>abc)   # named (.NET / PCRE)
\1, \2         # backreference
(?=abc)        # positive lookahead
(?!abc)        # negative lookahead
(?<=abc)       # positive lookbehind
(?<!abc)       # negative lookbehind
```

## Useful patterns

```
^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$           # crude email
^https?://([\w.-]+)(:\d+)?(/.*)?$                          # url
^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$   # uuid
^\d{4}-\d{2}-\d{2}$                                        # ISO date
^[A-Z]{1,4}-\d+$                                            # ticket id (PROJ-123)
^#[0-9A-Fa-f]{6}$                                           # hex color
```

## Substitution

```
sed -E 's/^(\w+)\s+(\w+)$/\2 \1/'    # swap two words
```

```python
re.sub(r"(\d+)", lambda m: str(int(m.group(1)) * 2), "n=3")
```

```csharp
var rx = new Regex(@"(?<n>\d+)", RegexOptions.Compiled);
rx.Replace(input, m => (int.Parse(m.Groups["n"].Value) * 2).ToString());
```

## Flags

| Flag | Meaning |
|---|---|
| `i` | case-insensitive |
| `m` | multiline (`^`/`$` match per line) |
| `s` | dot matches newline |
| `x` | extended (whitespace + `#` comments allowed) |

## Common Pitfalls

- Greedy matches eating too much (`<.*>` matching across tags) — use `<.*?>` or `[^>]*`.
- Anchors in multiline contexts — set `m` flag or use `\A`/`\z`.
- Catastrophic backtracking — avoid nested quantifiers like `(a+)+`.
