# Modern CLI ergonomics

> The toolbox that makes shell life faster.

## Must-have replacements

| Classic | Modern | Why |
|---|---|---|
| `grep` | **`rg`** (ripgrep) | 10× faster, gitignore-aware |
| `find` | **`fd`** | nicer syntax, faster |
| `cat` | **`bat`** | syntax highlight, line numbers |
| `ls` | **`eza`** | colors, git status, tree |
| `cd` | **`zoxide`** (`z`) | jump by frecency |
| `ps` | **`procs`** | clearer columns |
| `top` | **`btop`** / **`htop`** | usable |
| `du` | **`dust`** | tree-shaped |
| `df` | **`duf`** | colorized |
| `awk`/`sed` | **`jq`/`yq`** + `rg` | for JSON/YAML especially |
| `curl` | **`xh`** / `httpie` (or curl) | nicer JSON client |
| ad-hoc TUI | **`fzf`** | fuzzy-pick anything |

## "To Be Dangerous" Cheatsheet

```bash
# search code
rg --type cs 'PlaceOrder' src/                  # only .cs
rg -A 3 -B 1 'TODO'                              # 3 after, 1 before

# find files
fd -e cs -E bin -E obj                           # .cs files, exclude bin/obj

# inspect JSON
curl -s api/orders | jq '.[] | .id'
yq '.spec.replicas' deployment.yaml

# fuzzy pick a file or branch
fzf --preview 'bat --color=always --style=numbers --line-range=:200 {}'
git branch | fzf | xargs git checkout

# pipe to clipboard (macOS / Linux / Windows)
... | pbcopy        # macOS
... | xclip -selection clipboard
... | clip.exe      # Windows
```

## Useful aliases (bash/zsh)

```bash
alias l='eza -lah --git'
alias ll='eza -laTh --git --level=2'
alias gl='git log --oneline --graph --decorate --all'
alias gst='git status -sb'
alias k='kubectl'
alias d='docker'
alias dco='docker compose'
```

## Examples in this folder

- [`cli-cheatsheet.md`](cli-cheatsheet.md)

## See also

- [../Bash](../Bash/) · [../PowerShell](../PowerShell/) · [../Git](../Git/)
