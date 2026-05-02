# Tools

> Developer productivity tooling, shells, scripting languages, and editors for the .NET 2026 stack.

## Core Concepts

- **Source control**: Git is the floor. Conventional commits + trunk-based + signed commits = pro baseline.
- **Shells**: pick one daily driver (Bash on macOS/Linux, PowerShell 7 cross-platform). Know both.
- **CLI ergonomics**: modern Rust-based tools (`rg`, `fd`, `bat`, `eza`, `fzf`) replace GNU classics for speed and UX.
- **Scripting**: small/glue work in Bash or PowerShell, anything bigger in Python (typed + tested).
- **Package managers**: every ecosystem has one (or three). Lockfiles + reproducible installs are non-negotiable.
- **Editors**: Visual Studio for heavy debugging, VS Code for everything else, Rider for power refactoring.

## "To Be Dangerous" Cheatsheet

Developer tooling map:

```
Source control       -> Git + GitHub CLI (gh)
Shell (cross-plat)   -> PowerShell 7
Shell (unix)         -> Bash 5 / Zsh
Search & navigate    -> ripgrep, fd, fzf, bat, eza
JSON / YAML          -> jq, yq
HTTP                 -> httpie, curl, REST Client (VS Code)
Glue scripting       -> Python 3.12 + uv + typer
Containers           -> Docker, podman, devcontainers
Editors              -> Visual Studio 2026, VS Code, Rider, Neovim
Package mgmt         -> NuGet, npm/pnpm, pip/uv, brew, winget
```

## Quick Reference

| Need                          | Tool                                           |
| ----------------------------- | ---------------------------------------------- |
| Fuzzy find files / history    | `fzf`, `Ctrl-R`                                |
| Search code                   | `rg pattern`                                   |
| Find files                    | `fd pattern`                                   |
| Pretty file print             | `bat file`                                     |
| Listing dirs                  | `eza -lah --git`                               |
| HTTP from CLI                 | `http GET url`                                 |
| JSON manipulation             | `jq '.field'`                                  |
| Run repeatable tasks          | `make`, `just`, `nx`, `task`                   |
| Cross-platform automation     | PowerShell 7 + `Invoke-RestMethod`             |

## Common Pitfalls

- Memorizing GUI workflows that aren't scriptable — automate everything.
- Skipping `.editorconfig` and pre-commit hooks until the team grows.
- Letting lockfiles drift between dev / CI.
- Mixing global and project-local tool installs (use `dotnet tool`, `npx`, `uv tool`, `pipx`).
- Storing secrets in shell history or committed files — use `direnv`, `1Password CLI`, `dotnet user-secrets`.

## Examples in this folder

- [Git/](./Git/README.md) - Git workflows + Conventional Commits.
- [CLI/](./CLI/README.md) - Modern shell tooling.
- [Bash/](./Bash/README.md) - Bash scripting essentials.
- [PowerShell/](./PowerShell/README.md) - PowerShell 7+ idioms.
- [Python/](./Python/README.md) - Python 3.12 scripting.
- [PackageManagers/](./PackageManagers/README.md) - NuGet, npm, pip, brew, winget, etc.
- [Editors/](./Editors/README.md) - VS 2026, VS Code, Rider.
- [VsCodeExtensions/](./VsCodeExtensions/README.md) - Must-have extensions.

## See also

- [.NET 2026 Senior/Architect Roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [DevOps/](../DevOps/README.md) - CI/CD pipelines.
