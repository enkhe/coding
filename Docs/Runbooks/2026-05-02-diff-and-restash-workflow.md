---
title: Diff a branch vs main, then re-apply changes onto a fresh branch (stash-style)
created: 2026-05-02T15:28:01Z
author: enkhe
source-branch: branch-updates-2605b
target-branch: updates-2605b-clean (new)
base: main
status: draft
---

# Diff + restash workflow — `branch-updates-2605b` → fresh branch off latest `main`

## Purpose

Two related tasks in one runbook:

1. **Inspect** — see exactly what differs between the current branch and `main`.
2. **Restash** — capture those changes, reset to the latest `main`, create a new branch, re-apply the changes (resolving any conflicts in the VS Code merge tool), commit, and push upstream.

This produces a clean linear branch off the latest `main` with one tidy commit, while keeping the original `branch-updates-2605b` intact as a safety net.

---

## Part 1 — See the difference between this branch and main

### Two-dot vs three-dot — pick the right one

| Syntax | Meaning | Use when |
|--------|---------|----------|
| `git diff main..HEAD` | Net difference between the two tips | You want raw "what would I get if I copied this branch over main" |
| `git diff main...HEAD` | What this branch added since it diverged from main (uses merge-base) | You want "my contribution" (this is what GitHub PRs show) |

**Use `...` for PR-style review.** The repo currently has a real divergence (1 commit each side), so `..` would also show what `main` did that you don't have — usually not what you want.

### Commands

```bash
# Make sure remote is current
git fetch origin

# 1) File list with add/modify/delete status
git diff --name-status origin/main...HEAD

# 2) Per-file line counts
git diff --stat origin/main...HEAD

# 3) Just the names
git diff --name-only origin/main...HEAD

# 4) Full unified diff (pipe to less or a file)
git diff origin/main...HEAD
git diff origin/main...HEAD > /tmp/branch-vs-main.patch

# 5) Commit list (commits on this branch but not on main)
git log --oneline origin/main..HEAD

# 6) Side-by-side in VS Code (uses the configured difftool)
git difftool -d origin/main...HEAD
```

### In VS Code (no terminal)

- **Source Control panel** → click the `…` menu → *Checkout to…* / *Compare with…*
- **GitLens** extension: *GitLens: Compare References…* → pick `origin/main` ↔ current branch.
- **Git Graph** extension: right-click a branch → *Compare with…*.
- Open any conflicted/changed file → bottom-left status bar shows the branch; the diff editor has *Open Changes* against the base.

---

## Part 2 — Restash workflow (committed changes → fresh branch off main)

### Why this isn't a literal `git stash`

`git stash` captures **uncommitted** working-tree changes. The changes on `branch-updates-2605b` are already **committed**, so a plain `git stash` here grabs nothing. The cleanest equivalent for committed work is `git merge --squash` — it stages every file change from the source branch on top of the current branch, runs the 3-way merge engine for conflicts, and lets you make one commit. Functionally: "bring everything across, let me resolve conflicts in the editor, then commit."

A literal stash variant is documented below as **Alternative A**.

### Pre-flight checks

- [ ] Working tree clean: `git status` shows nothing to commit.
- [ ] Source branch is the one with the changes: `git rev-parse --abbrev-ref HEAD` returns `branch-updates-2605b`.
- [ ] Remote is fresh: `git fetch origin`.
- [ ] Target branch name decided (this doc uses `updates-2605b-clean`).
- [ ] Original branch will be left untouched as a backup.

### Steps

