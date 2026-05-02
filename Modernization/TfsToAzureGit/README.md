# TFVC → Azure Git (or GitHub)

> Centralized version control to distributed. Get the team mental model right before the tools.

## Mental model shift

| TFVC | Git |
|---|---|
| Workspace = checked-out paths | Working tree = whole repo |
| Get latest | `git pull --rebase` |
| Check out (lock) | No locks; rely on small commits |
| Pending changes window | `git status` |
| Shelveset | Stash + branch (`git stash`, branch + commit) |
| Branch = server-side folder | Branch = lightweight pointer |
| Merge = path-based | Merge = commit graph |

## Steps

1. **Pick scope** — single project at first; not "the entire collection."
2. **Convert history** — [`git-tfs`](https://github.com/git-tfs/git-tfs) preserves changesets → commits.
3. **Decide history depth** — full history is high-fidelity but huge; "shallow + archived TFVC for old history" is often enough.
4. **Reorganize** — TFVC paths often have ad hoc structure. Use the move to enforce repo conventions (single root, src/, tests/).
5. **Migrate pipelines** — Classic XAML/build definitions → YAML (`azure-pipelines.yml` or GitHub Actions).
6. **Rebrand workflows** — branch policies, PR templates, conventional commits.

## "To Be Dangerous" Cheatsheet

| Need | Command |
|---|---|
| Convert | `git tfs clone <tfs-url> $/Project/Trunk` |
| Bootstrap minus history | Just copy the tip + first commit |
| Branch protection | Azure Repos branch policies / GitHub branch rulesets |
| Conventional commits | `commitlint` + `husky` |

## Common Pitfalls

- Importing all TFVC branches as Git branches → tangled history
- Lifting old broken builds — rewrite as YAML; don't preserve XAML for sentiment
- Old binaries in TFVC → Git LFS, or leave them in archive
- File locks in old TFVC habits → educate the team out of it before cutover

## See also

- [../../Tools/Git](../../Tools/Git/) · [../../DevOps/AzureDevOps](../../DevOps/AzureDevOps/) · [../../DevOps/GitHubActions](../../DevOps/GitHubActions/)
