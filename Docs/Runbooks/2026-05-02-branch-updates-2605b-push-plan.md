---
title: Push plan for branch-updates-2605b
created: 2026-05-02T15:16:06Z
author: enkhe
branch: branch-updates-2605b
base: main
status: draft
---

# Push plan — `branch-updates-2605b` → new remote branch

## Snapshot

- **Captured:** 2026-05-02T15:16:06Z
- **Local branch:** `branch-updates-2605b`
- **Remote base:** `origin/main` @ `3ee771f` (Merge PR #2 from `enkhe/branch-updates-2605a`)
- **Local HEAD:** `deb8dba` ("updates 2605b")
- **Divergence:** 1 commit local / 1 commit remote (real divergence, shared ancestor `0788a3e`)
- **Working tree:** clean

## Diff against `origin/main`

33 files changed, +1544 / -112.

| Status | Count | Notes |
|--------|-------|-------|
| Modified | 21 | Mostly `README.md` cheatsheets across AI-ML, AlgorithmsAndDataStructures, Docs, Security, Testing |
| Added | 12 | New System Design write-ups + new Docs templates |
| Build artifacts (modified, should be ignored) | 4 | See "Cleanup" below |

### New files (12)

- `AlgorithmsAndDataStructures/SystemDesign/design-chat-system.md`
- `AlgorithmsAndDataStructures/SystemDesign/design-newsfeed.md`
- `AlgorithmsAndDataStructures/SystemDesign/design-rate-limiter.md`
- `AlgorithmsAndDataStructures/SystemDesign/design-url-shortener.md`
- `Docs/Templates/adr-template.md`
- `Docs/Templates/incident-postmortem-template.md`
- `Docs/Templates/pull-request-template.md`
- `Docs/Templates/rfc-template.md`
- `Docs/Templates/runbook-template.md`
- `Docs/Templates/service-readme-template.md`
- `Docs/Templates/threat-model-template.md`

### Modified files (21)

AI-ML — `ComputerVision/README.md`, `MLOps/README.md`, `NLP/README.md`, `Notebooks/README.md`
AlgorithmsAndDataStructures — `CodeSignal/README.md`, `HackerRank/README.md`, `LeetCode/README.md`, `SystemDesign/README.md`
Docs — `Diagrams/README.md`, `Roadmaps/README.md`, `Templates/README.md`
Security — `Authentication/DualAuth/README.md`, `Authorization/README.md`, `Cryptography/README.md`, `OWASP/README.md`, `SecretsManagement/README.md`
Testing — `BehaviorDrivenDevelopment/README.md`, `TestDrivenDevelopment/README.md`

## Cleanup before push (recommended)

These four build artifacts should not be in git:

- `BackEnd/CSharp/NativeAOT/obj/Debug/net10.0/AotApi.AssemblyInfo.cs`
- `BackEnd/CSharp/NativeAOT/obj/Debug/net10.0/AotApi.AssemblyInfoInputs.cache`
- `BackEnd/CSharp/SourceGenerators/obj/Debug/netstandard2.0/SourceGen.AssemblyInfo.cs`
- `BackEnd/CSharp/SourceGenerators/obj/Debug/netstandard2.0/SourceGen.AssemblyInfoInputs.cache`

```bash
# Add to .gitignore (idempotent)
grep -qxF 'obj/' .gitignore || echo 'obj/' >> .gitignore
grep -qxF 'bin/' .gitignore || echo 'bin/' >> .gitignore

# Stop tracking already-committed obj/ trees
git rm -r --cached BackEnd/CSharp/NativeAOT/obj BackEnd/CSharp/SourceGenerators/obj

git commit -m "chore: ignore obj/ and bin/, untrack build artifacts"
```

## Options

### Option A — Push current branch as-is (simplest)

Pushes `branch-updates-2605b` to a remote branch of the same name. PR diff against `main` will show the same 33 files.

```bash
git push -u origin branch-updates-2605b
```

**Pros:** zero rewrite, preserves history. **Cons:** carries the divergence; PR will need merge or rebase server-side.

### Option B — Fresh branch from latest main, single squashed commit

```bash
git fetch origin
git checkout -b updates-2605b-clean origin/main
git checkout branch-updates-2605b -- .

# drop build artifacts from the staged tree
git reset HEAD -- BackEnd/CSharp/NativeAOT/obj BackEnd/CSharp/SourceGenerators/obj
git checkout -- BackEnd/CSharp/NativeAOT/obj BackEnd/CSharp/SourceGenerators/obj

git commit -m "updates 2605b: cheatsheets, system-design write-ups, doc templates"
git push -u origin updates-2605b-clean
```

**Pros:** clean linear history off the latest main, one tidy commit. **Cons:** rewrites the commit (loses `deb8dba` SHA).

### Option C — Cherry-pick existing commit onto fresh branch

```bash
git fetch origin
git checkout -b updates-2605b-clean origin/main
git cherry-pick deb8dba
git push -u origin updates-2605b-clean
```

**Pros:** preserves the original commit message, linear history. **Cons:** still carries the build-artifact files unless cleaned first.

## Decision

- [ ] Option chosen: _A / B / C_
- [ ] Cleanup applied: _yes / no_
- [ ] Pushed at: _<UTC timestamp>_
- [ ] Remote branch URL: _<fill in after push>_
- [ ] PR opened: _<PR # / URL>_

## Execution log

| Timestamp (UTC) | Step | Result |
|-----------------|------|--------|
| 2026-05-02T15:16:06Z | Plan drafted | — |
|  |  |  |