```bash
# 0. Safety: fetch and confirm we know the SHA we are preserving
git fetch origin
git rev-parse branch-updates-2605b   # note this SHA — recoverable from reflog if anything goes wrong

# 1. Switch to main and bring it up to date
git checkout main
git pull --ff-only origin main       # fast-forward only — fails loudly if local main has diverged

# 2. Create the new feature branch from the latest main
git checkout -b updates-2605b-clean

# 3. "Restash" — pull every change from the source branch into the index
#    --squash:  collapses the source branch's commits into staged changes (no commit yet)
#    --no-commit: belt-and-braces; --squash already implies it but explicit is clearer
git merge --squash --no-commit branch-updates-2605b

# 4. If conflicts: VS Code marks them in the Source Control panel.
#    For each conflicted file:
#      - click the file → use the inline "Accept Current / Incoming / Both" buttons
#      - or open the 3-way merge editor (button at top-right of the diff)
#    After resolving each:
git add <resolved-file>
#    Re-check status:
git status

# 5. (Optional cleanup) drop build artifacts that should not be tracked
git reset HEAD -- BackEnd/CSharp/NativeAOT/obj BackEnd/CSharp/SourceGenerators/obj
git checkout -- BackEnd/CSharp/NativeAOT/obj BackEnd/CSharp/SourceGenerators/obj

# 6. Commit
git commit -m "updates 2605b: cheatsheets, system-design write-ups, doc templates"

# 7. Push upstream and set tracking
git push -u origin updates-2605b-clean

# 8. Verify
git log --oneline -5
git status
gh pr create --base main --head updates-2605b-clean --fill   # optional: open PR
```

### Validation

```bash
# Same file set as the source branch's diff vs main? (should match)
git diff --name-status origin/main...HEAD

# Source branch still intact and unchanged?
git rev-parse branch-updates-2605b   # should equal the SHA captured in step 0
```

### Rollback (if anything goes sideways)

```bash
# Throw away the new branch entirely (safe — original branch untouched)
git checkout main
git branch -D updates-2605b-clean

# Or, if you already pushed and want to delete the remote copy
git push origin --delete updates-2605b-clean
```

The original `branch-updates-2605b` is never modified by this procedure. If it somehow is, recover via:

```bash
git reflog                                       # find the pre-procedure SHA
git update-ref refs/heads/branch-updates-2605b <sha>
```

---

## Alternative A — Literal stash workflow (uncommit + stash + pop)

Use this only if you specifically want to **destroy** the commit on `branch-updates-2605b` and migrate the work to the new branch. **The default workflow above is safer because it leaves the source branch intact.**

```bash
# On branch-updates-2605b — soft-reset moves HEAD back to main while keeping changes staged
git reset --soft origin/main

# Move staged changes into a stash entry (including any untracked files)
git stash push --include-untracked --message "branch-updates-2605b restash 2026-05-02"

# Switch to main, update, and branch
git checkout main
git pull --ff-only origin main
git checkout -b updates-2605b-clean

# Pop the stash — conflicts (if any) appear in VS Code Source Control
git stash pop

# Resolve conflicts in VS Code, then:
git add .
git commit -m "updates 2605b: ..."
git push -u origin updates-2605b-clean
```

**Caveat:** `git reset --soft` rewrote `branch-updates-2605b` — the original commit `deb8dba` is now only reachable via reflog. The default `merge --squash` workflow avoids this entirely.

---

## Alternative B — Patch file workflow

Useful when you want a portable artifact (email-able, archivable):

```bash
# Capture the diff as a patch
git diff origin/main...branch-updates-2605b > /tmp/branch-2605b.patch

# Update main, branch off
git checkout main
git pull --ff-only origin main
git checkout -b updates-2605b-clean

# Apply with 3-way fallback so conflicts are real conflicts (not "patch failed")
git apply --3way /tmp/branch-2605b.patch

# Resolve in VS Code → stage → commit → push
git add .
git commit -m "updates 2605b: ..."
git push -u origin updates-2605b-clean
```

---

## Decision

- [ ] Workflow chosen: _Default (merge --squash) / Alt A (stash) / Alt B (patch)_
- [ ] Cleanup applied (obj/, bin/): _yes / no_
- [ ] New branch name: _updates-2605b-clean_
- [ ] Pushed at: _<UTC timestamp>_
- [ ] Remote branch URL: _<fill in after push>_
- [ ] PR opened: _<PR # / URL>_

## Execution log

| Timestamp (UTC) | Step | Result |
|-----------------|------|--------|
| 2026-05-02T15:28:01Z | Plan drafted | — |
|  |  |  |

## Related

- [Push plan for branch-updates-2605b](2026-05-02-branch-updates-2605b-push-plan.md)
- [Runbook template](../Templates/runbook-template.md)
