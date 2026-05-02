# Bash

> Bash 5+. The first line of every script: `set -euo pipefail`.

## Core Concepts

- **`set -euo pipefail`** — fail on error / unset var / pipe failure
- **Parameter expansion** — `${var:-default}`, `${var:=default}`, `${var:?error}`, `${var:offset:length}`, `${var/foo/bar}`
- **Arrays** — `arr=(a b c)`, `${arr[@]}`, `${#arr[@]}`
- **Functions** — `local` everywhere, return via stdout
- **`trap`** — cleanup on EXIT/INT/TERM
- **Process substitution** — `<(cmd)` / `>(cmd)`
- **Heredocs** — `<<EOF`, `<<-EOF` for indented, `<<'EOF'` to disable expansion

## "To Be Dangerous" Cheatsheet

| Need | Pattern |
|---|---|
| Strict mode | `set -euo pipefail; IFS=$'\n\t'` |
| Cleanup | `trap 'rm -rf "$tmp"' EXIT` |
| Read all lines | `mapfile -t lines < file.txt` |
| Loop file | `while IFS= read -r line; do ...; done < file` |
| JSON parse | `jq -r '.field' file.json` |
| Required arg | `: "${VAR:?must be set}"` |
| Default | `: "${VAR:=default}"` |
| Parallel | `xargs -P 8` or `parallel` |
| Test runner | `bats` |

## Quick Reference (safe template)

```bash
#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

readonly script_dir=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
readonly tmp=$(mktemp -d)
trap 'rm -rf "$tmp"' EXIT

usage() {
  cat <<EOF
Usage: $(basename "$0") [-v] -i INPUT -o OUTPUT

Options:
  -i  Input path (required)
  -o  Output path (required)
  -v  Verbose
EOF
  exit 2
}

verbose=0
while getopts ":i:o:vh" opt; do
  case "$opt" in
    i) input=$OPTARG ;;
    o) output=$OPTARG ;;
    v) verbose=1 ;;
    h|*) usage ;;
  esac
done
: "${input:?-i required}"
: "${output:?-o required}"

(( verbose )) && set -x

# ... real work ...
echo "OK: $input -> $output"
```

## Common Pitfalls

- Forgetting quotes around `"$var"` → word-splitting bugs on paths with spaces
- `[ ... ]` instead of `[[ ... ]]` → no regex, weaker semantics
- Using `cat | grep` everywhere — `grep` reads files directly
- Pipes hide errors without `pipefail`
- `&&` chains in if-statements — use proper structure for clarity

## Examples in this folder

- [`safe-template.sh`](safe-template.sh)
- [`parallel-jobs.sh`](parallel-jobs.sh)
- [`cheatsheet.sh`](cheatsheet.sh)

## See also

- [../PowerShell](../PowerShell/) · [../CLI](../CLI/) · [../../DevOps/Docker](../../DevOps/Docker/)
