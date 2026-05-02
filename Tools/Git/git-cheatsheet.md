# Git Cheatsheet

## Setup

```bash
git config --global user.name "Your Name"
git config --global user.email "you@example.com"
git config --global pull.rebase true
git config --global rebase.autoStash true
git config --global init.defaultBranch main
git config --global core.autocrlf input          # on macOS/Linux
```

## Daily workflow

```bash
git pull --rebase                                 # update local main
git checkout -b feature/x
# ... edit files ...
git add -p                                        # stage by hunks
git commit -m "feat(x): ..."
git push -u origin feature/x
# open PR, address feedback
git rebase main
git push --force-with-lease
```

## Rewriting safely

```bash
git rebase -i HEAD~5                              # squash, reword, drop
git commit --fixup=<sha>                          # auto-squash candidate
git rebase -i --autosquash HEAD~10                # apply fixups
git commit --amend --no-edit                      # tweak last commit
```

## Recover from disasters

```bash
git reflog                                        # show last 90 days of HEAD movement
git reset --hard HEAD@{2}                         # jump back to where you were
git fsck --lost-found                             # find unreachable objects
```

## Investigate

```bash
git blame -L 50,100 path/file.cs
git log -p path/file.cs                           # history of a single file
git log -S "magicString"                          # commits that introduced/removed a string
git bisect start
git bisect good v1.0
git bisect bad HEAD
# script-friendly:
git bisect run ./scripts/repro.sh
```

## Worktrees

```bash
git worktree add ../wt-feature-x feature-x        # work two branches at once
git worktree list
git worktree remove ../wt-feature-x
```

## Submodules vs subtrees

Prefer subtrees when you must vendor; avoid submodules for new projects unless you genuinely need them.
