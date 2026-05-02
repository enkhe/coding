#!/usr/bin/env bash
# .git/hooks/pre-commit — fast, must not block normal flow.
# Install: ln -s ../../scripts/pre-commit-hook.sh .git/hooks/pre-commit
set -euo pipefail

# 1) Block direct commits to protected branches.
branch=$(git symbolic-ref --short HEAD)
if [[ "$branch" == "main" || "$branch" == "master" ]]; then
  echo "Direct commits to $branch are not allowed. Use a feature branch + PR." >&2
  exit 1
fi

# 2) Forbid common secret patterns.
if git diff --cached --name-only -z | xargs -0 grep -EnIH \
     -e 'AKIA[0-9A-Z]{16}' \
     -e 'AIza[0-9A-Za-z_\-]{35}' \
     -e 'ghp_[0-9A-Za-z]{36}' \
     -e 'sk-[A-Za-z0-9]{20,}' \
     -e '-----BEGIN (RSA )?PRIVATE KEY-----' 2>/dev/null; then
  echo "Possible secret detected. Aborting commit." >&2
  exit 1
fi

# 3) Format C# (if available) — non-blocking on missing tool.
if command -v dotnet >/dev/null && [[ -n "$(git diff --cached --name-only --diff-filter=ACM '*.cs')" ]]; then
  dotnet format --include $(git diff --cached --name-only --diff-filter=ACM '*.cs')
  git add $(git diff --cached --name-only --diff-filter=ACM '*.cs')
fi

# 4) Conventional Commits message check (delegated to commit-msg hook usually, but quick lint here).
exit 0
