# Git

> The non-negotiable tool. Master rebase, commit hygiene, and conflict resolution.

## "To Be Dangerous" Cheatsheet

| Need | Command |
|---|---|
| Rewrite the last commit | `git commit --amend` (only if not pushed) |
| Move recent commits onto another branch | `git rebase --onto <new-base> <old-base>` |
| Interactive rebase (squash/reword) | `git rebase -i HEAD~5` |
| Cherry-pick one commit | `git cherry-pick <sha>` |
| Stash work-in-progress | `git stash push -m "wip" -- <pathspec>` |
| Bring back stash | `git stash pop` |
| Multiple worktrees | `git worktree add ../feature-x feature-x` |
| Stage a hunk | `git add -p` |
| Reset file but keep changes | `git restore --staged <path>` |
| Discard local changes | `git restore <path>` (DESTRUCTIVE) |
| See unmerged commits | `git log --oneline --graph --decorate main..feature` |
| Find commit that broke | `git bisect start` / `good` / `bad` |
| Recover after `--hard` | `git reflog` |

## Commit hygiene (Conventional Commits)

```
feat(orders): add idempotency-key on POST /orders

Why: prevents double-charge on retry.
Closes #123.
```

Types: `feat | fix | refactor | docs | test | perf | build | ci | chore`.

## Branching strategy (trunk-based)

- Short-lived feature branches (< 3 days)
- PR rebased onto main before merge
- main is always deployable
- Tags for releases (`v1.4.2`)

## Conflict resolution

```bash
git mergetool      # opens configured tool (vimdiff, code, etc.)
git rebase --continue
# or, if it's hopeless:
git rebase --abort
```

## Common Pitfalls

- `git pull` (default merge) creates noise — set `pull.rebase = true`
- `git push --force` on shared branches — use `--force-with-lease`
- Big commits → painful reviews. Aim for < 200 lines of diff per commit
- Committing secrets → `git filter-repo` to scrub; rotate the secret regardless

## Useful aliases (`~/.gitconfig`)

```ini
[alias]
  st = status -sb
  co = checkout
  br = branch
  lg = log --oneline --graph --decorate --all
  amend = commit --amend --no-edit
  unstage = restore --staged
  fixup = commit --fixup
  please = push --force-with-lease
```

## Examples in this folder

- [`git-cheatsheet.md`](git-cheatsheet.md)
- [`pre-commit-hook.sh`](pre-commit-hook.sh)
- [`gitmessage.template`](gitmessage.template)

## See also

- [../../DevOps/GitHubActions](../../DevOps/GitHubActions/) · [../../DevOps/AzureDevOps](../../DevOps/AzureDevOps/)
